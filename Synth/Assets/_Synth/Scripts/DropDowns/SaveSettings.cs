using System.Collections;
using System.Collections.Generic;
using SFB;
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
        // globalSynthSettings = SynthInfo.instance;


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
        // SynthInfo synthState = SynthInfo.instance;
        // settingsSaver.SaveSettingsWithFileDialog(synthState);
    }


    public void LoadSettings()
    {
        // SynthInfo synthState = SynthInfo.instance;

        // Vraag de gebruiker om een bestand te kiezen om te laden
        var extensions = new[] {
        new ExtensionFilter("Text Files", "txt"),
    };

        // StandaloneFileBrowser.OpenFilePanel retourneert een array van paden, we gebruiken hier de eerste
        // Let op: OpenFilePanel retourneert een lege array als de gebruiker annuleert
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Load Settings", "", extensions, false);

        if (paths.Length > 0)
        {
            // Controleer of een pad is gekozen
            string filePath = paths[0];
            if (!string.IsNullOrEmpty(filePath))
            {
                // Laad de instellingen vanuit het gekozen bestandspad
                // settingsLoader.LoadSettings(filePath, synthState);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
