using UnityEngine;
using System.Runtime.InteropServices;
using FMODUnity;

public class CreateSynth
{
    private SynthInfo synth;

    public CreateSynth(SynthInfo _synth)
    {
        synth = _synth;
    }

    public void CreateDSP()
    {
        // Assign the callback to a member variable to avoid garbage collection
        synth.mReadCallback = DSPCallback.CaptureDSPReadCallback;

        // Allocate a data buffer large enough for 8 channels
        uint bufferLength;
        int numBuffers;
        FMODUnity.RuntimeManager.CoreSystem.getDSPBufferSize(out bufferLength, out numBuffers);
        synth.mDataBuffer = new float[bufferLength * 8];
        synth.mBufferLength = bufferLength;

        // Get a handle to this object to pass into the callback
        // SynthState synthState = new(sineFrequency, (uint)sampleRate, mDataBuffer);

        synth.mObjHandle = GCHandle.Alloc(synth);
        if (synth.mObjHandle != null)
        {
            // Define a basic DSP that receives a callback each mix to capture audio
            FMOD.DSP_DESCRIPTION desc = new FMOD.DSP_DESCRIPTION();
            desc.numinputbuffers = 1;
            desc.numoutputbuffers = 1;
            desc.read = synth.mReadCallback;
            desc.userdata = GCHandle.ToIntPtr(synth.mObjHandle);

            // Create an instance of the capture DSP and attach it to the master channel group to capture all audio
            FMOD.ChannelGroup masterCG;
            if (FMODUnity.RuntimeManager.CoreSystem.getMasterChannelGroup(out masterCG) == FMOD.RESULT.OK)
            {

                if (FMODUnity.RuntimeManager.CoreSystem.createDSP(ref desc, out synth.mCaptureDSP) == FMOD.RESULT.OK) //hier wordt de dsp aangemaakt
                {
                    synth.mCaptureDSP.setActive(false); //zet hem tijdelijk op inactief, dan kunnen we dat later aanpassen.
                    synth.mCaptureDSP.getActive(out synth.DSPIsActive); //sla het gelijk op zodat we het in andere scripts kunnen gebruiken.
                    if (masterCG.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, synth.mCaptureDSP) != FMOD.RESULT.OK) //hier voegen we hem toe aan de mastergroup (hierdoor kunnen we hem horen.)
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

}
