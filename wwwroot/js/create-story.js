// Story Create Page Scripts

/**
 * Preview uploaded file (image or video)
 * @param {HTMLInputElement} input - File input element
 */
function previewFile(input) {
    const file = input.files[0];
    if (!file) return;

    const placeholder = document.getElementById('uploadPlaceholder');
    const imgPreview = document.getElementById('imagePreview');
    const videoPreview = document.getElementById('videoPreview');
    const uploadZone = document.querySelector('.upload-zone');

    // Hide placeholder with animation
    placeholder.style.opacity = '0';
    placeholder.style.transform = 'translate(-50%, -50%) scale(0.8)';

    setTimeout(() => {
        placeholder.style.display = 'none';
    }, 300);

    // Reset previews
    imgPreview.style.display = 'none';
    videoPreview.style.display = 'none';
    imgPreview.src = '';
    videoPreview.src = '';

    // Change upload zone style
    uploadZone.style.border = '3px solid #667eea';
    uploadZone.style.background = '#000';

    // Read and display file
    const reader = new FileReader();
    reader.onload = function (e) {
        if (file.type.startsWith('video/')) {
            videoPreview.src = e.target.result;
            videoPreview.style.display = 'block';
        } else {
            imgPreview.src = e.target.result;
            imgPreview.style.display = 'block';
        }
    };
    reader.readAsDataURL(file);
}

/**
 * Handle form submission with loading state
 */
document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('createStoryForm');
    const submitBtn = document.getElementById('btnSubmit');

    if (form && submitBtn) {
        form.addEventListener('submit', function () {
            // Disable button and show loading state
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Uploading...';
            submitBtn.style.background = 'linear-gradient(135deg, #9ca3af 0%, #6b7280 100%)';
        });
    }
});