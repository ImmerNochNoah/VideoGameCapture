using System;
using System.IO;
using UnityEngine;

public class ScreenshotManager : MonoBehaviour
{
    public VideoGameCaptureController vgc;
    public string screenshotFolderPath;
    public void checkIfScreenshotFolderExists()
    {
        screenshotFolderPath = Path.Combine(vgc.applicationPath, "Screenshots");
        Debug.Log("screenshot folder: " + screenshotFolderPath);
        bool screenshotFolderExists = Directory.Exists(screenshotFolderPath);

        Debug.Log("screenshot folder exists: " + screenshotFolderExists);
        if (!screenshotFolderExists)
        {
            Directory.CreateDirectory(screenshotFolderPath);
            Debug.Log("created new screenshot folder");
            return;
        }
        Debug.Log("found screenshot folder.");
    }

    public void takeScreenshot()
    {
        string screenshotname = "vgc-" + DateTime.Now.ToString("MM-dd-HH-mm-ss-yyyy") + ".png";
        string newScreenshotPath = Path.Combine(screenshotFolderPath, screenshotname);
        Debug.Log("Saved new screenshot: " + newScreenshotPath);

        ScreenCapture.CaptureScreenshot(newScreenshotPath, 1);
        vgc.audioPureFMOD.PlaySoundEffektByName("SCREENSHOT");
    }
}
