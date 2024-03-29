using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public Synth SynthObject;
    private SynthInfo SynthInfo;
    public GameObject settingsPanel; // Verwijzing naar je Settings Panel
    public Button SettingsButton, CloseButton, RecordButtonStart, RecordButtonStop;
    private AudioRecorder audioRecorder;

    private void Start()
    {
        if (SynthInfo == null)
        {
            SynthInfo = SynthObject.synthState;
        }
        settingsPanel.SetActive(false);
        Initialization();
    }

    private void Initialization()
    {
        audioRecorder = new();

        SettingsButton.onClick.AddListener(ToggleSettingsWindow);
        CloseButton.onClick.AddListener(ToggleSettingsWindow);
        RecordButtonStart.onClick.AddListener(StartRecording);
        RecordButtonStop.onClick.AddListener(StopRecording);
    }

    public void ToggleSettingsWindow()
    {
        // Schakelt de zichtbaarheid van het settingsPanel
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void StartRecording()
    {
        audioRecorder.StartRecording();
    }

    public void StopRecording()
    {
        audioRecorder.StopRecording();
    }
}
