using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//CcColorController = CaptureCardColorController
public class CcColorController : MonoBehaviour
{
    public VideoGameCaptureController vgc;
    private Material captureMat;

    public int lastRangeMode = 0;
    public float lastBrightness = 0f;
    public float lastContrast = 1f;
    public float lastSaturation = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private String prefix = "ColorController:";
    private void Start()
    {
        captureMat = vgc.startCapture.webCamImage.material;
        ResetToDefault();
    }

    // Dropdown: 0 = None, 1 = Expand (16-235 -> 0-255), 2 = Compress
    public void SetRangeMode(int index)
    {
        lastRangeMode = index;
        captureMat.SetFloat("_RangeMode", (float)index);
    }

    // Slider: -1.0 - 1.0 (Default 0.0)
    public void SetBrightness(float value)
    {
        lastBrightness = value;
        captureMat.SetFloat("_Brightness", value);
        Debug.Log($"{prefix} brightness changed to {value}");
    }

    // Slider: 0.0 - 2.0 (Default 1.0)
    public void SetContrast(float value)
    {
        lastContrast = value;
        captureMat.SetFloat("_Contrast", value);
        Debug.Log($"{prefix} contrast changed to {value}");
    }

    // Slider: 0.0 - 2.0 (Default 1.0)
    public void SetSaturation(float value)
    {
        lastSaturation = value;
        captureMat.SetFloat("_Saturation", value);
        Debug.Log($"{prefix} saturation changed to {value}");
    }

    public void ResetToDefault()
    {
        SetRangeMode(0);
        SetBrightness(0f);
        SetContrast(1f);
        SetSaturation(1f);
    }

    public float getFloatValueFromShader(string name)
    {
        return captureMat.GetFloat(name);
    }

}
