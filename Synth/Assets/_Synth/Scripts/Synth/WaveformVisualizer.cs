using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(LineRenderer))]
public class WaveformVisualizer : MonoBehaviour
{
    public Material LineMat;
    public Synth mySynth;
    private LineRenderer lineRenderer;
    public float scale = 1;
    public Vector3 Offset;
    public Vector2 Size = new Vector2(887, 270); //scroll view height

    /// <summary>
    /// IMPORTANT: door problemen met de waveform in de scrollview heb ik deze class niet meer gebruikt. Ik laat hem er in zitten omdat het nog van pas kan komen
    /// </summary>

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

        float[] synthDataBuffer = mySynth.GetCurrentAudioBuffer();
        int samples = synthDataBuffer.Length;

        lineRenderer.positionCount = samples;
        Vector3[] points = new Vector3[samples];

        for (int i = 0; i < samples; i++)
        {
            float x = i * (1.0f / samples) * scale + Offset.x;
            float y = synthDataBuffer[i] + Offset.y;
            points[i] = new Vector3(x, y, Offset.z);
        }
        lineRenderer.SetPositions(points);

    }
}
