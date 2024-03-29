using UnityEngine;
using FMODUnity;
using System;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class Synth : MonoBehaviour
{
    public string Name;
    private FMOD.ChannelGroup masterCG;
    public CreateSynth createSynth;
    public SynthInfo synthState;
    int dspCount = 0;
    public int RecordIndex = 0;
    public bool SynthIsPlaying = false;
    // public Button OnOff;


    private void Awake()
    {
        if (synthState == null)
        {
            synthState = new()
            {
                name = Name
            };
        }
        CheckAudioSettings();
        if (createSynth == null)
        {
            createSynth = new(synthState, dspCount);
            createSynth.CreateDSP();

        }

        GameManager.Instance.AddSynthToList(this);
        // OnOff.onClick.AddListener(ToggleSynth);
    }

    void CheckAudioSettings()
    {
        // Verkrijg het FMOD systeem instance.
        FMOD.System system = RuntimeManager.CoreSystem; // Verkrijg het FMOD systeem
        FMOD.SPEAKERMODE speakerMode;
        int raw;

        // krijg de huidige softwareformat instellingen
        system.getSoftwareFormat(out synthState.sampleRate, out speakerMode, out raw);

        // Output de waarden naar de console voor debugging
        Debug.Log("Sample Rate: " + synthState.sampleRate);
        Debug.Log("Speaker Mode: " + speakerMode);

        //check active dsp's
        FMOD.ChannelGroup masterGroup;
        FMOD.RESULT result = RuntimeManager.CoreSystem.getMasterChannelGroup(out masterGroup);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("FMOD: Unable to get master channel group");
            return;
        }

        FMOD.DSP dspNode;
        for (int i = 0; masterGroup.getDSP(i, out dspNode) == FMOD.RESULT.OK; i++)
        {
            dspCount++;
            // dspNode.release(); // Zorg ervoor dat je de DSP vrijgeeft om memory leaks te voorkomen.
        }
        Debug.Log($"Aantal actieve DSP's: {dspCount}");

    }
    public float[] GetCurrentAudioBuffer()
    {
        // Retourneer hier de recente audio buffer data
        // Dit hangt af van hoe je audio data genereert of opslaat in je synth
        return synthState.sharedBuffer;
    }


    void OnDestroy()
    {
        //release alles
        synthState.ReleaseEvents();

        if (synthState.mObjHandle != null)
        {
            if (synthState.mCaptureDSP.hasHandle())
            {
                synthState.channelGroup.removeDSP(synthState.mCaptureDSP);

                // nu kan je eindelijk je dsp zonder problemen releasen
                synthState.mCaptureDSP.release();
            }
        }
    }
}