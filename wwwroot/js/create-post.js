document.addEventListener('DOMContentLoaded', function () {
    const mediaInput = document.getElementById('mediaInput');
    const mediaUploadArea = document.getElementById('mediaUploadArea');
    const mediaPreviewGrid = document.getElementById('mediaPreviewGrid');
    const captionTextarea = document.querySelector('textarea[name="Caption"]');
    const charCount = document.getElementById('charCount');
    const form = document.getElementById('createPostForm');
    const btnSelectFiles = document.querySelector('.btn-select-files');

    let selectedFiles = [];

    // Click on "Select Files" button
    if (btnSelectFiles) {
        btnSelectFiles.addEventListener('click', function (e) {
            e.preventDefault();
            mediaInput.click();
        });
    }

    // File input change
    if (mediaInput) {
        mediaInput.addEventListener('change', function (e) {
            handleFiles(e.target.files);
        });
    }

    // Drag and drop
    if (mediaUploadArea) {
        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            mediaUploadArea.addEventListener(eventName, preventDefaults, false);
        });

        function preventDefaults(e) {
            e.preventDefault();
            e.stopPropagation();
        }

        ['dragenter', 'dragover'].forEach(eventName => {
            mediaUploadArea.addEventListener(eventName, () => {
                mediaUploadArea.classList.add('drag-over');
            }, false);
        });

        ['dragleave', 'drop'].forEach(eventName => {
            mediaUploadArea.addEventListener(eventName, () => {
                mediaUploadArea.classList.remove('drag-over');
            }, false);
        });

        mediaUploadArea.addEventListener('drop', function (e) {
            const dt = e.dataTransfer;
            const files = dt.files;
            handleFiles(files);
        }, false);
    }

    // Handle files
    function handleFiles(files) {
        selectedFiles = Array.from(files);
        displayPreview();
    }

    // Display preview
    function displayPreview() {
        mediaPreviewGrid.innerHTML = '';

        if (selectedFiles.length > 0) {
            mediaPreviewGrid.classList.add('has-files');

            selectedFiles.forEach((file, index) => {
                const reader = new FileReader();

                reader.onload = function (e) {
                    const previewItem = document.createElement('div');
                    previewItem.className = 'media-preview-item';

                    let mediaElement;
                    if (file.type.startsWith('image/')) {
                        mediaElement = document.createElement('img');
                        mediaElement.src = e.target.result;
                        mediaElement.alt = file.name;
                    } else if (file.type.startsWith('video/')) {
                        mediaElement = document.createElement('video');
                        mediaElement.src = e.target.result;
                        mediaElement.controls = false;
                    }

                    const removeBtn = document.createElement('button');
                    removeBtn.className = 'remove-btn';
                    removeBtn.innerHTML = '×';
                    removeBtn.type = 'button';
                    removeBtn.onclick = function () {
                        removeFile(index);
                    };

                    previewItem.appendChild(mediaElement);
                    previewItem.appendChild(removeBtn);
                    mediaPreviewGrid.appendChild(previewItem);
                };

                reader.readAsDataURL(file);
            });
        } else {
            mediaPreviewGrid.classList.remove('has-files');
        }

        updateFileInput();
    }

    // Remove file
    function removeFile(index) {
        selectedFiles.splice(index, 1);
        displayPreview();
    }

    // Update file input
    function updateFileInput() {
        const dt = new DataTransfer();
        selectedFiles.forEach(file => {
            dt.items.add(file);
        });
        mediaInput.files = dt.files;
    }

    // Character count for caption
    if (captionTextarea && charCount) {
        captionTextarea.addEventListener('input', function () {
            const length = this.value.length;
            charCount.textContent = length;

            if (length > 2000) {
                charCount.style.color = '#e74c3c';
            } else if (length > 1800) {
                charCount.style.color = '#f39c12';
            } else {
                charCount.style.color = '#999';
            }
        });
    }

    // Auto-format hashtags
    const hashtagsInput = document.querySelector('input[name="Hashtags"]');
    if (hashtagsInput) {
        hashtagsInput.addEventListener('input', function () {
            let value = this.value;
            // Remove multiple spaces
            value = value.replace(/\s+/g, ' ');
            // Ensure hashtags start with #
            value = value.split(' ').map(tag => {
                tag = tag.trim();
                if (tag && !tag.startsWith('#')) {
                    return '#' + tag;
                }
                return tag;
            }).join(' ');
            this.value = value;
        });
    }

    // Form submission
    if (form) {
        form.addEventListener('submit', function (e) {
            const submitBtn = form.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.classList.add('loading');
                submitBtn.disabled = true;

                // Re-enable after 10 seconds as fallback
                setTimeout(() => {
                    submitBtn.classList.remove('loading');
                    submitBtn.disabled = false;
                }, 10000);
            }
        });
    }

    // Auto-resize textarea
    if (captionTextarea) {
        captionTextarea.addEventListener('input', function () {
            this.style.height = 'auto';
            this.style.height = (this.scrollHeight) + 'px';
        });
    }
});