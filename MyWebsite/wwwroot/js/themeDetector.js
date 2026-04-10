// Theme detector — reads the OS-level prefers-color-scheme media query and
// notifies Blazor when it changes.  Follows the namespaced init/dispose
// pattern described in CLAUDE.md so MainLayout can wire it up via JSInterop.

window.themeDetector = {
    _mql: null,
    _dotNetRef: null,
    _handler: null,

    // Returns the current OS preference (true = dark) and starts watching for
    // changes.  When the OS preference flips, invokes the [JSInvokable]
    // OnSystemThemeChanged method on the supplied .NET object reference.
    init: function (dotNetRef) {
        this._dotNetRef = dotNetRef;
        this._mql = window.matchMedia("(prefers-color-scheme: dark)");

        this._handler = function (e) {
            if (this._dotNetRef) {
                this._dotNetRef.invokeMethodAsync("OnSystemThemeChanged", e.matches);
            }
        }.bind(this);

        this._mql.addEventListener("change", this._handler);
        return this._mql.matches;
    },

    dispose: function () {
        if (this._mql && this._handler) {
            this._mql.removeEventListener("change", this._handler);
        }
        this._dotNetRef = null;
        this._mql = null;
        this._handler = null;
    }
};
