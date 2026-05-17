using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//I m STUUUUUUUUPID 
public class DropdownOutputTypeFMOD : MonoBehaviour
{
    public TMP_Dropdown apiDropdown;
    public VideoGameCaptureController vgc;

    void Start()
    {
        List<string> apiOptions = new List<string>();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        apiOptions.Add("Windows Standard (WASAPI)"); // Index 0
        apiOptions.Add("Pro Audio (ASIO)");          // Index 1
#elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
        apiOptions.Add("Linux Standard (PulseAudio)"); // Index 0
        apiOptions.Add("Linux Pro (ALSA)");            // Index 1
#endif

        apiDropdown.ClearOptions();
        apiDropdown.AddOptions(apiOptions);
    }

    public void HandleInputData(int val)
    {
        //vgc.audioPureFMOD.ChangeAudioAPI(val);
    }
}
