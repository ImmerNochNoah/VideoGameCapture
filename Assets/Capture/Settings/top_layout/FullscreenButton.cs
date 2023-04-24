using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FullscreenButton : MonoBehaviour
{
    public VideoGameCaptureController videoGameCaptureController;
    public void TaskOnClick()
    {
        videoGameCaptureController.changeWindowMode();
    }
}
