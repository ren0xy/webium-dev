# Webium-Dev — Vision

> The development and testing environment for [webium](https://github.com/ren0xy/webium) — a Unity project that consumes the webium UPM package locally, providing contributors with a ready-made workspace to build, test, and iterate on the engine.

## What Is Webium-Dev?

Webium-dev is a Unity project purpose-built for developing the webium package. It contains the Unity Editor environment, test scenes, setup automation, and OpenUPM registry configuration needed to work on webium day-to-day. It is not a library, not a package, and not shipped to end users — it exists solely to make contributing to webium fast and frictionless.

## Why a Separate Repository?

Webium is a distributable UPM package. Its repository contains only the package source — C# runtime code, TypeScript core, asmdef definitions, and package metadata. It must stay clean and self-contained so consumers can install it via git URL or OpenUPM.

A UPM package cannot contain its own Unity project. Unity projects carry `ProjectSettings/`, `Packages/manifest.json`, test scenes, and editor configuration that don't belong in a distributable package. Webium-dev fills that gap: it's the Unity project that references webium as a local package (via `file:` path in the package manifest), giving contributors a real Unity environment to develop against.

Keeping them separate also means:

- **webium** stays publishable — no Unity project artifacts pollute the package.
- **webium-dev** can evolve its project settings, test scenes, and tooling independently.
- Contributors clone both repos side-by-side and run a setup script to link them.

## How It Relates to Webium

Webium-dev depends on webium, not the other way around. The relationship is:

```
webium-dev/                      # Unity project (this repo)
  Packages/manifest.json         # References webium via file: path
  Assets/Scenes/                 # Test scenes for manual verification
  scripts/                       # Setup automation

../webium/                       # UPM package (sibling repo)
  packages~/core/                 # @webium/core TypeScript package
  src/                           # C# runtime, bridge, Unity backend
  package.json                   # UPM package manifest
```

Changes to webium source are immediately reflected in the webium-dev Unity Editor — no build step, no package re-import. This tight feedback loop is the primary reason webium-dev exists.

## Target Audience

Webium-dev is for **webium contributors** — people actively working on the engine's C# bridge, TypeScript core, Unity rendering backend, or test infrastructure. If you're using webium in your own game project, you don't need this repo. If you're building or debugging webium itself, this is your workspace.

## Goals

- Provide a zero-friction setup experience: clone, run a script, open Unity.
- Keep the Unity project minimal — only what's needed to develop and test webium.
- Maintain setup scripts for both Windows (PowerShell) and Unix (Bash).
- Serve as the canonical environment for running Unity-side tests (EditMode and PlayMode).
- Stay in sync with webium's Unity version and dependency requirements.

## Non-Goals

- Not a template for end-user projects. Games using webium create their own Unity projects.
- Not a showcase or demo. Test scenes exist for development verification, not presentation.
- Not a CI runner. CI configuration may be added later, but it's not the primary purpose today.
