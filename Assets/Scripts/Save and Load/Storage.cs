using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

public static class Storage
{
    // Make more properties like this to easily access file names
    public static List<string> Levels => GetFilesOfType(SaveType.LEVELS);
    public static List<string> Options => GetFilesOfType(SaveType.OPTIONS);

    //Points to User/AppData/LocalLow/DefaultCompany/YourGame/...
    public static string dataPath = Application.persistentDataPath;

    // Add other directories here to indicate where you want to save
    public static Dictionary<SaveType, string> gameDirectories = new Dictionary<SaveType, string>
    {
        {SaveType.LEVELS, dataPath + "/levels/"},
        {SaveType.OPTIONS, dataPath + "/options/"},
    };

    // Add your file extensions here - they can be anything!
    public static Dictionary<SaveType, string> gameFileExtensions = new Dictionary<SaveType, string>
    {
        {SaveType.LEVELS, ".levels"},
        {SaveType.OPTIONS, ".options"},
    };

    // Call this at the start of your game if you want unity to make your directories for you to ensure they are there.
    public static void CreateGameDirectories()
    {
        foreach (KeyValuePair<SaveType, string> directory in gameDirectories)
        {
            Directory.CreateDirectory(directory.Value);
        }
    }

    public static void SaveGame(Database callFrom, SaveFile file)
    {
        string fileDirectory = FileName(file.name, file.type);

        try
        {
            Debug.Log("FILE SAVING: " + fileDirectory);

            using (var writer = new BinaryWriter(File.Open(fileDirectory, FileMode.Create)))
            {
                callFrom.SaveData(new DataWriter(writer), file);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Storage.SaveGame(): Failed! (Reason: " + e.Message + ")");
        }
    }
    public static void LoadGame(Database callFrom, SaveFile file)
    {
        string fileDirectory = FileName(file.name, file.type);
        try
        {
            if (File.Exists(fileDirectory))
            {
                Debug.Log("FILE LOADING: " + fileDirectory);

                byte[] data = File.ReadAllBytes(fileDirectory);
                var reader = new BinaryReader(new MemoryStream(data));
                callFrom.LoadData(new DataReader(reader), file);
            }
            else
            {
                Debug.Log("FILE: " + file.name + " file does not exist!");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Storage.LoadGame(): Failed! (Reason: " + e.Message + ")");
            if (File.Exists(fileDirectory))
            {
                File.Delete(fileDirectory);
            }
        }
    }

    //Gets the full directory filename for a given filename
    private static string FileName(string fileName, SaveType fileType)
    {
        return gameDirectories[fileType] + fileName + gameFileExtensions[fileType];
    }

    //Get all files in your save directory of file type
    //great for filling out menus
    public static List<string> GetFilesOfType(SaveType fileType)
    {
        Debug.Log($"Getting files at {gameDirectories[fileType]} of extension type {gameFileExtensions[fileType]}");
        DirectoryInfo dir = new DirectoryInfo(gameDirectories[fileType]);
        FileInfo[] info = dir.GetFiles("*" + gameFileExtensions[fileType]);
        List<string> fileNames = new List<string>();
        foreach (FileInfo f in info)
        {
            fileNames.Add(Path.GetFileNameWithoutExtension(f.FullName));
        }
        return fileNames;
    }
}

[Serializable]
public struct SaveFile
{
    public int version;
    public string name;
    public SaveType type;

    public SaveFile(int version, string name, SaveType type)
    {
        this.version = version;
        this.name = name;
        this.type = type;
    }
}
