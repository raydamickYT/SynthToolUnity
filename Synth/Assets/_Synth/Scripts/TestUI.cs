using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUI : MonoBehaviour
{
    public Button OnOff;
    public Synth synth;
    bool SynthIsPlaying;
    // Start is called before the first frame update
    void Start()
    {
        OnOff.onClick.AddListener(ToggleSynth);
    }
    void Update()
    {
        // synth.synthState.mCaptureDSP.getActive(out bool test);
        // Debug.Log(test);
    }

    public void ToggleSynth()
    {
        switch (SynthIsPlaying)
        {
            case true:
                StopSynth();
                SynthIsPlaying = false;
                break;
            case false:
                StartSynth();
                SynthIsPlaying = true;
                break;
            default:
        }
    }
    private void StartSynth()
    {
        Debug.Log("synth staat aan");
        // Synth.instance.CreateCustomDSP();
        // Synth.mCaptureDSP.setActive(true); // zet de dsp op actief.
        synth.synthState.mCaptureDSP.setActive(true);

    }

    private void StopSynth()
    {
        Debug.Log("synth staat uit");
        // Synth.instance.GlobalDSP.release();
        // SynthInfo.mCaptureDSP.setActive(false); // zet de dsp op inactief. 
        synth.synthState.mCaptureDSP.setActive(false);

    }
}
