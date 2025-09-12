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