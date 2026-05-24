using System.Collections.Generic;
using UnityEngine;

public class CustomSettingsManager : MonoBehaviour
{
    public ScreenAnimation sa;
    public GameObject settingsObject;
    public List<GameObject> otherSettingsViews = new List<GameObject>();
    void Start()
    {
        
    }

    public void OnButtonClose()
    {
        gameObject.SetActive(false);
    }

    public void OnButtonSettingsMenu()
    {
        settingsObject.SetActive(!settingsObject.active);
    }

    public void CloseMenu()
    {
        settingsObject.SetActive(false);
    }

    public void CloseWithAnimation()
    {
        sa.show(false);
    }

    public void OnButtonSettingsMenuWithAnimation()
    {
        if (otherSettingsViews.Count > 0) { 
         foreach (GameObject settings in otherSettingsViews)
            {
                if (settings.active)
                {
                    ScreenAnimation saSettings = settings.GetComponent<ScreenAnimation>();
                    saSettings.show(false);
                }
            }
        }
        sa.show(!settingsObject.active);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMenu();
        }
    }
}
