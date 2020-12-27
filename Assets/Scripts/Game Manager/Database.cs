using System.Collections.Generic;
using Game.IO;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class Database : MonoBehaviour
    {
        // Save Files Data
        [BoxGroup("Save Files` Data"), SerializeField]
        private SaveFile levelsFile = new SaveFile(1, "levels", SaveType.LEVELS);

        public SaveFile LevelsFile => levelsFile;

        [BoxGroup("Save Files` Data"), SerializeField]
        private SaveFile optionsFile = new SaveFile(1, "options", SaveType.OPTIONS);

        public SaveFile OptionsFile => optionsFile;

        public IDictionary<int, bool> levelsCompletionStatue;
        [ReadOnly] public int lastCompletedLevel = 0;
        [ReadOnly] public bool hasSavedLevelFile = false;
        public int LastCompletedLevel => lastCompletedLevel;
        public int LastLevel => levelsCompletionStatue.Count;
        public bool HasSavedLevelFile => hasSavedLevelFile;

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
        /// This method is called by Storage class after user calls Save() method in Database class.
        /// Put all save logic into this method.
        /// </summary>
        public void SaveData(DataWriter writer, SaveFile file)
        {
            if (file.name == levelsFile.name)
            {
                writer.Write(file.version);
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
                Debug.Log("FILE: " + "Database has no name of the " + file.name + " file!");
            }
        }

        /// <summary>
        /// This method is called by Storage class after user calls Load() method in Database class.
        /// Put all load logic into this method.
        /// </summary>
        public void LoadData(DataReader reader, SaveFile file)
        {
            if (file.name == levelsFile.name)
            {
                hasSavedLevelFile = true;
                file.version = reader.ReadInt();
                if (file.version == 1)
                {
                    for (int i = 0; i < levelsCompletionStatue.Count; i++)
                    {
                        levelsCompletionStatue[i + 1] = reader.ReadBool();
                        if (levelsCompletionStatue[i + 1])
                            lastCompletedLevel = i + 1;
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
}
