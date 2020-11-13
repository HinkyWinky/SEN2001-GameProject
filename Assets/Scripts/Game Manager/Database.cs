using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    // Save Files Data
    [Header("Save Files` Data")]
    [SerializeField] private SaveFile levelsFile = new SaveFile(1, "levels", SaveType.LEVELS);
    public SaveFile LevelsFile => levelsFile;
    [SerializeField] private SaveFile optionsFile = new SaveFile(1, "options", SaveType.OPTIONS);
    public SaveFile OptionsFile => optionsFile;

    #region Save Load
    // This method is called by Storage class after user calls Save() method in Database class.
    // Put all save logic into this method.
    public void SaveData(DataWriter writer, SaveFile file)
    {
        if (file.name == levelsFile.name)
        {
            writer.Write(file.version);
            // TODO Save levels
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
            Debug.Log("FILE: " + "Database has no name of the " + file.name + " file!");
        }
    }
    // This method is called by Storage class after user calls Load() method in Database class.
    // Put all load logic into this method.
    public void LoadData(DataReader reader, SaveFile file)
    {
        if (file.name == levelsFile.name)
        {
            file.version = reader.ReadInt();
            if (file.version == 1)
            {
                // TODO Load levels 
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

    // Call this method to save the file. 
    public void Save(SaveFile file)
    {
        Storage.SaveGame(this, file);
    }
    // Call this method to load the file. 
    public void Load(SaveFile file)
    {
        Storage.LoadGame(this, file);
    }
    #endregion
}
