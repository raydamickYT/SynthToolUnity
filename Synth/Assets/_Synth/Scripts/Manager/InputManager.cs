using System.Collections;
using System.Collections.Generic;
using FMOD;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public Button DisableAllSynthsButton;
    private bool InputIsAllowed = false;

    // Start is called before the first frame update
    void Start()
    {
        DisableAllSynthsButton.onClick.AddListener(DisableAllSynthsvoid);
    }

    // Update is called once per frame
    void Update()
    {
        if (!InputIsAllowed) return;

        foreach (var action in KeybindManager.Instance.synthActions)
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

    void DisableAllSynthsvoid()
    {
        foreach (var Synth in GameManager.Instance.synths)
        {
            Synth.synthState.mCaptureDSP.getActive(out bool IsActive);            
            if(IsActive) Synth.synthState.mCaptureDSP.setActive(true); //we moeten er wel voor zorgen dat de synth altijd aanstaat als ze input willen.
            Synth.synthState.mCaptureDSP.setBypass(true);
        }
        InputIsAllowed = true;
    }
}
