using FMODUnity;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public Synth SynthObject;
    private SynthInfo SynthInfo;
    public Button OnOffBtn;
    public Dropdown ChangeWave, dropdown;
    public Slider FrequencySlider, VolumeSlider;
    public Text VolumeText, FrequencyText;

    private void Start()
    {
        if (SynthInfo == null && SynthObject != null)
        {
            SynthInfo = SynthObject.synthState;
            SynthInfo.uIManager = this;
        }
        Initialization();
    }

    private void Initialization()
    {
        if (FrequencySlider != null)
        {
            FrequencySlider.value = SynthInfo.sineFrequency;
            FrequencySlider.onValueChanged.AddListener(ChangeFreq);
            if (FrequencyText != null)
            {
                FrequencyText.text = "Frequency: " + SynthInfo.sineFrequency;
            }
        }

        if (VolumeSlider != null)
        {
            VolumeSlider.value = SynthInfo.volume;
            VolumeSlider.onValueChanged.AddListener(ChageVol);
            if (VolumeText != null)
            {
                VolumeText.text = "Volume: " + SynthInfo.volume * 100; //x 100 omdat het anders een komma getal is
            }
        }


        ChangeWave.onValueChanged.AddListener(delegate
        {
            WaveChanged(ChangeWave);
        });

        OnOffBtn.onClick.AddListener(ToggleSynth);

        ToggleSynth(); //WARNING: Dit staat er alleen in omdat ik niet meer kan vinden waar ik de synth aan zet. 

        FMODUnity.RuntimeManager.CoreSystem.getNumDrivers(out int test);
        for (int i = 0; i < test; i++)
        {
            RuntimeManager.CoreSystem.getRecordDriverInfo(i, out string name, 256, out _, out int sampleRate, out FMOD.SPEAKERMODE speakerMode, out int channels, out _);
        }
    }
    public void ToggleSynth()
    {
        SynthInfo.DSPIsActive = !SynthInfo.DSPIsActive;

        switch (SynthInfo.DSPIsActive)
        {
            case true:
                StopSynth();
                break;
            case false:
                StartSynth();
                break;
            default:
        }
    }
    private void StartSynth()
    {
        Debug.Log("synth staat aan");
        // zet de dsp op actief.
        SynthInfo.mCaptureDSP.setActive(true);

    }

    private void StopSynth()
    {
        Debug.Log("synth staat uit");
        // zet de dsp op inactief. 
        SynthInfo.mCaptureDSP.setActive(false);

    }

    public void WaveChanged(Dropdown change)
    {
        switch (change.value)
        {
            case 0: //sine wave
                SynthInfo.CurrentWaveForm = WaveForm.Sine;
                break;
            case 1: // sawtooth
                SynthInfo.CurrentWaveForm = WaveForm.Sawtooth;
                break;
            case 2: //square
                SynthInfo.CurrentWaveForm = WaveForm.Square;
                break;
            case 3: //triangle
                SynthInfo.CurrentWaveForm = WaveForm.Triangle;
                break;
            default:
                break;
        }

    }

    public void ChangeFreq(float Freq)
    {
        Debug.Log(Freq + "Synthname: " + SynthInfo.name);
        SynthInfo.sineFrequency = Freq;
        FrequencyText.text = "Frequency: " + Freq;
        FrequencySlider.value = Freq;
    }

    public void ChageVol(float vol)
    {
        SynthInfo.volume = vol;
        VolumeText.text = "Volume: " + vol * 100;
        VolumeSlider.value = vol;
    }
    private void OnDestroy()
    {
        if (SynthInfo != null)
        {
            // SynthInfo.OnDSPIsActiveChanged -= DSPIsActiveChanged;
        }
    }
}
