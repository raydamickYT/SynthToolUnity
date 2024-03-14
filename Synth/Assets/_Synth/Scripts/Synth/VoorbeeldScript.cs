using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System;
using System.Runtime.InteropServices;
using Unity.VisualScripting;

using UnityEngine.UI;

class VoorbeeldScript : MonoBehaviour
{
    private float savedSampleValue;
    public Dropdown ChangeWave;
    public bool DSPIsActive = false;
    private FMOD.DSP_READ_CALLBACK mReadCallback;
    public FMOD.DSP mCaptureDSP;
    private float[] mDataBuffer, sharedBuffer;
    private readonly object bufferLock = new object();

    private GCHandle mObjHandle;
    public WaveForm CurrentWaveForm = WaveForm.Sawtooth; // Standaard golfvorm
    private uint mBufferLength;
    private int mChannels = 0;
    public float sineFrequency = 440f; // Frequentie van de sinusgolf in Hz
    private float phase = 0f; // Fase van de sinusgolf
    private int sampleRate = 48000; // Stel dit in op de daadwerkelijke sample rate van je systeem


    [AOT.MonoPInvokeCallback(typeof(FMOD.DSP_READ_CALLBACK))]
    public static FMOD.RESULT CaptureDSPReadCallback(ref FMOD.DSP_STATE dsp_state, IntPtr inbuffer, IntPtr outbuffer, uint length, int inchannels, ref int outchannels)
    {
        //hiermee halen we de data op die we megeven aan deze callback
        FMOD.DSP_STATE_FUNCTIONS functions = (FMOD.DSP_STATE_FUNCTIONS)Marshal.PtrToStructure(dsp_state.functions, typeof(FMOD.DSP_STATE_FUNCTIONS));

        IntPtr userData;
        functions.getuserdata(ref dsp_state, out userData);

        GCHandle objHandle = GCHandle.FromIntPtr(userData);
        VoorbeeldScript obj = objHandle.Target as VoorbeeldScript;

        // Save the channel count out for the update function
        obj.mChannels = inchannels;

        //-------------------------
        //geluid
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
                    sampleValue = obj.GenerateSquareWave(sampleIndex, (uint)obj.sampleRate, obj.sineFrequency);

                    break;
                case WaveForm.Triangle:
                    // Bereken de driehoeksgolf sample
                    sampleValue = obj.GenerateTriangleWave(sampleIndex, (uint)obj.sampleRate, obj.sineFrequency);
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

    private void Awake()
    {
        CheckAudioSettings();
        CreateDSP();
    }
    void CheckAudioSettings()
    {
        // Verkrijg het FMOD systeem instance.
        FMOD.System system = RuntimeManager.CoreSystem; // Verkrijg het FMOD systeem
        FMOD.SPEAKERMODE speakerMode;
        int raw;

        // krijg de huidige softwareformat instellingen
        system.getSoftwareFormat(out sampleRate, out speakerMode, out raw);

        // Output de waarden naar de console voor debugging
        Debug.Log("Sample Rate: " + sampleRate);
        Debug.Log("Speaker Mode: " + speakerMode);
    }

    Texture2D GenerateWaveformTexture(float[] audioSamples, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        for (int x = 0; x < width; x++)
        {
            float sample = audioSamples[(int)(((float)x / width) * audioSamples.Length)];
            int y = (int)((sample + 1f) / 2f * (height - 1));  // Normalize sample to 0..height
            texture.SetPixel(x, y, Color.black);
        }
        texture.Apply();
        return texture;
    }


    void CreateDSP()
    {
        // Assign the callback to a member variable to avoid garbage collection
        mReadCallback = CaptureDSPReadCallback;

        // Allocate a data buffer large enough for 8 channels
        uint bufferLength;
        int numBuffers;
        FMODUnity.RuntimeManager.CoreSystem.getDSPBufferSize(out bufferLength, out numBuffers);
        mDataBuffer = new float[bufferLength * 8];
        mBufferLength = bufferLength;

        // Get a handle to this object to pass into the callback
        // SynthState synthState = new(sineFrequency, (uint)sampleRate, mDataBuffer);

        mObjHandle = GCHandle.Alloc(this);
        if (mObjHandle != null)
        {
            // Define a basic DSP that receives a callback each mix to capture audio
            FMOD.DSP_DESCRIPTION desc = new FMOD.DSP_DESCRIPTION();
            desc.numinputbuffers = 1;
            desc.numoutputbuffers = 1;
            desc.read = mReadCallback;
            desc.userdata = GCHandle.ToIntPtr(mObjHandle);

            // Create an instance of the capture DSP and attach it to the master channel group to capture all audio
            FMOD.ChannelGroup masterCG;
            if (FMODUnity.RuntimeManager.CoreSystem.getMasterChannelGroup(out masterCG) == FMOD.RESULT.OK)
            {
                if (FMODUnity.RuntimeManager.CoreSystem.createDSP(ref desc, out mCaptureDSP) == FMOD.RESULT.OK) //hier wordt de dsp aangemaakt
                {
                    mCaptureDSP.setActive(false); //zet hem tijdelijk op inactief, dan kunnen we dat later aanpassen.
                    mCaptureDSP.getActive(out DSPIsActive); //sla het gelijk op zodat we het in andere scripts kunnen gebruiken.
                    if (masterCG.addDSP(0, mCaptureDSP) != FMOD.RESULT.OK) //hier voegen we hem toe aan de mastergroup (hierdoor kunnen we hem horen.)
                    {
                        Debug.LogWarningFormat("FMOD: Unable to add mCaptureDSP to the master channel group");
                    }
                }
                else
                {
                    Debug.LogWarningFormat("FMOD: Unable to create a DSP: mCaptureDSP");
                }
            }
            else
            {
                Debug.LogWarningFormat("FMOD: Unable to create a master channel group: masterCG");
            }
        }
        else
        {
            Debug.LogWarningFormat("FMOD: Unable to create a GCHandle: mObjHandle");
        }
    }


    const float WIDTH = 0.01f;
    const float HEIGHT = 10.0f;
    const float YOFFSET = 5.0f;

    void Update()
    {
        Debug.Log(sharedBuffer);
        float[] bufferCopy;
        lock (bufferLock)
        {
            bufferCopy = new float[sharedBuffer.Length];
            Array.Copy(sharedBuffer, bufferCopy, sharedBuffer.Length);
        }
        WaveformVisualizer.instance.UpdateWaveform(bufferCopy);

        // Do what you want with the captured data
        for (int j = 0; j < mBufferLength; j++)
        {
            for (int i = 0; i < mChannels; i++)
            {
                float x = j * WIDTH;
                float y = mDataBuffer[(j * mChannels) + i] * HEIGHT;

                // Make sure Gizmos is enabled in the Unity Editor to show debug line draw for the captured channel data
                Debug.DrawLine(new Vector3(x, (YOFFSET * i) + y, 0), new Vector3(x, (YOFFSET * i) - y, 0), Color.green);
            }
        }
    }


    // //wordt gebruikt door ui
    // private void WaveChanged(Dropdown change)
    // {
    //     Debug.Log("dropdown changed to value: " + change.value);
    //     switch (change.value)
    //     {
    //         case 0: //sine wave
    //             Synth.currentWaveForm = WaveForm.Sine;
    //             break;
    //         case 1: // sawtooth
    //             Synth.currentWaveForm = WaveForm.Sawtooth;
    //             break;
    //         case 2: //square
    //             Synth.currentWaveForm = WaveForm.Square;
    //             break;
    //         default:
    //             break;
    //     }

    // }

    public float GenerateSineWave(float frequency, uint sampleRate, ref float phase, uint index)
    {
        float sample = Mathf.Sin(phase);
        float phaseIncrement = 2f * Mathf.PI * frequency / sampleRate;
        phase += phaseIncrement;
        if (phase >= 2f * Mathf.PI) phase -= 2f * Mathf.PI;

        return sample;
    }

    public float GenerateSawtoothWave(float frequency, uint sampleRate, ref float phase, uint index)
    {
        // Bereken de fase-increment per sample
        float phaseIncrement = 2f * Mathf.PI * frequency / sampleRate;

        // Verhoog de fase met de increment
        phase += phaseIncrement * index;

        // Normaliseer de fase zodat deze altijd tussen 0 en 2π blijft
        while (phase >= 2f * Mathf.PI)
            phase -= 2f * Mathf.PI;

        // Bereken de zaagtandwaarde, gemapt van fase naar een waarde tussen -1 en 1
        // De fase loopt lineair op, dus we mappen deze direct naar onze output
        float sawtooth = 2f * (phase / (2f * Mathf.PI)) - 1f;

        return sawtooth;
    }



    float GenerateSquareWave(uint index, uint sampleRate, float frequency)
    {
        // Bereken de periode van de golf
        float period = sampleRate / frequency;

        // Bereken de positie in de huidige periode
        float position = index % period;

        // De golf wisselt tussen 1 en -1 halverwege elke periode
        return position < period / 2 ? 1f : -1f;
    }

    float GenerateTriangleWave(uint index, uint sampleRate, float frequency)
    {
        // Bereken de periode van de golf
        float period = sampleRate / frequency;

        // Bereken de positie in de huidige periode
        float position = (index % period) / period;

        // Bereken de waarde van de driehoeksgolf gebaseerd op de positie binnen de periode
        if (position < 0.25f)
            return 4f * position; // Oplopend van 0 naar 1
        else if (position < 0.75f)
            return 2f - 4f * position; // Aflopend van 1 naar -1
        else
            return -4f + 4f * position; // Oplopend van -1 naar 0
    }



    void OnDestroy()
    {
        if (mObjHandle != null)
        {
            // Remove the capture DSP from the master channel group
            FMOD.ChannelGroup masterCG;
            if (FMODUnity.RuntimeManager.CoreSystem.getMasterChannelGroup(out masterCG) == FMOD.RESULT.OK)
            {
                if (mCaptureDSP.hasHandle())
                {
                    masterCG.removeDSP(mCaptureDSP);

                    // Release the DSP and free the object handle
                    mCaptureDSP.release();
                }
            }
            mObjHandle.Free();
        }
    }
}