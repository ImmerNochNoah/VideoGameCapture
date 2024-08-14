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
            if (name.Equals("No Audio Input"))
            {
                microfoneUsed = name;
                audioSource.Stop();
                return;
            }

            audioSource = GetComponent<AudioSource>();
            int minFreq, maxFreq;
            Microphone.GetDeviceCaps(name, out minFreq, out maxFreq);
            maxFreq = maxFreq / 2;
            Debug.Log("AUDIO MAX FREQ = " + maxFreq);

            audioSource.clip = Microphone.Start(name, true, 10, maxFreq);
            audioSource.loop = true;

            microfoneUsed = name;

            videoGameCaptureController.soundVolume = audioSource.volume;

            while (!(Microphone.GetPosition(name) > 0)) { }
            audioSource.PlayDelayed(0.1f);
        }
    }

    public void stopSound()
    {
        audioSource.Stop();
    }
    public void audioRestart()
    {
        if (microfoneUsed != null)
        {
            if (Microphone.IsRecording(microfoneUsed))
            {
                audioSource.Stop();
                Microphone.End(microfoneUsed);
                startSound(microfoneUsed);
            }
        }
    }


}
