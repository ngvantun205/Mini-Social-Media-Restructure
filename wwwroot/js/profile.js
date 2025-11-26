/* ============================================
   PROFILE - ENHANCED JAVASCRIPT
   ============================================ */

// ============================================
// INITIALIZATION
// ============================================

document.addEventListener('DOMContentLoaded', function () {
    initProfileTabs();
    initSettingsDropdown();
    initPostGridInteractions();
});

// ============================================
// TABS FUNCTIONALITY
// ============================================

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

// ============================================
// SETTINGS DROPDOWN
// ============================================

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

function toggleSettingsDropdown(button) {
    const dropdown = button.nextElementSibling;

    if (!dropdown) return;

    const isOpen = dropdown.classList.contains('show');

    closeSettingsDropdown();

    if (!isOpen) {
        dropdown.classList.add('show');
        settingsDropdown = dropdown;
    }
}

function closeSettingsDropdown() {
    if (settingsDropdown) {
        settingsDropdown.classList.remove('show');
        settingsDropdown = null;
    }
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

function toggleFollow(button, userId) {
    const icon = button.querySelector('i');
    const text = button.querySelector('.btn-text');
    const isFollowing = button.classList.contains('following');

    // Disable button during request
    button.disabled = true;

    fetch('/Follow/Toggle', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ userId: userId })
    })
        .then(res => {
            if (!res.ok) throw new Error("Failed to toggle follow");
            return res.json();
        })
        .then(data => {
            if (isFollowing) {
                // Unfollow
                button.classList.remove('following', 'btn-profile-danger');
                button.classList.add('btn-profile-primary');
                if (icon) icon.className = 'bi bi-person-plus';
                if (text) text.textContent = 'Follow';

                // Update follower count
                updateFollowerCount(-1);
            } else {
                // Follow
                button.classList.add('following', 'btn-profile-danger');
                button.classList.remove('btn-profile-primary');
                if (icon) icon.className = 'bi bi-person-check';
                if (text) text.textContent = 'Following';

                // Update follower count
                updateFollowerCount(1);
            }
        })
        .catch(err => {
            console.error("Follow toggle failed:", err);
            alert("Failed to update follow status. Please try again.");
        })
        .finally(() => {
            button.disabled = false;
        });
}

function updateFollowerCount(change) {
    const followerElement = document.querySelector('.follower-count');
    if (followerElement) {
        let currentCount = parseInt(followerElement.textContent) || 0;
        currentCount += change;
        followerElement.textContent = currentCount;

        // Add animation
        followerElement.style.animation = 'pulse 0.4s ease-out';
        setTimeout(() => {
            followerElement.style.animation = '';
        }, 400);
    }
}

// ============================================
// AVATAR UPLOAD (FOR EDIT PROFILE PAGE)
// ============================================

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
        // Fallback: Copy to clipboard
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

// ============================================
// NOTIFICATIONS
// ============================================

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

// ============================================
// LOAD MORE POSTS (IF NEEDED)
// ============================================

let postsPage = 1;
let isLoadingPosts = false;
let hasMorePosts = true;

function initInfiniteScrollPosts() {
    window.addEventListener('scroll', function () {
        if (isLoadingPosts || !hasMorePosts) return;

        const scrollPosition = window.innerHeight + window.scrollY;
        const threshold = document.body.offsetHeight - 500;

        if (scrollPosition >= threshold) {
            loadMorePosts();
        }
    });
}

function loadMorePosts() {
    isLoadingPosts = true;
    postsPage++;

    const userId = document.querySelector('.profile-page-container').dataset.userId;

    fetch(`/Profile/LoadMorePosts?userId=${userId}&page=${postsPage}`)
        .then(res => {
            if (res.status === 204) {
                hasMorePosts = false;
                return null;
            }
            return res.json();
        })
        .then(posts => {
            if (posts && posts.length > 0) {
                appendPosts(posts);
            }
        })
        .catch(err => console.error("Failed to load more posts:", err))
        .finally(() => {
            isLoadingPosts = false;
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

// ============================================
// ANIMATION HELPERS
// ============================================

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