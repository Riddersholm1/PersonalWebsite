"use strict";

window.sectionObserver = {
    _observer: null,
    init(dotNetRef) {
        const sections = document.querySelectorAll("section[id]");
        this._observer = new IntersectionObserver(entries => {
            for (const entry of entries) {
                if (entry.isIntersecting) {
                    dotNetRef.invokeMethodAsync("SetActiveSection", entry.target.id);
                }
            }
        }, {
            rootMargin: "0px 0px -70% 0px"
        });
        sections.forEach(s => this._observer.observe(s));
    },
    dispose() {
        this._observer?.disconnect();
        this._observer = null;
    }
};
