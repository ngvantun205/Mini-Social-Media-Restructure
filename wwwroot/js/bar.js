document.addEventListener('DOMContentLoaded', function () {

    // ==================== USER DROPDOWN ====================
    const userAvatar = document.querySelector('.user-avatar-top');
    const dropdownMenu = document.querySelector('.dropdown-menu');

    if (userAvatar && dropdownMenu) {
        userAvatar.addEventListener('click', function (e) {
            e.stopPropagation();
            dropdownMenu.classList.toggle('show');
        });

        document.addEventListener('click', function (e) {
            if (!dropdownMenu.contains(e.target) && e.target !== userAvatar) {
                dropdownMenu.classList.remove('show');
            }
        });
    }

    // ==================== SEARCH FUNCTIONALITY ====================
    const searchInput = document.querySelector('.search-input');

    if (searchInput) {
        let searchTimeout;

        searchInput.addEventListener('input', function (e) {
            clearTimeout(searchTimeout);

            searchTimeout = setTimeout(() => {
                const query = e.target.value.trim();

                if (query.length >= 2) {
                    performSearch(query);
                }
            }, 500);
        });

        searchInput.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                const query = e.target.value.trim();
                if (query.length >= 2) {
                    performSearch(query);
                }
            }
        });
    }

    function performSearch(query) {
        console.log('Searching for:', query);
    }

    const followButtons = document.querySelectorAll('.follow-btn');

    followButtons.forEach(button => {
        button.addEventListener('click', function (e) {
            e.preventDefault();
            const userId = this.getAttribute('data-user-id');

            if (this.textContent === 'Follow') {
                followUser(userId, this);
            } else {
                unfollowUser(userId, this);
            }
        });
    });

    function followUser(userId, button) {
        fetch(`/api/follow/${userId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            }
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    button.textContent = 'Following';
                    button.style.background = '#e4e6eb';
                    button.style.color = '#050505';
                    showNotification('Started following user');
                }
            })
            .catch(error => {
                console.error('Follow error:', error);
                showNotification('Failed to follow user', 'error');
            });
    }

    function unfollowUser(userId, button) {
        fetch(`/api/unfollow/${userId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            }
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    button.textContent = 'Follow';
                    button.style.background = '#667eea';
                    button.style.color = 'white';
                    showNotification('Unfollowed user');
                }
            })
            .catch(error => {
                console.error('Unfollow error:', error);
                showNotification('Failed to unfollow user', 'error');
            });
    }

    // ==================== TOP BAR ICONS ====================
    const homeIcon = document.querySelector('.top-icon[data-page="home"]');
    const messagesIcon = document.querySelector('.top-icon[data-page="messages"]');
    const notificationsIcon = document.querySelector('.top-icon[data-page="notifications"]');

    if (homeIcon) {
        homeIcon.addEventListener('click', function () {
            window.location.href = '/';
        });
    }

    if (messagesIcon) {
        messagesIcon.addEventListener('click', function () {
            window.location.href = '/messages';
        });
    }

    if (notificationsIcon) {
        notificationsIcon.addEventListener('click', function () {
            toggleNotificationDrawer();
        });
    }

    // ==================== HASHTAG CLICK ====================
    const hashtagItems = document.querySelectorAll('.hashtag-item');

    hashtagItems.forEach(item => {
        item.addEventListener('click', function () {
            const hashtag = this.querySelector('.hashtag-name').textContent;
            window.location.href = `/explore/tags/${hashtag.replace('#', '')}`;
        });
    });

    // ==================== TRENDING REEL CLICK ====================
    const trendingReels = document.querySelectorAll('.trending-reel');

    trendingReels.forEach(reel => {
        reel.addEventListener('click', function () {
            const reelId = this.getAttribute('data-reel-id');
            window.location.href = `/reels/${reelId}`;
        });
    });

    // ==================== SUGGESTED USER CLICK ====================
    const suggestedUsers = document.querySelectorAll('.suggested-user-name');

    suggestedUsers.forEach(user => {
        user.addEventListener('click', function () {
            const username = this.getAttribute('data-username');
            window.location.href = `/profile/${username}`;
        });
    });

    const suggestedAvatars = document.querySelectorAll('.suggested-user-avatar');

    suggestedAvatars.forEach(avatar => {
        avatar.addEventListener('click', function () {
            const username = this.getAttribute('data-username');
            window.location.href = `/profile/${username}`;
        });
    });

    // ==================== NOTIFICATION BADGE UPDATE ====================
    function updateNotificationBadge(count) {
        const badge = document.querySelector('.top-icon[data-page="notifications"] .badge');

        if (badge) {
            if (count > 0) {
                badge.textContent = count > 99 ? '99+' : count;
                badge.style.display = 'flex';
            } else {
                badge.style.display = 'none';
            }
        }
    }

    // ==================== MESSAGE BADGE UPDATE ====================
    function updateMessageBadge(count) {
        const badge = document.querySelector('.top-icon[data-page="messages"] .badge');

        if (badge) {
            if (count > 0) {
                badge.textContent = count > 99 ? '99+' : count;
                badge.style.display = 'flex';
            } else {
                badge.style.display = 'none';
            }
        }
    }

    // ==================== REAL-TIME UPDATES ====================
    function initializeRealTimeUpdates() {
        // Example using SignalR or WebSocket
        /*
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/hubs/notification")
            .build();

        connection.on("ReceiveNotification", function(notification) {
            updateNotificationBadge(notification.unreadCount);
            showNotification(notification.message);
        });

        connection.on("ReceiveMessage", function(message) {
            updateMessageBadge(message.unreadCount);
        });

        connection.start().catch(err => console.error(err));
        */

        // Or polling approach
        setInterval(() => {
            fetchUnreadCounts();
        }, 30000); // Every 30 seconds
    }

    function fetchUnreadCounts() {
        fetch('/api/unread-counts')
            .then(response => response.json())
            .then(data => {
                updateNotificationBadge(data.notifications);
                updateMessageBadge(data.messages);
            })
            .catch(error => {
                console.error('Error fetching unread counts:', error);
            });
    }

    // ==================== NOTIFICATION TOAST ====================
    function showNotification(message, type = 'success') {
        const container = document.getElementById('toast-container');

        if (!container) {
            const newContainer = document.createElement('div');
            newContainer.id = 'toast-container';
            document.body.appendChild(newContainer);
        }

        const toast = document.createElement('div');
        toast.className = 'toast-notification';
        toast.innerHTML = `
            <i class="fas ${type === 'success' ? 'fa-check-circle' : 'fa-exclamation-circle'}" style="color: ${type === 'success' ? '#667eea' : '#e74c3c'}; margin-right: 12px; font-size: 20px;"></i>
            <span>${message}</span>
        `;

        document.getElementById('toast-container').appendChild(toast);

        setTimeout(() => {
            toast.style.opacity = '0';
            toast.style.transform = 'translateY(20px)';
            setTimeout(() => toast.remove(), 300);
        }, 3000);

        toast.addEventListener('click', () => {
            toast.style.opacity = '0';
            toast.style.transform = 'translateY(20px)';
            setTimeout(() => toast.remove(), 300);
        });
    }

    // ==================== SMOOTH SCROLL ====================
    const rightSidebar = document.querySelector('.right-sidebar');

    if (rightSidebar) {
        rightSidebar.addEventListener('wheel', function (e) {
            e.stopPropagation();
        });
    }

    // ==================== LOAD MORE SUGGESTED USERS ====================
    const seeAllSuggested = document.querySelector('.see-all[href*="suggestions"]');

    if (seeAllSuggested) {
        seeAllSuggested.addEventListener('click', function (e) {
            e.preventDefault();
            loadMoreSuggestions();
        });
    }

    function loadMoreSuggestions() {
        fetch('/api/suggestions/more')
            .then(response => response.json())
            .then(data => {
                // Append new suggestions to the list
                const container = document.querySelector('.sidebar-section');
                data.users.forEach(user => {
                    const userElement = createSuggestedUserElement(user);
                    container.appendChild(userElement);
                });
            })
            .catch(error => {
                console.error('Error loading suggestions:', error);
            });
    }

    function createSuggestedUserElement(user) {
        const div = document.createElement('div');
        div.className = 'suggested-user';
        div.innerHTML = `
            <img src="${user.avatar}" alt="${user.name}" class="suggested-user-avatar" data-username="${user.username}">
            <div class="suggested-user-info">
                <p class="suggested-user-name" data-username="${user.username}">${user.name}</p>
                <p class="suggested-user-mutual">${user.mutualCount} mutual friends</p>
            </div>
            <button class="follow-btn" data-user-id="${user.id}">Follow</button>
        `;
        return div;
    }

    // ==================== INITIALIZE ====================
    initializeRealTimeUpdates();
    fetchUnreadCounts();

    // ==================== ACTIVE STATE FOR TOP BAR ICONS ====================
    const currentPage = window.location.pathname;
    const topIcons = document.querySelectorAll('.top-icon[data-page]');

    topIcons.forEach(icon => {
        const page = icon.getAttribute('data-page');
        if (currentPage.includes(page) || (page === 'home' && currentPage === '/')) {
            icon.style.background = 'rgba(102, 126, 234, 0.1)';
            icon.querySelector('i').style.color = '#667eea';
        }
    });
});