using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System;
using System.Runtime.InteropServices;
using Unity.VisualScripting;

using UnityEngine.UI;

public class DSPCallback
{
    [AOT.MonoPInvokeCallback(typeof(FMOD.DSP_READ_CALLBACK))]
    public static FMOD.RESULT CaptureDSPReadCallback(ref FMOD.DSP_STATE dsp_state, IntPtr inbuffer, IntPtr outbuffer, uint length, int inchannels, ref int outchannels)
    {
        //hiermee halen we de data op die we megeven aan deze callback
        FMOD.DSP_STATE_FUNCTIONS functions = (FMOD.DSP_STATE_FUNCTIONS)Marshal.PtrToStructure(dsp_state.functions, typeof(FMOD.DSP_STATE_FUNCTIONS));

        IntPtr userData;
        functions.getuserdata(ref dsp_state, out userData);

        GCHandle objHandle = GCHandle.FromIntPtr(userData);
        Synth obj = objHandle.Target as Synth;

        // Save the channel count out for the update function
        obj.mChannels = inchannels;

        //-------------------------
        //geluid
        // Pas de gain toe op elke sample
        obj.sharedBuffer = new float[length * obj.mChannels];


        for (uint sampleIndex = 0; sampleIndex < length; sampleIndex++)
        {
            float sampleValue = 0f;
            switch (obj.CurrentWaveForm) // Gebruik de huidige golfvorm
            {
                case WaveForm.Sine:
                    sampleValue = obj.GenerateSineWave(obj.sineFrequency, (uint)obj.sampleRate, ref obj.phase, sampleIndex);
                    break;
                case WaveForm.Sawtooth:
                    sampleValue = obj.GenerateSawtoothWave(obj.sineFrequency, (uint)obj.sampleRate, ref obj.phase, sampleIndex);
                    // Bereken de zaagtandgolf sample
                    break;
                case WaveForm.Square:
                    // Bereken de vierkantgolf sample
                    sampleValue = obj.GenerateSquareWave(sampleIndex, (uint)obj.sampleRate, obj.sineFrequency, ref obj.phase);

                    break;
                case WaveForm.Triangle:
                    // Bereken de driehoeksgolf sample
                    sampleValue = obj.GenerateTriangleWave(sampleIndex, (uint)obj.sampleRate, obj.sineFrequency, ref obj.phase);
                    break;
            }
            for (int channel = 0; channel < outchannels; channel++)
            {

                // Copy the incoming buffer to process later
                obj.sharedBuffer[sampleIndex * outchannels + channel] = sampleValue; //bereken de juiste index in tempbuffer, waar de sample value wordt opgeslagen
                obj.savedSampleValue = sampleValue;
                //voor stereo channels vermenigvuldigt het met *2 (twee output kanalen), en voegt channel toe aan deze waarde (oftewel links, channel 0, recht, channel 1).
            }
        }

        //--------
        // Copy the inbuffer to the outbuffer so we can still hear it
        // Vul het geheugen met de gegenereerde sample voor alle kanalen.
        Marshal.Copy(obj.sharedBuffer, 0, outbuffer, obj.sharedBuffer.Length);// kopieer de buffer naar de geheugen

        return FMOD.RESULT.OK;
    }
}
