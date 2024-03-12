using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System;
using System.Runtime.InteropServices;

public class Synth : MonoBehaviour
{

    public float frequency = 440f; // A4 noot
    private float increment;
    private float phase;
    private float sampling_frequency = 48000f;

    //static vars
    // Houd bij welke golfvorm momenteel wordt gebruikt.
    public static WaveForm currentWaveForm = WaveForm.Sine;

    FMOD.DSP_DESCRIPTION dspDesc = new FMOD.DSP_DESCRIPTION();

    void Start()
    {
        CheckAudioSettings();
        CreateCustomDSP();
        // Verkrijg het FMOD systeem instance.
        FMOD.System system = RuntimeManager.CoreSystem;

        // Andere setup of initialisatiecode hier...
    }
    void CheckAudioSettings()
    {
        FMOD.System system = RuntimeManager.CoreSystem; // Verkrijg het FMOD systeem
        int sampleRate;
        FMOD.SPEAKERMODE speakerMode;
        int raw;

        // Verkrijg de huidige softwareformat instellingen
        system.getSoftwareFormat(out sampleRate, out speakerMode, out raw);

        // Output de waarden naar de console voor debugging
        Debug.Log("Sample Rate: " + sampleRate);
        Debug.Log("Speaker Mode: " + speakerMode);
    }
    void CreateCustomDSP()
    {
        FMOD.DSP_DESCRIPTION dspDescription = new FMOD.DSP_DESCRIPTION();

        // Zet de naam van de DSP om naar een byte array
        string dspName = "MijnDSP";
        byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(dspName);

        // FMOD verwacht dat de naam afgesloten wordt met een null-byte ('\0'), dus voeg die toe.
        byte[] nameWithNullByte = new byte[nameBytes.Length + 1];
        Array.Copy(nameBytes, nameWithNullByte, nameBytes.Length);
        nameWithNullByte[nameWithNullByte.Length - 1] = 0; // Voeg null-byte toe aan het einde

        dspDescription.name = nameWithNullByte; // Zorg ervoor dat de naam eindigt met een null-terminator
        dspDescription.read = DSPReadCallback;

        FMOD.RESULT result;
        FMOD.DSP myDsp;
        result = RuntimeManager.CoreSystem.createDSP(ref dspDescription, out myDsp);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Failed to create DSP: " + result);
            return;
        }

        FMOD.ChannelGroup masterGroup;
        RuntimeManager.CoreSystem.getMasterChannelGroup(out masterGroup);
        masterGroup.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, myDsp);
        myDsp.setBypass(false);
        myDsp.setActive(true);


        // Voeg je DSP toe aan de DSP-keten als dat nodig is
        // Bijvoorbeeld: RuntimeManager.CoreSystem.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, myDsp);
    }


    // DSP Read Callback
    public static FMOD.RESULT DSPReadCallback(ref FMOD.DSP_STATE dsp_state, IntPtr inbuffer, IntPtr outbuffer, uint length, int inchannels, ref int outchannels)
    {
        // Een buffer voor de samples.
        float[] buffer = new float[length * outchannels];

        for (uint sampleIndex = 0; sampleIndex < length; sampleIndex++)
        {
            float sampleValue = 0f;
            switch (currentWaveForm)
            {
                case WaveForm.Sine:
                    // Genereer een sinusgolf sample.
                    sampleValue = GenerateSineWave(sampleIndex, length);
                    break;
                case WaveForm.Sawtooth:
                    // Genereer een zaagtandgolf sample.
                    sampleValue = GenerateSawtoothWave(sampleIndex, length);
                    break;
                case WaveForm.Square:
                    // Genereer een vierkantgolf sample.
                    sampleValue = GenerateSquareWave(sampleIndex, length);
                    break;
                case WaveForm.Triangle:
                    // Genereer een driehoeksgolf sample.
                    sampleValue = GenerateTriangleWave(sampleIndex, length);
                    break;
            }

            // Vul de buffer met de gegenereerde sample voor alle kanalen.
            for (int channel = 0; channel < outchannels; channel++)
            {
                buffer[sampleIndex * outchannels + channel] = sampleValue;
            }
        }

        // Kopieer de buffer terug naar outbuffer.
        Marshal.Copy(buffer, 0, outbuffer, buffer.Length);

        return FMOD.RESULT.OK;
    }

    static float GenerateSineWave(uint index, uint length)
    {
        // Implementeer de daadwerkelijke sinusgolf generatie.
        // Dit is een voorbeeld en moet worden aangepast aan je specifieke behoeften.
        return Mathf.Sin(2 * Mathf.PI * 440 * index / 48000);
    }

    static float GenerateSawtoothWave(uint index, uint length)
    {
        // Implementeer zaagtandgolf generatie.
        return 2f * (index / (float)length) - 1f;
    }

    static float GenerateSquareWave(uint index, uint length)
    {
        // Implementeer vierkantgolf generatie.
        return index < length / 2 ? 1f : -1f;
    }

    static float GenerateTriangleWave(uint index, uint length)
    {
        // Implementeer driehoeksgolf generatie.
        float position = (index / (float)length) * 4f;
        if (position < 2f) return position - 1f;
        else return 3f - position;
    }


}
public enum WaveForm
{
    Sine,
    Sawtooth,
    Square,
    Triangle
}

