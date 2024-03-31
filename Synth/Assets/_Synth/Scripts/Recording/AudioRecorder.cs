using System;
using UnityEngine;
using FMOD;
using FMODUnity;
using SFB;
using System.IO;
using System.Runtime.InteropServices;
using FMODUnityResonance;
using System.Collections.Generic;
using UnityEngine.UI; // standalone file browser

public class AudioRecorder
{
    // Synth Synth;
    private int sampleRate = 44100; // Standaard sample rate
    private FMOD.System system; // FMOD low-level system instance
    private FMOD.Sound sound; // Sound object to hold the recording
    private bool isRecording = false; // Flag to check if currently recording
    private int numChannels = 2; // Aantal kanalen (stereo)
    private int bitDepth = 16; //16, 24 of 32
    public static int RecordIndex;
    public int RecordingLenghtInt = 10;

    public AudioRecorder()
    {
        // Synth = _synth;
        // Initialize the FMOD system
        system = RuntimeManager.CoreSystem;
    }

    public void ChangeRecordingLenght(int Index)
    {
        UnityEngine.Debug.Log(Index);
        switch (Index)
        {
            case 0:
                RecordingLenghtInt = 10;
                break;
            case 1:
                RecordingLenghtInt = 20;
                break;
            case 2:
                RecordingLenghtInt = 30;
                break;
            case 3:
                RecordingLenghtInt = 60;
                break;
            default:
                break;
        }
    }

    // Start the recording
    public void StartRecording()
    {
        if (isRecording) return; // Check if already recording
        FMOD.CREATESOUNDEXINFO soundExInfo = new FMOD.CREATESOUNDEXINFO
        {
            cbsize = Marshal.SizeOf(typeof(FMOD.CREATESOUNDEXINFO)),
            numchannels = 2, // Stereo
            defaultfrequency = 44100, // 44100 Hz
            format = FMOD.SOUND_FORMAT.PCM16, // 16-bit PCM audio
            length = (uint)(RecordingLenghtInt * sampleRate * numChannels * sizeof(short)) // Voor 10 seconden opname
        };

        FMOD.RESULT result = system.createSound((String)null, FMOD.MODE.CREATESAMPLE | FMOD.MODE.LOOP_OFF | FMOD.MODE.OPENUSER, ref soundExInfo, out sound);
        if (result != FMOD.RESULT.OK)
        {
            UnityEngine.Debug.LogError("Failed to create sound for recording: " + result.ToString());
            return;
        }

        UnityEngine.Debug.Log("Recording index: " + RecordIndex);
        system.recordStart(RecordIndex, sound, true);
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
        string path = StandaloneFileBrowser.SaveFilePanel("Save Recording", "", "MyRecording", "wav");
        if (!string.IsNullOrEmpty(path))
        {
            SaveRecording(path);
        }
    }

    private void SaveRecording(string path)
    {
        FMOD.RESULT result;
        IntPtr ptr1 = IntPtr.Zero, ptr2 = IntPtr.Zero;
        uint len1 = 0, len2 = 0;
        uint length = 0;

        // Eerst ophalen van de geluidslengte
        result = sound.getLength(out length, FMOD.TIMEUNIT.PCMBYTES);
        if (result != FMOD.RESULT.OK)
        {
            UnityEngine.Debug.LogError("Failed to get sound length: " + result.ToString());
            return;
        }

        // Nu het geluid vergrendelen met de verkregen lengte
        result = sound.@lock(0, length, out ptr1, out ptr2, out len1, out len2);
        if (result != FMOD.RESULT.OK)
        {
            UnityEngine.Debug.LogError("Failed to lock sound: " + result.ToString());
            return;
        }

        // Kopieer de audio data van de pointer naar een byte array.
        byte[] audioData = new byte[len1];
        Marshal.Copy(ptr1, audioData, 0, (int)len1);

        // Nu het geluid ontgrendelen
        result = sound.unlock(ptr1, ptr2, len1, len2);
        if (result != FMOD.RESULT.OK)
        {
            UnityEngine.Debug.LogError("Failed to unlock sound: " + result.ToString());
        }

        using (FileStream fileStream = new FileStream(path, FileMode.Create))
        {
            WriteWavHeader(fileStream, audioData.Length, sampleRate, numChannels, bitDepth); // Schrijf een WAV-header naar het bestand.
            fileStream.Write(audioData, 0, audioData.Length); // Schrijf de audio data.
        }

        UnityEngine.Debug.Log($"Recording saved to: {path}");
    }


    private void WriteWavHeader(FileStream stream, int dataLength, int sampleRate, int numChannels, int bitsPerSample)
    {
        int blockAlign = numChannels * (bitsPerSample / 8);
        int byteRate = sampleRate * blockAlign;

        byte[] header = new byte[44];

        // RIFF header
        WriteBytes(header, 0, "RIFF");
        WriteInt32(header, 4, 36 + dataLength); // Bestandsgrootte min> de eerste 8 bytes van de RIFF beschrijving
        WriteBytes(header, 8, "WAVE");

        // fmt subchunk
        WriteBytes(header, 12, "fmt ");
        WriteInt32(header, 16, 16); // Lengte van 'fmt' subchunk (16 voor PCM)
        WriteInt16(header, 20, 1); // Audioformat (1 = PCM)
        WriteInt16(header, 22, (short)numChannels);
        WriteInt32(header, 24, sampleRate);
        WriteInt32(header, 28, byteRate); // Byte rate
        WriteInt16(header, 32, (short)blockAlign); // Block align
        WriteInt16(header, 34, (short)bitsPerSample);

        // data subchunk
        WriteBytes(header, 36, "data");
        WriteInt32(header, 40, dataLength); // Subchunk2Size = NumSamples * NumChannels * BitsPerSample/8

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

    void OnDestroy()
    {
        // Clean up
        sound.release();
    }
}
public enum RecordingLength
{
    TenSeconds,
    TwentySeconds,
    ThirtySeconds,
    OneMinute
}
