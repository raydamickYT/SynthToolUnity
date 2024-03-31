using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SavePlayerPrefs
{
    private KeybindManager keybindManager;

    public SavePlayerPrefs(KeybindManager _keybindManager)
    {
        keybindManager = _keybindManager;
    }

    public void SaveKeybindsToFile()
    {
        string path = Path.Combine(Application.persistentDataPath, "keybinds.json");
        string json = JsonUtility.ToJson(keybindManager, true);
        File.WriteAllText(path, json);
    }

    public void LoadKeybindsFromFile()
    {
        string path = Path.Combine(Application.persistentDataPath, "keybinds.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            // Zorgt ervoor dat de gedeserialiseerde data correct wordt toegepast op keybindManager
            JsonUtility.FromJsonOverwrite(json, keybindManager);
            keybindManager.UpdateKeybindButtonTexts(); // Update UI
        }
        else
        { // Als het bestand nog niet bestaat, sla dan de huidige staat op
            SaveKeybindsToFile();
        }
    }
}

