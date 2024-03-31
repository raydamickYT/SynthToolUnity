using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System;
using System.Runtime.InteropServices;
using Unity.VisualScripting;

public enum WaveForm
{
    Sine,
    Sawtooth,
    Square,
    Triangle
}

public class SynthInfo
{
    // public static SynthInfo instance;
    public UIManager uIManager;
    public string name;
    public uint SamplingFrequency;
    public float CarrierPhase;
    public float[] mDataBuffer;
    public WaveForm CurrentWaveForm { get; internal set; }
    [Range(0f, 1f)]
    public float volume = 0.5f; // Standaard volume op 50%
    public float savedSampleValue;
    private bool dSPIsActive = false;
    public event Action<bool> OnDSPIsActiveChanged;

    public bool DSPIsActive
    {
        get => dSPIsActive;
        set
        {
            if (dSPIsActive != value)
            {
                dSPIsActive = value;
                OnDSPIsActiveChanged?.Invoke(dSPIsActive);
            }
        }
    }
    public FMOD.DSP_READ_CALLBACK mReadCallback;
    public FMOD.DSP mCaptureDSP;
    public FMOD.ChannelGroup channelGroup;
    public float[] sharedBuffer;
    public GCHandle mObjHandle;
    public uint mBufferLength;
    public int mChannels = 0;
    public float sineFrequency = 440f; 
    public float phase = 0f;
    public int sampleRate = 48000;

    public SynthWaves synthWaves = new();
    public SynthInfo()
    {
        SamplingFrequency = 44100;
        CurrentWaveForm = WaveForm.Sine; //sine is gwn de default
    }

    public void ReleaseEvents()
    {
        OnDSPIsActiveChanged = null;
    }
}

public class SynthWaves
{
    public float GenerateSineWave(float frequency, uint sampleRate, ref float phase, uint index, float volume)
    {
        float sample = Mathf.Sin(phase);
        float phaseIncrement = 2f * Mathf.PI * frequency / sampleRate;
        phase += phaseIncrement;
        if (phase >= 2f * Mathf.PI) phase -= 2f * Mathf.PI;

        return sample * volume;
    }

    public float GenerateSawtoothWave(float frequency, uint sampleRate, ref float phase, uint index, float volume)
    {
        float phaseIncrement = 2f * Mathf.PI * frequency / sampleRate;

        phase += phaseIncrement;

        if (phase >= 2f * Mathf.PI)
            phase -= 2f * Mathf.PI;

        float sawtooth = (phase / (2f * Mathf.PI)) * 2f - 1f;

        return sawtooth * volume;
    }



    public float GenerateSquareWave(uint index, uint sampleRate, float frequency, ref float phase, float volume)
    {
        float phaseIncrement = 2f * Mathf.PI * frequency / sampleRate;

        phase += phaseIncrement;

        if (phase >= 2f * Mathf.PI) phase -= 2f * Mathf.PI;

        float period = sampleRate / frequency;
        float position = (index + phase / phaseIncrement) % period;

        return (position < period / 2 ? 1f : -1f) * volume;
    }
    public float GenerateTriangleWave(uint index, uint sampleRate, float frequency, ref float phase, float volume)
    {
        float period = sampleRate / frequency;

        float phaseInTermsOfPeriod = phase / (2f * Mathf.PI) * period;

        float position = (index + phaseInTermsOfPeriod) % period / period;

        if (position < 0.25f)
            return (4f * position) * volume; // Oplopend van 0 naar 1
        else if (position < 0.75f)
            return (2f - 4f * position) * volume; // Aflopend van 1 naar -1
        else
            return (-4f + 4f * position) * volume; // Oplopend van -1 naar 0
    }
}

