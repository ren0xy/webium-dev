# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.2.0] - 2026-02-22

### Added

- UGUI and UIElements sandbox scenes for side-by-side backend comparison: `HelloWorld-UGUI`, `HelloWorld-UIElements`, `Interactive-UGUI`, `Interactive-UIElements`, `Styled-UGUI`, `Styled-UIElements`.
- `WebiumSurfaceConfig` ScriptableObject assets (`WebiumConfig-UGUI.asset`, `WebiumConfig-UIElements.asset`) in `Assets/Config/`.
- Scene folder reorganization: `Assets/Scenes/UGUI/` and `Assets/Scenes/UIElements/`.
- Updated `CreateWebiumTestScene.cs` editor utility with menu items for both UGUI and UIElements scene creation.
- Interactive and styled example UI projects exercising event handling, DOM mutations, and CSS rendering features.

### Changed

- Scenes now use `WebiumSurfaceConfig` for backend selection instead of hardcoded UGUI setup.

## [0.1.0] - 2026-02-16

### Added

- Initial webium-dev project setup as a Unity development shell for the [webium](https://github.com/ren0xy/webium) UPM package.
- Project structure: Unity project root with `Assets/`, `Packages/`, `ProjectSettings/`, and `UserSettings/` directories.
- Setup scripts (`scripts/setup.ps1`, `scripts/setup.sh`) for automated project initialization — clones or links the sibling webium repository into the Unity project.
- OpenUPM scoped registry configuration in `Packages/manifest.json` for PuerTS and related dependencies.
- Documentation folder with `CHANGELOG.md`, `VISION.md`, `ROADMAP.md`, and `unity-project-setup.md`.
