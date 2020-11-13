using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Cur { get; private set; }
    public Database Database { get; private set; }
    public EventController EventController { get; private set; }
    public CanvasController CanvasController { get; private set; }
    public AudioController AudioController { get; private set; }
    public StateController StateController { get; private set; }
    public SceneController SceneController { get; private set; }
    public SceneDatabase SceneDatabase { get; private set; }

    private void Awake()
    {
        // Only one instance of the GameManager can exist.
        if (Cur == null)
        {
            Cur = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Get all components of this game object.
        if (Database == null)
            Database = GetComponent<Database>();
        if (EventController == null)
            EventController = GetComponent<EventController>();
        if (CanvasController == null)
            CanvasController = GetComponent<CanvasController>();
        if (AudioController == null)
            AudioController = GetComponent<AudioController>();
        if (StateController == null)
            StateController = GetComponent<StateController>();
        if (SceneController == null)
            SceneController = GetComponent<SceneController>();

        // Creating save file`s directory.
        Storage.CreateGameDirectories();

        // Loading saved data on the start of the game.
        Database.Load(Database.LevelsFile);
        Database.Load(Database.OptionsFile);

    }

    private void Start()
    {
        StartCoroutine(SceneController.LoadSceneAdditive(SceneController.MainMenuSceneName, true, true));
    }

    public void SetSceneDatabase()
    {
        if (GameObject.FindGameObjectWithTag("Scene Database") != null)
        {
            SceneDatabase = GameObject.FindGameObjectWithTag("Scene Database").GetComponent<SceneDatabase>();
            Debug.Log("SCENE DATABASE: Found.");
        }
        else
        {
            Debug.Log("SCENE DATABASE: Not found.");
        }
    }
}
