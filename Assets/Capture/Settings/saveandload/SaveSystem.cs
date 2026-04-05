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
    public string audioInput;
    public float fps;
    public string aspectRatio;
    public int resolutionHight;
    public int resolutionWight;


    public float audioVolume;

    public float audioDelay = 0.1f;
    public bool restartAudio = true;
    public float autoRestartAudioEverySeconds = 1800f;
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
            Debug.Log("audioInput: " + loadedSettings.audioInput);
            Debug.Log("fps: " + loadedSettings.fps);
            Debug.Log("aspectRatio: " + loadedSettings.aspectRatio);
            Debug.Log("resolutionWight: " + loadedSettings.resolutionWight);
            Debug.Log("resolutionHight: " + loadedSettings.resolutionHight);
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

        vgcc.startAudio.audioSource.volume = loadedSettings.audioVolume;
        Debug.Log("AudioVolume loaded");

        vgcc.startAudio.startSound(loadedSettings.audioInput);
        Debug.Log("AudioInput loaded");

        vgcc.startAudio.audioDelayFormCaptureCard = loadedSettings.audioDelay;
        Debug.Log($"Audio Delay loaded: {vgcc.startAudio.audioDelayFormCaptureCard} seconds");

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

            loadedSettings.audioInput = vgcc.startAudio.microfoneUsed;
            loadedSettings.audioVolume = vgcc.soundVolume;
            
            loadedSettings.audioDelay = vgcc.startAudio.audioDelayFormCaptureCard;

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
}
