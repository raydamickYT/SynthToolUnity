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
        if (InputIsAllowed)
        {
            // Wanneer de A-toets wordt ingedrukt
            if (Input.GetKeyDown(KeyCode.Z) && GameManager.Instance.synths.Count > 0)
            {
                GameManager.Instance.synths[0].synthState.mCaptureDSP.setBypass(false); // Zet bypass uit
            }
            // Wanneer de A-toets wordt losgelaten
            else if (Input.GetKeyUp(KeyCode.Z) && GameManager.Instance.synths.Count > 0)
            {
                GameManager.Instance.synths[0].synthState.mCaptureDSP.setBypass(true); // Zet bypass aan
            }

            // Wanneer de S-toets wordt ingedrukt
            if (Input.GetKeyDown(KeyCode.X) && GameManager.Instance.synths.Count > 1)
            {
                GameManager.Instance.synths[1].synthState.mCaptureDSP.setBypass(false); // Zet bypass uit
            }
            // Wanneer de S-toets wordt losgelaten
            else if (Input.GetKeyUp(KeyCode.X) && GameManager.Instance.synths.Count > 1)
            {
                GameManager.Instance.synths[1].synthState.mCaptureDSP.setBypass(true); // Zet bypass aan
            }

            // Wanneer de D-toets wordt ingedrukt
            if (Input.GetKeyDown(KeyCode.C) && GameManager.Instance.synths.Count > 2)
            {
                GameManager.Instance.synths[2].synthState.mCaptureDSP.setBypass(false); // Zet bypass uit
            }
            // Wanneer de D-toets wordt losgelaten
            else if (Input.GetKeyUp(KeyCode.C) && GameManager.Instance.synths.Count > 2)
            {
                GameManager.Instance.synths[2].synthState.mCaptureDSP.setBypass(true); // Zet bypass aan
            }
        }

    }

    void DisableAllSynthsvoid()
    {
        foreach (var Synth in GameManager.Instance.synths)
        {
            Synth.synthState.mCaptureDSP.setBypass(true);
        }
        InputIsAllowed = true;
    }
}
