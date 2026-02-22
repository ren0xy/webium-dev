using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Webium.Core;
using Webium.Unity;

/// <summary>
/// Editor utility to create Webium test scenes for both UGUI and UIElements backends,
/// and bootstrap config assets.
/// </summary>
public static class CreateWebiumTestScene
{
    // ── Scene creators ──────────────────────────────────────────────

    [MenuItem("Webium/Create Test Scene")]
    public static void CreateDefault() => CreateUGUI();

    [MenuItem("Webium/Create UGUI Test Scene")]
    public static void CreateUGUI()
        => CreateScene("UGUI", "Assets/Config/WebiumConfig-UGUI.asset", "hello-world");

    [MenuItem("Webium/Create UIElements Test Scene")]
    public static void CreateUIElements()
        => CreateScene("UIElements", "Assets/Config/WebiumConfig-UIElements.asset", "hello-world");

    /// <summary>
    /// Creates a new scene with WebiumSurface + WebiumBootstrapper, assigns the
    /// config asset and UI folder path, and saves to Assets/Scenes/{backendName}/.
    /// </summary>
    private static void CreateScene(string backendName, string configAssetPath, string exampleFolder)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        var go = new GameObject("Webium");

        // Add WebiumSurface (required by WebiumBootstrapper)
        var surfaceType = System.Type.GetType("Webium.Unity.WebiumSurface, Assembly-CSharp");
        if (surfaceType == null)
            surfaceType = System.Type.GetType("Webium.Unity.WebiumSurface, webium.unity.runtime");

        if (surfaceType != null)
        {
            var surface = go.AddComponent(surfaceType);

            // Assign config asset via serialized property
            var config = AssetDatabase.LoadAssetAtPath<WebiumSurfaceConfig>(configAssetPath);
            if (config != null)
            {
                var so = new SerializedObject(surface);
                var configProp = so.FindProperty("_config");
                if (configProp != null)
                {
                    configProp.objectReferenceValue = config;
                    so.ApplyModifiedProperties();
                }
            }
            else
            {
                Debug.LogWarning($"Config asset not found at {configAssetPath}. Run Webium > Setup > Create Config Assets first.");
            }
        }
        else
        {
            Debug.LogWarning("WebiumSurface type not found. Ensure Webium assemblies are compiled.");
        }

        // Add WebiumBootstrapper + set UI folder path (relative — resolved at runtime)
        var bootstrapperType = System.Type.GetType("Webium.Unity.WebiumBootstrapper, Assembly-CSharp");
        if (bootstrapperType == null)
            bootstrapperType = System.Type.GetType("Webium.Unity.WebiumBootstrapper, webium.unity.runtime");

        if (bootstrapperType != null)
        {
            var bootstrapper = go.AddComponent(bootstrapperType);
            var bso = new SerializedObject(bootstrapper);
            var pathProp = bso.FindProperty("_uiFolderPath");
            if (pathProp != null)
            {
                pathProp.stringValue = $"examples~/{exampleFolder}";
                bso.ApplyModifiedProperties();
            }
        }
        else
        {
            Debug.LogWarning("WebiumBootstrapper type not found. Ensure Webium assemblies are compiled.");
        }

        // Save scene
        var savePath = $"Assets/Scenes/{backendName}/Test-{backendName}.unity";
        EnsureDirectoryExists(savePath);
        EditorSceneManager.SaveScene(scene, savePath);
        Debug.Log($"Scene created at {savePath}");
    }

    // ── Config asset bootstrap ──────────────────────────────────────

    [MenuItem("Webium/Setup/Create Config Assets")]
    public static void CreateConfigAssets()
    {
        CreateConfigAsset("Assets/Config/WebiumConfig-UGUI.asset", RenderBackendType.UGUI);
        CreateConfigAsset("Assets/Config/WebiumConfig-UIElements.asset", RenderBackendType.UIElements);
        AssetDatabase.SaveAssets();
        Debug.Log("Config assets created in Assets/Config/");
    }

    private static void CreateConfigAsset(string path, RenderBackendType type)
    {
        if (AssetDatabase.LoadAssetAtPath<WebiumSurfaceConfig>(path) != null)
        {
            Debug.Log($"Config asset already exists at {path}, skipping.");
            return;
        }

        var config = ScriptableObject.CreateInstance<WebiumSurfaceConfig>();
        config.backendType = type;

        // For UIElements, create a PanelSettings asset with the default runtime theme
        if (type == RenderBackendType.UIElements)
        {
            var panelSettingsPath = "Assets/Config/WebiumPanelSettings-UIElements.asset";
            var panelSettings = AssetDatabase.LoadAssetAtPath<UnityEngine.UIElements.PanelSettings>(panelSettingsPath);
            if (panelSettings == null)
            {
                panelSettings = ScriptableObject.CreateInstance<UnityEngine.UIElements.PanelSettings>();
                panelSettings.scaleMode = UnityEngine.UIElements.PanelScaleMode.ScaleWithScreenSize;
                panelSettings.referenceResolution = new Vector2Int(1920, 1080);

                // Try to find the default runtime theme
                var theme = FindDefaultRuntimeTheme();
                if (theme != null)
                    panelSettings.themeStyleSheet = theme;
                else
                    Debug.LogWarning("No ThemeStyleSheet found in the project. " +
                        "Create one via Assets > Create > UI Toolkit > Default Runtime Theme File, " +
                        $"then assign it to the PanelSettings at {panelSettingsPath}.");

                EnsureDirectoryExists(panelSettingsPath);
                AssetDatabase.CreateAsset(panelSettings, panelSettingsPath);
            }
            config.uiElementsPanelSettings = panelSettings;
        }

        EnsureDirectoryExists(path);
        AssetDatabase.CreateAsset(config, path);
    }

    // ── Theme discovery ────────────────────────────────────────────

    /// <summary>
    /// Searches for a ThemeStyleSheet in known paths, then falls back to
    /// a project-wide asset search.
    /// </summary>
    private static UnityEngine.UIElements.ThemeStyleSheet FindDefaultRuntimeTheme()
    {
        var searchPaths = new[]
        {
            "Assets/Config/UnityDefaultRuntimeTheme.tss",
            "Assets/UI Toolkit/UnityThemes/UnityDefaultRuntimeTheme.tss",
        };
        foreach (var path in searchPaths)
        {
            var theme = AssetDatabase.LoadAssetAtPath<UnityEngine.UIElements.ThemeStyleSheet>(path);
            if (theme != null) return theme;
        }
        // Fallback: search by type
        var guids = AssetDatabase.FindAssets("t:ThemeStyleSheet");
        if (guids.Length > 0)
            return AssetDatabase.LoadAssetAtPath<UnityEngine.UIElements.ThemeStyleSheet>(
                AssetDatabase.GUIDToAssetPath(guids[0]));
        return null;
    }

    // ── Helpers ─────────────────────────────────────────────────────

    /// <summary>
    /// Ensures all parent directories for the given asset path exist,
    /// creating them via AssetDatabase.CreateFolder as needed.
    /// </summary>
    private static void EnsureDirectoryExists(string assetPath)
    {
        var dir = System.IO.Path.GetDirectoryName(assetPath);
        if (string.IsNullOrEmpty(dir) || AssetDatabase.IsValidFolder(dir))
            return;

        // Walk up to find the first existing parent, then create downward
        var parts = dir.Replace("\\", "/").Split('/');
        var current = parts[0]; // "Assets"
        for (int i = 1; i < parts.Length; i++)
        {
            var next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }
}
