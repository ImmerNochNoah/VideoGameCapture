using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolDownButton : MonoBehaviour
{
    public VideoGameCaptureController videoGameCaptureController;

    public void onButtonClick()
    {
        videoGameCaptureController.changeAudioVolume(videoGameCaptureController.audioPureFMOD.volume - 0.10f);
    }
}
