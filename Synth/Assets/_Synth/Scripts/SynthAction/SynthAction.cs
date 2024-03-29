using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SynthAction
{
    public KeyCode Key;
    public int SynthIndex;

    public SynthAction(KeyCode key, int synthIndex)
    {
        Key = key;
        SynthIndex = synthIndex;
    }

    public void PerformAction(bool isKeyDown)
    {
        if (GameManager.Instance.synths.Count > SynthIndex)
        {
            var synth = GameManager.Instance.synths[SynthIndex];
            synth.synthState.mCaptureDSP.setBypass(!isKeyDown);
        }
    }
}

