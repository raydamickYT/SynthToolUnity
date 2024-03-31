using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public GameObject settingsPanel; // Verwijzing naar je Settings Panel
    public Button SettingsButton, CloseButton, RecordButtonStart, RecordButtonStop;
    public bool Isrecording = false;
    private AudioRecorder audioRecorder;
    public Color defaultColor; // Standaardkleur
    public Color toggledColor = Color.red; // Kleur wanneer getoggled
    public Dropdown RecordingLengthDropDown;

    private void Start()
    {

        if (RecordButtonStart != null)
        {
            defaultColor = RecordButtonStart.colors.normalColor;
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

        RecordingLengthDropDown.onValueChanged.AddListener(audioRecorder.ChangeRecordingLenght);
    }

    public void ToggleSettingsWindow()
    {
        // Schakelt de zichtbaarheid van het settingsPanel
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void StartRecording()
    {
        audioRecorder.StartRecording();
        Isrecording = true;
        ToggleButtonColor();
    }

    public void StopRecording()
    {
        audioRecorder.StopRecording();
        Isrecording = false;
        ToggleButtonColor();
    }

    public void ToggleButtonColor()
    {
        RecordButtonStart.GetComponent<Image>().color = Isrecording ? toggledColor : defaultColor; // Pas de kleur aan
    }
}
