using System;
using UnityEngine;
using FMOD;
using FMODUnity;
using SFB;
using System.IO;
using System.Runtime.InteropServices; // standalone file browser

public class AudioRecorder : MonoBehaviour
{
    [SerializeField] private VoorbeeldScript voorbeeldScript;
    private FMOD.System system; // FMOD low-level system instance
    private FMOD.Sound sound; // Sound object to hold the recording
    private uint soundLength; // Length of the recording
    private bool isRecording = false; // Flag to check if currently recording

    void Start()
    {
        // Initialize the FMOD system
        system = RuntimeManager.CoreSystem;
    }

    // Start the recording
    public void StartRecording()
    {
        if (isRecording) return; // Check if already recording

        system.createSound("record.wav", MODE.CREATESTREAM | MODE.LOOP_NORMAL | MODE.OPENUSER, out sound);
        system.recordStart(0, sound, true);
        isRecording = true;
        UnityEngine.Debug.Log("Recording started...");
    }

    public void StopRecording()
    {
        if (!isRecording) return; // Check if not currently recording

        system.recordStop(0);
        isRecording = false;
        UnityEngine.Debug.Log("Recording stopped.");

        // Laat de gebruiker kiezen waar de opname op te slaan
        string paths = StandaloneFileBrowser.SaveFilePanel("Save Recording", "", "MyRecording", "wav");
        if (paths.Length > 0 && !string.IsNullOrEmpty(paths))
        {
            SaveRecording(paths);
        }
    }

    // Slaat de opname op op de gespecificeerde locatie
    // Aangenomen dat deze methode de opnamepad krijgt waar het bestand moet worden opgeslagen
    private void SaveRecording(string path)
    {
        FMOD.RESULT result;
        IntPtr ptr1 = IntPtr.Zero, ptr2 = IntPtr.Zero;
        uint len1 = 0, len2 = 0;

        // Aangenomen dat 'sound' en 'soundLength' eerder correct zijn ingesteld.
        // Verkrijg de lengte van het geluid om te bepalen hoeveel data we moeten vergrendelen en lezen.
        sound.getLength(out uint soundLength, FMOD.TIMEUNIT.PCM);

        // Probeer het gehele geluid te vergrendelen voor toegang.
        // de @ is noodzakelijk om de compiler te vertellen dat j e de lock methode van het fmod.sound object
        // bedoelt. en niet het lock keyword van c#
    result = sound.@lock(0, soundLength, out ptr1, out ptr2, out len1, out len2);
        if (result != FMOD.RESULT.OK)
        {
            UnityEngine.Debug.LogError("Failed to lock sound: " + result.ToString());
            return;
        }

        // Kopieer de audio data van de pointer naar een byte array.
        byte[] audioData = new byte[len1];
        Marshal.Copy(ptr1, audioData, 0, (int)len1);

        // Hier zou je de logica implementeren om 'audioData' naar een bestand te schrijven, zoals eerder besproken.

        // Vergeet niet het geluid te ontgrendelen nadat je klaar bent.
        result = sound.unlock(ptr1, ptr2, len1, len2);
        if (result != FMOD.RESULT.OK)
        {
            UnityEngine.Debug.LogError("Failed to unlock sound: " + result.ToString());
        }
    }



    // Methode om een eenvoudige WAV-header te schrijven. Dit is vereenvoudigd en gaat ervan uit dat de audio 16-bit PCM is.
    private void WriteWavHeader(FileStream stream, int dataLength)
    {
        byte[] header = new byte[44];
        // RIFF header
        WriteBytes(header, 0, "RIFF");
        WriteInt32(header, 4, dataLength + 36); // Bestandsgrootte minus de eerste 8 bytes van de RIFF beschrijving
        WriteBytes(header, 8, "WAVE");
        WriteBytes(header, 12, "fmt ");
        WriteInt32(header, 16, 16); // Lengte van het formaat data
        WriteInt16(header, 20, 1); // Type formaat (1 is PCM)
        WriteInt16(header, 22, 2); // Aantal kanalen
        WriteInt32(header, 24, voorbeeldScript.sampleRate); // Samplefrequentie
        WriteInt32(header, 28, voorbeeldScript.sampleRate * 4); // Byte rate (Sample Rate * Block Align)
        WriteInt16(header, 32, 4); // Block align (Number of Channels * BitsPerSample / 8)
        WriteInt16(header, 34, 16); // Bits per sample
        WriteBytes(header, 36, "data");
        WriteInt32(header, 40, dataLength); // Subchunk2Size (NumSamples * NumChannels * BitsPerSample/8)

        stream.Write(header, 0, header.Length);
    }

    private void WriteBytes(byte[] buffer, int start, string value)
    {
        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(value);
        Buffer.BlockCopy(bytes, 0, buffer, start, bytes.Length);
    }

    private void WriteInt32(byte[] buffer, int offset, int value)
    {
        byte[] intBytes = BitConverter.GetBytes(value);
        Buffer.BlockCopy(intBytes, 0, buffer, offset, intBytes.Length);
    }

    private void WriteInt16(byte[] buffer, int offset, short value)
    {
        byte[] intBytes = BitConverter.GetBytes(value);
        Buffer.BlockCopy(intBytes, 0, buffer, offset, intBytes.Length);
    }

    // Andere delen van het script...

    // Example function to process the recorded sound
    void ProcessRecording()
    {
        // Implement processing or saving of the recording here
        // This might involve saving to disk, applying effects, etc.

        UnityEngine.Debug.Log("Processing recording...");
    }

    void OnDestroy()
    {
        // Clean up
        sound.release();
    }
}
