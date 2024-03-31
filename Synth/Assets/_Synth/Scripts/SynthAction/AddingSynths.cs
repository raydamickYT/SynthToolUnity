using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class AddingSynths : MonoBehaviour
{
    public static AddingSynths Instance;
    public GameObject SynthPrefab, SynthParent;
    public List<GameObject> SynthPrefabList = new();
    public Button AddSynth, RemoveSynth;
    private Vector3 SynthInitOffSet = new Vector3(12, -4.5f, 0), SynthSpawnOffSet = new Vector3(0, -2, 0);
    public RectTransform scrollView;
    void Awake()
    {
        if (AddSynth != null)
        {
            AddSynth.onClick.AddListener(OnAddSynth);
        }
        if (RemoveSynth != null)
        {
            RemoveSynth.onClick.AddListener(OnRemoveSynth);
        }
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        if (scrollView == null)
        {
            scrollView = SynthParent.GetComponent<RectTransform>();
        }

        OnAddSynth(); //voeg de eerste synth toe om alvast iets te hebbn.
    }

    public void OnAddSynth()
    {
        if (SynthPrefabList.Count < 10)
        {
            //voeg hier een synth toe aan de array
            GameObject tempPrefab = Instantiate(SynthPrefab);
            tempPrefab.transform.SetParent(SynthParent.transform);
            tempPrefab.transform.localScale = SynthPrefab.transform.localScale; //dit is nodig omdat anders de synth gigantisch wordt
            Vector3 newPosition = SynthParent.transform.position + SynthInitOffSet + new Vector3(0, SynthSpawnOffSet.y * SynthPrefabList.Count, 0);

            tempPrefab.transform.position = newPosition;
            SynthPrefabList.Add(tempPrefab);
            UpdateScrollViewHeight();
        }
    }
    public Synth AddExtraSynths()
    {
        if (SynthPrefabList.Count < 10)
        {
            //voeg hier een synth toe aan de array
            GameObject tempPrefab = Instantiate(SynthPrefab);
            tempPrefab.transform.SetParent(SynthParent.transform);
            tempPrefab.transform.localScale = SynthPrefab.transform.localScale; //dit is nodig omdat anders de synth gigantisch wordt
            Vector3 newPosition = SynthParent.transform.position + SynthInitOffSet + new Vector3(0, SynthSpawnOffSet.y * SynthPrefabList.Count, 0);

            tempPrefab.transform.position = newPosition;
            SynthPrefabList.Add(tempPrefab);
            UpdateScrollViewHeight();
            var test = SynthPrefab.GetComponentInChildren<Synth>();
            return test;
        }
        else
        {
            return null;
        }
    }

    public void OnRemoveSynth()
    {
        if (SynthPrefabList.Count > 0)
        {
            //verwijder hier een synth van je lijst.
            var lastElement = SynthPrefabList[SynthPrefabList.Count - 1];

            SynthPrefabList.RemoveAt(SynthPrefabList.Count - 1);

            if (lastElement != null)
            {
                GameObject.Destroy(lastElement);
            }

        }
    }
    public void UpdateScrollViewHeight()
    {
        // Stel dat elk item 100 units hoog is en er is een tussenruimte (margin) van 10 units tussen elk item
        float itemHeight = 100f;
        float margin = 10f;

        // Bereken de totale hoogte die nodig is voor alle items plus marges
        float totalHeight = SynthPrefabList.Count * (itemHeight + margin);

        scrollView.sizeDelta = new Vector2(scrollView.sizeDelta.x, totalHeight);
    }

}
