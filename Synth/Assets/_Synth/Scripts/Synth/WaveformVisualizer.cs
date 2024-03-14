using System;
using UnityEngine;
using UnityEngine.UI;

public class WaveformVisualizer : MonoBehaviour
{
    public static WaveformVisualizer instance;
    public RawImage rawImage; // Verbind dit met je RawImage component in de Inspector
    public int textureWidth = 1024;
    public int textureHeight = 128;
    private Texture2D waveformTexture;

    void Start()
    {
        instance = this;
        waveformTexture = new Texture2D(textureWidth, textureHeight);
        rawImage.texture = waveformTexture;
        GenerateWaveformTexture(); // Voorbeeldaanroep
    }

    void GenerateWaveformTexture()
    {
        // Voorbeeldgegevens: genereer een sinusgolf
        float[] audioSamples = new float[textureWidth];
        for (int i = 0; i < textureWidth; i++)
        {
            audioSamples[i] = Mathf.Sin(i * 0.1f); // Vervang dit door je echte audiogegevens
        }

        // Teken de golfvorm
        for (int x = 0; x < textureWidth; x++)
        {
            float sample = audioSamples[x];
            int y = (int)((sample + 1f) / 2f * (textureHeight - 1)); // Normaliseer en schaal
            waveformTexture.SetPixel(x, y, Color.white);
        }
        waveformTexture.Apply();
    }

    public void UpdateWaveform(float[] audioSamples)
    {
        // Definieer hoe dik de lijn moet zijn
        int lineThickness = 3; // Hoeveel pixels dik de lijn moet zijn
        int halfThickness = lineThickness / 2;

        for (int x = 0; x < textureWidth; x++)
        {
            // Zorg ervoor dat we niet buiten de arraygrenzen gaan
            if (x < audioSamples.Length)
            {
                float sample = audioSamples[x];
                int y = (int)((sample + 1f) / 2f * (textureHeight - 1)); // Normaliseer en schaal

                // Teken een "dikkere" lijn voor elk punt in de golfvorm
                for (int py = 0; py < textureHeight; py++)
                {
                    if (Math.Abs(py - y) <= halfThickness)
                    {
                        waveformTexture.SetPixel(x, py, Color.white);
                    }
                    else
                    {
                        waveformTexture.SetPixel(x, py, Color.clear);
                    }
                }
            }
        }
        waveformTexture.Apply();
    }


}
