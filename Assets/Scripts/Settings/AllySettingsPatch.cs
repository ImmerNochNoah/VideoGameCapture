// AllySettingsPatch.cs
// VGC-Ally — Patches SaveSystem to use XDG config path on SteamOS/Linux
//
// On upstream VGC, SaveSystem writes Settings.json next to the .exe
// (applicationPath). On SteamOS this fails silently because:
//   - The Flatpak sandbox may deny writes to the app directory
//   - SteamOS immutable rootfs puts apps in read-only locations
//
// This patch component sits alongside VideoGameCaptureController and
// overrides the filePath before SaveSystem.Start() reads it.
//
// Attach to the same GameObject as VideoGameCaptureController.
//
// License: GPLv3 — fork of ImmerNochNoah/VideoGameCapture

using System;
using System.IO;
using UnityEngine;

public class AllySettingsPatch : MonoBehaviour
{
    [Tooltip("Reference to the SaveSystem component on this GameObject")]
    public SaveSystem saveSystem;

    [Tooltip("Reference to VideoGameCaptureController for applicationPath override")]
    public VideoGameCaptureController vgcc;

    private void Awake()
    {
        // Override applicationPath before SaveSystem.Start() fires
        // SaveSystem uses: Path.Combine(vgcc.applicationPath, "Settings.json")
        // We redirect vgcc.applicationPath to the XDG config dir on Linux.

#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
        string xdgConfigHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        if (string.IsNullOrEmpty(xdgConfigHome))
            xdgConfigHome = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".config");

        string configDir = Path.Combine(xdgConfigHome, "vgc-ally");
        Directory.CreateDirectory(configDir);

        // Redirect applicationPath — SaveSystem will write Settings.json here
        vgcc.applicationPath = configDir;

        Debug.Log($"[VGC-Ally] Settings redirected to XDG path: {configDir}");

        // Migrate old Settings.json from app directory if it exists
        TryMigrateOldSettings(configDir);
#endif
    }

    private void TryMigrateOldSettings(string newDir)
    {
        // Check common locations for an old settings file
        string[] oldPaths = {
            Path.Combine(
                Path.GetDirectoryName(Application.dataPath) ?? "",
                "Settings.json"),
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".steam/root/steamapps/common/VideoGameCapture/Settings.json"),
        };

        string newPath = Path.Combine(newDir, "Settings.json");
        if (File.Exists(newPath)) return;  // already migrated

        foreach (var old in oldPaths)
        {
            if (!File.Exists(old)) continue;
            try
            {
                File.Copy(old, newPath);
                Debug.Log($"[VGC-Ally] Migrated settings from {old}");
                return;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[VGC-Ally] Migration failed from {old}: {ex.Message}");
            }
        }
    }
}
