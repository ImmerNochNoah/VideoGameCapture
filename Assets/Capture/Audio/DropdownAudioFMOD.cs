using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DropdownAudioFMOD : MonoBehaviour
{
    public TMPro.TMP_Dropdown captureCards;

    public VideoGameCaptureController videoGameCaptureController;
    // Start is called before the first frame update
    void Start()
    {

        FMOD.System coreSystem = FMODUnity.RuntimeManager.CoreSystem;
        int numRecordDrivers, numConnected;
        coreSystem.getRecordNumDrivers(out numRecordDrivers, out numConnected);

        List<string> options = new List<string>();
        //First selected item in dropdown menu
        options.Add("No Audio Input");

        for (int i = 0; i < numRecordDrivers; i++)
        {
            System.Guid guid;
            string name;
            int sampleRate;
            FMOD.SPEAKERMODE speakerMode;
            int channels;
            FMOD.DRIVER_STATE state;
            coreSystem.getRecordDriverInfo(i, out name, 256, out guid, out sampleRate, out speakerMode, out channels, out state);

            options.Add(name);
        }

        captureCards.ClearOptions();
        captureCards.AddOptions(options);



    }

    public void HandleInputData(int val)
    {
        
        Debug.Log($"AudioSource selected: {val}");
        if (val == 0)
        {
            Debug.Log("[FMOD DROPDOWN AUDIO INPUT] Stop Audio (No Audio Input)");
            videoGameCaptureController.audioPureFMOD.StopCapture();
        }
        else
        {
            // IMPORTEN: Since “No Audio Input” is in position 0,
            // we have to subtract 1 to get the actual FMOD index!
            int fmodIndex = val - 1;
            Debug.Log("[FMOD DROPDOWN AUDIO INPUT] Start FMOD recording with driver index: " + fmodIndex);
            videoGameCaptureController.audioPureFMOD.StartCaptureEngine(fmodIndex);
        }
    }
}
