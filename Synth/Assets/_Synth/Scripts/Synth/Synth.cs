using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System;
using System.Runtime.InteropServices;
using Unity.VisualScripting;

public class Synth : MonoBehaviour
{
    //fmod



    public static Synth instance;
    public float frequency = 440f; // A4 noot
    private float increment;
    private float phase = 0;
    private int sampling_frequency; //dit is alleen om te kijken wat de freq is nu
    public string DspNames = "";

    //static vars
    // Houd bij welke golfvorm momenteel wordt gebruikt.
    public static WaveForm currentWaveForm = WaveForm.Sine;

    GCHandle handle;
    public FMOD.DSP myDsp;


    void Start()
    {

        // FMOD.DSP_READ_CALLBACK mReadCallback;


        // Andere setup of initialisatiecode hier...
    }

    void Awake()
    {

        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this); //als er een duplicate is willen we dat het nieuwe object verwijdert wordt.
        }
        CheckAudioSettings();
        CreateCustomDSP();
        handle = GCHandle.Alloc(this, GCHandleType.Weak);
        IntPtr ptr = (IntPtr)handle;
        myDsp.setUserData(ptr);

    }
    void CheckAudioSettings()
    {
        // Verkrijg het FMOD systeem instance.
        FMOD.System system = RuntimeManager.CoreSystem; // Verkrijg het FMOD systeem
        FMOD.SPEAKERMODE speakerMode;
        int raw;

        // krijg de huidige softwareformat instellingen
        system.getSoftwareFormat(out sampling_frequency, out speakerMode, out raw);

        // Output de waarden naar de console voor debugging
        Debug.Log("Sample Rate: " + sampling_frequency);
        Debug.Log("Speaker Mode: " + speakerMode);
    }
    public void CreateCustomDSP()
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
                                                //.read is blijkbaar een delegate...
        dspDescription.read = DSPReadCallback; //zal altijd een float terug krijgen, dit zorgt voor andere geluiden.

        FMOD.RESULT result; //in deze var wordt het resultaat zo opgeslagen (ok, error)
        result = RuntimeManager.CoreSystem.createDSP(ref dspDescription, out myDsp); //maak de dsp met alle info die we net in het description obj hebben gestopt.
        if (result != FMOD.RESULT.OK) //check even of de task wel completed is.
        {
            Debug.LogError("Failed to create DSP: " + result);
            return;
        }
        //geef userdata mee
        // SynthState synthState = new(frequency, (uint)sampling_frequency, phase);
        // GCHandle handle = GCHandle.Alloc(synthState, GCHandleType.Pinned);

        IntPtr userDataPtr = GCHandle.ToIntPtr(handle);
        myDsp.setUserData(userDataPtr);

        FMOD.ChannelGroup masterGroup;
        RuntimeManager.CoreSystem.getMasterChannelGroup(out masterGroup);
        masterGroup.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, myDsp);

        // activeer je dsp
        myDsp.setBypass(false); // false: hij wordt niet bypassed
        /*
        wat de bypass doet is dat deze dsp wordt overgeslagen bij het afspelen. hij blijft in de verwerkingsketen, dit is handig als je 
        de dsp vaak aan en uit wilt zetten. Het is niet heel zuinig (omdat hij blijft doorwerken in de achtergrond). 
        */
        myDsp.setActive(false); //false: hij wordt niet aangezet
        /*
        Setactive is voor dsp's hetzelfde als met game objecten: hij is niet actief en wordt dus niet uitgevoerd. Zuinig en snel, naar mijn mening is de de beste manier voor 
        dit project om de dsp's te toggelen.
        */
    }


    // DSP Read Callback
    [AOT.MonoPInvokeCallback(typeof(FMOD.DSP_READ_CALLBACK))]
    public static FMOD.RESULT DSPReadCallback(ref FMOD.DSP_STATE dsp_state, IntPtr inbuffer, IntPtr outbuffer, uint length, int inchannels, ref int outchannels)
    {
        // Een buffer voor de samples.
        float[] buffer = new float[length * outchannels];
        FMOD.DSP_STATE_FUNCTIONS functions = (FMOD.DSP_STATE_FUNCTIONS)Marshal.PtrToStructure(dsp_state.functions, typeof(FMOD.DSP_STATE_FUNCTIONS));

        IntPtr userData;
        functions.getuserdata(ref dsp_state, out userData);

        GCHandle objHandle = GCHandle.FromIntPtr(userData);
        SynthState obj = objHandle.Target as SynthState;
        Debug.Log("obj");

        for (uint sampleIndex = 0; sampleIndex < length; sampleIndex++)
        {
            float sampleValue = 0f;
            switch (currentWaveForm)
            {
                case WaveForm.Sine:
                    // Genereer een sinusgolf sample.
                    // sampleValue = GenerateSineWave(sampleIndex, length);
                    sampleValue = GenerateSineWave(instance.frequency, (uint)instance.sampling_frequency, ref instance.phase, sampleIndex);

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

    public static float GenerateSineWave(float frequency, uint sampleRate, ref float phase, uint index)
    {
        float sample = Mathf.Sin(phase);
        float phaseIncrement = 2f * Mathf.PI * frequency / sampleRate;
        phase += phaseIncrement;
        // Zorg ervoor dat de fase niet te groot wordt
        if (phase >= 2f * Mathf.PI) phase -= 2f * Mathf.PI;

        return sample;
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

