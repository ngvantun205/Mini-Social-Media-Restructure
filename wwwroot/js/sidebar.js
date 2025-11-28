// ==================== SIDEBAR & NOTIFICATION MANAGEMENT ====================

document.addEventListener("DOMContentLoaded", function () {
    initializeSidebar();
});

// ==================== INITIALIZATION ====================
function initializeSidebar() {
    const notiBtn = document.getElementById("notiBtn");
    const notiDrawer = document.getElementById("notiDrawer");
    const notiList = document.getElementById("notiList");
    const notiBadge = document.getElementById("notiBadge");

    if (!notiBtn || !notiDrawer) {
        console.warn("Notification elements not found");
        return;
    }

    // Setup event listeners
    setupNotificationToggle(notiBtn, notiDrawer);
    setupClickOutside(notiDrawer, notiBtn);
    setupKeyboardShortcuts(notiDrawer);
    setupVisibilityChange(notiDrawer);
}

// ==================== NOTIFICATION DRAWER TOGGLE ====================
function setupNotificationToggle(notiBtn, notiDrawer) {
    notiBtn.addEventListener("click", function (e) {
        e.preventDefault();
        e.stopPropagation();

        if (notiDrawer.classList.contains("show")) {
            closeDrawer(notiDrawer, notiBtn);
        } else {
            openDrawer(notiDrawer, notiBtn);
        }
    });
}

function openDrawer(notiDrawer, notiBtn) {
    notiDrawer.classList.add("open");
    setTimeout(() => notiDrawer.classList.add("show"), 10);
    notiBtn.classList.add("active-icon");
    loadNotifications();
}

function closeDrawer(notiDrawer, notiBtn) {
    notiDrawer.classList.remove("show");
    notiBtn.classList.remove("active-icon");
    setTimeout(() => notiDrawer.classList.remove("open"), 400);
}

// ==================== LOAD NOTIFICATIONS ====================
function loadNotifications() {
    const notiList = document.getElementById("notiList");
    const notiBadge = document.getElementById("notiBadge");

    fetch('/Notifications/GetNotifications')
        .then(res => {
            if (!res.ok) throw new Error('Network response was not ok');
            return res.json();
        })
        .then(data => {
            if (!data || data.length === 0) {
                renderEmptyState(notiList);
                return;
            }

            renderNotifications(data, notiList, notiBadge);
        })
        .catch(err => {
            console.error('Error loading notifications:', err);
            renderErrorState(notiList);
        });
}

function renderEmptyState(notiList) {
    notiList.innerHTML = `
        <div class="text-center mt-5 text-muted">
            <i class="bi bi-bell-slash" style="font-size: 48px; color: #e4e6eb;"></i>
            <div class="mt-3" style="font-size: 14px;">No notifications yet</div>
        </div>
    `;
}

function renderNotifications(data, notiList, notiBadge) {
    let html = '';
    let anyUnread = false;

    data.forEach(noti => {
        if (!noti.isRead) anyUnread = true;

        const isUnreadClass = noti.isRead ? '' : 'unread';
        const dotHtml = !noti.isRead ? '<div class="unread-dot"></div>' : '';
        const actorAvatar = noti.actorAvatar || '/images/avatar.png';

        html += `
            <div class="noti-item ${isUnreadClass}" onclick="handleNotificationClick('${noti.postId}', event)">
                <img src="${actorAvatar}" 
                     class="rounded-circle me-3 border" 
                     width="44" 
                     height="44" 
                     style="object-fit: cover;"
                     alt="${escapeHtml(noti.actorName)}"
                     onerror="this.src='/images/avatar.png'">
                <div class="flex-grow-1" style="font-size: 14px; line-height: 1.5; padding-right: 8px;">
                    <span class="fw-bold text-dark">${escapeHtml(noti.actorName)}</span>
                    <span class="text-dark"> ${escapeHtml(noti.message)}</span>
                    <div class="text-muted" style="font-size: 12px; margin-top: 4px;">
                        <i class="bi bi-clock me-1"></i>${noti.timeAgo}
                    </div>
                </div>
                ${dotHtml}
            </div>
        `;
    });

    notiList.innerHTML = html;

    // Update badge
    if (notiBadge) {
        notiBadge.style.display = anyUnread ? "block" : "none";
    }
}

function renderErrorState(notiList) {
    notiList.innerHTML = `
        <div class="text-center mt-5 text-danger">
            <i class="bi bi-exclamation-triangle" style="font-size: 48px;"></i>
            <div class="mt-3" style="font-size: 14px;">Failed to load notifications</div>
            <button class="btn btn-sm btn-outline-primary mt-3" onclick="loadNotifications()">
                <i class="bi bi-arrow-clockwise me-1"></i>Retry
            </button>
        </div>
    `;
}

// ==================== NOTIFICATION CLICK HANDLER ====================
window.handleNotificationClick = function (postId, event) {
    if (event) event.stopPropagation();
    window.location.href = `/Post/PostDetails?id=${postId}`;
};

// ==================== MARK ALL AS READ ====================
window.markAllRead = function () {
    const notiBadge = document.getElementById("notiBadge");

    fetch('/Notifications/MarkAllRead', { method: 'POST' })
        .then(res => {
            if (!res.ok) throw new Error('Failed to mark as read');
            return res;
        })
        .then(() => {
            // Hide badge
            if (notiBadge) {
                notiBadge.style.display = "none";
            }

            // Remove unread styling from all items
            const unreadItems = document.querySelectorAll('.noti-item.unread');
            unreadItems.forEach(item => {
                // Remove unread class with animation
                item.style.transition = 'all 0.3s ease';
                item.classList.remove('unread');

                // Remove unread dot with fade out
                const dot = item.querySelector('.unread-dot');
                if (dot) {
                    dot.style.animation = 'none';
                    dot.style.opacity = '0';
                    setTimeout(() => dot.remove(), 300);
                }
            });

            // Show success feedback
            showToast('All notifications marked as read', 'success');
        })
        .catch(err => {
            console.error("Mark all read failed:", err);
            showToast('Failed to mark notifications as read', 'error');
        });
};

// ==================== CLICK OUTSIDE TO CLOSE ====================
function setupClickOutside(notiDrawer, notiBtn) {
    document.addEventListener("click", function (e) {
        if (notiDrawer.classList.contains("show") &&
            !notiDrawer.contains(e.target) &&
            !notiBtn.contains(e.target)) {
            closeDrawer(notiDrawer, notiBtn);
        }
    });
}

// ==================== KEYBOARD SHORTCUTS ====================
function setupKeyboardShortcuts(notiDrawer) {
    document.addEventListener('keydown', function (e) {
        // ESC to close drawer
        if (e.key === 'Escape' && notiDrawer.classList.contains('show')) {
            const notiBtn = document.getElementById("notiBtn");
            closeDrawer(notiDrawer, notiBtn);
        }
    });
}

// ==================== VISIBILITY CHANGE ====================
function setupVisibilityChange(notiDrawer) {
    document.addEventListener('visibilitychange', function () {
        if (!document.hidden && notiDrawer.classList.contains('show')) {
            loadNotifications();
        }
    });
}

// ==================== TOAST NOTIFICATION ====================
function showToast(message, type = 'info') {
    const toast = document.createElement('div');
    toast.className = 'toast-notification';

    const iconMap = {
        'success': { icon: 'check-circle-fill', color: '#31a24c' },
        'error': { icon: 'exclamation-circle-fill', color: '#e74c3c' },
        'info': { icon: 'info-circle-fill', color: '#667eea' }
    };

    const iconData = iconMap[type] || iconMap.info;

    toast.innerHTML = `
        <i class="bi bi-${iconData.icon} me-2" 
           style="font-size: 20px; color: ${iconData.color};"></i>
        <span>${message}</span>
    `;

    let container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        document.body.appendChild(container);
    }

    container.appendChild(toast);

    // Auto remove after 3 seconds
    setTimeout(() => {
        toast.style.opacity = '0';
        toast.style.transform = 'translateY(20px)';
        setTimeout(() => toast.remove(), 400);
    }, 3000);
}

// ==================== UTILITY FUNCTIONS ====================
function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// ==================== EXPORT FOR EXTERNAL USE ====================
window.SidebarManager = {
    loadNotifications,
    openDrawer,
    closeDrawer,
    showToast,
    escapeHtml
};