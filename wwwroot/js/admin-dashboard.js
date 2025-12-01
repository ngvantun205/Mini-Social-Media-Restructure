// ================= GLOBAL VARIABLES =================
let deleteCallback = null;
let deleteType = '';

// ================= TAB MANAGEMENT =================
function showTab(tabId) {
    // Remove active class from all tabs
    document.querySelectorAll('.tab-content').forEach(tab => {
        tab.classList.remove('active');
    });
    document.querySelectorAll('.tab-btn').forEach(btn => {
        btn.classList.remove('active');
    });

    // Add active class to selected tab
    document.getElementById(tabId).classList.add('active');
    event.target.closest('.tab-btn').classList.add('active');
}

// ================= USERS MANAGEMENT =================
async function loadUsers() {
    try {
        const tbody = document.querySelector("#usersTable tbody");
        tbody.innerHTML = '<tr><td colspan="5" class="loading">Loading users...</td></tr>';

        const res = await fetch('/Admin/Users');
        const users = await res.json();

        tbody.innerHTML = "";

        if (!users || users.length === 0) {
            tbody.innerHTML = '<tr><td colspan="5" style="text-align: center; padding: 2rem; color: var(--text-secondary);">No users found</td></tr>';
            return;
        }

        users.forEach(u => {
            const row = document.createElement('tr');
            row.style.animation = 'fadeInUp 0.5s ease';
            row.innerHTML = `
                <td><a href="/Profile/UserProfile/${u.userId}" target="_blank">#${u.userId}</a></td>
                <td><img src="${u.avatarUrl || '/images/default-avatar.png'}" class="avatar-img" alt="${u.fullName}"></td>
                <td><a href="/Profile/UserProfile/${u.userId}" target="_blank">${u.fullName || 'N/A'}</a></td>
                <td><a href="/Profile/UserProfile/${u.userId}" target="_blank">@${u.userName}</a></td>
                <td>
                    <button class="btn btn-danger btn-sm" onclick="showDeleteModal('user', ${u.userId}, '${u.userName}')">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <polyline points="3 6 5 6 21 6"></polyline>
                            <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path>
                        </svg>
                        Delete
                    </button>
                </td>
            `;
            tbody.appendChild(row);
        });
    } catch (error) {
        console.error('Error loading users:', error);
        showErrorMessage('Failed to load users');
    }
}

async function searchUser() {
    const searchInput = document.getElementById("searchUserInput");
    const searchTerm = searchInput.value.trim();

    if (!searchTerm) {
        loadUsers();
        return;
    }

    try {
        const tbody = document.querySelector("#usersTable tbody");
        tbody.innerHTML = '<tr><td colspan="5" class="loading">Searching...</td></tr>';

        const res = await fetch(`/Admin/SearchUser?searchinfo=${encodeURIComponent(searchTerm)}`);
        const users = await res.json();

        tbody.innerHTML = "";

        if (!users || users.length === 0) {
            tbody.innerHTML = '<tr><td colspan="5" style="text-align: center; padding: 2rem; color: var(--text-secondary);">No users found matching your search</td></tr>';
            return;
        }

        users.forEach(u => {
            const row = document.createElement('tr');
            row.style.animation = 'fadeInUp 0.5s ease';
            row.innerHTML = `
                <td><a href="/Profile/UserProfile/${u.userId}" target="_blank">#${u.userId}</a></td>
                <td><img src="${u.avatarUrl || '/images/default-avatar.png'}" class="avatar-img" alt="${u.fullName}"></td>
                <td><a href="/Profile/UserProfile/${u.userId}" target="_blank">${u.fullName || 'N/A'}</a></td>
                <td><a href="/Profile/UserProfile/${u.userId}" target="_blank">@${u.userName}</a></td>
                <td>
                    <button class="btn btn-danger btn-sm" onclick="showDeleteModal('user', ${u.userId}, '${u.userName}')">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <polyline points="3 6 5 6 21 6"></polyline>
                            <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path>
                        </svg>
                        Delete
                    </button>
                </td>
            `;
            tbody.appendChild(row);
        });
    } catch (error) {
        console.error('Error searching users:', error);
        showErrorMessage('Failed to search users');
    }
}

async function deleteUser(id) {
    try {
        await fetch("/Admin/DeleteUser", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(id)
        });
        loadUsers();
        showSuccessMessage('User deleted successfully');
    } catch (error) {
        console.error('Error deleting user:', error);
        showErrorMessage('Failed to delete user');
    }
}

// ================= POSTS MANAGEMENT =================
async function loadPosts() {
    try {
        const tbody = document.querySelector("#postsTable tbody");
        tbody.innerHTML = '<tr><td colspan="6" class="loading">Loading posts...</td></tr>';

        const res = await fetch('/Admin/Posts');
        const posts = await res.json();

        tbody.innerHTML = "";

        if (!posts || posts.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" style="text-align: center; padding: 2rem; color: var(--text-secondary);">No posts found</td></tr>';
            return;
        }

        posts.forEach(p => {
            const row = document.createElement('tr');
            row.style.animation = 'fadeInUp 0.5s ease';
            row.innerHTML = `
                <td><a href="/Post/PostDetails/${p.postId}" target="_blank">#${p.postId}</a></td>
                <td><a href="/Profile/UserProfile/${p.userId}" target="_blank">#${p.userId}</a></td>
                <td>${p.likeCount || 0}</td>
                <td>${p.commentCount || 0}</td>
                <td>${p.mediaUrl ? `<a href="${p.mediaUrl}" target="_blank">View Media</a>` : 'No media'}</td>
                <td>
                    <button class="btn btn-danger btn-sm" onclick="showDeleteModal('post', ${p.postId}, 'Post #${p.postId}')">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <polyline points="3 6 5 6 21 6"></polyline>
                            <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path>
                        </svg>
                        Delete
                    </button>
                </td>
            `;
            tbody.appendChild(row);
        });
    } catch (error) {
        console.error('Error loading posts:', error);
        showErrorMessage('Failed to load posts');
    }
}

async function searchPost() {
    const searchInput = document.getElementById("searchPostInput");
    const searchTerm = searchInput.value.trim();

    if (!searchTerm) {
        loadPosts();
        return;
    }

    try {
        const tbody = document.querySelector("#postsTable tbody");
        tbody.innerHTML = '<tr><td colspan="6" class="loading">Searching...</td></tr>';

        const res = await fetch(`/Admin/SearchPost?searchinfo=${encodeURIComponent(searchTerm)}`);
        const posts = await res.json();

        tbody.innerHTML = "";

        if (!posts || posts.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" style="text-align: center; padding: 2rem; color: var(--text-secondary);">No posts found matching your search</td></tr>';
            return;
        }

        posts.forEach(p => {
            const row = document.createElement('tr');
            row.style.animation = 'fadeInUp 0.5s ease';
            row.innerHTML = `
                <td><a href="/Post/PostDetails/${p.postId}" target="_blank">#${p.postId}</a></td>
                <td><a href="/Profile/UserProfile/${p.userId}" target="_blank">#${p.userId}</a></td>
                <td>${p.likeCount || 0}</td>
                <td>${p.commentCount || 0}</td>
                <td>${p.mediaUrl ? `<a href="${p.mediaUrl}" target="_blank">View Media</a>` : 'No media'}</td>
                <td>
                    <button class="btn btn-danger btn-sm" onclick="showDeleteModal('post', ${p.postId}, 'Post #${p.postId}')">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <polyline points="3 6 5 6 21 6"></polyline>
                            <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path>
                        </svg>
                        Delete
                    </button>
                </td>
            `;
            tbody.appendChild(row);
        });
    } catch (error) {
        console.error('Error searching posts:', error);
        showErrorMessage('Failed to search posts');
    }
}

async function deletePost(id) {
    try {
        await fetch("/Admin/DeletePost", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(id)
        });
        loadPosts();
        showSuccessMessage('Post deleted successfully');
    } catch (error) {
        console.error('Error deleting post:', error);
        showErrorMessage('Failed to delete post');
    }
}

// ================= REPORTS MANAGEMENT =================
async function loadReports() {
    try {
        const tbody = document.querySelector("#reportsTable tbody");
        tbody.innerHTML = '<tr><td colspan="8" class="loading">Loading reports...</td></tr>';

        const res = await fetch('/Admin/Reports');
        const reports = await res.json();

        renderReports(reports);
    } catch (error) {
        console.error('Error loading reports:', error);
        showErrorMessage('Failed to load reports');
    }
}

async function filterReport() {
    const status = document.getElementById("reportFilter").value;

    try {
        const tbody = document.querySelector("#reportsTable tbody");
        tbody.innerHTML = '<tr><td colspan="8" class="loading">Loading reports...</td></tr>';

        const res = await fetch(`/Admin/GetReportsByStatus?status=${status}`);
        const reports = await res.json();
        renderReports(reports);
    } catch (error) {
        console.error('Error filtering reports:', error);
        showErrorMessage('Failed to filter reports');
    }
}

function renderReports(reports) {
    const tbody = document.querySelector("#reportsTable tbody");
    tbody.innerHTML = "";

    if (!reports || reports.length === 0) {
        tbody.innerHTML = '<tr><td colspan="8" style="text-align: center; padding: 2rem; color: var(--text-secondary);">No reports found</td></tr>';
        return;
    }

    reports.forEach(r => {
        const row = document.createElement('tr');
        row.style.animation = 'fadeInUp 0.5s ease';

        const entityLink = r.type === 'Post'
            ? `/Post/PostDetails/${r.entityId}`
            : `/Profile/UserProfile/${r.entityId}`;

        row.innerHTML = `
            <td>#${r.reportId}</td>
            <td><a href="/Profile/UserProfile/${r.reporterId}" target="_blank">#${r.reporterId}</a></td>
            <td><span class="badge ${r.type === 'Post' ? 'badge-pending' : 'badge-executed'}">${r.type}</span></td>
            <td><a href="${entityLink}" target="_blank">#${r.entityId}</a></td>
            <td>${r.content || 'No content'}</td>
            <td>${new Date(r.createdAt).toLocaleDateString()}</td>
            <td><span class="badge ${r.isExecuted ? 'badge-executed' : 'badge-pending'}">${r.isExecuted ? 'Executed' : 'Pending'}</span></td>
            <td>
                ${!r.isExecuted ? `
                    <button class="btn btn-success btn-sm" onclick="showExecuteModal(${r.reportId})">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <polyline points="20 6 9 17 4 12"></polyline>
                        </svg>
                        Execute
                    </button>
                ` : '<span style="color: var(--text-secondary);">Completed</span>'}
            </td>
        `;
        tbody.appendChild(row);
    });
}

async function executeReport(id) {
    try {
        await fetch("/Admin/ExecuteReport", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(id)
        });
        filterReport();
        showSuccessMessage('Report executed successfully');
    } catch (error) {
        console.error('Error executing report:', error);
        showErrorMessage('Failed to execute report');
    }
}

// ================= MODAL MANAGEMENT =================
function showDeleteModal(type, id, name) {
    deleteType = type;
    deleteCallback = () => {
        if (type === 'user') {
            deleteUser(id);
        } else if (type === 'post') {
            deletePost(id);
        }
    };

    const message = document.getElementById('deleteMessage');
    message.textContent = `Are you sure you want to delete ${name}? This action cannot be undone.`;

    const modal = document.getElementById('deleteModal');
    modal.classList.add('show');
}

function showExecuteModal(reportId) {
    deleteCallback = () => executeReport(reportId);

    const message = document.getElementById('deleteMessage');
    message.textContent = `Are you sure you want to execute this report? This will take action on the reported content.`;

    const modal = document.getElementById('deleteModal');
    modal.classList.add('show');
}

function closeDeleteModal() {
    const modal = document.getElementById('deleteModal');
    modal.classList.remove('show');
    deleteCallback = null;
    deleteType = '';
}

function confirmDelete() {
    if (deleteCallback) {
        deleteCallback();
    }
    closeDeleteModal();
}

// Close modal when clicking outside
document.addEventListener('click', (e) => {
    const modal = document.getElementById('deleteModal');
    if (e.target === modal) {
        closeDeleteModal();
    }
});

// ================= NOTIFICATION MESSAGES =================
function showSuccessMessage(message) {
    showNotification(message, 'success');
}

function showErrorMessage(message) {
    showNotification(message, 'error');
}

function showNotification(message, type) {
    const notification = document.createElement('div');
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 1rem 1.5rem;
        background: ${type === 'success' ? 'var(--success-color)' : 'var(--danger-color)'};
        color: white;
        border-radius: var(--radius);
        box-shadow: var(--shadow-lg);
        z-index: 9999;
        animation: slideInRight 0.3s ease;
    `;
    notification.textContent = message;

    document.body.appendChild(notification);

    setTimeout(() => {
        notification.style.animation = 'fadeOut 0.3s ease';
        setTimeout(() => notification.remove(), 300);
    }, 3000);
}

// ================= KEYBOARD SHORTCUTS =================
document.addEventListener('keydown', (e) => {
    // ESC to close modal
    if (e.key === 'Escape') {
        closeDeleteModal();
    }

    // Enter to search
    if (e.key === 'Enter') {
        const activeTab = document.querySelector('.tab-content.active');
        if (activeTab) {
            if (activeTab.id === 'usersTab' && document.activeElement.id === 'searchUserInput') {
                searchUser();
            } else if (activeTab.id === 'postsTab' && document.activeElement.id === 'searchPostInput') {
                searchPost();
            }
        }
    }
});

// ================= INITIALIZATION =================
window.onload = () => {
    loadUsers();
};