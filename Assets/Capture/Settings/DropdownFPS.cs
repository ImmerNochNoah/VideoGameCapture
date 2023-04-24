using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class DropdownFPS : MonoBehaviour
{
    public TMP_Dropdown fpsOptions;

    public StartCapture StartCapture;


    public List<string> fps = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        fps.Add("60");
        fps.Add("30");

        fpsOptions.options.Clear();

        foreach (string t in fps)
        {
            fpsOptions.options.Add(new TMP_Dropdown.OptionData() { text = t });
        }

    }

    public void HandleInputData(int val)
    {
        int newFPS = int.Parse(fps[val]);
        StartCapture.targetFPS = newFPS;

        if (StartCapture.webCameraTexture != null)
        {
            Debug.Log("==START==");
            Debug.Log("Before FPS Change: " + StartCapture.webCameraTexture.requestedFPS);

            StartCapture.setFps(newFPS);

            Debug.Log("After FPS Change: " + StartCapture.webCameraTexture.requestedFPS);
            Debug.Log("==END==");
        }
    }
}
