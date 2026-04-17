(function () {
    const STORAGE_KEY = "theme-mode";

    function getSystemTheme() {
        return window.matchMedia && window.matchMedia("(prefers-color-scheme: dark)").matches
            ? "dark"
            : "light";
    }


    function getStoredTheme() {
        try {
            const stored = localStorage.getItem(STORAGE_KEY);
            return stored === "dark" || stored === "light" ? stored : null;
        } catch (e) {
            return null;
        }
    }


    function getCurrentTheme() {
        const current = document.documentElement.getAttribute("data-bs-theme");
        return current === "dark" ? "dark" : "light";
    }


    function applyTheme(theme, persist) {
        document.documentElement.setAttribute("data-bs-theme", theme);
        
        const toggleButton = document.getElementById("theme-toggle");
        if (toggleButton) {
            const nextLabel = theme === "dark" ? "Light theme" : "Dark theme";
            toggleButton.setAttribute("aria-pressed", theme === "dark" ? "true" : "false");
            toggleButton.setAttribute("aria-label", nextLabel);
            toggleButton.setAttribute("title", nextLabel);

            const label = toggleButton.querySelector(".theme-label");
            if (label) {
                label.textContent = nextLabel;
            }
        }

        if (persist) {
            try {
                localStorage.setItem(STORAGE_KEY, theme);
            } catch (e) {
            }
        }
    }

    function initSuccessToasts() {
        if (typeof bootstrap === "undefined" || !bootstrap.Toast) {
            return;
        }

        const toastElements = document.querySelectorAll(".js-success-toast");

        toastElements.forEach(function (toastElement) {
            const toast = bootstrap.Toast.getOrCreateInstance(toastElement, {
                autohide: true,
                delay: 5000
            });

            toast.show();
        });
    }

    document.addEventListener("DOMContentLoaded", function () {
        applyTheme(getCurrentTheme(), false);

        const toggleButton = document.getElementById("theme-toggle");
        if (toggleButton) {
            toggleButton.addEventListener("click", function () {
                const nextTheme = getCurrentTheme() === "dark" ? "light" : "dark";
                applyTheme(nextTheme, true);
            });
        }

        if (window.matchMedia) {
            const mediaQuery = window.matchMedia("(prefers-color-scheme: dark)");

            const handleSystemThemeChange = function (e) {
                if (getStoredTheme() !== null) {
                    return;
                }

                applyTheme(e.matches ? "dark" : "light", false);
            };

            if (typeof mediaQuery.addEventListener === "function") {
                mediaQuery.addEventListener("change", handleSystemThemeChange);
            } else if (typeof mediaQuery.addListener === "function") {
                mediaQuery.addListener(handleSystemThemeChange);
            }

            if (getStoredTheme() === null) {
                applyTheme(getSystemTheme(), false);
            }
        }

        initSuccessToasts();
    });
})();