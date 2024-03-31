using System.Collections;
using System.Collections.Generic;
using FMOD;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public Button DisableAllSynthsButton;
    public Color defaultColor; // Standaardkleur
    public Color toggledColor = Color.green; // Kleur wanneer getoggled
    public bool ToggleBool = false;

    private bool InputIsAllowed = false;

    void Start()
    {
        DisableAllSynthsButton.onClick.AddListener(ToggleAllSynths);
        defaultColor = DisableAllSynthsButton.colors.normalColor;
    }

    void Update()
    {
        if (!InputIsAllowed) return;

        foreach (var action in KeybindManager.Instance.keyData.synthActions)
        {
            if (Input.GetKeyDown(action.Key))
            {
                action.PerformAction(true); // Bypass uitzetten
            }
            else if (Input.GetKeyUp(action.Key))
            {
                action.PerformAction(false); // Bypass aanzetten
            }
        }
    }
    public void ToggleAllSynths()
    {
        InputIsAllowed = !InputIsAllowed;
        foreach (var Synth in GameManager.Instance.synths)
        {
            // Synth.synthState.mCaptureDSP.getActive(out bool IsActive); //dit was om te checken of de synth actief is, maar het werkte niet
            
            if (InputIsAllowed) Synth.synthState.mCaptureDSP.setActive(InputIsAllowed);
            else if(!InputIsAllowed) Synth.synthState.mCaptureDSP.setActive(InputIsAllowed);
             //we moeten er wel voor zorgen dat de synth altijd aanstaat als ze input willen.
            
            Synth.synthState.mCaptureDSP.setBypass(InputIsAllowed);
        }
        UnityEngine.Debug.Log("All Synths bypassed");
        ToggleButtonColor();
    }
    public void ToggleButtonColor()
    {
        DisableAllSynthsButton.GetComponent<Image>().color = InputIsAllowed ? toggledColor : defaultColor; // Pas de kleur aan
    }
}
