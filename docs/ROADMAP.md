# webium-dev — Roadmap

## v0.1.0 — Project Shell ✅

Initial release. Unity development shell for the [webium](https://github.com/ren0xy/webium) UPM package.

What was delivered:
- Unity project structure with local UPM package reference to webium
- OpenUPM scoped registry configuration for PuerTS dependencies
- Cross-platform setup scripts (`scripts/setup.ps1`, `scripts/setup.sh`) that create the webium symlink
- Project documentation (CHANGELOG, VISION, ROADMAP, unity-project-setup guide)

## v0.2.0 — Dual Backend Sandbox ✅

Expanded dev environment with sandbox scenes for both UGUI and UIElements backends.

What was delivered:
- UGUI and UIElements sandbox scenes: HelloWorld, Interactive, Styled for each backend
- `WebiumSurfaceConfig` ScriptableObject assets for backend selection
- Scene folder reorganization (`Assets/Scenes/UGUI/`, `Assets/Scenes/UIElements/`)
- Updated editor scene creator with menu items for both backends
- Interactive and styled example UI projects for testing event handling and CSS rendering

## v0.3.0 — Expanded Testing & Tooling (next)

Expand test coverage and development workflow tooling for the richer feature set in webium v0.3.0.

Planned direction:
- Integration test scenes exercising interactivity (button clicks, input fields, DOM mutations)
- Editor workflow improvements (custom inspectors, debug overlays)
- CI-friendly validation (project opens cleanly, no missing references)
- Developer onboarding refinements based on contributor feedback

---

See [VISION.md](VISION.md) for the full project vision and purpose.
