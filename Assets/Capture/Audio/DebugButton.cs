using UnityEngine;

public class DebugButton : MonoBehaviour
{
    public VideoGameCaptureController vgc;

    public void OnButtonClick() //Where?
    {
        vgc.audioPureFMOD.PrintFMODDebugInfo();
    }
}
