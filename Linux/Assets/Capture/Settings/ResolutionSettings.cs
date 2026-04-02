using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class ResolutionSettings : MonoBehaviour
{
    public TMP_Dropdown resolutionOptions;
    public TMP_Text resolutionText;

    public StartCapture StartCapture;
    public RectTransform outputTransform;

    public List<string> resolutions = new List<string>();

    void Start()
    {
        resolutions.Add("1920x1080");
        resolutions.Add("3840x2160");
        resolutions.Add("2560x1440");
        resolutions.Add("1280x720");
        resolutions.Add("854x480");
        resolutions.Add("640x360");
        resolutions.Add("426x240");

        resolutionOptions.options.Clear();

        foreach (string t in resolutions)
        {
            resolutionOptions.options.Add(new TMP_Dropdown.OptionData() { text = t });
        }

    }

    public void HandleInputData(int val)
    {
        if (StartCapture.webCameraTexture != null)
        {
            setResolutionByID(val);
        }
    }

    private void setResolutionByID(int res)
    {
        string resolution = resolutions[res];
        string[] split = resolution.Split(' ', 'x');
        int wight = int.Parse(split[0]);
        int hight = int.Parse(split[1]);
        changeResolutionInPxl(wight, hight);
        return;
    }

    public void changeResolutionInPxl(int wight, int hight)
    {
        StartCapture.setResolution(wight, hight);
        outputTransform.sizeDelta = new Vector2(wight, hight);
        resolutionText.text = wight + "x" + hight;
    }
}
