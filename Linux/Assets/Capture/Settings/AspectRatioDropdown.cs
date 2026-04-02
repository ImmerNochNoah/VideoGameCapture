using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class AspectRatioDropdown : MonoBehaviour
{
    public TMP_Dropdown aspectRatioOptions;

    public RectTransform outputTransform;
    public ResolutionSettings resolutionSettings;
    public string selectedAspectRatio = "16:9";
    public TMP_Text aspectRatioDropdownLabel;

    public List<string> aspectRatioList = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        aspectRatioList.Add("16:9");
        aspectRatioList.Add("4:9");

        aspectRatioOptions.options.Clear();

        foreach (string t in aspectRatioList)
        {
            aspectRatioOptions.options.Add(new TMP_Dropdown.OptionData() { text = t });
        }
    }

    //I know this might not be the smartest or best way of changing the aspect ratio. I can't do math, so I don't want to waste more time on this. It works, so I don't care.
    public void HandleInputData(int val)
    {
        changeAspectRatio(aspectRatioList[val]);
    }

    public void changeAspectRatio(string aspectRatio)
    {
        Debug.Log(aspectRatio);

        if (aspectRatio.Equals("16:9"))
        {
            resolutionSettings.StartCapture.SetNativeAspectFromWebcam(resolutionSettings.StartCapture.webCameraTexture);
            selectedAspectRatio = "16:9";
            aspectRatioDropdownLabel.text = selectedAspectRatio;
            Debug.Log("Changed to 16:9");
        }
        else
        {
            resolutionSettings.StartCapture.SetAspect43();
            selectedAspectRatio = "4:9";
            aspectRatioDropdownLabel.text = selectedAspectRatio;
            Debug.Log("Changed to 4:9");
        }
    }
}


