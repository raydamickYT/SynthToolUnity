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

    private void Start()
    {
        if (SynthInfo == null && SynthObject != null)
        {
            SynthInfo = SynthObject.synthState;
            // Debug.LogError("Synth niet assigned in: " + gameObject.name);
        }
        Initialization();
    }
    private void Initialization()
    {
        SynthInfo.OnDSPIsActiveChanged += DSPIsActiveChanged;
        // SynthInfo.mCaptureDSP.getActive(out SynthObject.synthState.DSPIsActive);

        FrequencySlider.value = SynthInfo.sineFrequency;
        FrequencySlider.onValueChanged.AddListener(ChangeFreq);

        VolumeSlider.value = SynthInfo.volume;
        VolumeSlider.onValueChanged.AddListener(ChageVol);

        ChangeWave.onValueChanged.AddListener(delegate
        {
            WaveChanged(ChangeWave);
        });

        OnOffBtn.onClick.AddListener(ToggleSynth);
        FMODUnity.RuntimeManager.CoreSystem.getNumDrivers(out int test);
        for (int i = 0; i < test; i++)
        {
            RuntimeManager.CoreSystem.getRecordDriverInfo(i, out string name, 256, out _, out int sampleRate, out FMOD.SPEAKERMODE speakerMode, out int channels, out _);
            // UnityEngine.Debug.Log($"Apparaat {i}: {name}, SampleRate: {sampleRate}, SpeakerMode: {speakerMode}, Channels: {channels}");
        }
    }
    private void DSPIsActiveChanged(bool isActive)
    {
        Debug.Log("we zitten hier");
        OnDSPActiveChanged(isActive);
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
    public void OnDSPActiveChanged(bool active)
    {
        switch (active)
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

    private void ChangeFreq(float vol)
    {
        SynthInfo.sineFrequency = vol;
    }

    private void ChageVol(float vol)
    {
        SynthInfo.volume = vol;
    }
    private void OnDestroy()
    {
        if (SynthInfo != null)
        {
            SynthInfo.OnDSPIsActiveChanged -= DSPIsActiveChanged;
        }
    }
}
