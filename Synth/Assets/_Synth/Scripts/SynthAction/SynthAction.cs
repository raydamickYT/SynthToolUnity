using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SynthAction
{
    public KeyCode Key;
    public int SynthIndex;
    public String SynthName;

    public SynthAction(KeyCode key, int synthIndex, string name)
    {
        Key = key;
        SynthIndex = synthIndex;
        SynthName = name;
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

