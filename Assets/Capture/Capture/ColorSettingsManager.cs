using System.Globalization;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorSettingsManager : MonoBehaviour
{
    public CcColorController cccController;
    public TMP_InputField saturationInput;
    public TMP_InputField brightnessInput;
    public TMP_InputField contrastInput;

    public Slider brightnessSlider;
    public Slider saturationSlider;
    public Slider contrastSlider;


    public ScreenAnimation sa;

    public ScreenAnimation leftSettings;
    public ScreenAnimation rightSettings;

    public void Awake()
    {
        //sa.show(true);
    }
    public void Start()
    {
        leftSettings.show(false);
        rightSettings.show(false);
        brightnessInput.text = $"{cccController.getFloatValueFromShader("_Brightness")}";
        saturationInput.text = $"{cccController.getFloatValueFromShader("_Saturation")}";
        contrastInput.text = $"{cccController.getFloatValueFromShader("_Contrast")}";

        brightnessSlider.value = cccController.getFloatValueFromShader("_Brightness");
        saturationSlider.value = cccController.getFloatValueFromShader("_Saturation");
        contrastSlider.value = cccController.getFloatValueFromShader("_Contrast");
    }

    public void SetBrightness(float value)
    {
        cccController.SetBrightness(value);
        brightnessSlider.value = value;
        brightnessInput.text = $"{cccController.getFloatValueFromShader("_Brightness")}";
    }

    public void SetSaturation(float value)
    {
        cccController.SetSaturation(value);
        saturationSlider.value = value;
        saturationInput.text = $"{cccController.getFloatValueFromShader("_Saturation")}";
    }

    public void SetContrast(float value)
    {
        cccController.SetContrast(value);
        contrastSlider.value = value;
        contrastInput.text = $"{cccController.getFloatValueFromShader("_Contrast")}";
    }

    public void OnBrightnessInputEndEdit(string rawInput)
    {
        float validatedValue = cccController.vgc.ValidateAndClamp(rawInput, -1f, 1.0f, 0.0f);
        brightnessInput.text = validatedValue.ToString("F1", CultureInfo.InvariantCulture);
        SetBrightness(validatedValue);
    }

    public void OnSaturationInputEndEdit(string rawInput)
    {
        float validatedValue = cccController.vgc.ValidateAndClamp(rawInput, 0f, 2.0f, 1.0f);
        saturationInput.text = validatedValue.ToString("F2", CultureInfo.InvariantCulture);
        SetSaturation(validatedValue);
    }

    public void OnContrastInputEndEdit(string rawInput)
    {
        float validatedValue = cccController.vgc.ValidateAndClamp(rawInput, 0f, 2.0f, 1.0f);
        contrastInput.text = validatedValue.ToString("F2", CultureInfo.InvariantCulture);
        SetContrast(validatedValue);
    }

    public void SetDefaultBrightness()
    {
        SetBrightness(0f);
    }

    public void SetDefaultSaturation()
    {
        SetSaturation(1f);
    }

    public void SetDefaultContrast()
    {
        SetContrast(1f);
    }
}
