using System;
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

    public VideoGameCaptureController vgc;

    public ScreenAnimation screenAnimation;

    public void UpdateVolumeBar()
    {
        volumeBar.value = vgc.audioPureFMOD.volume;
        //captureController.playSound(soundChangedAudioClip, volumeBar.value);
        vgc.audioPureFMOD.PlaySoundEffektByName("UI_CLICK");
        inactive = false;
    }

    private void OnEnable()
    {
        inactive = false;
        volumeBar.value = vgc.soundVolume;

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
