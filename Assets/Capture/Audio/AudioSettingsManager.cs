using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    public VideoGameCaptureController vgcc;

    public TMP_InputField latencyInput;
    public TMP_InputField bufferInput;

    public Slider latencySlider;
    public Slider bufferSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        latencySlider.value = vgcc.audioPureFMOD.targetLatencySeconds;
        bufferSlider.value = vgcc.audioPureFMOD.bufferSizeSeconds;

        latencyInput.text = $"{vgcc.audioPureFMOD.targetLatencySeconds}";
        bufferInput.text = $"{vgcc.audioPureFMOD.bufferSizeSeconds}";

    }

    public void SetLatency(float latency)
    {
        latencySlider.value = latency;
        latencyInput.text = $"{latency}";
        vgcc.audioPureFMOD.targetLatencySeconds = latency;
    }

    public void SetBuffer(float buffer)
    {
        bufferSlider.value = buffer;
        bufferInput.text = $"{buffer}";
        vgcc.audioPureFMOD.bufferSizeSeconds = buffer;
    }

    public void OnLatencyInputEndEdit(string rawInput)
    {
        float validatedValue = vgcc.ValidateAndClamp(rawInput, 0.1f, 0.2f, vgcc.audioPureFMOD.getDefaultLatency());
        SetLatency(validatedValue);
    }

    public void OnBufferInputEndEdit(string rawInput)
    {
        float validatedValue = vgcc.ValidateAndClamp(rawInput, 0.5f, 5f, vgcc.audioPureFMOD.getDefaultBufferSize());
        SetBuffer(validatedValue);
    }

    public void SetDefaultLatency()
    {
        SetLatency(vgcc.audioPureFMOD.getDefaultLatency());
    }
    public void SetDefaultBuffer()
    {
        SetBuffer(vgcc.audioPureFMOD.getDefaultBufferSize());
    }
}
