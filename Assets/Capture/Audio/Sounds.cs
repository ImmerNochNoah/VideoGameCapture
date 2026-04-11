using FMODUnity;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public void PlayClickSound()
    {
        // Spielt den Sound exakt einmal ab und kümmert sich um den Speicher
        RuntimeManager.PlayOneShot("event:/UI_CLICK");
    }

    public void PlayScreenshotSound()
    {
        // Spielt den Sound exakt einmal ab und kümmert sich um den Speicher
        RuntimeManager.PlayOneShot("event:/SCREENSHOT");
    }
}
