using UnityEngine;

public class AudioDataProcessor : MonoBehaviour
{
    [SerializeField] private Synth voorbeeldScript;
    // Veronderstel dat deze buffer gevuld wordt met audio-data van je DSP callback
    private float[] audioDataBuffer;
    private int bufferSize = 1024; // Een voorbeeld buffer grootte, pas aan aan je DSP configuratie

    void Start()
    {
        // Initialisatie van je audioDataBuffer, misschien ergens in je DSP callback
        audioDataBuffer = voorbeeldScript.sharedBuffer;
        bufferSize = (int)voorbeeldScript.mBufferLength;
    }

    // Een voorbeeld methode die aangeroepen kan worden om de gemiddelde amplitude te berekenen
    public float CalculateAverageAmplitude()
    {
        if (audioDataBuffer == null) return 0f;

        float sum = 0f;

        // Loop door de buffer en bereken de som van de absolute waarden van de samples
        for (int i = 0; i < audioDataBuffer.Length; i++)
        {
            sum += Mathf.Abs(audioDataBuffer[i]);
        }

        // Bereken het gemiddelde door de som te delen door het aantal samples
        return sum / audioDataBuffer.Length;
    }

    void Update()
    {
        // Voorbeeld van het updaten van de audioDataBuffer met nieuwe data uit je DSP callback
        // Dit zou typisch worden gedaan in de DSP callback zelf of in een methode die door de callback wordt aangeroepen

        // Voorbeeld van het gebruik van CalculateAverageAmplitude
        float averageAmplitude = CalculateAverageAmplitude();
        Debug.Log($"Gemiddelde amplitude: {averageAmplitude}");
    }
}
