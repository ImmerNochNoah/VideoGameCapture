using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SampelRate : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public VideoGameCaptureController vgc;


    void Start()
    {
        List<string> options = new List<string>();
        //for (int i = 0; i < vgc.audioPureFMOD.availableRates.Length; i++)
        //{
            //options.Add(vgc.audioPureFMOD.availableRates[i] + " Hz");
        //}
        //dropdown.ClearOptions();
        //dropdown.AddOptions(options);

        //int savedIndex = PlayerPrefs.GetInt("SavedSampleRateIndex", 1);
        //if (savedIndex >= 0 && savedIndex < vgc.audioPureFMOD.availableRates.Length)
        //{
            //dropdown.value = savedIndex;
        //}
        //else
        //{
         //   dropdown.value = 1;
        //}
        //dropdown.RefreshShownValue();
    }

    public void HandleInputData(int val)
    {
        PlayerPrefs.SetInt("SavedSampleRateIndex", val);
        PlayerPrefs.Save();

        //int selectedHz = vgc.audioPureFMOD.availableRates[val];
        //vgc.audioPureFMOD.UpdateSampleRate(selectedHz);
    }
}
