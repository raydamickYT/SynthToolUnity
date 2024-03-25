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
    public bool SynthIsPlaying = false;
    // public Button OnOff;


    private void Awake()
    {
        synthState = new();
        synthState.name = Name;
        // OnOff.onClick.AddListener(ToggleSynth);

    }
    private void Start()
    {
        CheckAudioSettings();
        createSynth = new(synthState, dspCount);
        createSynth.CreateDSP();
        synthState.mCaptureDSP.setActive(false);
        synthState.mCaptureDSP.getActive(out bool test);
        Debug.Log(test);
    }
    void Update()
    {
        ToggleSynth();
    }
    public void ToggleSynth()
    {
        synthState.DSPIsActive = SynthIsPlaying;
        switch (SynthIsPlaying)
        {
            case true:
                StopSynth();
                // SynthIsPlaying = false;
                break;
            case false:
                StartSynth();
                // SynthIsPlaying = true;
                break;
            default:
        }
    }
    private void StartSynth()
    {
        Debug.Log("synth staat aan");
        // Synth.instance.CreateCustomDSP();
        // Synth.mCaptureDSP.setActive(true); // zet de dsp op actief.
        synthState.mCaptureDSP.setActive(true);

    }

    private void StopSynth()
    {
        Debug.Log("synth staat uit");
        // Synth.instance.GlobalDSP.release();
        // SynthInfo.mCaptureDSP.setActive(false); // zet de dsp op inactief. 
        synthState.mCaptureDSP.setActive(false);

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

    void OnDestroy()
    {
        //release je dsp en group
        if (synthState.mObjHandle != null)
        {
            if (FMODUnity.RuntimeManager.CoreSystem.getMasterChannelGroup(out masterCG) == FMOD.RESULT.OK)
            {
                if (synthState.mCaptureDSP.hasHandle())
                {
                    masterCG.removeDSP(synthState.mCaptureDSP);

                    // nu kan je eindelijk je dsp zonder problemen releasen
                    synthState.mCaptureDSP.release();
                }
            }
            synthState.mObjHandle.Free();
        }
    }
}