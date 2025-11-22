// Initialize Particles.js for background animation
particlesJS('particles-js', {
    particles: {
        number: {
            value: 80,
            density: {
                enable: true,
                value_area: 800
            }
        },
        color: {
            value: '#ffffff'
        },
        shape: {
            type: 'circle',
            stroke: {
                width: 0,
                color: '#000000'
            }
        },
        opacity: {
            value: 0.5,
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
            opacity: 0.4,
            width: 1
        },
        move: {
            enable: true,
            speed: 2,
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
                    opacity: 1
                }
            },
            push: {
                particles_nb: 4
            }
        }
    },
    retina_detect: true
});

// Track removed media URLs
let removed = [];

/**
 * Remove media from the current media grid
 */
function removeMedia(url, elementId) {
    // Add URL to removed list
    removed.push(url);
    document.getElementById('RemovedMedia').value = removed.join(',');

    // Get the media item element
    const element = document.getElementById(elementId);

    if (element) {
        // Add removing animation class
        element.classList.add('removing');

        // Remove from DOM after animation completes
        setTimeout(() => {
            element.remove();

            // Update media count
            updateMediaCount();

            // Show notification
            showNotification('Media removed successfully', 'success');
        }, 400);
    }
}

/**
 * Preview newly selected media files
 */
function previewNewMedia(input) {
    const previewContainer = document.getElementById('newMediaPreview');

    if (!input.files || input.files.length === 0) {
        previewContainer.innerHTML = '';
        return;
    }

    // Clear previous previews
    previewContainer.innerHTML = '';

    // Limit to 10 files
    const files = Array.from(input.files).slice(0, 10);

    if (input.files.length > 10) {
        showNotification('Maximum 10 files allowed. Only first 10 will be uploaded.', 'warning');
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
                             <svg width="48" height="48" viewBox="0 0 24 24" fill="white">
                                 <polygon points="5 3 19 12 5 21 5 3"></polygon>
                             </svg>
                         </div>` :
                    `<img src="${e.target.result}" class="media-content" alt="New media preview" />`
                }
                    <button type="button" class="remove-btn" onclick="removeNewMedia(${index})" title="Remove">
                        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <line x1="18" y1="6" x2="6" y2="18"></line>
                            <line x1="6" y1="6" x2="18" y2="18"></line>
                        </svg>
                    </button>
                </div>
            `;

            previewContainer.appendChild(mediaItem);
        };

        reader.readAsDataURL(file);
    });

    showNotification(`${files.length} file(s) selected for upload`, 'info');
}

/**
 * Remove a new media file from preview
 */
function removeNewMedia(index) {
    const input = document.getElementById('newMediaInput');
    const dt = new DataTransfer();

    // Rebuild file list without the removed file
    Array.from(input.files).forEach((file, i) => {
        if (i !== index) {
            dt.items.add(file);
        }
    });

    input.files = dt.files;

    // Refresh preview
    previewNewMedia(input);
}

/**
 * Update media count display
 */
function updateMediaCount() {
    const mediaGrid = document.getElementById('mediaGrid');
    const mediaItems = mediaGrid ? mediaGrid.querySelectorAll('.media-item:not(.removing)') : [];
    const countElement = document.querySelector('.media-count');

    if (countElement) {
        countElement.textContent = `(${mediaItems.length})`;
    }
}

/**
 * Show notification toast
 */
function showNotification(message, type = 'info') {
    // Remove existing notification
    const existing = document.querySelector('.notification-toast');
    if (existing) {
        existing.remove();
    }

    // Create notification element
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

    // Add notification styles if not exists
    if (!document.querySelector('#notification-styles')) {
        const style = document.createElement('style');
        style.id = 'notification-styles';
        style.textContent = `
            .notification-toast {
                position: fixed;
                top: 20px;
                right: 20px;
                display: flex;
                align-items: center;
                gap: 12px;
                padding: 16px 20px;
                background: white;
                border-radius: 12px;
                box-shadow: 0 10px 40px rgba(0, 0, 0, 0.15);
                z-index: 9999;
                animation: slideInRight 0.4s ease-out, fadeOut 0.3s ease-out 2.7s forwards;
                max-width: 400px;
            }
            
            @keyframes slideInRight {
                from {
                    opacity: 0;
                    transform: translateX(100px);
                }
                to {
                    opacity: 1;
                    transform: translateX(0);
                }
            }
            
            @keyframes fadeOut {
                to {
                    opacity: 0;
                    transform: translateX(50px);
                }
            }
            
            .notification-icon {
                flex-shrink: 0;
                display: flex;
                align-items: center;
                justify-content: center;
            }
            
            .notification-message {
                color: #333;
                font-size: 14px;
                font-weight: 500;
            }
            
            .notification-success .notification-icon { color: #27ae60; }
            .notification-warning .notification-icon { color: #f39c12; }
            .notification-error .notification-icon { color: #e74c3c; }
            .notification-info .notification-icon { color: #4facfe; }
            
            @media (max-width: 768px) {
                .notification-toast {
                    top: 10px;
                    right: 10px;
                    left: 10px;
                    max-width: none;
                }
            }
        `;
        document.head.appendChild(style);
    }

    document.body.appendChild(notification);

    // Auto remove after 3 seconds
    setTimeout(() => {
        notification.remove();
    }, 3000);
}

/**
 * Form submission handler
 */
document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('editPostForm');

    if (form) {
        form.addEventListener('submit', function (e) {
            const submitBtn = form.querySelector('.btn-primary');

            // Add loading state
            submitBtn.classList.add('loading');
            submitBtn.disabled = true;

            // Note: Form will submit normally, this just adds visual feedback
        });
    }

    // Drag and drop support for file upload
    const fileUploadLabel = document.querySelector('.file-upload-label');
    const fileInput = document.getElementById('newMediaInput');

    if (fileUploadLabel && fileInput) {
        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            fileUploadLabel.addEventListener(eventName, preventDefaults, false);
        });

        function preventDefaults(e) {
            e.preventDefault();
            e.stopPropagation();
        }

        ['dragenter', 'dragover'].forEach(eventName => {
            fileUploadLabel.addEventListener(eventName, () => {
                fileUploadLabel.style.borderColor = '#4facfe';
                fileUploadLabel.style.background = 'rgba(79, 172, 254, 0.1)';
            });
        });

        ['dragleave', 'drop'].forEach(eventName => {
            fileUploadLabel.addEventListener(eventName, () => {
                fileUploadLabel.style.borderColor = '#d0d0d0';
                fileUploadLabel.style.background = '#fafafa';
            });
        });

        fileUploadLabel.addEventListener('drop', (e) => {
            const dt = e.dataTransfer;
            const files = dt.files;

            fileInput.files = files;
            previewNewMedia(fileInput);
        });
    }

    // Auto-resize textarea
    const textareas = document.querySelectorAll('textarea.form-control');
    textareas.forEach(textarea => {
        textarea.addEventListener('input', function () {
            this.style.height = 'auto';
            this.style.height = (this.scrollHeight) + 'px';
        });
    });
});