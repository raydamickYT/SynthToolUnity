using UnityEngine;

// public class 2GlobalSynthSettings : MonoBehaviour
// {
//     public static GlobalSynthSettings instance;
//     public bool DSPIsActive = false;
//     public WaveForm CurrentWaveForm = WaveForm.Sawtooth; // Standaard golfvorm
//     public int mChannels = 0;
//     public float Frequency = 440f; // Frequentie van de sinusgolf in Hz
//     public int SampleRate = 48000; // Stel dit in op de daadwerkelijke sample rate van je systeem

//     // Start is called before the first frame update
//     private void Awake()
//     {
//         if (instance == null)
//         {
//             instance = this;
//         }
//         else
//         {
//             Destroy(this);
//         }
//     }

//     void OnDestroy()
//     {
//         instance = null;
//     }

//     public void UpdateSettings(bool isactive, WaveForm current, int channels, float sincefreq, int sample)
//     {
//         DSPIsActive = isactive;
//         CurrentWaveForm = current;
//         mChannels = channels;
//         Frequency = sincefreq;
//         SampleRate = sample;
//     }
// }
