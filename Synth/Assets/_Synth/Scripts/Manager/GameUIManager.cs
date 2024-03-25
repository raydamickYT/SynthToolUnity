using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public Synth SynthObject;
    private SynthInfo SynthInfo;
    public SettingsSaver SettingsSaverScript = new();
    public SettingsLoader settingsLoader = new();
    public GameObject settingsPanel; // Verwijzing naar je Settings Panel
    public Button SettingsButton, CloseButton, SaveSettingsBtn, LoadSettingsBtn, RecordButtonStart, RecordButtonStop;
    public Dropdown RecordingOptions;
    private AudioRecorder audioRecorder;

    private void Start()
    {
        if (SynthInfo == null)
        {
            SynthInfo = SynthObject.synthState;
        }
        settingsPanel.SetActive(false);
        Initialization();
    }

    private void Initialization()
    {
        PopulateDropdownWithRecordDevices();
        audioRecorder = new(SynthObject.RecordIndex);
        SettingsButton.onClick.AddListener(ToggleSettingsWindow);
        CloseButton.onClick.AddListener(ToggleSettingsWindow);
        RecordButtonStart.onClick.AddListener(StartRecording);
        RecordButtonStop.onClick.AddListener(StopRecording);
        SaveSettingsBtn.onClick.AddListener(SettingsSaverScript.SaveAllSynthsSettingsWithFileDialog);
        LoadSettingsBtn.onClick.AddListener(settingsLoader.OpenFileBrowser);
        void PopulateDropdownWithRecordDevices()
        {
            FMOD.System system = RuntimeManager.CoreSystem;
            system.getNumDrivers(out int numRecordDevices);

            RecordingOptions.ClearOptions();
            for (int i = 0; i < numRecordDevices; i++)
            {
                system.getRecordDriverInfo(i, out string name, 256, out _, out int sampleRate, out FMOD.SPEAKERMODE speakerMode, out int channels, out _);
                string deviceInfo = $"Apparaat {i}: {name}";
                RecordingOptions.options.Add(new Dropdown.OptionData(deviceInfo));
            }

            RecordingOptions.RefreshShownValue();

            RecordingOptions.onValueChanged.AddListener(SetSelectedRecordDevice);
        }
    }
    public void ToggleSettingsWindow()
    {
        // Schakelt de zichtbaarheid van het settingsPanel
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
    public void SetSelectedRecordDevice(int selectedIndex)
    {
        // FMODUnity.RuntimeManager.CoreSystem.setRecordDriver(selectedIndex);
        SynthObject.RecordIndex = selectedIndex;
        Debug.Log($"Geselecteerd opnameapparaat: {RecordingOptions.options[selectedIndex].text}");
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
