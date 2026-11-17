var App = App || {};

App.BCNSSATheme = (function () {
    let revealObserver = null;
    let revealDomObserver = null;

    function updateMetadata() {
        const path = (window.location.pathname || '/').replace(/\/+$/, '') || '/';
        const pages = {
            '/': {
                title: 'Severance Sign Art | Custom Signs in Bristol, FL',
                description: 'Custom signs, lettering, decals, banners, displays, vehicle graphics, and project support from Severance Sign Art in Bristol, Florida.'
            },
            '/contact-us': {
                title: 'Request a Sign Quote | Severance Sign Art',
                description: 'Request a custom sign quote from Severance Sign Art in Bristol, Florida. Share project size, colors, timing, photos, and artwork online.'
            },
            '/custom-signs': { title: 'Custom Sign Shop in Bristol, FL | Severance Sign Art', description: 'Custom business, directional, interior, exterior, and specialty signs made by Severance Sign Art in Bristol, Florida.' },
            '/lettering-decals': { title: 'Lettering & Decals in Bristol, FL | Severance Sign Art', description: 'Custom lettering and decals for windows, equipment, vehicles, and business identification across Northwest Florida.' },
            '/banners-displays': { title: 'Banners & Displays in Bristol, FL | Severance Sign Art', description: 'Custom banners and display graphics for promotions, events, announcements, and temporary spaces in Northwest Florida.' },
            '/vehicle-graphics': { title: 'Vehicle Graphics in Bristol, FL | Severance Sign Art', description: 'Vehicle lettering and graphics from Severance Sign Art that turn work vehicles into clear, professional mobile advertising.' },
            '/yard-event-signs': { title: 'Yard & Event Signs in Bristol, FL | Severance Sign Art', description: 'Readable custom yard and event signs for directions, promotions, campaigns, celebrations, and Northwest Florida events.' },
            '/design-project-support': { title: 'Sign Design Support in Bristol, FL | Severance Sign Art', description: 'Practical help with sign artwork, materials, sizing, layout, and project planning from a local Bristol sign shop.' },
            '/special-projects': { title: 'Special Sign Projects in Bristol, FL | Severance Sign Art', description: 'One-of-a-kind sign and graphic projects made around your message, space, and purpose in Northwest Florida.' }
        };
        const current = pages[path];
        if (!current) return;

        document.title = current.title;
        let description = document.querySelector('meta[name="description"]');
        if (!description) {
            description = document.createElement('meta');
            description.name = 'description';
            document.head.appendChild(description);
        }
        description.content = current.description;

        if (!document.getElementById('severance-local-business')) {
            const structuredData = document.createElement('script');
            structuredData.id = 'severance-local-business';
            structuredData.type = 'application/ld+json';
            structuredData.text = JSON.stringify({
                '@context': 'https://schema.org',
                '@type': 'LocalBusiness',
                name: 'Severance Sign Art',
                description: 'Custom sign shop serving Bristol and Northwest Florida.',
                telephone: '+1-850-643-5712',
                priceRange: '$$',
                address: {
                    '@type': 'PostalAddress',
                    streetAddress: '10629 NW SR 20',
                    addressLocality: 'Bristol',
                    addressRegion: 'FL',
                    postalCode: '32321',
                    addressCountry: 'US'
                },
                areaServed: 'Northwest Florida'
            });
            document.head.appendChild(structuredData);
        }
    }

    function initHeader() {
        const header = document.querySelector('.site-header');
        if (!header) {
            return;
        }

        if (header.dataset.ssaHeaderInit === 'true') {
            return;
        }

        header.dataset.ssaHeaderInit = 'true';

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

        if (navLinks.dataset.ssaNavInit === 'true') {
            return;
        }

        navLinks.dataset.ssaNavInit = 'true';

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
        document.body.classList.add('reveal-ready');

        if ('IntersectionObserver' in window) {
            if (!revealObserver) {
                revealObserver = new IntersectionObserver(function (entries) {
                    entries.forEach(function (entry) {
                        if (entry.isIntersecting) {
                            entry.target.classList.add('is-visible');
                            revealObserver.unobserve(entry.target);
                        }
                    });
                }, { threshold: 0.08 });
            }

            observeRevealItems(document);

            if (!revealDomObserver) {
                revealDomObserver = new MutationObserver(function (mutations) {
                    mutations.forEach(function (mutation) {
                        mutation.addedNodes.forEach(function (node) {
                            if (node.nodeType === 1) {
                                observeRevealItems(node);
                            }
                        });
                    });
                });
                revealDomObserver.observe(document.body, { childList: true, subtree: true });
            }
        }
        else {
            document.querySelectorAll('.reveal').forEach(function (item) {
                item.classList.add('is-visible');
            });
        }
    }

    function observeRevealItems(root) {
        const revealItems = [];
        if (root.matches && root.matches('.reveal')) {
            revealItems.push(root);
        }
        if (root.querySelectorAll) {
            root.querySelectorAll('.reveal').forEach(function (item) {
                revealItems.push(item);
            });
        }

        revealItems.forEach(function (item) {
            if (!item.classList.contains('is-visible')) {
                revealObserver.observe(item);
            }
        });
    }

    function initGalleryNavigation() {
        if (document.documentElement.dataset.ssaGalleryInit === 'true') {
            return;
        }

        document.documentElement.dataset.ssaGalleryInit = 'true';
        document.addEventListener('click', function (event) {
            const button = event.target && event.target.closest('[data-gallery-scroll]');
            if (!button) {
                return;
            }

            const gallery = button.closest('.portfolio-section');
            const track = gallery && gallery.querySelector('.portfolio-grid');
            const card = track && track.querySelector('.portfolio-card');
            if (!track || !card) {
                return;
            }

            const styles = window.getComputedStyle(track);
            const gap = parseFloat(styles.columnGap || styles.gap) || 18;
            const distance = card.getBoundingClientRect().width + gap;
            const direction = button.dataset.galleryScroll === 'previous' ? -1 : 1;
            track.scrollBy({ left: distance * direction, behavior: 'smooth' });
        });
    }

    // Public API
    return {
        init: function () {
            updateMetadata();
            initHeader();
            initNavigation();
            initReveal();
            initGalleryNavigation();
        }
    };
})();
