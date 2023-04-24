using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public VideoGameCaptureController videoGameCaptureController;
    public string microfoneUsed;

    public void startSound(string name)
    {
        if (name != null)
        {
            audioSource = GetComponent<AudioSource>();
            int minFreq, maxFreq;
            Microphone.GetDeviceCaps(name, out minFreq, out maxFreq);

            audioSource.clip = Microphone.Start(name, true, 3, maxFreq);
            audioSource.loop = true;

            microfoneUsed = name;

            videoGameCaptureController.soundVolume = audioSource.volume;

            while (!(Microphone.GetPosition(name) > 0)) { }
            audioSource.Play();
        }
    }

    public void stopSound()
    {
        audioSource.Stop();
    }

}
