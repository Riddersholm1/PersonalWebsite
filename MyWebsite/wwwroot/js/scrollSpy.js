let scrollHandler;
let resizeHandler;
let dotNetRef;
let sectionIds = [];

function getActiveSection() {
    const offset = 120;
    let active = sectionIds[0] ?? "home";

    for (const sectionId of sectionIds) {
        const element = document.getElementById(sectionId);
        if (!element) {
            continue;
        }

        const rect = element.getBoundingClientRect();
        if (rect.top <= offset) {
            active = sectionId;
        }
    }

    return active;
}

function notifyActiveSection() {
    if (!dotNetRef) {
        return;
    }

    const activeSection = getActiveSection();
    dotNetRef.invokeMethodAsync("SetActiveSection", activeSection);
}

export function initializeScrollSpy(reference, ids) {
    dotNetRef = reference;
    sectionIds = ids ?? [];

    scrollHandler = () => requestAnimationFrame(notifyActiveSection);
    resizeHandler = () => requestAnimationFrame(notifyActiveSection);

    window.addEventListener("scroll", scrollHandler, { passive: true });
    window.addEventListener("resize", resizeHandler);

    notifyActiveSection();
}

export function disposeScrollSpy() {
    if (scrollHandler) {
        window.removeEventListener("scroll", scrollHandler);
        scrollHandler = undefined;
    }

    if (resizeHandler) {
        window.removeEventListener("resize", resizeHandler);
        resizeHandler = undefined;
    }

    dotNetRef = undefined;
    sectionIds = [];
}
