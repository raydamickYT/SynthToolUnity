using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SaveSettings : MonoBehaviour
{
    private GlobalSynthSettings globalSynthSettings;
    private SettingsSaver settingsSaver = new();
    private SettingsLoader settingsLoader = new();
    private Dropdown dropdown;
    private Button button;
    // Start is called before the first frame update
    void Start()
    {
        globalSynthSettings = GlobalSynthSettings.instance;

        dropdown = GetComponentInChildren<Dropdown>();
        dropdown.gameObject.SetActive(false);
        dropdown.onValueChanged.AddListener(DropDownSettings);
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(ToggleDropdown);
    }

    public void ToggleDropdown()
    {
        dropdown.gameObject.SetActive(!dropdown.gameObject.activeSelf); // Toggle de zichtbaarheid van de Dropdown
    }


    public void DropDownSettings(int r)
    {
        switch (r)
        {
            case 0: //default
                break;
            case 1: //save
                SaveSynthSettings();
                break;
            case 2:
                LoadSettings();
                break;
            default:
                break;
        }
    }

    public void SaveSynthSettings()
    {
        SynthState synthState = new SynthState(globalSynthSettings.Frequency, (uint)globalSynthSettings.SampleRate, globalSynthSettings.CurrentWaveForm);
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

    // Update is called once per frame
    void Update()
    {

    }
}
