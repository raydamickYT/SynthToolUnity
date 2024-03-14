using System;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

class VoorbeeldScript : MonoBehaviour
{
    public Dropdown ChangeWave;
    public bool DSPIsActive = false;
    private FMOD.DSP_READ_CALLBACK mReadCallback;
    protected FMOD.DSP mCaptureDSP;
    public FMOD.DSP GetDSP(){ return mCaptureDSP; }
    private float[] mDataBuffer;
    private GCHandle mObjHandle;
    public WaveForm CurrentWaveForm = WaveForm.Sine; // Standaard golfvorm
    private uint mBufferLength;
    private int mChannels = 0;
    public float sineFrequency = 440f; // Frequentie van de sinusgolf in Hz
    private float phase = 0f; // Fase van de sinusgolf
    private float sampleRate = 48000f; // Stel dit in op de daadwerkelijke sample rate van je systeem


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
        float[] tempBuffer = new float[length * obj.mChannels];
        for (uint sampleIndex = 0; sampleIndex < length; sampleIndex++)
        {
            float sampleValue = 0f;
            switch (obj.CurrentWaveForm) // Gebruik de huidige golfvorm
            {
                case WaveForm.Sine:
                    sampleValue = obj.GenerateSineWave(obj.sineFrequency, (uint)obj.sampleRate, ref obj.phase, sampleIndex);
                    break;
                case WaveForm.Sawtooth:
                    sampleValue = obj.GenerateSawtoothWave(sampleIndex, length);
                    // Bereken de zaagtandgolf sample
                    break;
                case WaveForm.Square:
                    // Bereken de vierkantgolf sample
                    sampleValue = obj.GenerateSquareWave(sampleIndex, length);

                    break;
                case WaveForm.Triangle:
                    // Bereken de driehoeksgolf sample
                    sampleValue = obj.GenerateTriangleWave(sampleIndex, length);
                    break;
            }
            for (int channel = 0; channel < outchannels; channel++)
            {
                // Copy the incoming buffer to process later
                tempBuffer[sampleIndex * outchannels + channel] = sampleValue; //bereken de juiste index in tempbuffer, waar de sample value wordt opgeslagen
                //voor stereo channels vermenigvuldigt het met *2 (twee output kanalen), en voegt channel toe aan deze waarde (oftewel links, channel 0, recht, channel 1).
            }
        }

        //--------
        // Copy the inbuffer to the outbuffer so we can still hear it
        // Vul het geheugen met de gegenereerde sample voor alle kanalen.
        Marshal.Copy(tempBuffer, 0, outbuffer, tempBuffer.Length);// kopieer de buffer naar de geheugen

        return FMOD.RESULT.OK;
    }

    void Start()
    {
        CreateDSP();
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

    public float GenerateSineWave(float frequency, uint sampleRate, ref float phase, uint index)
    {
        float sample = Mathf.Sin(phase);
        float phaseIncrement = 2f * Mathf.PI * frequency / sampleRate;
        phase += phaseIncrement;
        if (phase >= 2f * Mathf.PI) phase -= 2f * Mathf.PI;

        return sample;
    }

    private void WaveChanged(Dropdown change)
    {
        Debug.Log("dropdown changed to value: " + change.value);
        switch (change.value)
        {
            case 0: //sine wave
                Synth.currentWaveForm = WaveForm.Sine;
                break;
            case 1: // sawtooth
                Synth.currentWaveForm = WaveForm.Sawtooth;
                break;
            case 2: //square
                Synth.currentWaveForm = WaveForm.Square;
                break;
            default:
                break;
        }

    }


    float GenerateSawtoothWave(uint index, uint length)
    {
        // Implementeer zaagtandgolf generatie.
        return 2f * (index / (float)length) - 1f;
    }

    float GenerateSquareWave(uint index, uint length)
    {
        // Implementeer vierkantgolf generatie.
        return index < length / 2 ? 1f : -1f;
    }

    float GenerateTriangleWave(uint index, uint length)
    {
        // Implementeer driehoeksgolf generatie.
        float position = (index / (float)length) * 4f;
        if (position < 2f) return position - 1f;
        else return 3f - position;
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