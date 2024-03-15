using UnityEngine;
using FMODUnity;
using System;
using System.Runtime.InteropServices;

public class Synth : MonoBehaviour
{
    CreateSynth createSynth;
    public float savedSampleValue;
    public bool DSPIsActive = false;
    public FMOD.DSP_READ_CALLBACK mReadCallback;
    public FMOD.DSP mCaptureDSP;
    public float[] mDataBuffer, sharedBuffer;
    private readonly object bufferLock = new object();

    public GCHandle mObjHandle;
    public WaveForm CurrentWaveForm = WaveForm.Sawtooth; // Standaard golfvorm
    public uint mBufferLength;
    public int mChannels = 0;
    public float sineFrequency = 440f; // Frequentie van de sinusgolf in Hz
    public float phase = 0f; // Fase van de sinusgolf
    public int sampleRate = 48000; // Stel dit in op de daadwerkelijke sample rate van je systeem

    private void Awake()
    {
        createSynth = new(this);
        CheckAudioSettings();
        CreateDSP();
    }
    public void CreateDSP()
    {
        // Assign the callback to a member variable to avoid garbage collection
        mReadCallback = DSPCallback.CaptureDSPReadCallback;

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
                    if (masterCG.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, mCaptureDSP) != FMOD.RESULT.OK) //hier voegen we hem toe aan de mastergroup (hierdoor kunnen we hem horen.)
                    {
                        Debug.LogWarningFormat("FMOD: Unable to add mCaptureDSP to the master channel group");
                    }
                    else
                    {
                        masterCG.getNumChannels(out int channels);
                        Debug.Log(channels);
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

    void Update()
    {
        float[] bufferCopy;
        lock (bufferLock)
        {
            bufferCopy = new float[sharedBuffer.Length];
            Array.Copy(sharedBuffer, bufferCopy, sharedBuffer.Length);
        }
        WaveformVisualizer.instance.UpdateWaveform(bufferCopy);


    }
    private void LateUpdate()
    {
        //sla de settings op in een ander script waar iedereen bij kan.
        GlobalSynthSettings.instance.UpdateSettings(DSPIsActive, CurrentWaveForm, mChannels, sineFrequency, sampleRate);
    }

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
        phase += phaseIncrement;

        // Normaliseer de fase zodat deze altijd tussen 0 en 2π blijft
        if (phase >= 2f * Mathf.PI)
            phase -= 2f * Mathf.PI;

        // Bereken de zaagtandwaarde, gemapt van fase naar een waarde tussen -1 en 1
        // De fase loopt lineair op, dus we mappen deze direct naar onze output
        float sawtooth = (phase / (2f * Mathf.PI)) * 2f - 1f;

        return sawtooth;
    }



    public float GenerateSquareWave(uint index, uint sampleRate, float frequency, ref float phase)
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
        return position < period / 2 ? 1f : -1f;
    }

    public float GenerateTriangleWave(uint index, uint sampleRate, float frequency)
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