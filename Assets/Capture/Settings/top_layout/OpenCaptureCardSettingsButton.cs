using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCaptureCardSettingsButton : MonoBehaviour
{
    public ScreenAnimation screenAnimation;
    public GameObject captureCardSettings;
    public void clickButton()
    {
        bool show = !captureCardSettings.active;
        screenAnimation.show(show);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (captureCardSettings.active)
            {
                screenAnimation.show(false);
            }
        }
    }
}
