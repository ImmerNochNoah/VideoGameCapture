using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeBarManager : MonoBehaviour
{
    public GameObject volumeBarScreen;

    public VolumeBar volumeBar;

    private void Start()
    {
        volumeBarScreen.SetActive(false);
    }

    public void updateVolumeBar()
    {
        if (!volumeBarScreen.active)
        {
            volumeBarScreen.SetActive(true);
            volumeBar.UpdateVolumeBar();
        } else
        {
            volumeBar.UpdateVolumeBar();
        }
    }
}
