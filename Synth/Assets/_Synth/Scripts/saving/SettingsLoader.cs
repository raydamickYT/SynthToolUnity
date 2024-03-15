using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SettingsLoader : MonoBehaviour
{
    [SerializeField] private UIManager uIManager;
    // Methode om instellingen te laden vanuit een bestand
    public void LoadSettings(string filePath, SynthInfo _state)
    {
        try
        {
            // Lees alle regels uit het opgegeven bestand
            string[] lines = File.ReadAllLines(filePath);

            // Loop door elke regel en verwerk de instellingen
            foreach (string line in lines)
            {
                // Split de regel op de ':' om de sleutel en waarde te scheiden
                string[] parts = line.Split(':');
                string key = parts[0].Trim();
                string value = parts[1].Trim();

                // Verwerk de instelling (hier gaan we ervan uit dat elke instelling slechts één woord is)
                switch (key)
                {
                    case "Frequency":
                        _state.Frequency = float.Parse(value);
                        break;
                    case "SamplingFrequency":
                        _state.SamplingFrequency = uint.Parse(value);
                        break;
                    case "CarrierPhase":
                        _state.CarrierPhase = float.Parse(value);
                        break;
                    case "currentWaveForm":
                        // Omzetten van string naar enum
                        _state.CurrentWaveForm = (WaveForm)Enum.Parse(typeof(WaveForm), value);
                        break;
                    // Voeg hier meer cases toe voor andere instellingen indien nodig
                    default:
                        Debug.LogWarning("Unknown setting: " + key);
                        break;
                }
            }

            Debug.Log("Settings loaded successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError("Error loading settings: " + e.Message);
        }
        ApplySettings(_state);
    }


    void ApplySettings(SynthInfo _state)
    {
        //uimanager updaten
        Debug.Log(uIManager);
        uIManager.ChangeWave.value = (int)_state.CurrentWaveForm;
        uIManager.FrequencySlider.value = _state.Frequency;
    }
}
