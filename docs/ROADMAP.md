# webium-dev — Roadmap

## v0.1.0 — Project Shell

Initial release. Unity development shell for the [webium](https://github.com/ren0xy/webium) UPM package.

What was delivered:
- Unity project structure with local UPM package reference to webium
- OpenUPM scoped registry configuration for PuerTS dependencies
- Cross-platform setup scripts (`scripts/setup.ps1`, `scripts/setup.sh`) that create the webium symlink
- Project documentation (CHANGELOG, VISION, ROADMAP, unity-project-setup guide)

## v0.2.0 — Development Tooling

Expand the dev environment with better test coverage and workflow tooling.

Planned direction:
- Integration test scenes exercising webium rendering and event handling
- Editor workflow improvements (custom inspectors, debug overlays)
- Expanded test coverage for setup scripts and project configuration
- CI-friendly validation (project opens cleanly, no missing references)
- Developer onboarding refinements based on contributor feedback

---

See [VISION.md](VISION.md) for the full project vision and purpose.
