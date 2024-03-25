using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public GameObject settingsPanel; // Verwijzing naar je Settings Panel
    public Button SettingsButton;
    private void Start()
    {
        settingsPanel.SetActive(false);
        SettingsButton.onClick.AddListener(ToggleSettingsWindow);
    }
    public void ToggleSettingsWindow()
    {
        // Schakelt de zichtbaarheid van het settingsPanel
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    private void Initialization()
    {
        PopulateDropdownWithRecordDevices();
    }
    void PopulateDropdownWithRecordDevices()
    {
        FMOD.System system = RuntimeManager.CoreSystem;
        system.getNumDrivers(out int numRecordDevices);

        dropdown.ClearOptions();
        for (int i = 0; i < numRecordDevices; i++)
        {
            system.getRecordDriverInfo(i, out string name, 256, out _, out int sampleRate, out FMOD.SPEAKERMODE speakerMode, out int channels, out _);
            string deviceInfo = $"Apparaat {i}: {name}";
            dropdown.options.Add(new Dropdown.OptionData(deviceInfo));
        }

        dropdown.RefreshShownValue();

        dropdown.onValueChanged.AddListener(SetSelectedRecordDevice);
    }
    public void StartRecording()
    {
        audioRecorder.StartRecording();
    }

    public void StopRecording()
    {
        audioRecorder.StopRecording(SynthInfo);
    }
}
