// FitTrack UI enhancements. Everything here is progressive: pages work without it,
// and all motion respects prefers-reduced-motion.
(() => {
    const root = document.documentElement;
    const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    // Enable reveal styles before first paint of the body content to avoid a flash.
    if ('IntersectionObserver' in window && !reduceMotion) {
        root.classList.add('js-reveal');
    }

    document.addEventListener('DOMContentLoaded', () => {
        initNavAndProgress();
        initReveal();
        initCountUp();
        initSpotlight();
        initSubmitState();
    });

    // Sticky nav condenses on scroll; the top bar shows reading progress.
    // A single passive listener, coalesced to one update per animation frame.
    function initNavAndProgress() {
        const nav = document.querySelector('.fittrack-nav');
        const bar = document.querySelector('.fit-progress');
        let ticking = false;

        const update = () => {
            const max = root.scrollHeight - window.innerHeight;
            const y = window.scrollY;
            nav?.classList.toggle('is-scrolled', y > 8);
            bar?.style.setProperty('--fit-scroll', max > 0 ? (y / max).toFixed(4) : 0);
            ticking = false;
        };

        window.addEventListener('scroll', () => {
            if (!ticking) {
                ticking = true;
                requestAnimationFrame(update);
            }
        }, { passive: true });
        update();

        // Show an indeterminate bar while navigating to another page in the app.
        document.addEventListener('click', (event) => {
            const link = event.target.closest('a[href]');
            if (!link || !bar || event.defaultPrevented || event.button !== 0 ||
                event.metaKey || event.ctrlKey || event.shiftKey || link.target === '_blank' ||
                link.origin !== location.origin || link.hash && link.pathname === location.pathname) {
                return;
            }
            bar.classList.add('is-loading');
        });
        window.addEventListener('pageshow', () => bar?.classList.remove('is-loading'));
    }

    // Staggered fade-up for cards, panels, and table rows as they enter the viewport.
    function initReveal() {
        if (!root.classList.contains('js-reveal')) {
            return;
        }

        const selector = [
            '.summary-card', '.dashboard-panel', '.progress-chart-panel', '.details-card',
            '.bmi-card', '.app-form', '.filter-toolbar', '.table-responsive', '.empty-state-panel',
            '.landing-feature-card', '.landing-how', '.landing-portfolio', '.landing-heading',
            '.landing-step', '.tech-pill', '.auth-card', '.status-page'
        ].join(',');

        const observer = new IntersectionObserver((entries) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('is-visible');
                    observer.unobserve(entry.target);
                }
            });
        }, { rootMargin: '0px 0px -8% 0px', threshold: 0.08 });

        // Siblings within the same parent cascade in; delay caps out so long lists stay snappy.
        const siblingIndex = new Map();
        document.querySelectorAll(selector).forEach((el) => {
            const group = el.parentElement?.parentElement ?? el.parentElement;
            const index = siblingIndex.get(group) ?? 0;
            siblingIndex.set(group, index + 1);
            el.dataset.reveal = '';
            el.style.setProperty('--reveal-delay', `${Math.min(index, 6) * 70}ms`);
            observer.observe(el);
        });
    }

    // Animate dashboard numbers from zero, preserving decimals and any trailing unit markup.
    function initCountUp() {
        if (reduceMotion) {
            return;
        }

        document.querySelectorAll('.summary-value, .bmi-value').forEach((el) => {
            const textNode = [...el.childNodes].find((node) => node.nodeType === Node.TEXT_NODE && node.textContent.trim());
            const match = textNode?.textContent.trim().match(/^-?[\d.,]+$/);
            if (!match) {
                return;
            }

            const raw = textNode.textContent.trim();
            const target = Number.parseFloat(raw.replace(/,/g, ''));
            const decimals = (raw.split('.')[1] || '').length;
            if (!Number.isFinite(target)) {
                return;
            }

            const trailing = textNode.textContent.match(/\s*$/)[0];
            const duration = 1100;
            let start;

            const step = (now) => {
                start ??= now;
                const t = Math.min((now - start) / duration, 1);
                const eased = 1 - Math.pow(1 - t, 4);
                textNode.textContent = (target * eased).toFixed(decimals) + trailing;
                if (t < 1) {
                    requestAnimationFrame(step);
                }
            };

            textNode.textContent = (0).toFixed(decimals) + trailing;
            requestAnimationFrame(step);
        });
    }

    // Cursor-following glow on cards. Uses delegated pointer events and CSS variables,
    // so there is one listener for the whole page and no layout thrash.
    function initSpotlight() {
        if (!window.matchMedia('(hover: hover)').matches) {
            return;
        }

        const selector = '.summary-card, .dashboard-panel, .landing-feature-card, .details-card, .app-form, .progress-chart-panel, .bmi-card';
        document.querySelectorAll(selector).forEach((el) => el.classList.add('has-spotlight'));

        let pending = null;
        document.addEventListener('pointermove', (event) => {
            const card = event.target.closest?.('.has-spotlight');
            if (!card) {
                return;
            }
            if (pending) {
                cancelAnimationFrame(pending);
            }
            pending = requestAnimationFrame(() => {
                const rect = card.getBoundingClientRect();
                card.style.setProperty('--mx', `${event.clientX - rect.left}px`);
                card.style.setProperty('--my', `${event.clientY - rect.top}px`);
                pending = null;
            });
        }, { passive: true });
    }

    // Give submitted forms a clear busy state while the request is processed.
    function initSubmitState() {
        document.querySelectorAll('form[method="post"]').forEach((form) => {
            form.addEventListener('submit', () => {
                const submitButton = form.querySelector('button[type="submit"]');

                // Let jQuery validation block invalid submissions without locking the button.
                if (!submitButton || form.dataset.submitting === 'true' ||
                    (window.jQuery && jQuery(form).data('validator') && !jQuery(form).valid())) {
                    return;
                }

                form.dataset.submitting = 'true';
                submitButton.disabled = true;
                submitButton.setAttribute('aria-busy', 'true');
                const label = submitButton.textContent.trim();
                const busyText = label === 'Logout' ? 'Signing out…' : label === 'Login' ? 'Signing in…' : 'Saving…';
                [...submitButton.childNodes]
                    .filter((node) => node.nodeType === Node.TEXT_NODE && node.textContent.trim())
                    .forEach((node, i) => { node.textContent = i === 0 ? ` ${busyText}` : ''; });
            });
        });
    }
})();
