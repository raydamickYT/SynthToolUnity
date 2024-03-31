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
[System.Serializable]
public class KeyData
{
    public KeyData()
    {
        foreach (SynthAction action in synthActions)
        {
            synthActionsBackup.Add(new SynthAction(action.Key, action.SynthIndex, action.SynthName));
        }
    }
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

    public List<SynthAction> synthActionsBackup = new List<SynthAction>(); //deze is er voor de reset
}

