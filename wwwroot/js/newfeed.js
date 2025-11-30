let page = 1;
let isLoading = false;
let isFinished = false;
let scrollTimeout;

function initInfiniteScroll() {
    window.addEventListener("scroll", handleScroll);
}

function handleScroll() {
    if (scrollTimeout) clearTimeout(scrollTimeout);

    scrollTimeout = setTimeout(() => {
        if (isLoading || isFinished) return;

        const scrollPosition = window.innerHeight + window.scrollY;
        const threshold = document.body.offsetHeight - 500;

        if (scrollPosition >= threshold) {
            loadMore();
        }
    }, 100);
}

function loadMore() {
    isLoading = true;
    const loadingEl = document.getElementById("loading");
    if (loadingEl) loadingEl.style.display = "block";

    const nextPage = page + 1;

    fetch(`/Home/LoadMore?page=${nextPage}`)
        .then(res => {
            if (res.status === 204) {
                isFinished = true;
                const endEl = document.getElementById("endOfFeed");
                if (endEl) endEl.style.display = "block";
                return null;
            }
            return res.text();
        })
        .then(html => {
            if (html) {
                const container = document.getElementById("postContainer");
                if (container) {
                    container.insertAdjacentHTML("beforeend", html);
                    page++;
                }
            }
        })
        .catch(err => {
            console.error("Load failed:", err);
            alert("Failed to load more posts. Please try again.");
        })
        .finally(() => {
            if (loadingEl) loadingEl.style.display = "none";
            isLoading = false;
        });
}

// ============================================
// DROPDOWN MENU
// ============================================

let currentOpenDropdown = null;

function initDropdowns() {
    // Click outside to close
    document.addEventListener('click', function (event) {
        if (!event.target.closest('.post-dropdown')) {
            closeAllDropdowns();
        }
    });
}

function togglePostMenu(button, postId, isOwner) {
    event.stopPropagation();

    const dropdown = button.closest('.post-dropdown');
    const menu = dropdown.querySelector('.post-dropdown-menu');

    // Close other dropdowns
    if (currentOpenDropdown && currentOpenDropdown !== menu) {
        currentOpenDropdown.classList.remove('show');
    }

    // Toggle current dropdown
    const isOpen = menu.classList.contains('show');

    if (isOpen) {
        menu.classList.remove('show');
        currentOpenDropdown = null;
    } else {
        menu.classList.add('show');
        currentOpenDropdown = menu;
    }
}

function closeAllDropdowns() {
    const dropdowns = document.querySelectorAll('.post-dropdown-menu.show');
    dropdowns.forEach(dropdown => {
        dropdown.classList.remove('show');
    });
    currentOpenDropdown = null;
}

function goToEditPost(postId) {
    window.location.href = `/Post/EditPost?postId=${postId}`;
}

function confirmDeletePost(postId) {
    closeAllDropdowns();

    if (confirm("Are you sure you want to delete this post?")) {
        deletePost(postId);
    }
}

function deletePost(postId) {
    fetch('/Post/DeletePost', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: `postId=${postId}`
    })
        .then(res => {
            if (res.ok) {
                const postCard = document.querySelector(`[data-post-id="${postId}"]`);
                if (postCard) {
                    postCard.style.animation = 'fadeOutDown 0.4s ease-out';
                    setTimeout(() => {
                        postCard.remove();
                    }, 400);
                }
            } else {
                throw new Error("Failed to delete");
            }
        })
        .catch(err => {
            console.error("Delete failed:", err);
            alert("Failed to delete post. Please try again.");
        });
}





function toggleLike(button, postId) {
    const icon = button.querySelector("i");
    const container = button.closest(".post-actions-section");
    const countElement = container.querySelector(".like-number");

    if (!icon || !countElement) return;

    let currentCount = parseInt(countElement.innerText) || 0;
    let isLiking = false;

    // Optimistic UI update
    if (icon.classList.contains("bi-heart-fill")) {
        // Unlike
        icon.classList.remove("bi-heart-fill", "text-danger");
        icon.classList.add("bi-heart");
        button.classList.remove("liked");
        currentCount--;
    } else {
        // Like
        icon.classList.remove("bi-heart");
        icon.classList.add("bi-heart-fill", "text-danger");
        button.classList.add("liked");
        currentCount++;
        isLiking = true;
    }

    countElement.innerText = currentCount;

    // Send request to server
    fetch('/Like/Like', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ postId: postId })
    })
        .then(res => {
            if (!res.ok) throw new Error("API Error");
            return res.json();
        })
        .catch(err => {
            console.error("Like failed:", err);

            // Rollback UI on error
            if (isLiking) {
                icon.classList.remove("bi-heart-fill", "text-danger");
                icon.classList.add("bi-heart");
                button.classList.remove("liked");
                currentCount--;
            } else {
                icon.classList.remove("bi-heart");
                icon.classList.add("bi-heart-fill", "text-danger");
                button.classList.add("liked");
                currentCount++;
            }

            countElement.innerText = currentCount;
            alert("Failed to update like. Please try again.");
        });
}

document.addEventListener('DOMContentLoaded', function () {
    initInfiniteScroll();
    initDropdowns();
});


const style = document.createElement('style');
style.textContent = `
    @keyframes fadeOutDown {
        from {
            opacity: 1;
            transform: translateY(0);
        }
        to {
            opacity: 0;
            transform: translateY(30px);
        }
    }
`;
document.head.appendChild(style);


function openReportModal(postId) {
    // Gán ID vào thẻ input ẩn
    document.getElementById('hiddenReportPostId').value = postId;

    var myModal = new bootstrap.Modal(document.getElementById('reportModal'));
    myModal.show();
}

// 2. Hàm gửi báo cáo
function submitReport(reason) {
    // Lấy ID từ thẻ input ẩn
    const postIdVal = document.getElementById('hiddenReportPostId').value;
    const postId = parseInt(postIdVal);

    if (!postId || postId === 0) {
        alert("Error: Cannot find Post ID");
        return;
    }

    // Đóng modal trước cho mượt
    var modalEl = document.getElementById('reportModal');
    var modal = bootstrap.Modal.getInstance(modalEl);
    modal.hide();

    // In ra console để kiểm tra xem dữ liệu có đúng không
    console.log("Sending Report:", { entityId: postId, content: reason, type: 'Post' });

    fetch('/Report/AddReport', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            entityId: postId,
            content: reason,
            type: 'Post'
        })
    })
        .then(res => {
            if (res.ok) {
                alert("Report sent successfully.");
            } else {
                console.error("Server Error:", res.status, res.statusText);
                alert("Failed to send report. Check console for details.");
            }
        })
        .catch(err => {
            console.error("Fetch Error:", err);
            alert("Network error.");
        });
}