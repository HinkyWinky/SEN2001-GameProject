using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDatabase : MonoBehaviour
{
    // Save Files Data
    [Header("Save Files` Data")]
    [SerializeField] private SaveFile levelsFile = new SaveFile(1, "levels", SaveType.LEVELS);
    public SaveFile LevelsFile => levelsFile;
    [SerializeField] private SaveFile optionsFile = new SaveFile(1, "options", SaveType.OPTIONS);
    public SaveFile OptionsFile => optionsFile;

    public IDictionary<int, bool> levelsCompletionStatue;

    public void CreateLevelsDictionary()
    {
        levelsCompletionStatue = new Dictionary<int, bool>()
        {
            {1, false},
            {2, false},
            {3, false}
        };
    }

    #region Save Load
    /// <summary>
    /// This method is called by Storage class after user calls Save() method in GameDatabase class.
    /// Put all save logic into this method.
    /// </summary>
    public void SaveData(DataWriter writer, SaveFile file)
    {
        if (file.name == levelsFile.name)
        {
            writer.Write(file.version);
            // TODO Save levels
            for (int i = 0; i < levelsCompletionStatue.Count; i++)
            {
                writer.Write(levelsCompletionStatue[i + 1]);
            }
            Debug.Log("FILE: " + file.name + " file saved.");

        }
        else if (file.name == optionsFile.name)
        {
            writer.Write(file.version);
            // TODO Save options 
            Debug.Log("FILE: " + file.name + " file saved.");
        }
        else
        {
            Debug.Log("FILE: " + "GameDatabase has no name of the " + file.name + " file!");
        }
    }
    /// <summary>
    /// This method is called by Storage class after user calls Load() method in GameDatabase class.
    /// Put all load logic into this method.
    /// </summary>
    public void LoadData(DataReader reader, SaveFile file)
    {
        if (file.name == levelsFile.name)
        {
            file.version = reader.ReadInt();
            if (file.version == 1)
            {
                // TODO Load levels
                for (int i = 0; i < levelsCompletionStatue.Count; i++)
                {
                    levelsCompletionStatue[i + 1] = reader.ReadBool();
                }
            }
            Debug.Log("FILE: " + file.name + " file loaded.");
        }
        else if (file.name == optionsFile.name)
        {
            file.version = reader.ReadInt();
            if (file.version == 1)
            {
                // TODO Load options
            }
            Debug.Log("FILE: " + file.name + " file loaded.");
        }
        else
        {
            Debug.Log("FILE: " + file.name + " file does not exist!");
        }
    }

    /// <summary>
    /// Call this method to save the file. 
    /// </summary>
    public void Save(SaveFile file)
    {
        Storage.SaveGame(this, file);
    }
    /// <summary>
    /// Call this method to load the file. 
    /// </summary>
    public void Load(SaveFile file)
    {
        Storage.LoadGame(this, file);
    }
    #endregion
}
