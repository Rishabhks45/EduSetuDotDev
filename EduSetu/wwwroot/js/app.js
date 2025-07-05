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
