// ==================== GLOBAL VARIABLES ====================
const currentUserId = parseInt(document.getElementById("currentUserId").value);
let currentPartnerId = 0;
let conversations = [];

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
            await openChat(user.id, user.userName, user.avatarUrl || "/images/avatar.png");
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
            showError("Failed to load conversations");
            return [];
        });
}

function renderConversations(data) {
    const list = document.getElementById("conversationList");
    list.innerHTML = "";

    if (data.length === 0) {
        list.innerHTML = `
            <div class="empty-state" style="padding: 40px 20px;">
                <p style="color: var(--text-secondary); text-align: center;">No conversations yet</p>
            </div>
        `;
        return;
    }

    data.forEach((c, index) => {
        const active = c.partnerId === currentPartnerId ? "active" : "";
        const unread = !c.isRead ? "unread" : "";
        const avatar = c.partnerAvatar || "/images/avatar.png";
        const lastMessage = c.lastMessage || "Start a conversation";

        const conversationEl = document.createElement("div");
        conversationEl.className = `conversation-item ${active} ${unread}`;
        conversationEl.style.animationDelay = `${index * 0.05}s`;
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
                    <span>${escapeHtml(lastMessage)}</span>
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
    currentPartnerId = pid;

    // Mark as read and remove unread styling immediately
    await markAsRead(pid);

    // Update UI immediately
    updateConversationReadStatus(pid);

    // Show chat container
    document.getElementById("emptyState").classList.add("hidden");
    const chatContainer = document.getElementById("chatContainer");
    chatContainer.classList.remove("hidden");

    // Update header
    document.getElementById("headerName").innerText = name;
    document.getElementById("headerAvatar").src = avatar;

    // Update active state
    updateActiveConversation(name);

    // Show loading spinner
    const messageBox = document.getElementById("messageBox");
    messageBox.innerHTML = `
        <div class="loading-state">
            <div class="spinner"></div>
        </div>
    `;

    // Load message history
    try {
        const msgs = await fetch(`/Message/History/${pid}`).then(r => r.json());
        messageBox.innerHTML = "";
        msgs.forEach(m => appendMessage(m));
        scrollToBottom();
    } catch (err) {
        console.error("Error loading message history:", err);
        showError("Failed to load messages");
    }
}

async function markAsRead(partnerId) {
    try {
        await fetch(`/Message/MarkAsRead?partnerId=${partnerId}`, {
            method: "POST"
        });
    } catch (err) {
        console.error("Error marking as read:", err);
    }
}

function updateConversationReadStatus(partnerId) {
    const conversationItems = document.querySelectorAll(".conversation-item");
    conversationItems.forEach(item => {
        const nameEl = item.querySelector(".conversation-name");
        if (nameEl && item.onclick.toString().includes(`openChat(${partnerId}`)) {
            // Remove unread class
            item.classList.remove("unread");

            // Remove unread dot
            const unreadDot = item.querySelector(".unread-dot");
            if (unreadDot) {
                unreadDot.remove();
            }

            // Update message styling
            const messageEl = item.querySelector(".conversation-message");
            if (messageEl) {
                messageEl.style.fontWeight = "normal";
                messageEl.style.color = "var(--text-secondary)";
            }
        }
    });
}

function updateActiveConversation(name) {
    document.querySelectorAll(".conversation-item").forEach(el => {
        el.classList.remove("active");
    });

    const activeItem = [...document.querySelectorAll(".conversation-item")]
        .find(el => el.querySelector(".conversation-name").innerText === name);

    if (activeItem) {
        activeItem.classList.add("active");
    }
}

// ==================== MESSAGE OPERATIONS ====================
function sendMessage() {
    const input = document.getElementById("messageInput");
    const text = input.value.trim();

    if (!text || currentPartnerId === 0) return;

    // Create optimistic message
    const fakeMessage = {
        content: text,
        senderId: currentUserId,
        createdAt: new Date().toISOString()
    };

    appendMessage(fakeMessage);
    scrollToBottom();

    // Clear input
    input.value = "";
    document.getElementById("sendBtn").disabled = true;

    // Send to server
    fetch("/Message/Send", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            receiverId: currentPartnerId,
            content: text
        })
    })
        .then(() => loadConversations())
        .catch(err => {
            console.error("Error sending message:", err);
            showError("Failed to send message");
        });
}

function appendMessage(msg) {
    const box = document.getElementById("messageBox");
    const mine = msg.senderId === currentUserId;
    const time = formatMessageTime(msg.createdAt);

    const messageRow = document.createElement("div");
    messageRow.className = `message-row ${mine ? "message-out" : "message-in"}`;

    if (mine) {
        messageRow.innerHTML = `
            <div class="message-bubble" title="${time}">
                ${escapeHtml(msg.content)}
            </div>
        `;
    } else {
        const avatar = document.getElementById("headerAvatar").src;
        messageRow.innerHTML = `
            <img src="${avatar}" class="avatar-tiny" alt="Avatar">
            <div class="message-bubble" title="${time}">
                ${escapeHtml(msg.content)}
            </div>
        `;
    }

    box.appendChild(messageRow);
}

function scrollToBottom() {
    const box = document.getElementById("messageBox");
    setTimeout(() => {
        box.scrollTop = box.scrollHeight;
    }, 100);
}

// ==================== UTILITY FUNCTIONS ====================
function formatTime(dateString) {
    if (!dateString) return "";

    const date = new Date(dateString);
    const now = new Date();
    const diffMs = now - date;
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1) return "Just now";
    if (diffMins < 60) return `${diffMins}m`;
    if (diffHours < 24) return `${diffHours}h`;
    if (diffDays < 7) return `${diffDays}d`;

    return date.toLocaleDateString();
}

function formatMessageTime(dateString) {
    const date = new Date(dateString);
    return date.toLocaleTimeString([], {
        hour: "2-digit",
        minute: "2-digit"
    });
}

function escapeHtml(text) {
    const div = document.createElement("div");
    div.textContent = text;
    return div.innerHTML;
}

function showError(message) {
    console.error(message);
    // You can implement a toast notification here
}

// ==================== VISIBILITY CHANGE ====================
document.addEventListener("visibilitychange", function () {
    if (!document.hidden && currentPartnerId) {
        loadConversations();
    }
});