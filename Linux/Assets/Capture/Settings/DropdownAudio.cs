using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DropdownAudio : MonoBehaviour
{
    public TMPro.TMP_Dropdown captureCards;

    public VideoGameCaptureController videoGameCaptureController;

    public List<string> microfone = new List<string>();
    // Start is called before the first frame update
    void Start()
    {

        //First selected item in dropdown menu
        microfone.Add("No Audio Input");

        for (int i = 0; i < Microphone.devices.Length; i++)
        {
            microfone.Add(Microphone.devices[i]);
        }

        captureCards.options.Clear();

        foreach (string t in microfone)
        {
            captureCards.options.Add(new TMP_Dropdown.OptionData() { text = t });
        }
    }

    public void HandleInputData(int val)
    {
        Debug.Log(microfone[val]);
        videoGameCaptureController.startAudio.startSound(microfone[val]);
    }
}
