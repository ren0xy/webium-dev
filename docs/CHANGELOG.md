# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.1.0] - 2026-02-16

### Added

- Initial webium-dev project setup as a Unity development shell for the [webium](https://github.com/ren0xy/webium) UPM package.
- Project structure: Unity project root with `Assets/`, `Packages/`, `ProjectSettings/`, and `UserSettings/` directories.
- Setup scripts (`scripts/setup.ps1`, `scripts/setup.sh`) for automated project initialization — clones or links the sibling webium repository into the Unity project.
- OpenUPM scoped registry configuration in `Packages/manifest.json` for PuerTS and related dependencies.
- Documentation folder with `CHANGELOG.md`, `VISION.md`, `ROADMAP.md`, and `unity-project-setup.md`.
