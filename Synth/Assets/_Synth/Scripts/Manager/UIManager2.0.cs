// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections.Generic;
// using System.Linq;

// public class UIManager2 : MonoBehaviour
// {
//     public Dropdown synthSelector;
//     public Slider frequencySlider, volumeSlider;
//     public Dropdown waveformDropdown;

//     // private List<Synth> synths = new List<Synth>();
//     public Synth selectedSynth;

//     void Start()
//     {
//         // Initieer UI elementen
//         InitializeSynthSelector();
//         InitializeControls();
//     }

//     void InitializeSynthSelector()
//     {
//         synthSelector.ClearOptions();
//         List<string> synthNames = new List<string>();

//         // Veronderstel dat je een manier hebt om alle synths te verzamelen
//         // synths = FindObjectsOfType<Synth>().ToList();
//         // foreach (var synth in synths)
//         // {
//         //     synthNames.Add(synth.name); // Of een andere identificerende eigenschap
//         // }

//         synthSelector.AddOptions(synthNames);
//         synthSelector.onValueChanged.AddListener(delegate { SelectSynth(synthSelector.value); });
//     }

//     void InitializeControls()
//     {
//         frequencySlider.onValueChanged.AddListener(delegate { UpdateSynthParameter(); });
//         volumeSlider.onValueChanged.AddListener(delegate { UpdateSynthParameter(); });
//         waveformDropdown.onValueChanged.AddListener(delegate { UpdateSynthParameter(); });
//     }

//     public void SelectSynth(int index)
//     {
//         // selectedSynth = synths[index];
//         // UpdateUIWithSynthParameters();
//     }

//     void UpdateSynthParameter()
//     {
//         if (selectedSynth == null) return;

//         // Pas de waarden aan van de geselecteerde synth
//         selectedSynth.SetFrequency(frequencySlider.value);
//         selectedSynth.SetVolume(volumeSlider.value);
//         selectedSynth.SetWaveForm(waveformDropdown);
//     }

//     void UpdateUIWithSynthParameters()
//     {
//         // Update UI elementen om de parameters van de geselecteerde synth weer te geven
//         frequencySlider.value = selectedSynth.GetFrequency();
//         volumeSlider.value = selectedSynth.GetVolume();
//         waveformDropdown.value = (int)selectedSynth.GetWaveForm();
//     }
// }
