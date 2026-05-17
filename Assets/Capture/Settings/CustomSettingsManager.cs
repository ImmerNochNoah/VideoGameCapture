using UnityEngine;

public class CustomSettingsManager : MonoBehaviour
{
    public ScreenAnimation sa;
    public GameObject settingsObject;

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
