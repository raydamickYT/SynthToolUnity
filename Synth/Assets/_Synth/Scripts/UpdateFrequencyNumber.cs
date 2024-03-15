using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateFrequencyNumber : MonoBehaviour
{
    public Text text;

    private void Start() {
        text = GetComponentInChildren<Text>();
    }

    private void LateUpdate() {
        text.text = SynthInfo.instance.Frequency.ToString();
    }
}
