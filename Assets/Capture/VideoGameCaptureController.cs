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

    public GitHubUpdateChecker updateChecker;

    public AudioSource audioSource;

    public WebCamDevice[] devices;
    public WebCamTexture webCameraTexture;

    public StartCapture startCapture;
    public StartAudio startAudio;

    public ScreenAnimation settingsAnimation;
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
        //we need this to store data like screenshots and settings.
        applicationPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        Debug.Log($"Application Path: {applicationPath}");


        screenshotManager.checkIfScreenshotFolderExists();
        StartCoroutine(userDefaults());
        StartCoroutine(updateChecker.CheckForUpdates());

        //StartCoroutine(stopAudioOnFirstStart());

    }

    // Update is called once per frame
    void Update()
    {
        appKeys();
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

        //take screenshot
        if (Input.GetKeyDown(KeyCode.F9))
        {
            screenshotManager.takeScreenshot();
        }

        volumeCheck();
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
            volumeBarManager.updateVolumeBar();
        }
        else
        {
            if (soundVolume == 0f)
            {
                startAudio.audioSource.volume = 0.3f;
                volumeBarManager.updateVolumeBar();
                return;
            }
            startAudio.audioSource.volume = soundVolume;
            volumeBarManager.updateVolumeBar();
        }
    }

    public void changeAudioVolume(float volume)
    {
        startAudio.audioSource.volume = volume;
        soundVolume = startAudio.audioSource.volume;
        volumeBarManager.updateVolumeBar();
    }

    //left settings menu

    public bool getSettingsMenuOpen()
    {
        return settingsMenu.active;
    }

    public void openSettingsMenu()
    {
        bool show = !getSettingsMenuOpen();
        openSettingsMenu(show);
    }

    public void openSettingsMenu(bool show)
    {
        settingsAnimation.show(show);
        Cursor.visible = show;
    }

    IEnumerator userDefaults()
    {
        yield return new WaitForSeconds(0.050F);
        if (saveSystem.settingsExist())
        {
            bool openSettings = saveSystem.getSetting().settingsOpen;
            openSettingsMenu(openSettings);
            Debug.Log($"User configuration open settings on  start: {openSettings}");
        }
        else
        {
            Debug.Log("Opening settings: User configuration dont exists!");
            openSettingsMenu(true);
        }

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

    public void restartAudio()
    {
        startAudio.audioRestart();
    }

    public void playSound(AudioSource audioSource, AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip, startAudio.audioSource.volume);
    } 
}
