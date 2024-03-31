using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class KeybindManager : MonoBehaviour
{
    public static KeybindManager Instance;
    public KeyData keyData = new();
    public List<Button> actionButtons; 
    public GameObject ButtonsParent;
    public Text waitingForKeyText; 
    private SynthAction actionToRebind = null; 
    public Button ApplyBtn, ResetBtn;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        InitializeButtons();
    }
    private void Start()
    {
        LoadKeybindsFromFile();
    }

    void Update()
    {
        if (actionToRebind != null)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    // Update de SynthAction met de nieuwe KeyCode
                    actionToRebind.Key = keyCode;
                    UpdateKeybindButtonTexts();
                    actionToRebind = null; // Reset de actie die opnieuw gebonden wordt
                    waitingForKeyText.gameObject.SetActive(false); // Verberg de 'wacht op key' indicator
                    break;
                }
            }
        }
    }

    public void InitializeButtons()
    {
        int Index = 0;
        if (ButtonsParent != null)
        {
            actionButtons.AddRange(ButtonsParent.GetComponentsInChildren<Button>());
            foreach (Button button in actionButtons)
            {
                var currentIndex = Index;
                button.onClick.AddListener(() => StartRebindingAction("Synth" + currentIndex));
                Index++;
            }
            UpdateKeybindButtonTexts();
        }
        if (ApplyBtn != null)
        {
            ApplyBtn.onClick.AddListener(SaveKeybindsToFile);
        }
        if (ResetBtn != null)
        {
            ResetBtn.onClick.AddListener(ResetKeybinds);
        }
    }
    public void UpdateKeybindButtonTexts()
    {
        int Index = 0;
        foreach (var action in keyData.synthActions)
        {
            Button button = actionButtons[Index];
            if (button != null)
            {
                Text buttonText = button.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = action.Key.ToString();
                }
            }
            Index++;
        }
    }

    public void StartRebindingAction(string actionName)
    {
        actionToRebind = keyData.synthActions.Find(action => action.SynthName == actionName);
        if (actionToRebind != null)
        {
            waitingForKeyText.gameObject.SetActive(true);
            Debug.Log(actionName);
            waitingForKeyText.text = "Press any key for " + actionName;
        }
    }

    public void SaveKeybindsToFile()
    {
        string path = Path.Combine(Application.persistentDataPath, "keybinds.json");
        string json = JsonUtility.ToJson(keyData, true);
        File.WriteAllText(path, json);
    }

    public void LoadKeybindsFromFile()
    {
        string path = Path.Combine(Application.persistentDataPath, "keybinds.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, keyData);
            UpdateKeybindButtonTexts();
        }
        else
        {
            // Als het bestand niet bestaat, sla dan de huidige keybinds op 
            SaveKeybindsToFile();
        }
    }
    public void ResetKeybinds()
    {
        Debug.Log("resetting");
        keyData.synthActions = new List<SynthAction>(keyData.synthActionsBackup.Count);
        foreach (SynthAction action in keyData.synthActionsBackup)
        {
            keyData.synthActions.Add(new SynthAction(action.Key, action.SynthIndex, action.SynthName));
        }
        UpdateKeybindButtonTexts();
        SaveKeybindsToFile();
    }
}
