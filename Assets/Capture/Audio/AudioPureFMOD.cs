using FMOD;
using FMODUnity;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AudioPureFMOD : MonoBehaviour
{
    //Range(0.01f, 0.2f - The delay in seconds before the sound plays.
    public float targetLatencySeconds = 0.01f;

    //Range0.5f, 5.0f - How much audio is kept in memory.
    public float bufferSizeSeconds = 0.5f;
    //Range0f, 1f
    public float volume = 1.0f;

    public string lastUsedAudioSource;
    public string lastUsedAudioOutput;
    public TMP_Text audioInputdropdownLabel;
    public TMP_Text audioOutputdropdownLabel;

    // FMOD Interne Variablen
    private FMOD.Sound _captureSound;
    private FMOD.Channel _captureChannel;
    private int _recordDeviceIndex = -1;

    public void StartCaptureEngine(int fmodDeviceIndex)
    {
        StopCapture();

        FMOD.System coreSystem = FMODUnity.RuntimeManager.CoreSystem;
        _recordDeviceIndex = fmodDeviceIndex;

        System.Guid guid;
        int sampleRate;
        FMOD.SPEAKERMODE speakerMode;
        int channels;
        FMOD.DRIVER_STATE state;

        coreSystem.getRecordDriverInfo(_recordDeviceIndex, out lastUsedAudioSource, 256, out guid, out sampleRate, out speakerMode, out channels, out state);

        FMOD.CREATESOUNDEXINFO exinfo = new FMOD.CREATESOUNDEXINFO();
        exinfo.cbsize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(FMOD.CREATESOUNDEXINFO));
        exinfo.numchannels = channels;
        exinfo.format = FMOD.SOUND_FORMAT.PCM16;
        exinfo.defaultfrequency = sampleRate;
        exinfo.length = (uint)(sampleRate * sizeof(short) * channels * bufferSizeSeconds);

        coreSystem.createSound("FMOD_Capture", FMOD.MODE.OPENUSER | FMOD.MODE.LOOP_NORMAL, ref exinfo, out _captureSound);
        coreSystem.recordStart(_recordDeviceIndex, _captureSound, true);

        StartCoroutine(WaitAndPlay(sampleRate));

        audioInputdropdownLabel.text = lastUsedAudioSource;
    }

    private IEnumerator WaitAndPlay(int sampleRate)
    {
        FMOD.System coreSystem = FMODUnity.RuntimeManager.CoreSystem;
        uint recordPosition = 0;
        uint safeDelaySamples = (uint)(sampleRate * targetLatencySeconds);

        while (recordPosition < safeDelaySamples)
        {
            coreSystem.getRecordPosition(_recordDeviceIndex, out recordPosition);
            yield return null;
        }

        // play Sound
        coreSystem.playSound(_captureSound, new FMOD.ChannelGroup(), false, out _captureChannel);
        _captureChannel.setVolume(volume);
    }

    public void SetVolume(float newVolume)
    {
        // Mathf.Clamp01 ensures that the value never falls below 0 or exceeds 1
        volume = Mathf.Clamp01(newVolume);
        if (_captureChannel.hasHandle())
        {
            _captureChannel.setVolume(volume);
        }
    }

    public void StopCapture()
    {
        if (_recordDeviceIndex != -1)
        {
            // 1. Immediately terminate the coroutine (if it is still waiting for the buffer)
            StopAllCoroutines();

            // 2. Mute the channel immediately and pause it completely before we stop it!
            if (_captureChannel.hasHandle())
            {
                _captureChannel.setVolume(0f); // MUTE
                _captureChannel.setPaused(true); // Pause playback
                _captureChannel.stop(); // Destroy the channel
                _captureChannel.clearHandle(); // IMPORTANT: Delete from memory
            }

            // 3. Stop recording on the hardware
            FMODUnity.RuntimeManager.CoreSystem.recordStop(_recordDeviceIndex);

            // 4. Free the sound buffer (memory)
            if (_captureSound.hasHandle())
            {
                _captureSound.release();
                _captureSound.clearHandle(); // IMPORTANT: Delete the link
            }

            _recordDeviceIndex = -1;
         
            Debug.Log("[FMOD] The capture stream has been completely shut down.");
        }
    }

    public void RestartAudio()
    {
        if (_recordDeviceIndex != -1)
        {
            int lastDeviceIndex = _recordDeviceIndex;
            StopCapture();
            StartCaptureEngine(lastDeviceIndex);
        }
    }

    public List<string> getAudioSources()
    {
        FMOD.System coreSystem = FMODUnity.RuntimeManager.CoreSystem;
        int numRecordDrivers, numConnected;
        coreSystem.getRecordNumDrivers(out numRecordDrivers, out numConnected);

        List<string> options = new List<string>();

        for (int i = 0; i < numRecordDrivers; i++)
        {
            System.Guid guid;
            string name;
            int sampleRate;
            FMOD.SPEAKERMODE speakerMode;
            int channels;
            FMOD.DRIVER_STATE state;
            coreSystem.getRecordDriverInfo(i, out name, 256, out guid, out sampleRate, out speakerMode, out channels, out state);
            options.Add(name);
        }
        return options;
    }

    public List<string> getOutputSources()
    {
        FMOD.System coreSystem = FMODUnity.RuntimeManager.CoreSystem;

        // 2. Check the number of output devices
        int numDrivers;
        coreSystem.getNumDrivers(out numDrivers);

        List<string> options = new List<string>();

        // 3. Check all output devices
        for (int i = 0; i < numDrivers; i++)
        {
            System.Guid guid;
            string name;
            int sampleRate;
            FMOD.SPEAKERMODE speakerMode;
            int channels;

            // Retrieve the information for the OUTPUT
            coreSystem.getDriverInfo(i, out name, 256, out guid, out sampleRate, out speakerMode, out channels);
            options.Add(name);
        }
        return options;
    }

    public string getOutputSourceById(int id)
    {
        FMOD.System coreSystem = FMODUnity.RuntimeManager.CoreSystem;

        System.Guid guid;
        string name;
        int sampleRate;
        FMOD.SPEAKERMODE speakerMode;
        int channels;
        coreSystem.getDriverInfo(id, out name, 256, out guid, out sampleRate, out speakerMode, out channels);
        return name;
    }
    public void SetAudioOutputDevice(int deviceIndex)
    {
        FMOD.System coreSystem = FMODUnity.RuntimeManager.CoreSystem;
        FMOD.RESULT result = coreSystem.setDriver(deviceIndex);

        System.Guid guid;
        int sampleRate;
        FMOD.SPEAKERMODE speakerMode;
        int channels;
        coreSystem.getDriverInfo(deviceIndex, out lastUsedAudioOutput, 256, out guid, out sampleRate, out speakerMode, out channels);
        if (_recordDeviceIndex != -1)
        {
            Debug.Log("[FMOD] New Audio-Output restart Audio capture for better latency!");
            StartCaptureEngine(_recordDeviceIndex);
        }
        if (result == FMOD.RESULT.OK)
        {
            Debug.Log($"[FMOD] Audio output successfully switched to Index: {deviceIndex}");
            audioOutputdropdownLabel.text = lastUsedAudioOutput;
        }
        else
        {
            Debug.LogError($"[FMOD] Error when switching the output: {result}");
        }
    }

    public void PlaySoundEffektByName(string name)
    {
        FMOD.Studio.EventInstance myEvent = RuntimeManager.CreateInstance($"event:/{name}");
        myEvent.setVolume(volume);
        myEvent.start();
        myEvent.release();
    }
}