using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Editor utility to create the WebiumTest scene with the required components.
/// Run via menu: Webium > Create Test Scene
/// </summary>
public static class CreateWebiumTestScene
{
    [MenuItem("Webium/Create Test Scene")]
    public static void Create()
    {
        // Create a new empty scene
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Create the Webium GameObject
        var go = new GameObject("Webium");

        // Add WebiumSurface first (required by WebiumBootstrapper)
        var surfaceType = System.Type.GetType("Webium.Unity.WebiumSurface, Assembly-CSharp");
        if (surfaceType == null)
            surfaceType = System.Type.GetType("Webium.Unity.WebiumSurface, webium.unity.runtime");
        if (surfaceType != null)
            go.AddComponent(surfaceType);
        else
            Debug.LogWarning("WebiumSurface type not found. Ensure Webium assemblies are compiled.");

        // Add WebiumBootstrapper
        var bootstrapperType = System.Type.GetType("Webium.Unity.WebiumBootstrapper, Assembly-CSharp");
        if (bootstrapperType == null)
            bootstrapperType = System.Type.GetType("Webium.Unity.WebiumBootstrapper, webium.unity.runtime");
        if (bootstrapperType != null)
        {
            // Resolve the Webium package root dynamically via PackageInfo API
            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(
                typeof(Webium.Unity.WebiumSurface).Assembly);

            if (packageInfo == null)
            {
                Debug.LogError("Webium package not found. Ensure com.webium.core is installed.");
                return;
            }

            var examplesPath = System.IO.Path.Combine(packageInfo.resolvedPath, "examples", "hello-world");

            var bootstrapper = go.AddComponent(bootstrapperType);
            // Set _uiFolderPath via serialized property
            var so = new SerializedObject(bootstrapper);
            var prop = so.FindProperty("_uiFolderPath");
            if (prop != null)
            {
                prop.stringValue = examplesPath;
                so.ApplyModifiedProperties();
            }
        }
        else
        {
            Debug.LogWarning("WebiumBootstrapper type not found. Ensure Webium assemblies are compiled.");
        }

        // Save the scene
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/WebiumTest.unity");
        Debug.Log("WebiumTest scene created at Assets/Scenes/WebiumTest.unity");
    }
}
