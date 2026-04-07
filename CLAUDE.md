# CLAUDE.md

Guidance for AI assistants (Claude Code) working in this repository.

## Project overview

Personal portfolio website for Jesper Bruhn Riddersholm. It is a **Blazor
WebAssembly** single-page application built on **.NET 10** / **C# 14** and
styled with **MudBlazor 9.2**. The site is a single-scroll page (Danish UI)
with sections for Home, About, Skills, Experience, Projects, Education and
Contact.

- **Hosting:** Cloudflare Pages (static WebAssembly bundle, no server runtime)
- **Error monitoring:** Sentry (see "Error monitoring" below)
- **Source control:** GitHub

> There is also a `.github/copilot-instructions.md` aimed at GitHub Copilot.
> The principles in this CLAUDE.md are intentionally aligned with that file —
> if the two ever drift, treat the stricter rule as authoritative and update
> both.

## Repository layout

```
PersonalWebsite/
├── PersonalWebsite.slnx          # Solution file (new XML format)
├── README.md                     # Minimal placeholder
├── LICENSE.txt
├── .github/
│   ├── copilot-instructions.md   # Companion AI guide (keep in sync with this file)
│   └── workflows/deploy.yml      # CI: builds on push to main
└── MyWebsite/                    # The Blazor WASM project
    ├── MyWebsite.csproj          # net10.0, MudBlazor 9.2.0
    ├── Program.cs                # WASM host setup, registers MudBlazor + HttpClient
    ├── App.razor                 # Router with NotFoundPage
    ├── _Imports.razor            # Global @using directives
    ├── Properties/launchSettings.json
    ├── Layout/
    │   ├── MainLayout.razor      # Top app bar, theme provider, nav buttons, footer
    │   ├── MainLayout.razor.cs   # Code-behind: dark mode toggle, section observer
    │   └── Footer.razor
    ├── Pages/
    │   ├── Home.razor            # @page "/" – composes all other section components
    │   ├── About.razor           # Section components (no @page directive)
    │   ├── Skills.razor
    │   ├── Experience.razor
    │   ├── Projects.razor
    │   ├── Education.razor
    │   ├── Contact.razor
    │   └── NotFound.razor        # @page "/not-found"
    ├── Theme/
    │   ├── MudBlazorTheme.cs     # Light + dark palette factory
    │   └── MudBlazorColors.cs    # Brand color constants
    └── wwwroot/
        ├── index.html            # Loads MudBlazor css/js + sectionObserver.js
        ├── css/app.css
        ├── js/sectionObserver.js # IntersectionObserver → JSInterop
        └── images/               # Favicons / app icons
```

## How the page is structured

- The site is a **single-page scroll layout**, not a multi-route SPA. Only
  `Home.razor` and `NotFound.razor` carry `@page` directives.
- `Home.razor` renders a hero `<section id="home">` and then includes the other
  page components (`<About />`, `<Skills />`, …) directly. Each of those
  components is wrapped in its own `<section id="...">` element.
- `MainLayout.razor` shows a top `MudAppBar` whose `MudButton`s use anchor
  hrefs (`#about`, `#skills`, …) to scroll to those sections.
- `MainLayout.razor.cs` wires up `js/sectionObserver.js` via JSInterop. The JS
  uses an `IntersectionObserver` to detect the visible section and calls back
  into `[JSInvokable] SetActiveSection`, which highlights the matching nav
  button (`Variant.Filled` vs `Variant.Text`).
- Dark mode is the default and can be toggled from the app bar. The state lives
  on `MainLayout` (`_isDarkMode`).

## Core principles

These are the non-negotiable rules for any code suggestion. They mirror
`.github/copilot-instructions.md`.

### 1. MudBlazor first

- **Always prefer MudBlazor components** over plain HTML elements. Do not use
  `<div>`, `<span>`, `<p>`, `<h1>`, etc. when a MudBlazor equivalent exists
  (`MudContainer`, `MudStack`, `MudGrid`, `MudItem`, `MudPaper`, `MudText`,
  `MudButton`, `MudIcon`, …).
- Use built-in MudBlazor props (`Spacing`, `Justify`, `AlignItems`, `Elevation`,
  `Typo`, `MaxWidth`, `Variant`, `Color`, …) for layout and visual configuration
  before reaching for CSS.
- The one current exception is the `<section id="...">` wrappers used by each
  page component. They exist because `wwwroot/js/sectionObserver.js` queries
  `section[id]` to drive nav highlighting. Keep using `<section>` here until
  the observer is replaced — do **not** introduce new raw HTML elements
  elsewhere.

### 2. Styling strategy

- **All custom CSS lives in `wwwroot/css/app.css`.**
- Define reusable classes there and apply them to MudBlazor components via the
  `Class` attribute, e.g.:
  ```razor
  <MudContainer MaxWidth="MaxWidth.Large" Class="hero-section">
      ...
  </MudContainer>
  ```
- **Do NOT use the `Style="..."` attribute.** Inline styles are forbidden on
  both MudBlazor components and HTML elements.
- **Do NOT add `<style>` blocks** inside `.razor` files, and do not introduce
  scoped `*.razor.css` files (none exist today; the `MyWebsite.styles.css`
  link in `index.html` is intentionally commented out).
- When a new visual treatment is needed, add a class to `app.css` first, then
  reference it via `Class`.

### 3. Minimal JavaScript

- **Avoid JavaScript whenever possible.** Solve problems with C#, MudBlazor,
  HTML and CSS first.
- Prefer CSS-only solutions for animations, transitions, hover states,
  scroll effects and responsive behavior.
- Only fall back to JS / JSInterop when there is no reasonable C# or CSS
  alternative, and document *why* in the code. The existing
  `js/sectionObserver.js` is the canonical example: it uses
  `IntersectionObserver` because Blazor has no built-in equivalent.
- New interop modules must follow the `sectionObserver` pattern: a namespaced
  object on `window` with explicit `init` / `dispose`, registered as a
  `<script>` tag in `index.html`, and disposed from the consuming component's
  `IAsyncDisposable` implementation.

### 4. Razor page cleanliness

- Razor markup must be **declarative and minimal** — composition only, no
  styling noise, no business logic.
- Push non-trivial logic into a code-behind partial (`*.razor.cs`) or a
  service. `Layout/MainLayout.razor.cs` is the reference example.
- Keep components small and composable; one section per file under `Pages/`.

### Quick checklist before committing

- [ ] Am I using a MudBlazor component instead of a raw HTML element?
- [ ] Did I avoid `Style="..."` and put any styling in `app.css` as a class?
- [ ] Did I avoid adding JavaScript when CSS or C# would work?
- [ ] Is the razor markup clean, free of inline styling and free of `<style>`?
- [ ] Will any new exceptions be observable via Sentry?
- [ ] Does `dotnet publish MyWebsite -c Release` still succeed?

## Conventions

### Razor / C#

- Target framework: **net10.0**, `Nullable` and `ImplicitUsings` are enabled.
- Namespaces follow the folder layout: `MyWebsite`, `MyWebsite.Layout`,
  `MyWebsite.Theme`. New folders should follow the same pattern.
- Prefer **code-behind partial classes** (`*.razor.cs`) for non-trivial logic
  (see `MainLayout.razor.cs`). Keep `.razor` files focused on markup.
- Use **file-scoped namespaces** and standard C# nullable annotations.
- Common usings live in `_Imports.razor` (`MudBlazor`, `MyWebsite.Layout`,
  etc.) – don't re-import them in individual components.

### Adding a new section to the site

1. Create `MyWebsite/Pages/<Name>.razor`. **Do not** add an `@page` directive.
2. Wrap the content in `<section id="<name>" class="page-section"> … </section>`
   so the IntersectionObserver and the nav buttons can target it. This is the
   only place a raw HTML element should appear in a page component (see "Core
   principles → MudBlazor first").
3. Inside the section, use MudBlazor components (`MudContainer`, `MudStack`,
   `MudText`, `MudGrid`, …) for layout and typography. Configure them with
   MudBlazor props (`Spacing`, `Justify`, `AlignItems`, `Typo`, …).
4. If a new visual treatment is needed, add a class to `wwwroot/css/app.css`
   and apply it via `Class="..."`. Never use `Style="..."` or inline `<style>`
   blocks.
5. Reference the new component from `Home.razor` in the desired order.
6. Add a corresponding `MudButton Href="#<name>"` to the nav stack in
   `Layout/MainLayout.razor`.

### Adding a new routed page (rare)

- Add `@page "/route"` at the top of the `.razor` file. The router in
  `App.razor` uses `MainLayout` as the default layout, so a `@layout` directive
  is only needed when overriding it (see `NotFound.razor`).

### Theming & colors

- All palette values come from `Theme/MudBlazorTheme.cs`, which pulls brand
  colors from `Theme/MudBlazorColors.cs`. Add new shared colors there rather
  than hard-coding hex values inside components.
- Both `PaletteLight` and `PaletteDark` should be updated together when a
  theme change is made.

### Localization

- The visible UI is written in **Danish** (e.g. "Hjem", "Om mig",
  "Kompetencer"). Keep new user-facing strings in Danish to match.
- Code, identifiers, comments and commit messages stay in English.

### Static assets

- CSS lives in `wwwroot/css/app.css`. The MudBlazor stylesheet is loaded from
  `_content/MudBlazor/MudBlazor.min.css` in `index.html` – when MudBlazor is
  upgraded, bump the `?v=` query string on both the CSS and JS `<link>`/
  `<script>` tags in `index.html` to bust the cache (the comments in
  `index.html` reiterate this).
- Custom JS goes under `wwwroot/js/` and must be added as a `<script>` tag in
  `index.html`. Use the `sectionObserver` pattern (namespaced object on
  `window`, explicit `init`/`dispose`) for new interop modules.

## Development workflow

This project uses the standard .NET CLI. From the repo root:

```bash
# Restore dependencies
dotnet restore

# Run the dev server with hot reload
dotnet watch --project MyWebsite

# Plain run
dotnet run --project MyWebsite

# Build / publish (matches CI)
dotnet build MyWebsite -c Release
dotnet publish MyWebsite -c Release
```

There are currently **no unit tests** in this repository. If you add a test
project, place it as a sibling of `MyWebsite/` and add it to
`PersonalWebsite.slnx`.

## Error monitoring (Sentry)

- All unhandled exceptions and meaningful runtime errors should be captured by
  Sentry. Use the Sentry .NET / Blazor SDK and configure it during application
  startup in `Program.cs`.
- **Never** log secrets or PII to Sentry — scrub user-supplied content before
  attaching it to events / breadcrumbs.
- Sentry is **not yet wired up** in `Program.cs`. Until it is, treat the
  Sentry rules as the target state: any new error-handling code should be
  written so it will route through Sentry once the SDK is registered.

## Deployment

- The site is hosted on **Cloudflare Pages** as a static WebAssembly bundle.
- There is **no server-side runtime** at request time. Do not introduce
  features that need one (server-side rendering, server endpoints, SignalR,
  in-process databases, etc.). Use external HTTP APIs or static data instead.
- `.github/workflows/deploy.yml` runs on every push to `main` (and on manual
  dispatch). Today it only checks out the repo, installs `dotnet 10.0.x`, and
  runs `dotnet publish MyWebsite -c Release` — there is no upload step yet, so
  treat the workflow as a build smoke-test. Cloudflare Pages currently builds
  the site itself from the connected GitHub repo.
- Before pushing, make sure `dotnet publish MyWebsite -c Release` succeeds
  locally.

## Working in this repo as Claude

- Work on the branch you have been assigned. Do not push to `main` directly.
- Prefer editing existing files over creating new ones; only add files when a
  feature genuinely requires it.
- Read the relevant `.razor` and code-behind file before modifying a section,
  and keep changes minimal and focused on what was asked.
- Don't introduce additional NuGet packages unless necessary – the project
  intentionally relies on just `Microsoft.AspNetCore.Components.WebAssembly`
  (+ DevServer) and `MudBlazor`.
- Don't create a pull request unless explicitly asked.
