using System;
using System.Collections;
using System.Collections.Generic;
using Game.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Cur { get; private set; }
    public Database Database { get; private set; }
    public EventController EventController { get; private set; }
    public AudioController AudioController { get; private set; }
    public StateController StateController { get; private set; }
    public SceneController SceneController { get; private set; }

    public SceneDatabase SceneDatabase { get; private set; }
    public CanvasX Canvas { get; private set; }

    public MainMenuCanvas MainMenuCanvas
    {
        get
        {
            if (Canvas as MainMenuCanvas != null)
                return Canvas as MainMenuCanvas;

            return null;
        }
    }
    public LevelCanvas LevelCanvas
    {
        get
        {
            if (Canvas as LevelCanvas != null)
                return Canvas as LevelCanvas;

            return null;
        }
    }

    public GameManagerCanvas gameManagerCanvas;
    public Camera gameManagerCamera;

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
        StartCoroutine(SceneController.LoadMainMenuScene(false));
    }

    public void SetSceneDatabase()
    {
        if (GameObject.FindGameObjectWithTag("Scene Database") != null)
        {
            SceneDatabase = GameObject.FindGameObjectWithTag("Scene Database").GetComponent<SceneDatabase>();
            Debug.Log("SCENE DATABASE: found");

            if (SceneDatabase.Canvas != null)
                Canvas = SceneDatabase.Canvas;

            if (MainMenuCanvas != null)
                Debug.Log("MAIN MENU CANVAS: found");

            if (LevelCanvas != null)
                Debug.Log("LEVEL CANVAS: found");
        }
        else
        {
            Debug.Log("SCENE DATABASE: Not found");
        }
    }

    public void OnSceneLoadStarted()
    {
        gameManagerCanvas.Activate(true);
        gameManagerCamera.gameObject.SetActive(true);
    }

    public void OnSceneLoaded()
    {
        gameManagerCanvas.Activate(false);
        gameManagerCamera.gameObject.SetActive(false);
    }
}
