using UnityEngine;
using System.Runtime.InteropServices;
using FMODUnity;

public class CreateSynth
{
    public SynthInfo synthInfo;
    int DspCount;

    public CreateSynth(SynthInfo _synth, int _count)
    {
        synthInfo = _synth;
        DspCount = _count;
    }

    public void CreateDSP()
    {
        // Assign the callback to a member variable to avoid garbage collection
        synthInfo.mReadCallback = DSPCallback.CaptureDSPReadCallback;


        // Allocate a data buffer large enough for 8 channels
        uint bufferLength;
        int numBuffers;
        FMODUnity.RuntimeManager.CoreSystem.getDSPBufferSize(out bufferLength, out numBuffers);
        synthInfo.mDataBuffer = new float[bufferLength * 8];
        synthInfo.mBufferLength = bufferLength;

        // Get a handle to this object to pass into the callback
        // SynthState synthState = new(sineFrequency, (uint)sampleRate, mDataBuffer);

        synthInfo.mObjHandle = GCHandle.Alloc(synthInfo);
        if (synthInfo.mObjHandle != null)
        {
            // Define a basic DSP that receives a callback each mix to capture audio
            FMOD.DSP_DESCRIPTION desc = new FMOD.DSP_DESCRIPTION();
            desc.numinputbuffers = 1;
            desc.numoutputbuffers = 1;
            desc.read = synthInfo.mReadCallback;
            desc.userdata = GCHandle.ToIntPtr(synthInfo.mObjHandle);
            desc.name = new byte[32]; // Naam moet een byte-array zijn
            System.Text.Encoding.UTF8.GetBytes(synthInfo.name, 0, synthInfo.name.Length, desc.name, 0); // Zet string om naar byte-array
            desc.version = 0x00010000; // Versie 1.0, kan aangepast worden naar behoefte
            desc.read = synthInfo.mReadCallback;

            FMOD.ChannelGroup synthChannelGroup;
            // Create an instance of the capture DSP and attach it to the master channel group to capture all audio
            if (FMODUnity.RuntimeManager.CoreSystem.createChannelGroup(synthInfo.name, out synthChannelGroup) == FMOD.RESULT.OK)
            {
                if (FMODUnity.RuntimeManager.CoreSystem.createDSP(ref desc, out synthInfo.mCaptureDSP) == FMOD.RESULT.OK) //hier wordt de dsp aangemaakt
                {
                    synthInfo.mCaptureDSP.setActive(false); //zet hem tijdelijk op inactief, dan kunnen we dat later aanpassen.
                    synthInfo.mCaptureDSP.getActive(out synthInfo.DSPIsActive); //sla het gelijk op zodat we het in andere scripts kunnen gebruiken.
                    synthInfo.channelGroup = synthChannelGroup;
                    Debug.Log("synth is: " + synthInfo.DSPIsActive);
                    if (synthChannelGroup.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, synthInfo.mCaptureDSP) != FMOD.RESULT.OK) //hier voegen we hem toe aan de mastergroup (hierdoor kunnen we hem horen.)
                    {
                        Debug.LogWarningFormat("FMOD: Unable to add mCaptureDSP to the master channel group");
                    }
                    else
                    {
                        synthChannelGroup.getNumChannels(out int channels);
                        // Debug.Log(channels);
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

    public void InitializeDSPForSynth(SynthInfo synthInfo)
    {
        FMOD.DSP_DESCRIPTION dspDescription = new FMOD.DSP_DESCRIPTION();
        // Configureer de dspDescription zoals nodig, inclusief het instellen van de read callback
        dspDescription.read = synthInfo.mReadCallback;

        FMOD.DSP dsp;
        FMODUnity.RuntimeManager.CoreSystem.createDSP(ref dspDescription, out dsp);

        // Stel DSP eigenschappen in
        dsp.setActive(false);  // Start standaard als niet-actief

        // Sla de DSP op in SynthInfo voor latere referentie
        synthInfo.mCaptureDSP = dsp;

        // Voeg DSP toe aan de Master Channel Group
        FMOD.ChannelGroup masterGroup;
        FMODUnity.RuntimeManager.CoreSystem.getMasterChannelGroup(out masterGroup);
        masterGroup.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, dsp);
    }


}
