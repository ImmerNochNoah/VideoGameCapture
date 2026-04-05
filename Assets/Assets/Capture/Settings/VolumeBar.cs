using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeBar : MonoBehaviour
{
    private float nextActionTime = 10f;
    float timePassed = 0f;

    bool inactive;

    public Slider volumeBar;

    public VideoGameCaptureController captureController;

    public ScreenAnimation screenAnimation;

    public AudioSource audioSource;
    public AudioClip soundChangedAudioClip;

    public void UpdateVolumeBar()
    {
        volumeBar.value = captureController.soundVolume;
        audioSource.PlayOneShot(soundChangedAudioClip, volumeBar.value);
        inactive = false;
    }

    private void OnEnable()
    {
        inactive = false;
        volumeBar.value = captureController.soundVolume;

        StartCoroutine(CheckIfInactive());
        screenAnimation.show(true);
    }

    IEnumerator CheckIfInactive()
    {
        yield return new WaitForSeconds(1.5f);
        if (inactive)
        {
            screenAnimation.show(false);
        } else
        {
            inactive = true;
            StartCoroutine(CheckIfInactive());
        }
    }
}
