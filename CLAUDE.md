# CLAUDE.md

Guidance for AI assistants (Claude Code) working in this repository.

## Project overview

Personal portfolio website for Jesper Bruhn Riddersholm. It is a **Blazor
WebAssembly** single-page application built on **.NET 10** and styled with
**MudBlazor 9.2**. The site is a single-scroll page (Danish UI) with sections
for Home, About, Skills, Experience, Projects, Education and Contact.

## Repository layout

```
PersonalWebsite/
├── PersonalWebsite.slnx          # Solution file (new XML format)
├── README.md                     # Minimal placeholder
├── LICENSE.txt
├── .github/workflows/deploy.yml  # CI: builds on push to main
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
   so the IntersectionObserver and the nav buttons can target it.
3. Use MudBlazor components (`MudContainer`, `MudStack`, `MudText`, …) for
   layout and typography – avoid hand-rolled CSS where a Mud component exists.
4. Reference the new component from `Home.razor` in the desired order.
5. Add a corresponding `MudButton Href="#<name>"` to the nav stack in
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

## CI / deployment

`.github/workflows/deploy.yml` runs on every push to `main` (and on manual
dispatch). It checks out the repo, installs `dotnet 10.0.x`, and runs
`dotnet publish MyWebsite -c Release`. There is no actual upload step yet, so
treat this as a build smoke-test for now.

When making changes, make sure `dotnet publish MyWebsite -c Release` succeeds
locally before pushing.

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
