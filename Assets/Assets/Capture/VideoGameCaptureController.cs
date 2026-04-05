using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

/* 
 * The VideoGameCaptureController class is the "Main" class here i do most of the stuff
 * Its the first class that gets called after starting the Software.
 * 
 */
public class VideoGameCaptureController : MonoBehaviour
{

    public AudioSource audioSource;

    public WebCamDevice[] devices;
    public WebCamTexture webCameraTexture;

    public StartCapture startCapture;
    public StartAudio startAudio;

    public SettingsAnimation settingsAnimation;
    public ScreenAnimation screenAnimation;

    public VolumeBarManager volumeBarManager;

    public GameObject settingsMenu;

    public GameObject pauseScreen;

    public float soundVolume;

    public TextMeshProUGUI selectedAudioText;

    public ScreenshotManager screenshotManager;

    bool userUsingApp;

    public String applicationPath;

    public SaveSystem saveSystem;

    // Start is called before the first frame update
    void Start()
    {

        devices = WebCamTexture.devices;
        Application.targetFrameRate = 60;
        
        //will show the normal settings menu
        settingsAnimation.showSettings(true);
        Cursor.visible = true;

        //we need this to store data like screenshots and settings.
        applicationPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        Debug.Log("Application Path: " + applicationPath);


        screenshotManager.checkIfScreenshotFolderExists();
        StartCoroutine(autoRestartAudio());
        //StartCoroutine(stopAudioOnFirstStart());

    }

    // Update is called once per frame
    void Update()
    {
        appKeys();
        volumeCheck();
        takeScreenshotFromGameScene();
    }

    void appKeys()
    {
        //Audio Reset
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            openSettingsMenu();
        }

        //Switch from Full to Window mode
        if (Input.GetKeyDown(KeyCode.F5))
        {
            changeWindowMode();
        }
    }
    void volumeCheck()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            changeAudioVolume(startAudio.audioSource.volume + 0.10f);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            changeAudioVolume(startAudio.audioSource.volume - 0.10f);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            muteSound();
        }
    }

    public void changeWindowMode()
    {
        if (!Screen.fullScreen)
        {
            Screen.fullScreen = true;
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
        }
        else
        {
            Screen.fullScreen = false;
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
        }
    }

    public void muteSound()
    {
        if (startAudio.audioSource.volume != 0f)
        {
            startAudio.audioSource.volume = 0f;
        }
        else
        {
            if (soundVolume == 0f)
            {
                startAudio.audioSource.volume = 0.3f;
                return;
            }
            startAudio.audioSource.volume = soundVolume;
        }
    }

    public void changeAudioVolume(float volume)
    {
        startAudio.audioSource.volume = volume;
        soundVolume = startAudio.audioSource.volume;
        volumeBarManager.updateVolumeBar();
    }

    public void openSettingsMenu()
    {
        bool show = !settingsMenu.active;
        settingsAnimation.showSettings(show);
        Cursor.visible = show;
    }

    IEnumerator stopAudioOnFirstStart()
    {
        yield return new WaitForSeconds(0.050F);
        startAudio.stopSound();
    }
    public void restartAudio()
    {
        startAudio.audioRestart();
    }

    IEnumerator autoRestartAudio()
    {
        Debug.Log("Check if audio restart is activated... waiting 3 seconds for settings to load");
        //wait 3 seconds so there is enough time for the settings to load
        yield return new WaitForSeconds(3);
        bool audioRestart = saveSystem.getSetting().restartAudio;
        Debug.Log($"audio restart activated: " + audioRestart);
        if (audioRestart)
        {
            float restartEverySeconds = saveSystem.getSetting().autoRestartAudioEverySeconds;
            InvokeRepeating(nameof(restartAudio), 0f, restartEverySeconds);
            Debug.Log($"Restart audio every: {restartEverySeconds} seconds");
        }
    }


    public void takeScreenshotFromGameScene()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            screenshotManager.takeScreenshot();
        }
    }

    public void playSound(AudioSource audioSource, AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip, startAudio.audioSource.volume);
    } 
}
