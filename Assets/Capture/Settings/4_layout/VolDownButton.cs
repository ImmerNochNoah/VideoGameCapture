using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolDownButton : MonoBehaviour
{
    public VideoGameCaptureController videoGameCaptureController;

    public void onButtonClick()
    {
        videoGameCaptureController.changeAudioVolume(videoGameCaptureController.startAudio.audioSource.volume - 0.10f);
    }
}
