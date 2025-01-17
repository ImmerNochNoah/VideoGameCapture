using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    bool userUsingApp;

    // Start is called before the first frame update
    void Start()
    {
        devices = WebCamTexture.devices;
        Application.targetFrameRate = 60;
        
        //will show the normal settings menu
        settingsAnimation.showSettings(true);
        Cursor.visible = true;

        //this will automaticliy restart the audio every 30m and so will fix the bug with the fuzzy sound
        InvokeRepeating(nameof(restartAudio), 0, 1800f);


        //StartCoroutine(stopAudioOnFirstStart());

    }

    // Update is called once per frame
    void Update()
    {
        appKeys();
        volumeCheck();
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
            Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
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
}
