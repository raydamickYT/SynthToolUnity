using System;
using System.IO;
using System.Linq;
using UnityEngine;
using SFB;

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
                    var synth = GameManager.Instance.synths.FirstOrDefault(s => s.name == synthName);
                    if (synth != null)
                    {
                        currentSynthState = synth.synthState;
                    }
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
                            currentSynthState.sineFrequency = float.Parse(value);
                            break;
                        case "SamplingFrequency":
                            currentSynthState.SamplingFrequency = uint.Parse(value);
                            break;
                        case "CarrierPhase":
                            currentSynthState.CarrierPhase = float.Parse(value);
                            break;
                        case "Volume":
                            currentSynthState.volume = float.Parse(value);
                            break;
                        case "Enabled":
                            if (bool.TryParse(value, out bool result))
                            {
                                Debug.Log(currentSynthState.DSPIsActive);
                                currentSynthState.DSPIsActive = result;
                                Debug.Log(currentSynthState.DSPIsActive);
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
                        // Add more cases as needed
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
