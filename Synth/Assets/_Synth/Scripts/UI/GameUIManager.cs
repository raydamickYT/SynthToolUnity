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
    private bool Isrecording = false, IsCoroutineRunning = false;
    private AudioRecorder audioRecorder;
    private Color defaultColor; // Standaardkleur
    private Color toggledColor = Color.red; // Kleur wanneer getoggled
    public Dropdown RecordingLengthDropDown;
    public Text TimerText;

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
        StartCoroutine(CountdownCoroutine(audioRecorder.RecordingLenghtInt));
    }

    public void StopRecording()
    {
        audioRecorder.StopRecording();
        Isrecording = false;
        ToggleButtonColor();
        if (IsCoroutineRunning)
        {
            StopCoroutine(CountdownCoroutine(audioRecorder.RecordingLenghtInt));
            TimerText.GetComponent<Transform>().gameObject.SetActive(false);
        }
    }

    public void ToggleButtonColor()
    {
        RecordButtonStart.GetComponent<Image>().color = Isrecording ? toggledColor : defaultColor; // Pas de kleur aan
    }

    IEnumerator CountdownCoroutine(float duration)
    {
        IsCoroutineRunning = true;
        TimerText.GetComponent<Transform>().gameObject.SetActive(true);
        float remainingTime = duration;
        while (remainingTime > 0)
        {
            // Update de tekst van het Text object elke frame
            TimerText.text = "Recording Time: " + Mathf.Ceil(remainingTime).ToString();

            yield return null;

            remainingTime -= Time.deltaTime;
        }

        // Zet de timer op 0 als de aftelling klaar is
        TimerText.text = "Recording Time: 0";
        TimerText.GetComponent<Transform>().gameObject.SetActive(false);
        IsCoroutineRunning = false;
        StopRecording(); //stop automatisch de recording als de timer klaar is
    }
}
