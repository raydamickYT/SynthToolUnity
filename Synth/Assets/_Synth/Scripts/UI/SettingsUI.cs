using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public SettingsSaver SettingsSaverScript = new();
    public SettingsLoader settingsLoader = new();
    public GameObject KeybindsPanel, SettingsPanel;
    public Button SaveSettingsBtn, LoadSettingsBtn, KeyBindsBtn, CloseKeybindsBtn;
    public Dropdown RecordingOptions;

    // Start is called before the first frame update
    void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        PopulateDropdownWithRecordDevices();

        KeyBindsBtn.onClick.AddListener(ToggleKeybindsWindow);
        CloseKeybindsBtn.onClick.AddListener(ToggleKeybindsWindow);
        SaveSettingsBtn.onClick.AddListener(SettingsSaverScript.SaveAllSynthsSettingsWithFileDialog);
        LoadSettingsBtn.onClick.AddListener(settingsLoader.OpenFileBrowser);
    }
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
    public void SetSelectedRecordDevice(int selectedIndex)
    {
        AudioRecorder.RecordIndex = selectedIndex;
        Debug.Log($"Geselecteerd opnameapparaat: {RecordingOptions.options[selectedIndex].text}");
    }
    public void ToggleKeybindsWindow()
    {
        // Schakelt de zichtbaarheid van het settingsPanel
        KeybindsPanel.SetActive(!KeybindsPanel.activeSelf);
        SettingsPanel.SetActive(!SettingsPanel.activeSelf);
    }
}
