using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WaveformVisualizer : MonoBehaviour
{
    public Material LineMat;
    public Synth mySynth; // Verwijs naar je Synth component
    private LineRenderer lineRenderer;
    public float scale = 1;
    public Vector3 Offset;

    void Start()
    {
        if (lineRenderer == null)
        {
            if (LineMat != null)
            {
                lineRenderer = GetComponent<LineRenderer>();
                lineRenderer.material = LineMat;
            }
        }
    }

    void Update()
    {
        VisualizeWaveform();
    }

    void VisualizeWaveform()
    {
        float[] synthDataBuffer = mySynth.GetCurrentAudioBuffer(); // Implementeer deze methode in je Synth klasse
        int samples = synthDataBuffer.Length;

        lineRenderer.positionCount = samples;
        Vector3[] points = new Vector3[samples];

        // Vector3 offset = new Vector3(-4, 3, 0); // Beginpositie voor de waveform
        for (int i = 0; i < samples; i++)
        {
            float x = i * (1.0f / samples) * scale + Offset.x;
            float y = synthDataBuffer[i] + Offset.y; // 'scale' is jouw schaalverdeling voor y
            points[i] = new Vector3(x, y, Offset.z);
        }
        lineRenderer.SetPositions(points);

    }
}
