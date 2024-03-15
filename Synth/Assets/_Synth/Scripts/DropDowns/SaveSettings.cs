using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SaveSettings : MonoBehaviour
{
    private SynthInfo globalSynthSettings;
    private SettingsSaver settingsSaver = new();
    public SettingsLoader settingsLoader;
    private Dropdown dropdown;
    private Button button;
    // Start is called before the first frame update
    void Start()
    {
        globalSynthSettings = SynthInfo.instance;

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
        SynthInfo synthState = SynthInfo.instance;
        settingsSaver.SaveSettingsWithFileDialog(synthState);
    }

    public void LoadSettings()
    {
        SynthInfo synthState = SynthInfo.instance;

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
