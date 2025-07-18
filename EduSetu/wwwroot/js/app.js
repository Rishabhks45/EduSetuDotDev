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
