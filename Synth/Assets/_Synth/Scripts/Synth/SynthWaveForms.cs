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

public class SynthState
{
    public float Frequency;
    public uint SamplingFrequency;
    public float CarrierPhase;
    public float[] mDataBuffer;
    public WaveForm CurrentWaveForm { get; internal set; }

    public SynthState()
    {
        Frequency = 0;
        SamplingFrequency = 0;
        CurrentWaveForm = WaveForm.Sine; //sine is gwn de default
    }
    public SynthState(float frequency, uint samplingFrequency, WaveForm _current)
    {
        Frequency = frequency;
        SamplingFrequency = samplingFrequency;
        CurrentWaveForm = _current;
    }

}

