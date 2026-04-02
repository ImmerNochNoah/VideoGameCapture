using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dropdown : MonoBehaviour
{
    public TMPro.TMP_Dropdown captureCards;

    public VideoGameCaptureController videoGameCaptureController;

    public List<string> cards = new List<string>();
    // Start is called before the first frame update
    void Start()
    {

        //First selected item in dropdown menu
        cards.Add("No Capture Card");

        for (int i = 0; i < WebCamTexture.devices.Length; i++)
        {
            cards.Add(WebCamTexture.devices[i].name);
        }


        captureCards.options.Clear();
        foreach (string t in cards)
        {
            captureCards.options.Add(new TMP_Dropdown.OptionData() { text = t });
        }

    }

    public void HandleInputData(int val)
    {
        Debug.Log(cards[val]);
        videoGameCaptureController.startCapture.setCaptureCard(cards[val]);
    }
}
