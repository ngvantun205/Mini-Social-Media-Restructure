// Search Page Scripts

/**
 * Initialize search page functionality
 */
document.addEventListener('DOMContentLoaded', function () {
    initializeTabs();
    initializeSearchInput();
});

/**
 * Initialize tab switching functionality
 */
function initializeTabs() {
    const tabButtons = document.querySelectorAll('.tab-btn');
    const tabContents = document.querySelectorAll('.tab-content');

    tabButtons.forEach(button => {
        button.addEventListener('click', function () {
            const targetTab = this.getAttribute('data-tab');

            // Remove active class from all tabs and contents
            tabButtons.forEach(btn => btn.classList.remove('active'));
            tabContents.forEach(content => content.classList.remove('active'));

            // Add active class to clicked tab and corresponding content
            this.classList.add('active');
            const targetContent = document.querySelector(`[data-content="${targetTab}"]`);
            if (targetContent) {
                targetContent.classList.add('active');
            }
        });
    });
}

/**
 * Initialize search input with auto-submit on Enter
 */
function initializeSearchInput() {
    const searchInput = document.querySelector('.search-input');
    const searchForm = document.getElementById('searchForm');

    if (searchInput && searchForm) {
        // Auto-submit on Enter key
        searchInput.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                if (this.value.trim()) {
                    searchForm.submit();
                }
            }
        });

        // Optional: Auto-submit after typing stops (debounce)
        let searchTimeout;
        searchInput.addEventListener('input', function () {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                if (this.value.trim()) {
                    // Uncomment the line below to enable auto-search while typing
                    // searchForm.submit();
                }
            }, 800);
        });
    }
}

/**
 * Clear search input and redirect to search page
 */
function clearSearch() {
    const searchInput = document.querySelector('.search-input');
    if (searchInput) {
        searchInput.value = '';
        window.location.href = '/Search/Search';
    }
}

/**
 * Search for specific hashtag
 * @param {string} hashtag - The hashtag to search for
 */
function searchHashtag(hashtag) {
    const searchInput = document.querySelector('.search-input');
    if (searchInput) {
        searchInput.value = hashtag;
        document.getElementById('searchForm').submit();
    }
}

/**
 * Smooth scroll to top when switching tabs
 */
function scrollToTop() {
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
}