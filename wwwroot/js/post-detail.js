// Carousel functionality
let currentSlide = 0;

function moveSlide(direction) {
    const slides = document.querySelector('.media-slides');
    const totalSlides = document.querySelectorAll('.media-slide').length;

    if (!slides || totalSlides === 0) return;

    currentSlide += direction;

    if (currentSlide < 0) {
        currentSlide = totalSlides - 1;
    } else if (currentSlide >= totalSlides) {
        currentSlide = 0;
    }

    updateCarousel();
}

function goToSlide(index) {
    currentSlide = index;
    updateCarousel();
}

function updateCarousel() {
    const slides = document.querySelector('.media-slides');
    const indicators = document.querySelectorAll('.indicator');

    if (!slides) return;

    slides.style.transform = `translateX(-${currentSlide * 100}%)`;

    indicators.forEach((indicator, index) => {
        if (index === currentSlide) {
            indicator.classList.add('active');
        } else {
            indicator.classList.remove('active');
        }
    });
}

// Like button functionality
document.addEventListener('DOMContentLoaded', function () {
    const likeBtn = document.querySelector('.like-btn');
    const bookmarkBtn = document.querySelector('.bookmark-btn');

    if (likeBtn) {
        likeBtn.addEventListener('click', function () {
            this.classList.toggle('active');

            // Animate
            this.style.transform = 'scale(1.3)';
            setTimeout(() => {
                this.style.transform = 'scale(1)';
            }, 200);
        });
    }

    if (bookmarkBtn) {
        bookmarkBtn.addEventListener('click', function () {
            this.classList.toggle('active');
        });
    }

    // Comment input
    const commentInput = document.querySelector('.comment-input');
    const postCommentBtn = document.querySelector('.post-comment-btn');

    if (commentInput && postCommentBtn) {
        commentInput.addEventListener('input', function () {
            if (this.value.trim()) {
                postCommentBtn.style.display = 'block';
            } else {
                postCommentBtn.style.display = 'none';
            }
        });

        postCommentBtn.addEventListener('click', function () {
            const comment = commentInput.value.trim();
            if (comment) {
                // TODO: Submit comment via AJAX
                console.log('Post comment:', comment);
                commentInput.value = '';
                postCommentBtn.style.display = 'none';
            }
        });
    }

    // Keyboard navigation for carousel
    document.addEventListener('keydown', function (e) {
        const carousel = document.querySelector('.media-carousel');
        if (!carousel) return;

        if (e.key === 'ArrowLeft') {
            moveSlide(-1);
        } else if (e.key === 'ArrowRight') {
            moveSlide(1);
        }
    });
});

// Delete confirmation
function confirmDelete(postId) {
    if (confirm('Are you sure you want to delete this post? This action cannot be undone.')) {
        document.getElementById('deletePostId').value = postId;
        document.getElementById('deleteForm').submit();
    }
}

// Touch swipe for carousel on mobile
let touchStartX = 0;
let touchEndX = 0;

const carousel = document.querySelector('.media-carousel');
if (carousel) {
    carousel.addEventListener('touchstart', function (e) {
        touchStartX = e.changedTouches[0].screenX;
    });

    carousel.addEventListener('touchend', function (e) {
        touchEndX = e.changedTouches[0].screenX;
        handleSwipe();
    });
}

function handleSwipe() {
    const swipeThreshold = 50;
    const diff = touchStartX - touchEndX;

    if (Math.abs(diff) > swipeThreshold) {
        if (diff > 0) {
            moveSlide(1); // Swipe left
        } else {
            moveSlide(-1); // Swipe right
        }
    }
}