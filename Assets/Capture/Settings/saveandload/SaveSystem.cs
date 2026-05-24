using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Unity.VisualScripting;

[Serializable]
public class vgcSettings
{
    public string captureCardName;
    public float fps;
    public string aspectRatio;
    public int resolutionHight;
    public int resolutionWight;

    public string audioInput = "";
    public string audioOutput = "";
    public float audioVolume = 1f;
    public float audioLatency = 0.08f;
    public float audioBufferSize = 0.5f;
    public bool restartAudio = true;
    public float autoRestartAudioEverySeconds = 1800f;
    public int savedAudioAPI = 0;

    public int lastRangeMode = 0;
    public float lastBrightness = 0f;
    public float lastContrast = 1f;
    public float lastSaturation = 1f;

    public bool settingsOpen = true;
    public bool checkForUpdates = true;
}
public class SaveSystem : MonoBehaviour
{
    string filePath;
    static vgcSettings loadedSettings;


    public VideoGameCaptureController vgcc;

    void Start()
    {
        filePath = Path.Combine(vgcc.applicationPath, "Settings.json");
        loadSettings();
    }

    public void loadSettings()
    {
        if (File.Exists(filePath))
        {
            Debug.Log("Settings found on: " + filePath);
            string json = File.ReadAllText(filePath);
            Debug.Log("Settings json string: " + json);
            loadedSettings = JsonUtility.FromJson<vgcSettings>(json);

            Debug.Log("Settings:");
            Debug.Log("captureCardName: " + loadedSettings.captureCardName);
            Debug.Log("fps: " + loadedSettings.fps);
            Debug.Log("aspectRatio: " + loadedSettings.aspectRatio);
            Debug.Log("resolutionWight: " + loadedSettings.resolutionWight);
            Debug.Log("resolutionHight: " + loadedSettings.resolutionHight);
            Debug.Log("audioInput: " + loadedSettings.audioInput);
            Debug.Log("audioVolume: " + loadedSettings.audioVolume);

            if (loadedSettings.restartAudio)
            {
                Debug.Log("Restart Auto is on!");
                Debug.Log($"autoRestartAudioEverySeconds: {loadedSettings.autoRestartAudioEverySeconds}");

            }
            //applying settings after small delay...
            Invoke(nameof(applySettings), 0.15f);
            return;
        }
        Debug.Log("No Settings found. (maybe first time starting), using default settings");
        loadedSettings = new vgcSettings();
    }

    public void applySettings()
    {
        Debug.Log("Applying Settings");
        vgcc.startCapture.setCaptureCard(loadedSettings.captureCardName);
        Debug.Log("Capturecard loaded");

        vgcc.startCapture.aspectRatioDropdown.changeAspectRatio(loadedSettings.aspectRatio);
        Debug.Log("AspectRatio loaded");

        vgcc.startCapture.setResolution(loadedSettings.resolutionWight, loadedSettings.resolutionHight);
        Debug.Log("Resolution loaded");

        vgcc.startCapture.setFps((int)loadedSettings.fps);
        Debug.Log("FPS loaded");


        vgcc.colorController.SetRangeMode(loadedSettings.lastRangeMode);
        vgcc.colorController.SetContrast(loadedSettings.lastContrast);
        vgcc.colorController.SetBrightness(loadedSettings.lastBrightness);
        vgcc.colorController.SetSaturation(loadedSettings.lastSaturation);

        //savedAudioAPI

        vgcc.audioPureFMOD.targetLatencySeconds = loadedSettings.audioLatency;
        Debug.Log($"Audio Delay loaded: {vgcc.audioPureFMOD.targetLatencySeconds} seconds");
        vgcc.audioPureFMOD.bufferSizeSeconds = loadedSettings.audioBufferSize;
        Debug.Log($"Audio Buffer Size: {vgcc.audioPureFMOD.bufferSizeSeconds} seconds");


        vgcc.audioPureFMOD.SetVolume(loadedSettings.audioVolume);
        Debug.Log("AudioVolume loaded");

        Debug.Log($"AudioInput: {vgcc.audioPureFMOD.getAudioSources().IndexOf(loadedSettings.audioInput)}");
        vgcc.audioPureFMOD.StartCaptureEngine(vgcc.audioPureFMOD.getAudioSources().IndexOf(loadedSettings.audioInput));

        Debug.Log($"AudioOutput loaded {vgcc.audioPureFMOD.getOutputSources().IndexOf(loadedSettings.audioOutput)}");
        vgcc.audioPureFMOD.SetAudioOutputDevice(vgcc.audioPureFMOD.getOutputSources().IndexOf(loadedSettings.audioOutput));

        Debug.Log($"Restart audio loaded: {loadedSettings.restartAudio}");
    }

    public void saveSettingsToJson()
    {
        //only save when user has a capture card selected
        if (vgcc.startCapture.webCameraTexture != null)
        {

            loadedSettings.captureCardName = vgcc.startCapture.webCameraTexture.deviceName;
            loadedSettings.fps = vgcc.startCapture.webCameraTexture.requestedFPS;
            loadedSettings.resolutionWight = vgcc.startCapture.webCameraTexture.requestedWidth;
            loadedSettings.resolutionHight = vgcc.startCapture.webCameraTexture.requestedHeight;
            loadedSettings.aspectRatio = vgcc.startCapture.aspectRatioDropdown.selectedAspectRatio;

            loadedSettings.lastRangeMode = vgcc.colorController.lastRangeMode;
            loadedSettings.lastContrast = vgcc.colorController.lastContrast;
            loadedSettings.lastBrightness = vgcc.colorController.lastBrightness;
            loadedSettings.lastSaturation = vgcc.colorController.lastSaturation;

            loadedSettings.audioLatency = vgcc.audioPureFMOD.targetLatencySeconds;
            loadedSettings.audioBufferSize = vgcc.audioPureFMOD.bufferSizeSeconds;
            loadedSettings.audioInput = vgcc.audioPureFMOD.lastUsedAudioSource;
            loadedSettings.audioVolume = vgcc.audioPureFMOD.volume;          
            loadedSettings.audioOutput = vgcc.audioPureFMOD.lastUsedAudioOutput;

            loadedSettings.settingsOpen = vgcc.getSettingsMenuOpen();

            string json = JsonUtility.ToJson(loadedSettings);
            Debug.Log(json);
            File.WriteAllText(filePath, json);

            Debug.Log(filePath);
        }

    }
    private void OnApplicationQuit()
    {
        saveSettingsToJson();
    }

    public vgcSettings getSetting()
    {
        return loadedSettings;
    }

    public void setSetting(vgcSettings settings)
    {
        loadedSettings = settings;
    }
    public bool settingsExist()
    {
        return File.Exists(Path.Combine(vgcc.applicationPath, "Settings.json"));
    }
}
