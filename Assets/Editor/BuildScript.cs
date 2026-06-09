using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// BuildScript.cs — VGC-Ally Linux IL2CPP build automation
/// 
/// Called by GitHub Actions CI/CD via:
///   -executeMethod BuildScript.BuildLinux
/// 
/// Usage:
///   Unity -quit -batchmode -executeMethod BuildScript.BuildLinux \
///     -projectPath . \
///     -logFile build/build.log
/// 
/// License: GPLv3 — fork of ImmerNochNoah/VideoGameCapture
/// </summary>
public class BuildScript
{
    private static readonly string[] SCENES = FindEnabledScenes();
    private static readonly string BUILD_PATH = "build/linux/output";
    private static readonly string BUILD_NAME = "VGCAlly";

    /// <summary>
    /// Main build entry point for GitHub Actions.
    /// Handles all build configuration and error reporting.
    /// </summary>
    public static void BuildLinux()
    {
        Debug.Log("[VGC-Ally CI] Starting Linux IL2CPP build...");

        try
        {
            // Step 1: Validate build prerequisites
            ValidateBuildEnvironment();

            // Step 2: Configure build settings
            ConfigureBuildSettings();

            // Step 3: Execute the build
            PerformBuild();

            Debug.Log("[VGC-Ally CI] Build completed successfully!");
            EditorApplication.Exit(0);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[VGC-Ally CI] Build failed: {ex.Message}\n{ex.StackTrace}");
            EditorApplication.Exit(1);
        }
    }

    /// <summary>
    /// Validate that the build environment is properly configured.
    /// </summary>
    private static void ValidateBuildEnvironment()
    {
        Debug.Log("[VGC-Ally CI] Validating build environment...");

        // Check that the native plugin exists
        string nativePluginPath = "Assets/Plugins/Linux/x86_64/libvgc_v4l2.so";
        if (!System.IO.File.Exists(nativePluginPath))
        {
            throw new System.Exception($"Native plugin not found at {nativePluginPath}. " +
                "Did the native build step succeed?");
        }

        Debug.Log($"✓ Native plugin found: {nativePluginPath}");

        // Check that required scenes exist
        if (SCENES.Length == 0)
        {
            throw new System.Exception("No enabled scenes found in build settings. " +
                "Add scenes to File > Build Settings > Scenes In Build.");
        }

        Debug.Log($"✓ Found {SCENES.Length} scene(s) to build");
    }

    /// <summary>
    /// Configure all player build settings for Linux IL2CPP.
    /// </summary>
    private static void ConfigureBuildSettings()
    {
        Debug.Log("[VGC-Ally CI] Configuring build settings...");

        // Set target platform to Linux
        EditorUserBuildSettings.selectedStandaloneTarget = StandaloneBuildTarget.Linux64;

        // Use IL2CPP scripting backend for performance
        PlayerSettings.SetScriptingBackend(
            BuildTargetGroup.Standalone,
            ScriptingImplementation.IL2CPP);

        Debug.Log("✓ Scripting backend: IL2CPP");

        // Configure IL2CPP compiler settings
        IL2CPPCompilerConfiguration compilerConfig = IL2CPPCompilerConfiguration.Release;
        PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.Standalone, compilerConfig);
        Debug.Log("✓ IL2CPP compiler: Release mode");

        // Graphics settings
        PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneLinux64,
            new GraphicsDeviceType[] {
                GraphicsDeviceType.Vulkan,  // Primary: RDNA 3 optimized
                GraphicsDeviceType.OpenGL43 // Fallback
            });
        Debug.Log("✓ Graphics: Vulkan (primary) + OpenGL43 (fallback)");

        // Resolution and quality
        PlayerSettings.defaultScreenWidth = 1920;
        PlayerSettings.defaultScreenHeight = 1080;
        PlayerSettings.runInBackground = true;
        Debug.Log("✓ Resolution: 1920x1080");

        // Build name
        PlayerSettings.productName = "VGC-Ally";
        Debug.Log("✓ Product name: VGC-Ally");

        // Disable development build for release builds
        EditorUserBuildSettings.development = false;
        EditorUserBuildSettings.allowDebugging = false;
        Debug.Log("✓ Development build: Disabled");
    }

    /// <summary>
    /// Execute the actual build process.
    /// </summary>
    private static void PerformBuild()
    {
        Debug.Log($"[VGC-Ally CI] Building to {BUILD_PATH}...");

        // Ensure output directory exists
        System.IO.Directory.CreateDirectory(BUILD_PATH);

        // Configure build options
        BuildOptions buildOptions = BuildOptions.None;
        buildOptions |= BuildOptions.StrictMode;  // Fail on any errors/warnings

        // Execute the build
        BuildReport report = BuildPipeline.BuildPlayer(
            scenes: SCENES,
            locationPathName: $"{BUILD_PATH}/{BUILD_NAME}.x86_64",
            target: BuildTarget.StandaloneLinux64,
            options: buildOptions);

        // Check for build success
        if (report.summary.result == BuildResult.Failed)
        {
            throw new System.Exception(
                $"Build failed with {report.summary.totalErrors} error(s). " +
                "See Editor.log for details.");
        }

        if (report.summary.totalErrors > 0)
        {
            Debug.LogWarning($"Build completed with {report.summary.totalErrors} warning(s).");
        }

        // Log build summary
        Debug.Log($"✓ Build successful!");
        Debug.Log($"  Output: {BUILD_PATH}/{BUILD_NAME}.x86_64");
        Debug.Log($"  Size: {report.summary.totalSize} bytes");
        Debug.Log($"  Time: {report.summary.buildSeconds}s");
    }

    /// <summary>
    /// Find all enabled scenes in the project.
    /// Falls back to finding the main scene if Build Settings is empty.
    /// </summary>
    private static string[] FindEnabledScenes()
    {
        var scenes = EditorBuildSettingsScene.GetActiveScenes(EditorSceneManager.GetSceneByPath(""));

        if (scenes.Length == 0)
        {
            Debug.LogWarning("[VGC-Ally CI] No scenes in Build Settings. Searching for main scene...");

            // Try to find Main scene
            string[] mainSceneGUIDs = AssetDatabase.FindAssets("Main t:Scene");
            if (mainSceneGUIDs.Length > 0)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(mainSceneGUIDs[0]);
                Debug.Log($"Found Main scene: {scenePath}");
                return new[] { scenePath };
            }

            // Fallback: find ANY scene
            string[] allScenes = AssetDatabase.FindAssets("t:Scene");
            if (allScenes.Length > 0)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(allScenes[0]);
                Debug.Log($"Using first available scene: {scenePath}");
                return new[] { scenePath };
            }

            return new string[] { };
        }

        return scenes;
    }
}
