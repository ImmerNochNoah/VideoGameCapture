using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseButton : MonoBehaviour
{
    public VideoGameCaptureController videoGameCaptureController;
    public OpenCaptureCardSettingsButton openCaptureCardSettingsButton;
    public void clickCloseButton()
    {
        videoGameCaptureController.openSettingsMenu();
        if (openCaptureCardSettingsButton.captureCardSettings.active)
        {
            openCaptureCardSettingsButton.screenAnimation.show(false);
        }
    }
}
