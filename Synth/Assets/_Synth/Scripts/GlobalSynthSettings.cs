using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class GlobalSynthSettings : MonoBehaviour
{
    public static GlobalSynthSettings instance;
    public bool DSPIsActive = false;
    public WaveForm CurrentWaveForm = WaveForm.Sawtooth; // Standaard golfvorm
    public int mChannels = 0;
    public float sineFrequency = 440f; // Frequentie van de sinusgolf in Hz
    public int sampleRate = 48000; // Stel dit in op de daadwerkelijke sample rate van je systeem

    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }

    void OnDestroy()
    {
        instance = null;
    }

    public void UpdateSettings(bool isactive, WaveForm current, int channels, float sincefreq, int sample)
    {
        DSPIsActive = isactive;
        CurrentWaveForm = current;
        mChannels = channels;
        sineFrequency = sincefreq;
        sampleRate = sample;
    }
}
