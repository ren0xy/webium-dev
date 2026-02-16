# Unity Project Setup

> Step-by-step guide for setting up the webium-dev Unity project — the development environment for [webium](https://github.com/ren0xy/webium). After following these steps you'll have a working Unity Editor workspace linked to a local webium clone.

## Prerequisites

- **Unity 6+** (6000.0 or newer), installed via Unity Hub
- **Git** on PATH
- **Node.js** and **npm** on PATH (for building the JS bundle)
- **.NET SDK** on PATH (for `dotnet build` / `dotnet test`)

## Folder Structure

Both repositories must be cloned as siblings in the same parent directory:

```
parent-folder/
  webium/          ← the Webium UPM package
  webium-dev/      ← this Unity project
```

The Unity package manifest (`Packages/manifest.json`) references webium via a relative `file:` path. If the repos aren't siblings, the reference won't resolve.

## Automated Setup

Setup scripts handle npm dependency installation, JS bundle building, and resource copying. Run one of:

**PowerShell (Windows):**

```powershell
.\scripts\setup.ps1
```

**Bash (macOS / Linux):**

```bash
./scripts/setup.sh
```

### What the Scripts Do

1. Verify the sibling `../webium` clone exists
2. Run `npm install` in `webium/packages/core`
3. Run `npm run build:bundle` to produce `webium/build/webium-bootstrap.js`
4. Copy the bundle to `webium/Resources/webium-bootstrap.txt` (Unity TextAsset)

If the script reports "Webium clone not found", clone webium first:

```bash
cd ..
git clone https://github.com/ren0xy/webium.git
```

Then re-run the setup script.

## Manual Setup

If you prefer to run the steps yourself instead of using the scripts:

1. **Clone webium** as a sibling folder (if not already done):

   ```bash
   cd ..
   git clone https://github.com/ren0xy/webium.git
   ```

2. **Install npm dependencies and build the JS bundle:**

   ```bash
   cd ../webium/packages/core
   npm install
   npm run build:bundle
   ```

3. **Copy the bundle as a Unity TextAsset:**

   ```bash
   mkdir -p ../webium/Resources
   cp ../webium/build/webium-bootstrap.js ../webium/Resources/webium-bootstrap.txt
   ```

   The `.txt` extension is required so Unity treats it as a `TextAsset`. The C# runtime loads it via `Resources.Load<TextAsset>("webium-bootstrap")`.

4. **Open the webium-dev project** in Unity Hub and let it compile.

## OpenUPM Scoped Registry

The project's `Packages/manifest.json` includes a pre-configured OpenUPM scoped registry for PuerTS (the JavaScript runtime used in Unity):

```json
{
  "scopedRegistries": [
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": ["com.tencent.puerts"]
    }
  ]
}
```

No manual registry configuration is needed — Unity resolves PuerTS automatically on first open.

## Verification Checklist

After setup, confirm the following in the Unity Editor:

- [ ] Unity compiles with no errors in the Console window
- [ ] PuerTS (`com.tencent.puerts.core`) appears in Window → Package Manager
- [ ] TextMeshPro and UGUI appear in Package Manager (auto-resolved from webium's dependencies)
- [ ] `webium/Resources/webium-bootstrap.txt` exists and contains JS code
- [ ] Opening a test scene and pressing Play shows rendered HTML content

## Rebuilding the JS Bundle

The bundle must be rebuilt whenever `@webium/core` TypeScript source changes:

```bash
cd ../webium/packages/core
npm run build:bundle
```

Then re-copy to Resources:

```bash
cp ../webium/build/webium-bootstrap.js ../webium/Resources/webium-bootstrap.txt
```

Or re-run the setup script — it's safe to run multiple times.

## Troubleshooting

| Problem | Fix |
|---|---|
| "Webium clone not found" from setup script | Clone webium as a sibling: `git clone https://github.com/ren0xy/webium.git ../webium` |
| Unity compilation errors after opening | Re-run the setup script to ensure the JS bundle is built and copied |
| PuerTS not found in Package Manager | Check that `Packages/manifest.json` has the OpenUPM scoped registry entry |
| `Resources/webium-bootstrap.txt` missing | Run `npm run build:bundle` in `webium/packages/core` and copy the output |
