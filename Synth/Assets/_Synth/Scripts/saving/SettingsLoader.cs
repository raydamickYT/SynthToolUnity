using System;
using System.IO;
using System.Linq;
using UnityEngine;
using SFB;
using Unity.VisualScripting;

public class SettingsLoader
{
    public void OpenFileBrowser()
    {
        // Open een bestandskiezer dialoog en vraag de gebruiker om een bestand te selecteren
        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "", false);

        // Controleer of de gebruiker een bestand heeft geselecteerd
        if (paths.Length > 0)
        {
            string filePath = paths[0];
            Debug.Log("Geselecteerd bestandspad: " + filePath);
            LoadSettings(filePath);
        }
    }

    // Methode om instellingen te laden vanuit een bestand
    private void LoadSettings(string filePath)
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);
            SynthInfo currentSynthState = null;

            foreach (string line in lines)
            {
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    // Extract synth name from the section
                    string synthName = line.Trim('[', ']');
                    // Find the corresponding Synth based on its name
                    Synth synth = GameManager.Instance.synths.FirstOrDefault(s => s.name == synthName);
                    if (synth == null)
                    {
                        // Als de synth niet bestaat, maak dan een nieuwe aan.
                        Debug.Log("Pipo");
                        if (AddingSynths.Instance != null)
                        {
                            synth = AddingSynths.Instance.AddExtraSynths(); // Zorg dat OnAddSynth de synthnaam accepteert en een nieuwe Synth retourneert
                        }
                        Debug.Log($"Nieuwe synth toegevoegd: {synthName}");
                    }
                    else
                    {
                        Debug.Log($"Synth gevonden: {synthName}");
                    }
                    currentSynthState = synth.synthState;
                }
                else if (currentSynthState != null && line.Contains(":"))
                {
                    // Process the settings
                    string[] parts = line.Split(':');
                    if (parts.Length < 2) continue; // Guard clause for malformed lines

                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    switch (key)
                    {
                        case "Frequency":
                            currentSynthState.uIManager.ChangeFreq(float.Parse(value));
                            break;
                        case "SamplingFrequency":
                            currentSynthState.SamplingFrequency = uint.Parse(value);
                            break;
                        case "CarrierPhase":
                            currentSynthState.CarrierPhase = float.Parse(value);
                            break;
                        case "Volume":
                            // currentSynthState.volume = float.Parse(value);
                            currentSynthState.uIManager.ChageVol(float.Parse(value));
                            break;
                        case "Enabled":
                            if (bool.TryParse(value, out bool result))
                            {
                                currentSynthState.DSPIsActive = result;
                            }
                            else
                            {
                                Debug.LogWarning("Onjuiste waarde voor Enabled: " + value);
                            }
                            break;
                        case "currentWaveForm":
                            WaveForm waveFormResult;
                            if (Enum.TryParse(value, out waveFormResult))
                            {
                                currentSynthState.CurrentWaveForm = waveFormResult;
                            }
                            break;
                        default:
                            Debug.LogWarning($"Unknown setting '{key}'");
                            break;
                    }
                }
            }
            Debug.Log("Settings loaded successfully for all synths.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading settings: {e.Message}");
        }
    }
    
}
