window.clipboardHelper = {
    copyText: async function (text) {
        try {
            await navigator.clipboard.writeText(text);
            return true;
        } catch {
            // fallback dla starszych przeglądarek
            const el = document.createElement('textarea');
            el.value = text;
            el.style.position = 'fixed';
            el.style.opacity = '0';
            document.body.appendChild(el);
            el.select();
            const ok = document.execCommand('copy');
            document.body.removeChild(el);
            return ok;
        }
    }
};

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
