// ===== Scroll Listener for Header Toggle =====
if (typeof window.addScrollListener !== "function") {
window.addScrollListener = function (dotNetHelper) {
    console.log("Scroll listener added");
    window.addEventListener("scroll", function () {
        const isScrolled = window.scrollY > 5;
        console.log("Scroll position:", window.scrollY, "isScrolled:", isScrolled);
        dotNetHelper.invokeMethodAsync("OnScroll", isScrolled);
    });
};
}

// ===== Safe Permissions API Helper =====
window.queryClipboardPermission = function () {
    try {
        if (navigator && navigator.permissions && typeof navigator.permissions.query === 'function') {
            return navigator.permissions.query({ name: 'clipboard-read' })
                .then(result => result.state)
                .catch(() => "error");
        } else {
            return Promise.resolve("not-available");
        }
    } catch (e) {
        return Promise.resolve("error");
    }
};

// ===== Theme Management Functions =====
window.getThemePreference = function () {
    try {
        const savedTheme = localStorage.getItem('theme-preference');
        if (savedTheme !== null) {
            return savedTheme === 'dark';
        }
        // Default to system preference if no saved preference
        return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
    } catch (e) {
        console.warn('Failed to get theme preference:', e);
        return false; // Default to light mode
    }
};

window.setThemePreference = function (isDarkMode) {
    try {
        localStorage.setItem('theme-preference', isDarkMode ? 'dark' : 'light');
        
        // Apply theme to document
        if (isDarkMode) {
            document.documentElement.classList.add('dark');
        } else {
            document.documentElement.classList.remove('dark');
        }
        
        console.log('Theme preference set to:', isDarkMode ? 'dark' : 'light');
    } catch (e) {
        console.warn('Failed to set theme preference:', e);
    }
};

// Initialize theme on page load
if (typeof window.initializeTheme !== "function") {
    window.initializeTheme = function () {
        const isDarkMode = window.getThemePreference();
        window.setThemePreference(isDarkMode);
    };
    
    // Auto-initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', window.initializeTheme);
    } else {
        window.initializeTheme();
    }
}

// ===== Header Component Reference Management =====

// Global reference to the header component
let dotNetReference = null;

if (typeof window.setHeaderReference !== 'function') {
    window.setHeaderReference = function (reference) {
        dotNetReference = reference;
    };
}

if (typeof window.setHeaderUserInfo !== 'function') {
    window.setHeaderUserInfo = function (userName, useremail, profileUrl) {
        if (dotNetReference) {
            dotNetReference.invokeMethodAsync('SetUserInfo', userName, useremail, profileUrl);
            return true;
        }
        return false;
    };
}
// ===== Image Cropping Functions =====

let cropper = null;

if (typeof window.showCropper !== 'function') {
    window.showCropper = function (imageSrc) {
        try {
            if (cropper) {
                cropper.destroy();
                cropper = null;
            }
            const imageElement = document.getElementById('imageToCrop');
            if (!imageElement) {
                console.error('Image element not found');
                return;
            }
            imageElement.src = imageSrc;
            setTimeout(() => {
                cropper = new Cropper(imageElement, {
                    aspectRatio: 1,
                    viewMode: 1,
                    dragMode: 'move',
                    autoCropArea: 0.8,
                    restore: false,
                    guides: true,
                    center: true,
                    highlight: true,
                    cropBoxMovable: true,
                    cropBoxResizable: true,
                    toggleDragModeOnDblclick: false,
                    ready: function () {
                        cropper.resize();
                    }
                });
            }, 300);
        } catch (error) {
            console.error('Error initializing cropper:', error);
        }
    };
}



if (typeof window.getCroppedImageBase64 !== 'function') {
    window.getCroppedImageBase64 = function () {
        return new Promise((resolve, reject) => {
            try {
                if (!cropper) {
                    console.error("Cropper not initialized");
                    return reject("Cropper not initialized");
                }

                const canvas = cropper.getCroppedCanvas({
                    width: 300,
                    height: 300,
                    imageSmoothingEnabled: true,
                    imageSmoothingQuality: 'high'
                });

                if (!canvas) {
                    console.error("Could not create cropped canvas");
                    return reject("Could not create cropped canvas");
                }

                setTimeout(() => {
                    try {
                        const result = canvas.toDataURL('image/jpeg', 0.85); // 85% quality
                        console.log("Cropped image size:", result.length);
                        resolve(result);
                    } catch (e) {
                        console.error("Error in toDataURL:", e);
                        reject("Error in toDataURL");
                    }
                }, 100); // Small delay
            } catch (e) {
                console.error("Error in getCroppedImageBase64:", e);
                reject("Error in getCroppedImageBase64");
            }
        });
    };
}

if (typeof window.destroyCropper !== 'function') {
    window.destroyCropper = function () {
        if (cropper) {
            cropper.destroy();
            cropper = null;
            // Clear the image source
            const imageElement = document.getElementById('imageToCrop');
            if (imageElement) {
                imageElement.src = "";
            }
            console.log('Cropper destroyed');
        }
    };
}

// ===== Utility Functions =====

function clickHiddenColorPicker() {
    const element = document.getElementById("inputColorPicker");
    if (element) {
        element.focus();
    }
}

// ===== File Input Reset Function =====

if (typeof window.resetFileInput !== 'function') {
    window.resetFileInput = function (inputId) {
        const fileInput = document.getElementById(inputId);
        if (fileInput) {
            fileInput.value = '';
        }
    };
}


window.compressAndReturnImageBase64 = async (inputFileElementId) => {
    const fileInput = document.getElementById(inputFileElementId);
    if (!fileInput || fileInput.files.length === 0) return null;
    fileInput.src = "";
    const _img = document.getElementById("imageToCrop");
    _img.src = "";

    const file = fileInput.files[0];
    const img = new Image();

    const reader = new FileReader();
    const imageLoadPromise = new Promise((resolve, reject) => {
        reader.onload = (e) => {
            img.onload = () => resolve();
            img.onerror = reject;
            img.src = e.target.result;
        };
        reader.onerror = reject;
    });

    reader.readAsDataURL(file);
    await imageLoadPromise;

    // Resize logic
    const MAX_SIZE = 1024;
    const scale = Math.min(MAX_SIZE / img.width, MAX_SIZE / img.height, 1);
    const canvas = document.createElement("canvas");
    canvas.width = img.width * scale;
    canvas.height = img.height * scale;

    const ctx = canvas.getContext("2d");
    ctx.drawImage(img, 0, 0, canvas.width, canvas.height);

    // Export as JPEG with 70% quality
    const base64 = canvas.toDataURL("image/jpeg", 0.7); // 70% quality

    // Calculate actual size of image data in bytes
    const base64Data = base64.split(',')[1];
    const sizeInBytes = Math.round((base64Data.length * 3) / 4);

    const sizeInKB = sizeInBytes / 1024;
    const sizeInMB = sizeInKB / 1024;

    console.log(`Size: ${sizeInBytes} bytes`);
    console.log(`Size: ${sizeInKB.toFixed(2)} KB`);
    console.log(`Size: ${sizeInMB.toFixed(3)} MB`);

    return base64;
};


//-------------- end image crop --------////


// ===== Header Component Reference Management =====

// Global reference to the header component


if (typeof window.setHeaderReference !== 'function') {
    window.setHeaderReference = function (reference) {
        dotNetReference = reference;
    };
}

if (typeof window.setHeaderUserInfo !== 'function') {
    window.setHeaderUserInfo = function (userName, useremail, profileUrl) {
        if (dotNetReference) {
            dotNetReference.invokeMethodAsync('SetUserInfo', userName, useremail, profileUrl);
            return true;
        }
        return false;
    };
}

// ===== Dropdown Management (merged from dropdown.js) =====

let dropdownElement = null;
let dropdownComponentRef = null;
let dropdownClickHandler = null;

if (typeof window.initializeDropdown !== 'function') {
    window.initializeDropdown = function (element, dotNetRef) {
        dropdownElement = element;
        dropdownComponentRef = dotNetRef;

        // Remove any existing event listener
        if (dropdownClickHandler) {
            document.removeEventListener('click', dropdownClickHandler);
        }

        // Create new click handler
        dropdownClickHandler = function (event) {
            // Check if the click is outside the dropdown container
            if (dropdownElement && !dropdownElement.contains(event.target)) {
                // Call the Blazor component to close the dropdown
                dropdownComponentRef.invokeMethodAsync('CloseDropdown');
            }
        };

        // Add event listener to document
        document.addEventListener('click', dropdownClickHandler);
    };
}