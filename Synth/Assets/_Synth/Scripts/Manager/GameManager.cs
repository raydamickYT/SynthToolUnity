using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private int Index = 0;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    public List<Synth> synths;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Zorg ervoor dat er slechts één instantie is
        }
    }

    public void AddSynthToList(Synth synth)
    {
        synths.Add(synth);
        synth.name = "Synth: " + Index.ToString();
        Index++;
    }
    public void RemoveSynthFromList(Synth synth)
    {
        synths.Remove(synth);
        Index--;
    }
}
