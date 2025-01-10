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
        aspectRatioList.Add("4:9 - FullHD");
        aspectRatioList.Add("4:9 - 2K");
        aspectRatioList.Add("4:9 - 4K");
        aspectRatioList.Add("4:9 - HD");

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
            resolutionSettings.changeResolutionInPxl(Screen.mainWindowDisplayInfo.width, Screen.mainWindowDisplayInfo.height);
            selectedAspectRatio = "16:9";
            aspectRatioDropdownLabel.text = selectedAspectRatio;
        }
        else if (aspectRatio.Equals("4:9 - 4K"))
        {
            resolutionSettings.changeResolutionInPxl(3840, 2880);
            selectedAspectRatio = "4:9 - 4K";
            aspectRatioDropdownLabel.text = selectedAspectRatio;
        }
        else if (aspectRatio.Equals("4:9 - 2K"))
        {
            resolutionSettings.changeResolutionInPxl(2560, 1920);
            selectedAspectRatio = "4:9 - 2K";
            aspectRatioDropdownLabel.text = selectedAspectRatio;
        }
        else if (aspectRatio.Equals("4:9 - FullHD"))
        {
            resolutionSettings.changeResolutionInPxl(1440, 1080);
            selectedAspectRatio = "4:9 - FullHD";
            aspectRatioDropdownLabel.text = selectedAspectRatio;
        }
        else if (aspectRatio.Equals("4:9 - HD"))
        {
            resolutionSettings.changeResolutionInPxl(960, 720);
            selectedAspectRatio = "4:9 - HD";
            aspectRatioDropdownLabel.text = selectedAspectRatio;
        }
    }
}


