using UnityEngine;

public class ScreenshotButton : MonoBehaviour
{

    public VideoGameCaptureController videoGameCaptureController;

    public void onButttonClick()
    {
        videoGameCaptureController.screenshotManager.takeScreenshot();
    }

}
