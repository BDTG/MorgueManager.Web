(function() {
    var saved = localStorage.getItem('morguemanager-theme');
    if (saved) {
        document.documentElement.setAttribute('data-theme', saved);
    } else {
        var prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        document.documentElement.setAttribute('data-theme', prefersDark ? 'dark' : 'dark');
    }
})();
