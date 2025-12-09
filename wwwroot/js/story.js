// --- File: wwwroot/js/story.js ---

var storiesData = [];
var currentUserIndex = 0;
var currentStoryIndex = 0;
var storyTimer;
var LOGGED_IN_USER_ID = 0; // Sẽ được gán sau
const IMAGE_DURATION = 10000;

// Hàm khởi tạo chính - Gọi hàm này từ Razor View
function initStoryModule(userId, userAvatar) {
    console.log("Init Story Module with UserID:", userId);
    LOGGED_IN_USER_ID = userId;

    // Gọi load ngay khi init
    loadStories();
}

function loadStories() {
    fetch('/Story/Stories')
        .then(res => res.json())
        .then(data => {
            console.log("Stories Loaded:", data);
            if (!data) data = [];
            storiesData = data;
            renderStoryTray(data);
        })
        .catch(err => console.error("Load failed", err));
}

function renderStoryTray(users) {
    const container = document.getElementById('friendsStoryList');
    if (!container) return;
    container.innerHTML = '';

    if (!users || users.length === 0) return;

    users.forEach((user, index) => {
        const stories = user.stories || user.Stories || [];
        const userName = user.userName || user.UserName;
        const avatar = user.avatarUrl || user.AvatarUrl || '/images/default-avatar.png';
        const ringClass = (stories.length > 0) ? 'unseen' : 'seen';

        // Gọi hàm mở viewer
        const html = `
            <div class="story-item" onclick="openStoryViewer(${index})">
                <div class="story-ring ${ringClass}">
                    <img src="${avatar}" class="story-avatar" onerror="this.onerror=null; this.src='/images/default-avatar.png';">
                </div>
                <span class="story-username">${userName}</span>
            </div>
        `;
        container.innerHTML += html;
    });
}

function openStoryViewer(userIndex) {
    currentUserIndex = userIndex;
    currentStoryIndex = 0;
    const viewer = document.getElementById('storyViewer');
    if (viewer) {
        viewer.style.display = 'flex';
        showCurrentStory();
    }
}

function closeStoryViewer() {
    const viewer = document.getElementById('storyViewer');
    if (viewer) viewer.style.display = 'none';
    clearTimeout(storyTimer);
    const video = document.getElementById('storyVideo');
    if (video) video.pause();
}

function showCurrentStory() {
    clearTimeout(storyTimer);

    const viewerAvatar = document.getElementById('viewerAvatar');
    const viewerUsername = document.getElementById('viewerUsername');
    const viewerTime = document.getElementById('viewerTime');
    const img = document.getElementById('storyImage');
    const video = document.getElementById('storyVideo');
    const caption = document.getElementById('storyCaptionOverlay');
    const btnDelete = document.getElementById('btnDeleteStory');

    if (!viewerAvatar) return;

    const user = storiesData[currentUserIndex];
    if (!user) { closeStoryViewer(); return; }

    const userStories = user.stories || user.Stories || [];
    if (!userStories[currentStoryIndex]) {
        closeStoryViewer();
        return;
    }

    const story = userStories[currentStoryIndex];

    // Render UI
    viewerAvatar.src = user.avatarUrl || user.AvatarUrl || '/images/default-avatar.png';
    viewerUsername.innerText = user.userName || user.UserName;

    // Time Ago
    const createdDate = story.createdAt || story.CreatedAt;
    if (createdDate) {
        const timeDiff = new Date() - new Date(createdDate);
        const hours = Math.floor(timeDiff / (1000 * 60 * 60));
        viewerTime.innerText = hours > 0 ? `${hours}h` : 'Just now';
    }

    // Delete Button Logic (So sánh chuẩn xác)
    const storyOwnerId = user.userId || user.UserId;
    // Ép kiểu về string để so sánh an toàn
    if (String(storyOwnerId) === String(LOGGED_IN_USER_ID)) {
        btnDelete.style.display = 'block';
    } else {
        btnDelete.style.display = 'none';
    }

    renderProgressBar(userStories.length, currentStoryIndex);

    // Caption
    const captionText = story.caption || story.Caption;
    caption.style.display = captionText ? 'block' : 'none';
    caption.innerText = captionText || "";

    // Media
    const mType = story.mediaType || story.MediaType || "image";
    const mUrl = story.mediaUrl || story.MediaUrl;

    if (mType.toLowerCase().includes("video")) {
        img.style.display = 'none';
        video.style.display = 'block';
        video.src = mUrl;
        video.load();
        var p = video.play();
        if (p !== undefined) p.catch(() => { });
        video.onended = function () { nextStory(); };
    } else {
        video.style.display = 'none';
        video.pause();
        img.style.display = 'block';
        img.src = mUrl;
        storyTimer = setTimeout(nextStory, IMAGE_DURATION);
    }
}

function renderProgressBar(total, current) {
    const container = document.getElementById('storyProgressBar');
    if (!container) return;
    container.innerHTML = '';
    for (let i = 0; i < total; i++) {
        let width = (i < current) ? '100%' : '0%';
        let transition = '';
        if (i === current) {
            width = '100%';
            transition = `transition: width ${IMAGE_DURATION}ms linear;`;
        }
        container.innerHTML += `<div class="progress-segment"><div class="progress-fill" style="width: ${width}; ${transition}"></div></div>`;
        if (i === current) {
            setTimeout(() => {
                const fills = container.getElementsByClassName('progress-fill');
                if (fills[i]) fills[i].style.width = '100%';
            }, 50);
        }
    }
}

function nextStory() {
    const user = storiesData[currentUserIndex];
    const userStories = user.stories || user.Stories || [];
    if (currentStoryIndex < userStories.length - 1) {
        currentStoryIndex++;
        showCurrentStory();
    } else {
        if (currentUserIndex < storiesData.length - 1) {
            currentUserIndex++;
            currentStoryIndex = 0;
            showCurrentStory();
        } else {
            closeStoryViewer();
        }
    }
}

function prevStory() {
    if (currentStoryIndex > 0) {
        currentStoryIndex--;
        showCurrentStory();
    } else {
        if (currentUserIndex > 0) {
            currentUserIndex--;
            const prevUser = storiesData[currentUserIndex];
            const prevStories = prevUser.stories || prevUser.Stories || [];
            currentStoryIndex = prevStories.length - 1;
            showCurrentStory();
        }
    }
}

function deleteCurrentStory() {
    if (!confirm("Delete this story?")) return;

    const user = storiesData[currentUserIndex];
    const userStories = user.stories || user.Stories || [];
    const story = userStories[currentStoryIndex];
    const storyId = story.storyId || story.StoryId;

    clearTimeout(storyTimer);
    document.getElementById('storyVideo').pause();

    fetch(`/Story/DeleteStory?storyId=${storyId}`, { method: 'POST' })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                userStories.splice(currentStoryIndex, 1);
                if (userStories.length === 0) {
                    storiesData.splice(currentUserIndex, 1);
                    closeStoryViewer();
                    renderStoryTray(storiesData);
                } else {
                    if (currentStoryIndex >= userStories.length) currentStoryIndex = userStories.length - 1;
                    showCurrentStory();
                    renderStoryTray(storiesData);
                }
            } else {
                alert("Failed to delete.");
            }
        });
}