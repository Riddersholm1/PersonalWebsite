// Section observer — detects which page section is currently in view and
// notifies Blazor so the nav bar can highlight the active section.
// Uses IntersectionObserver because Blazor has no built-in viewport-
// visibility API.  Follows the namespaced init/dispose pattern described
// in CLAUDE.md.

window.sectionObserver = {
    _observer: null,
    _ref: null,
    _visible: {},

    init(ref) {
        this._ref = ref;
        this._visible = {};
        const self = this;
        const sections = document.querySelectorAll("section[id]");

        this._observer = new IntersectionObserver((entries) => {
            for (const e of entries) {
                self._visible[e.target.id] = e.isIntersecting;
            }

            // Among visible sections, pick the one whose top edge is closest
            // to (and at or just above) the viewport top.  This correctly
            // highlights the section the user has scrolled into, including
            // the last section at the bottom of the page.
            let active = null;
            let best = -1e9;
            for (const id in self._visible) {
                if (!self._visible[id]) continue;
                const el = document.getElementById(id);
                if (!el) continue;
                const top = el.getBoundingClientRect().top;
                if (top <= 150 && top > best) {
                    best = top;
                    active = id;
                }
            }

            // Fallback: if no section has scrolled past the threshold (e.g.
            // at the very top of the page), pick the closest one below.
            if (!active) {
                best = 1e9;
                for (const id in self._visible) {
                    if (!self._visible[id]) continue;
                    const el = document.getElementById(id);
                    if (!el) continue;
                    const top = el.getBoundingClientRect().top;
                    if (top < best) {
                        best = top;
                        active = id;
                    }
                }
            }

            if (active) {
                self._ref.invokeMethodAsync("SetActiveSection", active);
            }
        }, { threshold: 0 });

        sections.forEach((s) => this._observer.observe(s));
    },

    dispose() {
        this._observer?.disconnect();
        this._ref = null;
        this._observer = null;
        this._visible = null;
    }
};
