using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSyncButton : MonoBehaviour
{
    public VideoGameCaptureController videoGameCaptureController;

    public void onButtonClick()
    {
        if (videoGameCaptureController.startAudio.microfoneUsed != null)
        {
            videoGameCaptureController.startAudio.startSound(videoGameCaptureController.startAudio.microfoneUsed);
        }
    }
}
