using TMPro;
using UnityEngine;
using UnityEngine.UI;


/*
 * This is the class where i grab the image from the webcam / capture card. 
 * I used the Unity WebCamTexture class for that bc its so fast bruh like i tryed the same shit with other libraries but naah this thing is wyld ngl
 * thats also the reason why i used Unity.
 *
 * Check this out to get more infos about the WebCamTexture class!
 * https://docs.unity3d.com/ScriptReference/WebCamTexture.html
 */

public class StartCapture : MonoBehaviour
{
    public WebCamTexture webCameraTexture;
    public RawImage webCamImage;

    public Texture2D noSignalImage;

    public int targetFPS;

    public int requestedHeight;
    public int requestedWidth;

    public StartAudio startAudio;
    public AspectRatioDropdown aspectRatioDropdown;


    public TMP_Text captureCardDropdownLabel;
    public TMP_Text resolutionDropdownLabel;
    public TMP_Text fpsDropdownLabel;

    //aspectratio text is gettings changed in AspectRatioDropdown class. Sorry for that horrible spaghetti code. i hate it my self! 
    //public TMP_Text aspectRatioDropdownLabel;

    public void setCaptureCard(string name)
    {
        //When "No Capture Card" is selected then stop and replace the texture 
        if (name.Equals("No Capture Card"))
        {
            webCameraTexture.Stop();
            webCamImage.texture = noSignalImage;
            return;
        }

        if (webCameraTexture != null && webCameraTexture.isPlaying)
        {
            webCameraTexture.Stop();
        }

        //Sets custom resolution 
        if (requestedWidth > 0 && requestedHeight > 0)
        {
            webCameraTexture = new WebCamTexture(requestedWidth, requestedHeight);
        }
        else
        {
            webCameraTexture = new WebCamTexture(1920, 1080);
        }
        webCameraTexture.deviceName = name;

        //Sets custom fps
        if (targetFPS > 0)
        {
            webCameraTexture.requestedFPS = targetFPS;
        }

        webCamImage.texture = webCameraTexture;

        Debug.Log("Resolution: " + webCameraTexture.requestedWidth + " x " + webCameraTexture.requestedHeight);
        Debug.Log("Fps: " + webCameraTexture.requestedFPS);

        webCameraTexture.Play();
        //startAudio.startSound(startAudio.microfoneUsed);


        ////This is there so the dropdown label updates when software starts and a capturecard was in settings.json
        captureCardDropdownLabel.text = webCameraTexture.deviceName;
        resolutionDropdownLabel.text = webCameraTexture.requestedWidth + " x " + webCameraTexture.requestedHeight;
        fpsDropdownLabel.text = "" + targetFPS;
    }

    public void setFps(int fps)
    {
        targetFPS = fps;
        reloadCaptureCardSettings();
    }
    public void setResolution(int widht, int hight)
    {
        requestedHeight = widht;
        requestedWidth = hight;
        reloadCaptureCardSettings();
    }
    public void reloadCaptureCardSettings()
    {
        if (webCameraTexture != null)
        {
            setCaptureCard(webCameraTexture.deviceName);
        }
    }
}
