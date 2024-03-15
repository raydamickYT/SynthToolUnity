using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;

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
    private string[] GetAllSettings(SynthState _state)
    {
        // Definieer de bestandsnaam voor het opslaan van de instellingen
        // string fileName = "settings.txt";

        // Definieer de instellingen die je wilt opslaan
        string[] settings = new string[]
        {
            "SampleRate: " + _state.SamplingFrequency,
            "currentWaveForm: " + _state.CurrentWaveForm.ToString(),
            "Frequency: " + _state.Frequency
            // Voeg hier meer instellingen toe indien nodig
        };

        // Roep de methode aan om de instellingen op te slaan
        // SaveSettings(fileName, settings);

        return settings;
    }

    public void SaveSettingsWithFileDialog(SynthState _state)
    {
        // Vraag de gebruiker om een bestandspad te kiezen
        string filePath = EditorUtility.SaveFilePanel("Save Settings", "", "settings.txt", "txt");

        // Controleer of de gebruiker een pad heeft gekozen
        if (!string.IsNullOrEmpty(filePath))
        {
            // Sla de instellingen op met het gekozen bestandspad
            SaveSettings(filePath, GetAllSettings(_state));
        }
    }
}
