// ==================== GLOBAL VARIABLES ====================
const currentUserId = parseInt(document.getElementById("currentUserId").value);
let currentPartnerId = 0;
let conversations = [];

// Biến cho tính năng Ghi âm
let mediaRecorder;
let audioChunks = [];
let recordingInterval;
let startTime;
let isRecordingCancelled = false;

// ==================== SIGNALR CONNECTION ====================
const chatConnection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect()
    .build();

chatConnection.on("ReceiveMessage", function (msg) {
    if (currentPartnerId === msg.senderId || currentPartnerId === msg.receiverId) {
        appendMessage(msg);
        scrollToBottom();
    }
    loadConversations();
});

chatConnection.start()
    .then(() => console.log("SignalR Connected"))
    .catch(err => console.error("SignalR Connection Error:", err));

// ==================== INITIALIZATION ====================
document.addEventListener("DOMContentLoaded", async function () {
    await initializeChat();
    setupEventListeners();
});

async function initializeChat() {
    conversations = await loadConversations();

    const params = new URLSearchParams(window.location.search);
    const cid = params.get("conversationId");
    const pid = params.get("partnerId");

    if (cid) {
        const target = conversations.find(c => c.conversationId == cid);
        if (target) {
            await openChat(target.partnerId, target.partnerName, target.partnerAvatar);
        }
    } else if (pid) {
        try {
            const user = await fetch(`/Profile/GetUserInfo/${pid}`).then(r => r.json());
            await openChat(user.id, user.userName, user.avatarUrl || "/images/default-avatar.png");
        } catch (err) {
            console.error("Error loading user info:", err);
        }
    }
}

function setupEventListeners() {
    const sendBtn = document.getElementById("sendBtn");
    const messageInput = document.getElementById("messageInput");
    const searchInput = document.getElementById("searchInput");

    sendBtn.onclick = sendMessage;

    messageInput.onkeypress = (e) => {
        if (e.key === "Enter" && !e.shiftKey) {
            e.preventDefault();
            sendMessage();
        }
    };

    messageInput.oninput = (e) => {
        sendBtn.disabled = e.target.value.trim() === "";
    };

    if (searchInput) {
        searchInput.oninput = (e) => {
            filterConversations(e.target.value);
        };
    }
}

// ==================== CONVERSATIONS ====================
function loadConversations() {
    return fetch("/Message/GetConversations")
        .then(r => r.json())
        .then(data => {
            conversations = data;
            renderConversations(data);
            return data;
        })
        .catch(err => {
            console.error("Error loading conversations:", err);
            return [];
        });
}

function renderConversations(data) {
    const list = document.getElementById("conversationList");
    list.innerHTML = "";

    if (data.length === 0) {
        list.innerHTML = `<div class="empty-state" style="padding: 40px 20px;"><p style="color: var(--text-secondary); text-align: center;">No conversations yet</p></div>`;
        return;
    }

    data.forEach((c, index) => {
        const active = c.partnerId === currentPartnerId ? "active" : "";
        const unread = !c.isRead ? "unread" : "";
        const avatar = c.partnerAvatar || "/images/default-avatar.png";

        // Hiển thị nội dung tin nhắn cuối (xử lý nếu là ảnh/voice)
        let lastMsgText = c.lastMessage || "Start a conversation";
        if (lastMsgText.includes("cloudinary.com")) {
            if (lastMsgText.includes(".mp3") || lastMsgText.includes(".webm") || lastMsgText.includes("resource_type=video")) {
                lastMsgText = "🎤 Voice message";
            } else {
                lastMsgText = "🖼️ Sent an image";
            }
        }

        const conversationEl = document.createElement("div");
        conversationEl.className = `conversation-item ${active} ${unread}`;
        conversationEl.onclick = () => openChat(c.partnerId, c.partnerName, avatar);

        conversationEl.innerHTML = `
            <div class="conversation-avatar-wrapper">
                <img src="${avatar}" class="conversation-avatar" alt="${c.partnerName}">
                ${c.isOnline ? '<div class="online-indicator"></div>' : ''}
            </div>
            <div class="conversation-content">
                <div class="conversation-header">
                    <span class="conversation-name">${escapeHtml(c.partnerName)}</span>
                    <span class="conversation-time">${formatTime(c.lastMessageTime)}</span>
                </div>
                <div class="conversation-message">
                    ${!c.isRead ? '<span class="unread-dot"></span>' : ''}
                    <span>${escapeHtml(lastMsgText)}</span>
                </div>
            </div>
        `;
        list.appendChild(conversationEl);
    });
}

function filterConversations(query) {
    const filtered = conversations.filter(c =>
        c.partnerName.toLowerCase().includes(query.toLowerCase())
    );
    renderConversations(filtered);
}

// ==================== CHAT OPERATIONS ====================
async function openChat(pid, name, avatar) {
    currentPartnerId = pid; // QUAN TRỌNG: Cập nhật ID người đang chat

    // UI Updates
    await markAsRead(pid);
    updateConversationReadStatus(pid);

    document.getElementById("emptyState").classList.add("hidden");
    document.getElementById("chatContainer").classList.remove("hidden");
    document.getElementById("headerName").innerText = name;
    document.getElementById("headerAvatar").src = avatar;

    updateActiveConversation(name);

    // Reset Chat Box
    const messageBox = document.getElementById("messageBox");
    messageBox.innerHTML = `<div class="loading-state"><div class="spinner"></div></div>`;

    // Load History
    try {
        const msgs = await fetch(`/Message/History/${pid}`).then(r => r.json());
        messageBox.innerHTML = "";
        msgs.forEach(m => appendMessage(m));
        scrollToBottom();
    } catch (err) {
        console.error("Error loading history:", err);
        messageBox.innerHTML = "<p class='text-center text-danger'>Failed to load messages</p>";
    }
}

async function markAsRead(partnerId) {
    try {
        await fetch(`/Message/MarkAsRead?partnerId=${partnerId}`, { method: "POST" });
    } catch (err) { console.error(err); }
}

function updateConversationReadStatus(partnerId) {
    // Logic update UI unread... (Giữ nguyên logic cũ của bạn nếu cần)
}

function updateActiveConversation(name) {
    document.querySelectorAll(".conversation-item").forEach(el => el.classList.remove("active"));
    // Logic tìm item active... (Giữ nguyên logic cũ)
}

// ==================== SEND TEXT MESSAGE ====================
function sendMessage() {
    const input = document.getElementById("messageInput");
    const text = input.value.trim();

    if (!text || currentPartnerId === 0) return;

    // Optimistic UI
    const fakeMessage = {
        content: text,
        senderId: currentUserId,
        createdAt: new Date().toISOString(),
        messageType: "text"
    };
    appendMessage(fakeMessage);
    scrollToBottom();

    input.value = "";
    document.getElementById("sendBtn").disabled = true;

    fetch("/Message/Send", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ receiverId: currentPartnerId, content: text })
    })
        .then(() => loadConversations())
        .catch(err => console.error("Send error:", err));
}

// ==================== RENDER MESSAGE (TEXT/IMAGE/VOICE) ====================
function appendMessage(msg) {
    const box = document.getElementById("messageBox");
    const mine = msg.senderId === currentUserId;
    const time = formatMessageTime(msg.createdAt);

    const messageRow = document.createElement("div");
    messageRow.className = `message-row ${mine ? "message-out" : "message-in"}`;

    // --- LOGIC ĐOÁN TYPE NẾU SERVER TRẢ VỀ NULL ---
    let type = (msg.messageType || "").toLowerCase();

    if (!type || type === "text") {
        const content = msg.content.toLowerCase();
        if (content.includes("cloudinary.com")) {
            if (content.endsWith(".mp4") || content.endsWith(".mov") || content.endsWith(".avi")) {
                type = "video"; // <--- Thêm dòng này
            } else if (content.endsWith(".mp3") || content.endsWith(".webm") || content.endsWith(".wav")) {
                type = "voice";
            } else {
                type = "image";
            }
        }
    }

    let contentHtml = "";

    // --- THÊM LOGIC RENDER VIDEO ---
    if (type === "image") {
        contentHtml = `<img src="${msg.content}" class="img-fluid rounded" style="max-width: 200px; cursor: pointer;" onclick="window.open(this.src, '_blank')">`;
    }
    else if (type === "video") { // <--- XỬ LÝ VIDEO
        contentHtml = `
            <div style="max-width: 250px;">
                <video controls style="width: 100%; border-radius: 10px;">
                    <source src="${msg.content}" type="video/mp4">
                    <source src="${msg.content}" type="video/webm">
                    Your browser does not support the video tag.
                </video>
            </div>`;
    }
    else if (type === "voice") {
        contentHtml = `
            <div style="min-width: 200px; display: flex; align-items: center;">
                <audio controls controlsList="nodownload" preload="metadata" style="width: 100%; height: 32px;">
                    <source src="${msg.content}" type="audio/webm">
                    <source src="${msg.content}" type="audio/mp4">
                    Your browser does not support audio.
                </audio>
            </div>`;
    }
    else {
        contentHtml = escapeHtml(msg.content);
    }

    if (mine) {
        messageRow.innerHTML = `<div class="message-bubble" title="${time}">${contentHtml}</div>`;
    } else {
        const avatar = document.getElementById("headerAvatar").src;
        messageRow.innerHTML = `<img src="${avatar}" class="avatar-tiny" alt="Avatar"><div class="message-bubble" title="${time}">${contentHtml}</div>`;
    }

    box.appendChild(messageRow);
}

function scrollToBottom() {
    const box = document.getElementById("messageBox");
    setTimeout(() => { box.scrollTop = box.scrollHeight; }, 100);
}

// ==================== FILE UPLOAD (IMAGE/VIDEO) ====================
async function handleFileUpload(input) {
    const file = input.files[0];
    if (!file) return;

    if (!currentPartnerId || currentPartnerId === 0) {
        alert("Please select a conversation first!");
        input.value = '';
        return;
    }

    const formData = new FormData();
    formData.append("ReceiverId", currentPartnerId);
    formData.append("MessageFile", file);

    try {
        console.log("Uploading...");
        const response = await fetch('/Message/SendFile', { method: 'POST', body: formData });

        if (response.ok) {
            const data = await response.json();
            appendMessage(data); // Hiện tin nhắn lên luôn
            loadConversations();
        } else {
            const txt = await response.text();
            alert("Upload failed: " + txt);
        }
    } catch (error) {
        console.error("Network error:", error);
    } finally {
        input.value = '';
    }
}

// ==================== VOICE RECORDING LOGIC ====================
async function startRecording() {
    if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
        alert("Microphone not supported/HTTPS required.");
        return;
    }

    try {
        const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
        mediaRecorder = new MediaRecorder(stream);
        audioChunks = [];
        isRecordingCancelled = false;

        // UI Changes
        document.querySelector('.input-actions-left').style.display = 'none';
        document.getElementById('messageInput').style.display = 'none';
        document.getElementById('sendBtn').style.display = 'none';
        document.getElementById('recordingState').style.display = 'flex';

        // Timer
        startTime = Date.now();
        document.getElementById('recordTimer').innerText = "00:00";
        recordingInterval = setInterval(() => {
            const elapsed = Math.floor((Date.now() - startTime) / 1000);
            const min = String(Math.floor(elapsed / 60)).padStart(2, '0');
            const sec = String(elapsed % 60).padStart(2, '0');
            const timerEl = document.getElementById('recordTimer');
            if (timerEl) timerEl.innerText = `${min}:${sec}`;
        }, 1000);

        mediaRecorder.ondataavailable = event => {
            if (event.data.size > 0) audioChunks.push(event.data);
        };

        mediaRecorder.onstop = async () => {
            stream.getTracks().forEach(t => t.stop()); // Tắt mic
            clearInterval(recordingInterval);

            if (isRecordingCancelled) return;

            if (audioChunks.length > 0) {
                const audioBlob = new Blob(audioChunks, { type: 'audio/webm' });
                const audioFile = new File([audioBlob], "voice_msg.webm", { type: 'audio/webm' });
                await sendVoiceFile(audioFile);
            }
        };

        mediaRecorder.start();

    } catch (err) {
        console.error("Mic Error:", err);
        alert("Cannot access microphone.");
    }
}

function finishAndSendVoice() {
    if (mediaRecorder && mediaRecorder.state !== "inactive") {
        isRecordingCancelled = false;
        mediaRecorder.stop(); // Trigger onstop
        resetRecordingUI();
    }
}

function cancelRecording() {
    if (mediaRecorder) {
        isRecordingCancelled = true;
        mediaRecorder.stop();
        resetRecordingUI();
    }
}

function resetRecordingUI() {
    document.getElementById('recordingState').style.display = 'none';
    document.querySelector('.input-actions-left').style.display = 'flex';
    document.getElementById('messageInput').style.display = 'block';
    document.getElementById('sendBtn').style.display = 'block';
    clearInterval(recordingInterval);
}

async function sendVoiceFile(file) {
    if (!currentPartnerId || currentPartnerId === 0) return;

    const formData = new FormData();
    formData.append("ReceiverId", currentPartnerId);
    formData.append("MessageFile", file);

    try {
        const response = await fetch('/Message/SendFile', { method: 'POST', body: formData });
        if (response.ok) {
            const data = await response.json();
            appendMessage(data);
            loadConversations();
        } else {
            console.error("Voice send failed");
        }
    } catch (error) {
        console.error("Voice network error:", error);
    }
}

// ==================== UTILS ====================
function formatTime(dateString) { return dateString ? new Date(dateString).toLocaleDateString() : ""; }
function formatMessageTime(dateString) { return new Date(dateString).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" }); }
function escapeHtml(text) {
    const div = document.createElement("div");
    div.textContent = text;
    return div.innerHTML;
}