# Copilot Instructions

## Project Overview
Personal website built as a **Blazor WebAssembly** application.

- **Language:** C# 14
- **Framework:** .NET 10
- **UI Library:** [MudBlazor](https://mudblazor.com/)
- **Source Control:** GitHub
- **Hosting:** Cloudflare Pages
- **Error Monitoring:** [Sentry](https://sentry.io)

## Core Principles

### 1. MudBlazor First
- **Always prefer MudBlazor components** over plain HTML elements.
- Do **not** use `<div>`, `<span>`, `<section>`, etc. when a MudBlazor equivalent exists.
  - Use `MudContainer`, `MudStack`, `MudGrid`, `MudItem`, `MudPaper`, `MudText`, etc.
- Use the built-in MudBlazor props (e.g. `Spacing`, `Justify`, `AlignItems`, `Elevation`, `Typo`) for layout and visual configuration whenever possible.

### 2. Styling Strategy
- **All custom CSS lives in `app.css`.**
- Define reusable CSS classes in `app.css` and apply them to MudBlazor components via the `Class` attribute:
  ```razor
  <MudContainer MaxWidth="MaxWidth.Large" Class="hero-section">
      ...
  </MudContainer>
  ```
- **Do NOT use the `Style="..."` attribute** on MudBlazor components or HTML elements. Inline styles are forbidden.
- **Do NOT write `<style>` blocks** inside `.razor` files.
- Razor pages should stay clean — markup and component composition only, no styling noise.
- When a new visual treatment is needed, add a class to `app.css` first, then reference it via `Class`.

### 3. Minimal JavaScript
- **Avoid JavaScript whenever possible.** Solve problems with C#, MudBlazor, HTML, and CSS first.
- Prefer CSS-only solutions for animations, transitions, scroll effects, hover states, and responsive behavior.
- Only fall back to JavaScript / JSInterop when there is no reasonable C# or CSS alternative — and document why in the code.

### 4. Razor Page Cleanliness
- Razor markup should be **declarative and minimal**.
- Push logic into code-behind (`.razor.cs`) or services where appropriate.
- Keep components small and composable.

## Error Monitoring (Sentry)
- All unhandled exceptions and meaningful runtime errors should be captured by Sentry.
- Use the Sentry .NET / Blazor SDK and configure it during application startup in `Program.cs`.
- Do not log secrets or PII to Sentry.

## Deployment
- The site is deployed to **Cloudflare Pages** via GitHub.
- Be mindful that the output is a static WebAssembly bundle — no server-side runtime is available at request time.
- Avoid features that require a server (e.g. server-side rendering, server endpoints). Use external APIs or static data instead.

## Quick Checklist Before Suggesting Code
- [ ] Am I using a MudBlazor component instead of a raw HTML element?
- [ ] Did I avoid `Style="..."` and put any styling in `app.css` as a class?
- [ ] Did I avoid adding JavaScript when CSS or C# would work?
- [ ] Is the razor markup clean and free of inline styling?
- [ ] Are errors observable via Sentry?
