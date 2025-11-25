/* ============================================
   POST DETAIL - ENHANCED JAVASCRIPT
   ============================================ */

// Biến toàn cục
let currentUserId = 0;

// ============================================
// KHỞI TẠO
// ============================================

function initializePostDetail(userId) {
    currentUserId = userId;
    setupEventListeners();
}

function setupEventListeners() {
    const commentInput = document.getElementById("commentContent");
    if (commentInput) {
        commentInput.addEventListener("keypress", function (event) {
            if (event.key === "Enter") {
                event.preventDefault();
                submitAction();
            }
        });
    }

    // Đóng dropdown khi click bên ngoài
    document.addEventListener('click', function (event) {
        const dropdowns = document.querySelectorAll('.comment-dropdown');
        dropdowns.forEach(dropdown => {
            const menu = dropdown.querySelector('.dropdown-menu');
            const toggle = dropdown.querySelector('.dropdown-toggle-btn');

            if (menu && !dropdown.contains(event.target)) {
                menu.classList.remove('show');
            }
        });
    });

    // Xử lý click vào dropdown toggle
    document.addEventListener('click', function (event) {
        if (event.target.closest('.dropdown-toggle-btn')) {
            event.preventDefault();
            event.stopPropagation();

            const toggle = event.target.closest('.dropdown-toggle-btn');
            const dropdown = toggle.closest('.comment-dropdown');
            const menu = dropdown.querySelector('.dropdown-menu');

            // Đóng tất cả dropdown khác
            document.querySelectorAll('.dropdown-menu.show').forEach(otherMenu => {
                if (otherMenu !== menu) {
                    otherMenu.classList.remove('show');
                }
            });

            // Toggle dropdown hiện tại
            if (menu) {
                menu.classList.toggle('show');
            }
        }
    });
}

// ============================================
// QUẢN LÝ GIAO DIỆN
// ============================================

function toggleInputShape(isOpen) {
    const container = document.getElementById("inputContainer");
    if (!container) return;

    if (isOpen) {
        container.classList.add("has-preview");
        container.style.borderTopLeftRadius = "0";
        container.style.borderTopRightRadius = "0";
    } else {
        container.classList.remove("has-preview");
        container.style.borderTopLeftRadius = "24px";
        container.style.borderTopRightRadius = "24px";
    }
}

function showActionPreview(label, content) {
    const preview = document.getElementById("actionPreview");
    const labelEl = document.getElementById("actionLabel");
    const contentEl = document.getElementById("actionContentPreview");

    if (preview && labelEl && contentEl) {
        labelEl.innerText = label;
        contentEl.innerText = content;
        preview.style.display = "flex";
        toggleInputShape(true);
    }
}

function hideActionPreview() {
    const preview = document.getElementById("actionPreview");
    if (preview) {
        preview.style.display = "none";
        toggleInputShape(false);
    }
}

// ============================================
// XỬ LÝ REPLIES
// ============================================

function loadReplies(commentId) {
    const container = document.getElementById(`replies-container-${commentId}`);
    const btn = document.getElementById(`btn-view-replies-${commentId}`);

    if (!container) return;

    // Toggle hiển thị nếu đã load
    if (container.style.display === 'block') {
        container.style.display = 'none';
        if (btn) btn.innerHTML = `<i class="bi bi-arrow-return-right me-1"></i>View replies`;
        return;
    }

    // Hiển thị nếu đã có dữ liệu
    if (container.innerHTML.trim() !== "") {
        container.style.display = 'block';
        if (btn) btn.innerHTML = `<i class="bi bi-arrow-return-right me-1"></i>Hide replies`;
        return;
    }

    // Load dữ liệu từ server
    if (btn) {
        btn.innerHTML = `<i class="spinner-border spinner-border-sm me-1"></i>Loading...`;
    }

    fetch(`/Comment/GetReplies/${commentId}`)
        .then(res => {
            if (!res.ok) throw new Error("Failed to load replies");
            return res.json();
        })
        .then(replies => {
            if (replies && replies.length > 0) {
                let html = "";
                replies.forEach(reply => {
                    html += generateCommentHtml(reply, true);
                });
                container.innerHTML = html;
                container.style.display = 'block';

                if (btn) {
                    btn.innerHTML = `<i class="bi bi-arrow-return-right me-1"></i>Hide replies`;
                }
            }
        })
        .catch(err => {
            console.error("Error loading replies:", err);
            if (btn) {
                btn.innerHTML = `<i class="bi bi-arrow-return-right me-1"></i>View replies`;
            }
        });
}

// ============================================
// GENERATE HTML
// ============================================

function generateCommentHtml(comment, isReply) {
    const safeContent = escapeHtml(comment.content);
    const safeUserName = escapeHtml(comment.userName || 'User');
    const indentClass = isReply ? "reply-comment" : "";

    let actionButtons = '';
    if (comment.userId === currentUserId) {
        actionButtons = `
            <div class="comment-dropdown d-inline">
                <button class="dropdown-toggle-btn" type="button">
                    <i class="bi bi-three-dots"></i>
                </button>
                <div class="dropdown-menu">
                    <a class="dropdown-item" href="javascript:void(0)" onclick="event.stopPropagation(); prepareEdit(${comment.commentId}, '${safeContent.replace(/'/g, "\\'")}')">
                        <i class="bi bi-pencil me-2"></i>Edit
                    </a>
                    <a class="dropdown-item text-danger" href="javascript:void(0)" onclick="event.stopPropagation(); deleteComment(${comment.commentId})">
                        <i class="bi bi-trash me-2"></i>Delete
                    </a>
                </div>
            </div>
        `;
    } else {
        actionButtons = `
            <div class="comment-dropdown d-inline">
                <button class="dropdown-toggle-btn" type="button">
                    <i class="bi bi-three-dots"></i>
                </button>
                <div class="dropdown-menu">
                    <a class="dropdown-item" href="javascript:void(0)" onclick="event.stopPropagation();">
                        <i class="bi bi-flag me-2"></i>Report
                    </a>
                </div>
            </div>
        `;
    }

    let viewRepliesBtn = '';
    if (comment.replyCount > 0) {
        viewRepliesBtn = `
            <div class="mt-1">
                <a href="javascript:void(0)" class="view-replies-btn"
                   id="btn-view-replies-${comment.commentId}"
                   onclick="loadReplies(${comment.commentId})">
                    <i class="bi bi-arrow-return-right me-1"></i>View ${comment.replyCount} ${comment.replyCount > 1 ? 'replies' : 'reply'}
                </a>
            </div>
            <div id="replies-container-${comment.commentId}" style="display: none;"></div>
        `;
    }

    return `
        <div class="comment-item ${indentClass}" id="comment-wrapper-${comment.commentId}">
            <div class="d-flex gap-2">
                <img src="/images/avatar.png" class="comment-avatar" alt="Avatar">
                <div class="flex-grow-1">
                    <div class="comment-bubble">
                        <strong class="comment-username">${safeUserName}</strong>
                        <p class="comment-text mb-0" id="comment-text-${comment.commentId}">${safeContent}</p>
                    </div>
                    <div class="comment-actions">
                        <button class="comment-action-btn" onclick="likeComment(${comment.commentId})">Like</button>
                        <button class="comment-action-btn" onclick="prepareReply(${comment.commentId}, '${safeUserName.replace(/'/g, "\\'")}', '${safeContent.replace(/'/g, "\\'")}')">Reply</button>
                        <span class="comment-timestamp">Just now</span>
                        ${actionButtons}
                    </div>
                    ${viewRepliesBtn}
                </div>
            </div>
        </div>
    `;
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// ============================================
// THÊM COMMENT MỚI VÀO DOM
// ============================================

function appendNewCommentHtml(newComment) {
    const isReply = newComment.parentCommentId != null;
    const html = generateCommentHtml(newComment, isReply);

    if (isReply) {
        let container = document.getElementById(`replies-container-${newComment.parentCommentId}`);

        if (!container) {
            // Tạo container nếu chưa có
            const parent = document.getElementById(`comment-wrapper-${newComment.parentCommentId}`);
            if (parent) {
                const newContainer = document.createElement('div');
                newContainer.id = `replies-container-${newComment.parentCommentId}`;
                newContainer.style.display = 'block';
                newContainer.innerHTML = html;
                parent.insertAdjacentElement('afterend', newContainer);
            }
        } else {
            container.style.display = 'block';
            container.insertAdjacentHTML('beforeend', html);
        }
    } else {
        const commentList = document.getElementById("commentList");
        if (commentList) {
            commentList.insertAdjacentHTML('beforeend', html);
            commentList.scrollTop = commentList.scrollHeight;
        }
    }

    // Ẩn thông báo "No comments"
    const noComments = document.querySelector(".no-comments");
    if (noComments) {
        noComments.style.display = 'none';
    }
}

// ============================================
// CHUẨN BỊ REPLY
// ============================================

function prepareReply(commentId, username, content) {
    cancelAction();

    document.getElementById("parentCommentId").value = commentId;
    showActionPreview("Replying to " + username, content);

    const input = document.getElementById("commentContent");
    if (input) {
        input.value = '@' + username + ' ';
        input.focus();
    }
}

// ============================================
// CHUẨN BỊ EDIT
// ============================================

function prepareEdit(commentId, content) {
    cancelAction();

    document.getElementById("editCommentId").value = commentId;
    showActionPreview("Editing", content);

    const input = document.getElementById("commentContent");
    if (input) {
        input.value = content;
        input.focus();
    }
}

// ============================================
// HỦY ACTION
// ============================================

function cancelAction() {
    document.getElementById("parentCommentId").value = "";
    document.getElementById("editCommentId").value = "";
    document.getElementById("commentContent").value = "";
    hideActionPreview();
}

// ============================================
// GỬI COMMENT/REPLY/EDIT
// ============================================

function submitAction() {
    const postId = document.getElementById("commentPostId").value;
    const contentInput = document.getElementById("commentContent");
    const content = contentInput.value.trim();
    const parentId = document.getElementById("parentCommentId").value;
    const editId = document.getElementById("editCommentId").value;

    if (!content) {
        contentInput.focus();
        return;
    }

    contentInput.disabled = true;

    let url = '/Comment/AddComment';
    let method = 'POST';
    let bodyData = {
        postId: parseInt(postId),
        content: content
    };

    if (editId) {
        url = '/Comment/EditComment';
        method = 'PUT';
        bodyData = {
            commentId: parseInt(editId),
            content: content
        };
    } else if (parentId) {
        url = '/Comment/ReplyComment';
        bodyData = {
            postId: parseInt(postId),
            content: content,
            parentCommentId: parseInt(parentId)
        };
    }

    fetch(url, {
        method: method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(bodyData)
    })
        .then(res => {
            if (!res.ok) throw new Error("Failed to process comment");
            return res.json();
        })
        .then(data => {
            if (editId) {
                // Cập nhật comment đã chỉnh sửa
                const textEl = document.getElementById(`comment-text-${editId}`);
                if (textEl) {
                    textEl.innerText = data.content;
                    textEl.classList.add('pulse');
                    setTimeout(() => textEl.classList.remove('pulse'), 500);
                }
            } else {
                // Thêm comment/reply mới
                appendNewCommentHtml(data);
            }
            cancelAction();
        })
        .catch(err => {
            console.error("Error:", err);
            alert("Failed to process your comment. Please try again.");
        })
        .finally(() => {
            contentInput.disabled = false;
            contentInput.focus();
        });
}

// ============================================
// XÓA COMMENT
// ============================================

function deleteComment(commentId) {
    if (!confirm("Are you sure you want to delete this comment?")) {
        return;
    }

    fetch('/Comment/DeleteComment', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(commentId)
    })
        .then(res => {
            if (!res.ok) throw new Error("Failed to delete comment");

            const element = document.getElementById(`comment-wrapper-${commentId}`);
            if (element) {
                element.style.animation = 'fadeOut 0.3s ease-out';
                setTimeout(() => element.remove(), 300);
            }
        })
        .catch(err => {
            console.error("Error:", err);
            alert("Failed to delete comment. Please try again.");
        });
}

// ============================================
// LIKE COMMENT
// ============================================

function likeComment(commentId) {
    // Placeholder - Implement nếu cần
    console.log("Like comment:", commentId);
}

// ============================================
// XÓA POST
// ============================================

function confirmDeletePost(postId) {
    if (confirm("Are you sure you want to delete this post?")) {
        document.getElementById('deletePostId').value = postId;
        document.getElementById('deleteForm').submit();
    }
}

// ============================================
// TOGGLE LIKE POST
// ============================================

function toggleLike(btnElement, postId) {
    const icon = btnElement.querySelector("i");
    const footer = btnElement.closest(".post-footer");
    const countEl = footer.querySelector(".likes-count");

    if (!icon || !countEl) return;

    let currentCount = parseInt(countEl.innerText);
    let isLiking = false;

    if (icon.classList.contains("bi-heart-fill")) {
        // Unlike
        icon.classList.remove("bi-heart-fill", "text-danger");
        icon.classList.add("bi-heart", "text-dark");
        btnElement.classList.remove("liked");
        currentCount--;
    } else {
        // Like
        icon.classList.remove("bi-heart", "text-dark");
        icon.classList.add("bi-heart-fill", "text-danger");
        btnElement.classList.add("liked");
        currentCount++;
        isLiking = true;
    }

    countEl.innerText = currentCount + " " + (currentCount === 1 ? "like" : "likes");

    // Gửi request đến server
    fetch('/Like/Like', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ postId: postId })
    })
        .then(res => {
            if (!res.ok) throw new Error("Failed to toggle like");
        })
        .catch(err => {
            console.error("Error:", err);
            // Revert nếu thất bại
            if (isLiking) {
                icon.classList.remove("bi-heart-fill", "text-danger");
                icon.classList.add("bi-heart", "text-dark");
                btnElement.classList.remove("liked");
                currentCount--;
            } else {
                icon.classList.remove("bi-heart", "text-dark");
                icon.classList.add("bi-heart-fill", "text-danger");
                btnElement.classList.add("liked");
                currentCount++;
            }
            countEl.innerText = currentCount + " " + (currentCount === 1 ? "like" : "likes");
        });
}

// ============================================
// CSS ANIMATION HELPER
// ============================================

const style = document.createElement('style');
style.textContent = `
    @keyframes fadeOut {
        from {
            opacity: 1;
            transform: translateX(0);
        }
        to {
            opacity: 0;
            transform: translateX(-20px);
        }
    }
`;
document.head.appendChild(style);