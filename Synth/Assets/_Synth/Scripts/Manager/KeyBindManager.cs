using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using ScrollView = UnityEngine.UIElements.ScrollView;

public class KeybindManager : MonoBehaviour
{
    public static KeybindManager Instance;
    public List<SynthAction> synthActions = new List<SynthAction>(){ 
        //vul hier wat leuks in
        new SynthAction(KeyCode.Z, 0, "Synth0"),
        new SynthAction(KeyCode.A, 1, "Synth1"),
        new SynthAction(KeyCode.S, 2, "Synth2"),
        new SynthAction(KeyCode.X, 3, "Synth3"),
        new SynthAction(KeyCode.C, 4, "Synth4"),
        new SynthAction(KeyCode.D, 5, "Synth5"),
        new SynthAction(KeyCode.F, 6, "Synth6"),
        new SynthAction(KeyCode.V, 7, "Synth7"),
        new SynthAction(KeyCode.E, 8, "Synth8"),
        new SynthAction(KeyCode.W, 9, "Synth9"),

    };
    public List<Button> actionButtons; // Een lijst met alle UI knoppen
    public GameObject ButtonsParent;
    public Text waitingForKeyText; // Een tekst UI element dat aangeeft dat het systeem wacht op een key press
    private SynthAction actionToRebind = null; // Houdt bij welke actie opnieuw gebonden wordt

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        InitializeButtons();
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
                button.onClick.AddListener(() => StartRebindingAction("Synth" + Index));
                Index++;
            }
            UpdateKeybindButtonTexts();
        }
    }
    public void UpdateKeybindButtonTexts()
    {
        int Index = 0;
        foreach (var action in synthActions)
        {
            // Vind de bijbehorende knop
            Button button = actionButtons[Index];
            if (button != null)
            {
                // Update de tekst van de knop
                Text buttonText = button.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = action.Key.ToString();
                }
            }
            Index++;
        }
    }

    // Een methode aangeroepen door UI knoppen om het proces van het herbinden van een key te starten
    public void StartRebindingAction(string actionName)
    {
        actionToRebind = synthActions.Find(action => action.SynthName == actionName);
        if (actionToRebind != null)
        {
            waitingForKeyText.gameObject.SetActive(true);
            waitingForKeyText.text = "Press any key for " + actionName;
        }
    }

    void UpdateKeybindUI(KeyCode keyCode)
    {
        Debug.Log("action complete, code: " + keyCode);
        // Implementeer logica om je UI te updaten op basis van de nieuwe keybindings
        // Dit kan betekenen dat je door je synthActions gaat en de UI elementen bijwerkt om de nieuwe keys te tonen
    }
}
