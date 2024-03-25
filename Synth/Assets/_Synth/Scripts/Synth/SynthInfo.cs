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
    public string name;
    public float Frequency;
    public uint SamplingFrequency;
    public float CarrierPhase;
    public float[] mDataBuffer;
    public WaveForm CurrentWaveForm { get; internal set; }
    [Range(0f, 1f)]
    public float volume = 0.5f; // Standaard volume op 50%
    public float savedSampleValue;
    public bool DSPIsActive = false;
    public FMOD.DSP_READ_CALLBACK mReadCallback;
    public FMOD.DSP mCaptureDSP;
    public readonly object bufferLock = new object();
    public float[] sharedBuffer;
    public GCHandle mObjHandle;
    public uint mBufferLength;
    public int mChannels = 0;
    public float sineFrequency = 440f; // Frequentie van de sinusgolf in Hz
    public float phase = 0f; // Fase van de sinusgolf
    public int sampleRate = 48000; // Stel dit in op de daadwerkelijke sample rate van je systeem

    public SynthWaves synthWaves = new();
    public SynthInfo()
    {
        // if (instance == null)
        // {
        //     instance = this;
        // }
        Frequency = 0;
        SamplingFrequency = 0;
        CurrentWaveForm = WaveForm.Sine; //sine is gwn de default
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
        // Bereken de fase-increment per sample
        float phaseIncrement = 2f * Mathf.PI * frequency / sampleRate;

        // Verhoog de fase met de increment
        phase += phaseIncrement;

        // Normaliseer de fase zodat deze altijd tussen 0 en 2π blijft
        if (phase >= 2f * Mathf.PI)
            phase -= 2f * Mathf.PI;

        // Bereken de zaagtandwaarde, gemapt van fase naar een waarde tussen -1 en 1
        // De fase loopt lineair op, dus we mappen deze direct naar onze output
        float sawtooth = (phase / (2f * Mathf.PI)) * 2f - 1f;

        return sawtooth * volume;
    }



    public float GenerateSquareWave(uint index, uint sampleRate, float frequency, ref float phase, float volume)
    {
        // Bereken de fase-increment per sample
        float phaseIncrement = 2f * Mathf.PI * frequency / sampleRate;

        // Verhoog de fase met de increment
        phase += phaseIncrement;

        // Normaliseer de fase zodat deze altijd tussen 0 en 2π blijft
        if (phase >= 2f * Mathf.PI) phase -= 2f * Mathf.PI;


        // Bereken de periode van de golf
        float period = sampleRate / frequency;
        // Bereken de positie in de huidige periode
        float position = (index + phase / phaseIncrement) % period;

        // De golf wisselt tussen 1 en -1 halverwege elke periode
        return (position < period / 2 ? 1f : -1f) * volume;
    }
    public float GenerateTriangleWave(uint index, uint sampleRate, float frequency, ref float phase, float volume)
    {
        // Bereken de golfperiode
        float period = sampleRate / frequency;

        // Fase in termen van perioden omzetten
        float phaseInTermsOfPeriod = phase / (2f * Mathf.PI) * period;

        // Rekening houden met de fase in de positieberekening
        // De modulo zorgt ervoor dat de waarde binnen de golfperiode blijft
        float position = (index + phaseInTermsOfPeriod) % period / period;

        // Formule voor de driehoeksgolf aangepast om fase te incorporeren
        if (position < 0.25f)
            return (4f * position) * volume; // Oplopend van 0 naar 1
        else if (position < 0.75f)
            return (2f - 4f * position) * volume; // Aflopend van 1 naar -1
        else
            return (-4f + 4f * position) * volume; // Oplopend van -1 naar 0
    }
}

