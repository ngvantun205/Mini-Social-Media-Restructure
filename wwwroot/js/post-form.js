// ==================== INITIALIZATION ====================
document.addEventListener('DOMContentLoaded', function () {
    initializeParticles();
    initializeCreatePost();
    initializeEditPost();
    initializeCommonFeatures();
});

// ==================== PARTICLES.JS INITIALIZATION ====================
function initializeParticles() {
    if (typeof particlesJS !== 'undefined' && document.getElementById('particles-js')) {
        particlesJS('particles-js', {
            particles: {
                number: {
                    value: 60,
                    density: {
                        enable: true,
                        value_area: 800
                    }
                },
                color: {
                    value: '#ffffff'
                },
                shape: {
                    type: 'circle'
                },
                opacity: {
                    value: 0.4,
                    random: true,
                    anim: {
                        enable: true,
                        speed: 1,
                        opacity_min: 0.1,
                        sync: false
                    }
                },
                size: {
                    value: 3,
                    random: true,
                    anim: {
                        enable: true,
                        speed: 2,
                        size_min: 0.1,
                        sync: false
                    }
                },
                line_linked: {
                    enable: true,
                    distance: 150,
                    color: '#ffffff',
                    opacity: 0.3,
                    width: 1
                },
                move: {
                    enable: true,
                    speed: 1.5,
                    direction: 'none',
                    random: false,
                    straight: false,
                    out_mode: 'out',
                    bounce: false
                }
            },
            interactivity: {
                detect_on: 'canvas',
                events: {
                    onhover: {
                        enable: true,
                        mode: 'grab'
                    },
                    onclick: {
                        enable: true,
                        mode: 'push'
                    },
                    resize: true
                },
                modes: {
                    grab: {
                        distance: 140,
                        line_linked: {
                            opacity: 0.8
                        }
                    },
                    push: {
                        particles_nb: 3
                    }
                }
            },
            retina_detect: true
        });
    }
}

// ==================== CREATE POST FEATURES ====================
function initializeCreatePost() {
    const mediaInput = document.getElementById('mediaInput');
    const mediaUploadArea = document.getElementById('mediaUploadArea');
    const mediaPreviewGrid = document.getElementById('mediaPreviewGrid');
    const btnSelectFiles = document.querySelector('.btn-select-files');
    const captionTextarea = document.querySelector('textarea[name="Caption"]');
    const charCount = document.getElementById('charCount');
    const hashtagsInput = document.querySelector('input[name="Hashtags"]');
    const form = document.getElementById('createPostForm');

    let selectedFiles = [];

    // File selection
    if (btnSelectFiles) {
        btnSelectFiles.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            mediaInput.click();
        });
    }

    if (mediaInput) {
        mediaInput.addEventListener('change', function (e) {
            handleFiles(e.target.files);
        });
    }

    // Drag and drop
    if (mediaUploadArea) {
        setupDragAndDrop(mediaUploadArea, handleFiles);
    }

    // Handle files
    function handleFiles(files) {
        const validFiles = Array.from(files).filter(file => {
            return file.type.startsWith('image/') || file.type.startsWith('video/');
        });

        if (validFiles.length > 0) {
            selectedFiles = validFiles;
            displayPreview();
        }
    }

    // Display preview
    function displayPreview() {
        if (!mediaPreviewGrid) return;

        mediaPreviewGrid.innerHTML = '';

        if (selectedFiles.length > 0) {
            mediaPreviewGrid.classList.add('has-files');

            selectedFiles.forEach((file, index) => {
                const reader = new FileReader();

                reader.onload = function (e) {
                    const previewItem = document.createElement('div');
                    previewItem.className = 'media-preview-item';
                    previewItem.style.animationDelay = `${index * 0.1}s`;

                    let mediaElement;
                    if (file.type.startsWith('image/')) {
                        mediaElement = document.createElement('img');
                        mediaElement.src = e.target.result;
                        mediaElement.alt = file.name;
                    } else if (file.type.startsWith('video/')) {
                        mediaElement = document.createElement('video');
                        mediaElement.src = e.target.result;
                        mediaElement.muted = true;

                        const playIcon = document.createElement('div');
                        playIcon.className = 'video-overlay';
                        playIcon.innerHTML = '<i class="bi bi-play-circle-fill"></i>';
                        previewItem.appendChild(playIcon);
                    }

                    const removeBtn = document.createElement('button');
                    removeBtn.className = 'remove-btn';
                    removeBtn.innerHTML = '×';
                    removeBtn.type = 'button';
                    removeBtn.onclick = function (e) {
                        e.stopPropagation();
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
        if (!mediaInput) return;

        const dt = new DataTransfer();
        selectedFiles.forEach(file => {
            dt.items.add(file);
        });
        mediaInput.files = dt.files;
    }

    // Character count
    if (captionTextarea && charCount) {
        captionTextarea.addEventListener('input', function () {
            const length = this.value.length;
            charCount.textContent = length;

            if (length > 2000) {
                charCount.style.color = '#e4344c';
                charCount.style.fontWeight = '700';
            } else if (length > 1800) {
                charCount.style.color = '#f39c12';
                charCount.style.fontWeight = '600';
            } else {
                charCount.style.color = '#65676b';
                charCount.style.fontWeight = '400';
            }
        });
    }

    // Auto-format hashtags
    if (hashtagsInput) {
        hashtagsInput.addEventListener('input', function () {
            let value = this.value;
            value = value.replace(/\s+/g, ' ');
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
            const submitBtn = form.querySelector('.btn-post, .btn-primary');

            if (submitBtn) {
                submitBtn.classList.add('loading');
                submitBtn.disabled = true;

                setTimeout(() => {
                    submitBtn.classList.remove('loading');
                    submitBtn.disabled = false;
                }, 10000);
            }
        });
    }
}

// ==================== EDIT POST FEATURES ====================
function initializeEditPost() {
    const newMediaInput = document.getElementById('newMediaInput');
    const fileUploadLabel = document.querySelector('.file-upload-label');

    // Track removed media
    window.removedMedia = [];

    // Remove existing media
    window.removeMedia = function (url, elementId) {
        window.removedMedia.push(url);
        const removedInput = document.getElementById('RemovedMedia');
        if (removedInput) {
            removedInput.value = window.removedMedia.join(',');
        }

        const element = document.getElementById(elementId);
        if (element) {
            element.classList.add('removing');
            setTimeout(() => {
                element.remove();
                updateMediaCount();
                showNotification('Media removed successfully', 'success');
            }, 400);
        }
    };

    // Preview new media
    window.previewNewMedia = function (input) {
        const previewContainer = document.getElementById('newMediaPreview');
        if (!previewContainer || !input.files || input.files.length === 0) {
            if (previewContainer) previewContainer.innerHTML = '';
            return;
        }

        previewContainer.innerHTML = '';
        const files = Array.from(input.files).slice(0, 10);

        if (input.files.length > 10) {
            showNotification('Maximum 10 files allowed', 'warning');
        }

        files.forEach((file, index) => {
            const reader = new FileReader();

            reader.onload = function (e) {
                const mediaItem = document.createElement('div');
                mediaItem.className = 'media-item';
                mediaItem.style.animationDelay = `${index * 0.1}s`;

                const isVideo = file.type.startsWith('video/');

                mediaItem.innerHTML = `
                    <div class="media-container">
                        ${isVideo ?
                        `<video src="${e.target.result}" class="media-content"></video>
                             <div class="video-overlay">
                                 <svg width="40" height="40" viewBox="0 0 24 24" fill="white">
                                     <polygon points="5 3 19 12 5 21 5 3"></polygon>
                                 </svg>
                             </div>` :
                        `<img src="${e.target.result}" class="media-content" alt="Preview" />`
                    }
                        <button type="button" class="remove-btn" onclick="removeNewMedia(${index})">
                            ×
                        </button>
                    </div>
                `;

                previewContainer.appendChild(mediaItem);
            };

            reader.readAsDataURL(file);
        });

        showNotification(`${files.length} file(s) selected`, 'info');
    };

    // Remove new media
    window.removeNewMedia = function (index) {
        const input = document.getElementById('newMediaInput');
        if (!input) return;

        const dt = new DataTransfer();
        Array.from(input.files).forEach((file, i) => {
            if (i !== index) {
                dt.items.add(file);
            }
        });
        input.files = dt.files;
        window.previewNewMedia(input);
    };

    // Update media count
    function updateMediaCount() {
        const mediaGrid = document.getElementById('mediaGrid');
        const mediaItems = mediaGrid ? mediaGrid.querySelectorAll('.media-item:not(.removing)') : [];
        const countElement = document.querySelector('.media-count');
        if (countElement) {
            countElement.textContent = `(${mediaItems.length})`;
        }
    }

    // Drag and drop for file upload
    if (fileUploadLabel && newMediaInput) {
        setupDragAndDrop(fileUploadLabel, (files) => {
            newMediaInput.files = files;
            window.previewNewMedia(newMediaInput);
        });
    }

    // Form submission
    const editForm = document.getElementById('editPostForm');
    if (editForm) {
        editForm.addEventListener('submit', function () {
            const submitBtn = editForm.querySelector('.btn-primary');
            if (submitBtn) {
                submitBtn.classList.add('loading');
                submitBtn.disabled = true;
            }
        });
    }
}

// ==================== COMMON FEATURES ====================
function initializeCommonFeatures() {
    // Auto-resize textareas
    const textareas = document.querySelectorAll('textarea.form-control');
    textareas.forEach(textarea => {
        textarea.addEventListener('input', function () {
            this.style.height = 'auto';
            this.style.height = Math.min(this.scrollHeight, 200) + 'px';
        });
    });

    // Keyboard shortcuts
    document.addEventListener('keydown', function (e) {
        // Ctrl/Cmd + Enter to submit
        if ((e.ctrlKey || e.metaKey) && e.key === 'Enter') {
            const form = document.querySelector('#createPostForm, #editPostForm');
            if (form) {
                form.requestSubmit();
            }
        }

        // ESC to cancel
        if (e.key === 'Escape') {
            const cancelBtn = document.querySelector('.btn-cancel, .btn-secondary');
            if (cancelBtn) {
                cancelBtn.click();
            }
        }
    });
}

// ==================== DRAG AND DROP UTILITY ====================
function setupDragAndDrop(element, handleFilesCallback) {
    ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
        element.addEventListener(eventName, preventDefaults, false);
        document.body.addEventListener(eventName, preventDefaults, false);
    });

    function preventDefaults(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    ['dragenter', 'dragover'].forEach(eventName => {
        element.addEventListener(eventName, () => {
            element.classList.add('drag-over');
        });
    });

    ['dragleave', 'drop'].forEach(eventName => {
        element.addEventListener(eventName, () => {
            element.classList.remove('drag-over');
        });
    });

    element.addEventListener('drop', function (e) {
        const files = e.dataTransfer.files;
        handleFilesCallback(files);
    });
}

// ==================== NOTIFICATION SYSTEM ====================
function showNotification(message, type = 'info') {
    // Remove existing notification
    const existing = document.querySelector('.notification-toast');
    if (existing) {
        existing.remove();
    }

    // Create notification
    const notification = document.createElement('div');
    notification.className = `notification-toast notification-${type}`;

    const icons = {
        success: '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="20 6 9 17 4 12"></polyline></svg>',
        warning: '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"></path><line x1="12" y1="9" x2="12" y2="13"></line><line x1="12" y1="17" x2="12.01" y2="17"></line></svg>',
        error: '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"></circle><line x1="15" y1="9" x2="9" y2="15"></line><line x1="9" y1="9" x2="15" y2="15"></line></svg>',
        info: '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="16" x2="12" y2="12"></line><line x1="12" y1="8" x2="12.01" y2="8"></line></svg>'
    };

    notification.innerHTML = `
        <div class="notification-icon">${icons[type] || icons.info}</div>
        <div class="notification-message">${message}</div>
    `;

    document.body.appendChild(notification);

    // Auto remove
    setTimeout(() => {
        notification.remove();
    }, 3000);
}

// Make showNotification available globally
window.showNotification = showNotification;

// ==================== CONSOLE LOG ====================
console.log('Post form script initialized successfully');