window.clipboardHelper = {
    copyText: function (text) {
        if (navigator.clipboard && navigator.clipboard.writeText) {
            return navigator.clipboard.writeText(text).then(function () {
                return true;
            }).catch(function () {
                return fallbackCopy(text);
            });
        }
        return Promise.resolve(fallbackCopy(text));
    }
};

function fallbackCopy(text) {
    try {
        var el = document.createElement('textarea');
        el.value = text;
        el.style.position = 'fixed';
        el.style.left = '-9999px';
        el.style.opacity = '0';
        document.body.appendChild(el);
        el.focus();
        el.select();
        var ok = document.execCommand('copy');
        document.body.removeChild(el);
        return ok;
    } catch (e) {
        return false;
    }
}

window.storage = {
    save: function (key, jsonStr) {
        try { localStorage.setItem(key, jsonStr); } catch {}
    },
    load: function (key) {
        try { return localStorage.getItem(key); } catch { return null; }
    },
    remove: function (key) {
        try { localStorage.removeItem(key); } catch {}
    }
};

// Theme management
window.setTheme = function (isDark) {
    if (isDark) {
        document.documentElement.classList.add("theme-dark");
        localStorage.setItem("theme", "dark");
    } else {
        document.documentElement.classList.remove("theme-dark");
        localStorage.setItem("theme", "light");
    }
};

window.getTheme = function () {
    return localStorage.getItem("theme") || "light";
};

// Scroll to element by ID
window.scrollToElement = function (elementId) {
    var element = document.getElementById(elementId);
    if (element) {
        element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
};
