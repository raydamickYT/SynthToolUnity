using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using SFB;

public class SettingsSaver
{
    // Methode om de instellingen op te slaan
    private void SaveSettings(string filePath, string[] settings)
    {
        try
        {
            // Combineer het opgegeven bestandspad
            string fullPath = Path.Combine(filePath);

            // Schrijf alle instellingen naar het opgegeven bestand
            File.WriteAllLines(fullPath, settings);
            Debug.Log("Settings saved successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving settings: " + e.Message);
        }
    }

    // Voorbeeldgebruik: Roep deze methode aan wanneer je de instellingen wilt opslaan
    private List<string> GetAllSynthsSettings()
    {
        List<string> allSettings = new List<string>();

        foreach (Synth synth in GameManager.Instance.synths)
        {
            // Haal de SynthInfo van de huidige synth
            SynthInfo _state = synth.synthState; // Zorg ervoor dat je een methode in Synth hebt die SynthInfo retourneert
            allSettings.Add("[" + synth.name + "]");
            allSettings.Add("SamplingFrequency: " + _state.SamplingFrequency);
            allSettings.Add("currentWaveForm: " + _state.CurrentWaveForm.ToString());
            allSettings.Add("Frequency: " + _state.sineFrequency);
            allSettings.Add("Volume: " + _state.volume);
            allSettings.Add("Enabled: " + _state.DSPIsActive);
            allSettings.Add(""); // Voeg een lege regel toe als scheiding tussen de instellingen van verschillende synths
        }
        return allSettings;
    }

    public void SaveAllSynthsSettingsWithFileDialog()
    {
        // Vraag de gebruiker om een bestandspad te kiezen
        var extensions = new[] {
        new ExtensionFilter("Text Files", "txt"),
        new ExtensionFilter("All Files", "*"),
    };

        // StandaloneFileBrowser.SaveFilePanel retourneert direct het pad als een string
        string filePath = StandaloneFileBrowser.SaveFilePanel("Save Settings", "", "synthSettings", extensions);

        // Controleer of de gebruiker een pad heeft gekozen
        if (!string.IsNullOrEmpty(filePath))
        {
            // Sla de instellingen op met het gekozen bestandspad
            SaveSettings(filePath, GetAllSynthsSettings().ToArray());
        }
    }
}
