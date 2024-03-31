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


    private bool InputIsAllowed = false;

    // Start is called before the first frame update
    void Start()
    {
        DisableAllSynthsButton.onClick.AddListener(ToggleAllSynths);
        defaultColor = DisableAllSynthsButton.colors.normalColor;
    }

    // Update is called once per frame
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
            Synth.synthState.mCaptureDSP.getActive(out bool IsActive);
            UnityEngine.Debug.Log(IsActive);
            if (!IsActive) Synth.synthState.mCaptureDSP.setActive(InputIsAllowed); //we moeten er wel voor zorgen dat de synth altijd aanstaat als ze input willen.
            Synth.synthState.mCaptureDSP.setBypass(InputIsAllowed);
        }
        UnityEngine.Debug.Log("All Synths bypassed");
        ToggleButtonColor();
    }

    void EnableALlSynthsVoid()
    {
        foreach (var Synth in GameManager.Instance.synths)
        {
            Synth.synthState.mCaptureDSP.getActive(out bool IsActive);
            if (IsActive) Synth.synthState.mCaptureDSP.setActive(false); //we moeten er wel voor zorgen dat de synth altijd aanstaat als ze input willen.
            Synth.synthState.mCaptureDSP.setBypass(false);
        }
        UnityEngine.Debug.Log("All Synths zijn weer enabled");
        InputIsAllowed = false;
        ToggleButtonColor();
    }
    public void ToggleButtonColor()
    {
        DisableAllSynthsButton.GetComponent<Image>().color = InputIsAllowed ? toggledColor : defaultColor; // Pas de kleur aan
    }
}
