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
        synthInfo.mReadCallback = DSPCallback.CaptureDSPReadCallback;


        uint bufferLength;
        int numBuffers;
        FMODUnity.RuntimeManager.CoreSystem.getDSPBufferSize(out bufferLength, out numBuffers);
        synthInfo.mDataBuffer = new float[bufferLength * 8];
        synthInfo.mBufferLength = bufferLength;

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
                    synthInfo.mCaptureDSP.setActive(false); //zet hem tijdelijk op inactief, dan kunnen we dat later aanpassen. (dit werkt dus niet om een of andere reden)

                    synthInfo.mCaptureDSP.getActive(out bool temp);
                    synthInfo.DSPIsActive = temp; //sla het gelijk op zodat we het in andere scripts kunnen gebruiken.
                    synthInfo.channelGroup = synthChannelGroup;
                    if (synthChannelGroup.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, synthInfo.mCaptureDSP) != FMOD.RESULT.OK) //hier voegen we hem toe aan de mastergroup (hierdoor kunnen we hem horen.)
                    {
                        Debug.LogWarningFormat("FMOD: Unable to add mCaptureDSP to the master channel group");
                    }
                    else
                    {
                        synthChannelGroup.getNumChannels(out int channels);
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
