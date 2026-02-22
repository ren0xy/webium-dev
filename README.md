# webium-dev

Development and testing environment for [Webium](https://github.com/ren0xy/webium) — a JS-first web rendering engine for Unity. This Unity project serves as the dev shell for building, testing, and iterating on the Webium UPM package using PuerTS as the JavaScript runtime.

## Status

**v0.2.0** — Dual backend sandbox with UGUI and UIElements scenes. Sandbox scenes for both render backends (HelloWorld, Interactive, Styled) with `WebiumSurfaceConfig` ScriptableObject assets for backend selection. Side-by-side comparison of UGUI and UIElements rendering for development and testing.

## Architecture

webium-dev is a standard Unity project that consumes the `webium` UPM package via a local path reference. The project structure:

```
webium-dev/              ← this repo (Unity project)
├── Assets/              ← Unity assets, test scenes, runtime resources
├── Packages/            ← manifest.json with local webium reference
├── scripts/             ← setup automation (PS1 + Bash)
├── Tests/               ← Unity test assemblies
└── docs/                ← project documentation

../webium/               ← sibling repo (UPM package, cloned separately)
```

The setup scripts handle cloning dependencies, building the JS bundle, and copying compiled artifacts into the Unity project so that pressing Play in the Editor runs Webium end-to-end.

## Dependencies / Prerequisites

- **Webium** — the sibling `webium` repository, cloned alongside this project
- **Node.js** — for npm (builds the JS bundle in `webium/packages~/core/`)
- **.NET SDK** — for `dotnet build` (builds C# bridge DLLs)
- **Unity 6+** (6000.0 or later)

## Setup

After cloning both repos side by side, run the setup script from the `scripts/` folder:

```powershell
# Windows (PowerShell)
./scripts/setup.ps1
```

```bash
# macOS / Linux
./scripts/setup.sh
```

The script:
1. Verifies `../webium` exists as a sibling directory
2. Installs npm dependencies (`packages~/core/`)
3. Builds the JS bundle and copies it to `src/Webium.Unity.Runtime/Resources/webium-bootstrap.txt`
4. Builds `Webium.Core.dll` and `Webium.JSRuntime.dll` and copies them to `Plugins/`

See [docs/unity-project-setup.md](docs/unity-project-setup.md) for detailed setup instructions and troubleshooting.

## OpenUPM Scoped Registry

This project's `Packages/manifest.json` includes an OpenUPM scoped registry for PuerTS:

```json
"scopedRegistries": [
  {
    "name": "OpenUPM",
    "url": "https://package.openupm.com",
    "scopes": ["com.tencent.puerts"]
  }
]
```

PuerTS (`com.tencent.puerts.core`) is a transitive dependency of Webium, required for JS execution in Unity. UPM does not allow packages to declare their own registries, so any project consuming Webium must include this registry block.

## Creating a Test Scene

In the Unity Editor:

**Webium → Create Test Scene**

This creates a scene with a `WebiumSurface` and `WebiumBootstrapper` pre-configured to load the `examples~/hello-world` sample from the Webium package.

## Next Milestone

v0.3.0 — Expanded testing and tooling. Integration test scenes for interactivity, editor workflow improvements, and CI-friendly validation. See [docs/ROADMAP.md](docs/ROADMAP.md) for details.

## Vision

See [docs~/VISION.md](docs/VISION.md) for the full project vision and goals.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
