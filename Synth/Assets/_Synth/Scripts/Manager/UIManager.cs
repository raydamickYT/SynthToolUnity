using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public Button OnOffBtn;
    public Dropdown ChangeWave;
    public Slider VolumeSlider;
    private bool SynthIsPlaying = true;

    private void Start()
    {
        Initialization();
    }
    private void Initialization()
    {
        // Synth.instance.myDsp.getActive(out SynthIsPlaying);
        VolumeSlider.value = Synth.instance.frequency;
        VolumeSlider.onValueChanged.AddListener(ChangeFreq);
        ChangeWave.onValueChanged.AddListener(delegate
        {
            WaveChanged(ChangeWave);
        });
        OnOffBtn.onClick.AddListener(ToggleSynth);
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
    private void StartSynth()
    {
        Debug.Log("synth staat aan");
        // Synth.instance.CreateCustomDSP();
        Synth.instance.myDsp.setActive(true); // zet de dsp op actief.

    }

    private void StopSynth()
    {
        Debug.Log("synth staat uit");
        // Synth.instance.GlobalDSP.release();
        Synth.instance.myDsp.setActive(false); // zet de dsp op inactief. 
    }

    private void ChangeFreq(float vol)
    {
        Debug.Log(vol);
        // Synth.instance.myDsp.setParameterFloat(FMODUnity.FMOD.DSP_INDEX.HEAD, vol); // hier is 0 aangegeven omdat (meestal) de default voor de volume parameter 0 is. 
        Synth.instance.frequency = vol;
    }
}