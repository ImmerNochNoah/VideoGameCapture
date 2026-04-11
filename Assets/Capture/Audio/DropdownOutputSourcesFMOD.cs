using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DropdownOutputSourcesFMOD : MonoBehaviour
{
    public TMPro.TMP_Dropdown outputSources;

    public VideoGameCaptureController videoGameCaptureController;
    // Start is called before the first frame update
    void Start()
    {
        FMOD.System coreSystem = FMODUnity.RuntimeManager.CoreSystem;

        // 2.  Check the number of output devices
        int numDrivers;
        coreSystem.getNumDrivers(out numDrivers);

        List<string> options = new List<string>();
        //First selected item in dropdown menu
        Debug.Log("[FMOD DROPDOWN OUTPUT] looking for output devices");
        // 3. Check all output devices
        for (int i = 0; i < numDrivers; i++)
        {
            System.Guid guid;
            string name;
            int sampleRate;
            FMOD.SPEAKERMODE speakerMode;
            int channels;

            // Retrieve the information for the OUTPUT
            coreSystem.getDriverInfo(i, out name, 256, out guid, out sampleRate, out speakerMode, out channels);
            options.Add(name);
        }

        Debug.Log($"[FMOD DROPDOWN OUTPUT] {options.Count} audio outputs found");
        outputSources.ClearOptions();
        outputSources.AddOptions(options);

        int currentDriver;
        coreSystem.getDriver(out currentDriver);
        outputSources.value = currentDriver;
        outputSources.RefreshShownValue();

        videoGameCaptureController.audioPureFMOD.lastUsedAudioOutput = videoGameCaptureController.audioPureFMOD.getOutputSourceById(currentDriver);
    }

    public void HandleInputData(int val)
    {
        videoGameCaptureController.audioPureFMOD.SetAudioOutputDevice(val);
    }
}
