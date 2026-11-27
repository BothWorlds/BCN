var App = App || {};

App.BCNBDTTheme = (function () {
    let revealObserver = null;

    function initHeader() {
        const header = document.querySelector('.site-header');
        if (!header) {
            return;
        }

        if (header.dataset.bcnHeaderInit === 'true') {
            return;
        }

        header.dataset.bcnHeaderInit = 'true';

        window.addEventListener('scroll', function () {
            if (window.scrollY > 0) {
                if (!header.classList.contains('scrolled')) {
                    header.classList.add('scrolled');
                }
            } else {
                header.classList.remove('scrolled');
            }
        });
    };

    function initNavigation() {
        const body = document.body;
        const navToggle = document.querySelector('[data-nav-toggle]');
        const navLinks = document.querySelector('[data-nav-links]');

        if (!body || !navToggle || !navLinks) {
            return;
        }

        if (navLinks.dataset.bcnNavInit === 'true') {
            return;
        }

        navLinks.dataset.bcnNavInit = 'true';

        navToggle.addEventListener('click', function () {
            const isOpen = body.classList.toggle('nav-open');
            navToggle.setAttribute('aria-expanded', String(isOpen));
            navToggle.textContent = isOpen ? 'Close' : 'Menu';
        });

        navLinks.addEventListener('click', function (event) {
            if (event.target && event.target.closest('a')) {
                body.classList.remove('nav-open');
                navToggle.setAttribute('aria-expanded', 'false');
                navToggle.textContent = 'Menu';
            }
        });
    }

    function initReveal() {
        const revealItems = document.querySelectorAll('.reveal');
        if (!revealItems.length) {
            return;
        }

        document.body.classList.add('reveal-ready');

        if (revealObserver) {
            revealObserver.disconnect();
            revealObserver = null;
        }

        if ('IntersectionObserver' in window) {
            revealObserver = new IntersectionObserver(function (entries) {
                entries.forEach(function (entry) {
                    if (entry.isIntersecting) {
                        entry.target.classList.add('is-visible');
                        revealObserver.unobserve(entry.target);
                    }
                });
            }, { threshold: 0.15 });

            revealItems.forEach(function (item) {
                if (!item.classList.contains('is-visible')) {
                    revealObserver.observe(item);
                }
            });
        }
        else {
            revealItems.forEach(function (item) {
                item.classList.add('is-visible');
            });
        }
    }

    // Public API
    return {
        init: function () {
            initHeader();
            initNavigation();
            initReveal();
        }
    };
})();