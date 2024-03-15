using System.Collections.Generic;
using FMODUnity;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public static int RecordIndex = 0;
    [SerializeField] private VoorbeeldScript voorbeeldScript;
    [SerializeField] private SettingsSaver settingsSaver;
    [SerializeField] private SettingsLoader settingsLoader;
    [SerializeField] private AudioRecorder audioRecorder;

    public Button OnOffBtn;
    public Dropdown ChangeWave, dropdown;
    public Slider VolumeSlider;
    private bool SynthIsPlaying = true;

    private void Start()
    {
        Initialization();
    }
    private void Initialization()
    {
        voorbeeldScript.mCaptureDSP.getActive(out SynthIsPlaying);

        VolumeSlider.value = voorbeeldScript.sineFrequency;
        VolumeSlider.onValueChanged.AddListener(ChangeFreq);

        ChangeWave.onValueChanged.AddListener(delegate
        {
            WaveChanged(ChangeWave);
        });

        PopulateDropdownWithRecordDevices();

        OnOffBtn.onClick.AddListener(ToggleSynth);
        FMODUnity.RuntimeManager.CoreSystem.getNumDrivers(out int test);
        for (int i = 0; i < test; i++)
        {
            RuntimeManager.CoreSystem.getRecordDriverInfo(i, out string name, 256, out _, out int sampleRate, out FMOD.SPEAKERMODE speakerMode, out int channels, out _);
            UnityEngine.Debug.Log($"Apparaat {i}: {name}, SampleRate: {sampleRate}, SpeakerMode: {speakerMode}, Channels: {channels}");
        }
        UnityEngine.Debug.Log(name);

    }


    void PopulateDropdownWithRecordDevices()
    {
        FMOD.System system = RuntimeManager.CoreSystem;
        system.getNumDrivers(out int numRecordDevices);

        dropdown.ClearOptions();
        for (int i = 0; i < numRecordDevices; i++)
        {
            system.getRecordDriverInfo(i, out string name, 256, out _, out int sampleRate, out FMOD.SPEAKERMODE speakerMode, out int channels, out _);
            string deviceInfo = $"Apparaat {i}: {name}, SampleRate: {sampleRate}, SpeakerMode: {speakerMode}, Channels: {channels}";
            dropdown.options.Add(new Dropdown.OptionData(deviceInfo));
        }

        dropdown.RefreshShownValue();

        dropdown.onValueChanged.AddListener(SetSelectedRecordDevice);
    }

    public void SetSelectedRecordDevice(int selectedIndex)
    {
        // FMODUnity.RuntimeManager.CoreSystem.setRecordDriver(selectedIndex);
        RecordIndex = selectedIndex;
        Debug.Log($"Geselecteerd opnameapparaat: {dropdown.options[selectedIndex].text}");
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
                voorbeeldScript.CurrentWaveForm = WaveForm.Sine;
                break;
            case 1: // sawtooth
                voorbeeldScript.CurrentWaveForm = WaveForm.Sawtooth;
                break;
            case 2: //square
                voorbeeldScript.CurrentWaveForm = WaveForm.Square;
                break;
            case 3: //triangle
                voorbeeldScript.CurrentWaveForm = WaveForm.Triangle;
                break;
            default:
                break;
        }

    }
    private void StartSynth()
    {
        Debug.Log("synth staat aan");
        // Synth.instance.CreateCustomDSP();
        voorbeeldScript.mCaptureDSP.setActive(true); // zet de dsp op actief.

    }

    private void StopSynth()
    {
        Debug.Log("synth staat uit");
        // Synth.instance.GlobalDSP.release();
        voorbeeldScript.mCaptureDSP.setActive(false); // zet de dsp op inactief. 
    }

    private void ChangeFreq(float vol)
    {
        // Debug.Log(vol);
        // Synth.instance.myDsp.setParameterFloat(FMODUnity.FMOD.DSP_INDEX.HEAD, vol); // hier is 0 aangegeven omdat (meestal) de default voor de volume parameter 0 is. 
        voorbeeldScript.sineFrequency = vol;
    }
    public void SaveSettings()
    {
        SynthState synthState = new SynthState(voorbeeldScript.sineFrequency, (uint)voorbeeldScript.sampleRate, voorbeeldScript.CurrentWaveForm);
        settingsSaver.SaveSettingsWithFileDialog(synthState);
    }

    public void LoadSettings()
    {
        SynthState synthState = new SynthState();

        // Vraag de gebruiker om een bestandspad te kiezen
        string filePath = EditorUtility.OpenFilePanel("Load Settings", "", "txt");

        // Controleer of de gebruiker een pad heeft gekozen
        if (!string.IsNullOrEmpty(filePath))
        {
            // Laad de instellingen vanuit het gekozen bestandspad
            settingsLoader.LoadSettings(filePath, synthState);
        }
    }

    public void StartRecording()
    {
        audioRecorder.StartRecording();
    }

    public void StopRecording()
    {
        audioRecorder.StopRecording();
    }
}
