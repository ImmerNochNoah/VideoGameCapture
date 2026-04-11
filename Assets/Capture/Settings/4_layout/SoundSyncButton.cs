using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSyncButton : MonoBehaviour
{
    public VideoGameCaptureController videoGameCaptureController;

    public void onButtonClick()
    {
        videoGameCaptureController.audioPureFMOD.RestartAudio();
    }
}
