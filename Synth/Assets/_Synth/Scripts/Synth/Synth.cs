using UnityEngine;
using FMODUnity;
using System;
using System.Runtime.InteropServices;

public class Synth : MonoBehaviour
{
    private FMOD.ChannelGroup masterCG;
    CreateSynth createSynth;
    SynthInfo synthState;


    private void Awake()
    {
        synthState = new();
    }
    private void Start()
    {
        createSynth = new(synthState);
        CheckAudioSettings();
        createSynth.CreateDSP();
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