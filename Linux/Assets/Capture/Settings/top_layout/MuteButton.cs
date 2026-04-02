using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteButton : MonoBehaviour
{
    public VideoGameCaptureController videoGameCaptureController;
    public void clickMuteButton()
    {
        videoGameCaptureController.muteSound();
    }
}
