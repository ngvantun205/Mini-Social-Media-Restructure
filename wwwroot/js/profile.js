document.addEventListener('DOMContentLoaded', function () {
    initProfileTabs();
    initSettingsDropdown();
    initPostGridInteractions();
});
function initProfileTabs() {
    const tabs = document.querySelectorAll('.profile-tab');

    tabs.forEach(tab => {
        tab.addEventListener('click', function (e) {
            e.preventDefault();

            // Remove active from all tabs
            tabs.forEach(t => t.classList.remove('active'));

            // Add active to clicked tab
            this.classList.add('active');

            // Get tab target
            const target = this.dataset.tab;

            // Hide all tab contents
            document.querySelectorAll('.tab-content').forEach(content => {
                content.style.display = 'none';
            });

            // Show target content
            const targetContent = document.getElementById(target);
            if (targetContent) {
                targetContent.style.display = 'block';
                // Add animation
                targetContent.style.animation = 'fadeIn 0.4s ease-out';
            }
        });
    });
}


let settingsDropdown = null;

function initSettingsDropdown() {
    document.addEventListener('click', function (event) {
        const settingsBtn = event.target.closest('.settings-toggle-btn');

        if (settingsBtn) {
            event.stopPropagation();
            toggleSettingsDropdown(settingsBtn);
        } else if (!event.target.closest('.settings-dropdown-menu')) {
            closeSettingsDropdown();
        }
    });
}

// ============================================
// POST GRID INTERACTIONS
// ============================================

function initPostGridInteractions() {
    const postItems = document.querySelectorAll('.post-grid-item');

    postItems.forEach(item => {
        item.addEventListener('click', function () {
            const postId = this.dataset.postId;
            if (postId) {
                navigateToPost(postId);
            }
        });
    });
}

function navigateToPost(postId) {
    window.location.href = `/Post/PostDetails?id=${postId}`;
}

// ============================================
// FOLLOW/UNFOLLOW FUNCTIONALITY
// ============================================

function toggleFollow(userId) {
    const btn = document.getElementById("followBtn");
    if (!btn) return;

    const isFollowing = btn.innerText.trim() === "Following";

    // 1. Optimistic UI (Cập nhật giao diện trước)
    if (isFollowing) {
        btn.innerText = "Follow";
        btn.classList.remove("btn-light");
        btn.classList.add("btn-primary");
        updateFollowerCount(-1);
    } else {
        btn.innerText = "Following";
        btn.classList.remove("btn-primary");
        btn.classList.add("btn-light");
        updateFollowerCount(1);
    }

    btn.disabled = true;

    // 2. Gọi API (SỬA ĐOẠN NÀY)
    const url = isFollowing ? '/Follow/Unfollow' : '/Follow/Follow';

    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        // Đưa followeeId vào Body JSON
        body: JSON.stringify({ followeeId: userId })
    })
        .then(res => {
            if (!res.ok) throw new Error("Failed");
            return res.json();
        })
        .then(data => {
            // Nếu Server trả về ErrorMessage trong data (logic cũ của bạn)
            if (data.errorMessage) {
                throw new Error(data.errorMessage);
            }
            // Thành công -> Không làm gì cả vì UI đã đổi rồi
        })
        .catch(err => {
            console.error(err);
            // 3. Rollback (Hoàn tác nếu lỗi)
            if (isFollowing) {
                btn.innerText = "Following";
                btn.classList.remove("btn-primary");
                btn.classList.add("btn-light");
                updateFollowerCount(1);
            } else {
                btn.innerText = "Follow";
                btn.classList.remove("btn-light");
                btn.classList.add("btn-primary");
                updateFollowerCount(-1);
            }
        })
        .finally(() => {
            btn.disabled = false;
        });
}

function updateFollowerCount(change) {
    const countEl = document.getElementById("followerCount");

    if (countEl) {
        let currentCount = parseInt(countEl.innerText) || 0;
        currentCount += change;
        countEl.innerText = currentCount;

        countEl.style.animation = 'pulse 0.4s ease-out';
        setTimeout(() => {
            countEl.style.animation = '';
        }, 400);
    } else {
        console.error("Không tìm thấy thẻ hiển thị số follower (id='followerCount')");
    }
}


function initAvatarUpload() {
    const avatarInput = document.getElementById('avatarInput');
    const avatarPreview = document.getElementById('avatarPreview');

    if (avatarInput && avatarPreview) {
        avatarInput.addEventListener('change', function (e) {
            const file = e.target.files[0];
            if (file) {
                if (file.size > 5 * 1024 * 1024) {
                    alert('Image size should be less than 5MB');
                    return;
                }

                const reader = new FileReader();
                reader.onload = function (event) {
                    avatarPreview.src = event.target.result;
                    avatarPreview.style.animation = 'fadeIn 0.4s ease-out';
                };
                reader.readAsDataURL(file);
            }
        });
    }
}

function triggerAvatarUpload() {
    const input = document.getElementById('avatarInput');
    if (input) {
        input.click();
    }
}

// ============================================
// SHARE PROFILE
// ============================================

function shareProfile() {
    const url = window.location.href;

    if (navigator.share) {
        navigator.share({
            title: document.title,
            url: url
        }).catch(err => console.log('Share cancelled'));
    } else {
        copyToClipboard(url);
    }
}

function copyToClipboard(text) {
    const textarea = document.createElement('textarea');
    textarea.value = text;
    textarea.style.position = 'fixed';
    textarea.style.opacity = '0';
    document.body.appendChild(textarea);
    textarea.select();

    try {
        document.execCommand('copy');
        showNotification('Profile link copied to clipboard!');
    } catch (err) {
        console.error('Copy failed:', err);
    }

    document.body.removeChild(textarea);
}


function showNotification(message) {
    const notification = document.createElement('div');
    notification.className = 'profile-notification';
    notification.textContent = message;
    notification.style.cssText = `
        position: fixed;
        bottom: 30px;
        left: 50%;
        transform: translateX(-50%);
        background: #262626;
        color: white;
        padding: 12px 24px;
        border-radius: 8px;
        font-size: 14px;
        z-index: 10000;
        animation: slideUp 0.3s ease-out;
    `;

    document.body.appendChild(notification);

    setTimeout(() => {
        notification.style.animation = 'slideDown 0.3s ease-out';
        setTimeout(() => {
            document.body.removeChild(notification);
        }, 300);
    }, 2500);
}


let postsPage = 1;
let isLoadingPosts = false;
let hasMorePosts = true;


function triggerFileInput() {
    document.getElementById('avatarInput').click();
}

function uploadAvatar(inputElement) {
    const file = inputElement.files[0];
    if (!file) return;

    if (!file.type.startsWith('image/')) {
        alert("Please select an image file.");
        return;
    }

    const formData = new FormData();
    formData.append('avatarFile', file);


    fetch('/Profile/UpdateAvatar', {
        method: 'POST',
        body: formData
    })
        .then(res => {
            if (res.ok) return res.json();
            throw new Error("Upload failed");
        })
        .then(data => {
            if (data.newUrl) {
                document.getElementById('avatarImage').src = data.newUrl;
            }
            alert("Avatar updated successfully!");
        })
        .catch(err => {
            console.error(err);
            alert("Failed to update avatar.");
        });
}
function openFollowModal(type, userId) {
    const modalTitle = document.getElementById('followModalTitle');
    const listContainer = document.getElementById('followListContainer');

    // 1. Set Title & Reset List
    if (type === 'followers') {
        modalTitle.innerText = "Followers";
    } else {
        modalTitle.innerText = "Following";
    }
    listContainer.innerHTML = '<div class="text-center py-5"><div class="spinner-border spinner-border-sm text-muted"></div></div>';

    // 2. Hiển thị Modal
    const myModal = new bootstrap.Modal(document.getElementById('followModal'));
    myModal.show();

    let url = '';
    if (type === 'followers') {
        url = `/Follow/FollowerList?requesterId=${userId}`;
    } else {
        url = `/Follow/FolloweeList?requesterId=${userId}`;
    }
    // 4. Gọi API
    fetch(url)
        .then(res => res.json())
        .then(data => {
            if (!data || data.length === 0) {
                listContainer.innerHTML = '<div class="text-center py-4 text-muted small">No users found.</div>';
                return;
            }

            let html = '';
            data.forEach(item => {
                // item.user là object chứa thông tin user (xem lại FollowViewModel của bạn)
                const user = item.user;

                html += `
                            <div class="d-flex align-items-center px-3 py-2 hover-bg-light">
                                <a href="/Profile/UserProfile/${user.userId}">
                                    <img src="${user.avatarUrl || '/images/default-avatar.png'}"
                                         class="rounded-circle border me-3"
                                         width="44" height="44" style="object-fit: cover;">
                                </a>

                                <div class="flex-grow-1" style="min-width: 0;">
                                    <a href="/Profile/UserProfile/${user.userId}" class="text-decoration-none text-dark fw-bold d-block text-truncate">
                                        ${user.userName}
                                    </a>
                                    <div class="text-muted small text-truncate">${user.fullName || ''}</div>
                                </div>

                                <button class="btn btn-light btn-sm border fw-bold px-3">Remove</button>
                            </div>
                        `;
            });
            listContainer.innerHTML = html;
        })
        .catch(err => {
            console.error(err);
            listContainer.innerHTML = '<div class="text-center py-4 text-danger small">Failed to load.</div>';
        });
}

function appendPosts(posts) {
    const grid = document.querySelector('.profile-posts-grid');
    if (!grid) return;

    posts.forEach(post => {
        const postItem = createPostGridItem(post);
        grid.appendChild(postItem);
    });
}

function createPostGridItem(post) {
    const item = document.createElement('div');
    item.className = 'post-grid-item';
    item.dataset.postId = post.postId;
    item.innerHTML = `
        <img src="${post.mediaUrl}" alt="Post" />
        <div class="post-grid-overlay">
            <div class="post-grid-stat">
                <i class="bi bi-heart-fill"></i>
                <span>${post.likeCount}</span>
            </div>
            <div class="post-grid-stat">
                <i class="bi bi-chat-fill"></i>
                <span>${post.commentCount}</span>
            </div>
        </div>
    `;

    item.addEventListener('click', function () {
        navigateToPost(post.postId);
    });

    return item;
}


const styleSheet = document.createElement('style');
styleSheet.textContent = `
    @keyframes slideUp {
        from {
            opacity: 0;
            transform: translate(-50%, 20px);
        }
        to {
            opacity: 1;
            transform: translate(-50%, 0);
        }
    }
    
    @keyframes slideDown {
        from {
            opacity: 1;
            transform: translate(-50%, 0);
        }
        to {
            opacity: 0;
            transform: translate(-50%, 20px);
        }
    }
    
    @keyframes pulse {
        0%, 100% { transform: scale(1); }
        50% { transform: scale(1.1); }
    }
`;
document.head.appendChild(styleSheet);