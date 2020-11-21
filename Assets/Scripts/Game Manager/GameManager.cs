using Game.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Cur { get; private set; }

    [SerializeField] private bool editorModeOn = false;
    public bool EditorModeOn => editorModeOn;

    public Database Database { get; private set; }
    public EventController EventController { get; private set; }
    public AudioController AudioController { get; private set; }
    public StateController StateController { get; private set; }
    public SceneController SceneController { get; private set; }
    public InputController InputController { get; private set; }

    public SceneDatabase SceneDatabase { get; private set; }
    public CanvasX Canvas { get; private set; }
    public CameraController CamController { get; private set; }
    public Player Player { get; private set; }
    public Enemy Enemy { get; private set; }

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

    [SerializeField] private GameManagerCanvas gameManagerCanvas = default;
    public GameManagerCanvas GameManagerCanvas => gameManagerCanvas;
    [SerializeField] private Camera gameManagerCamera = default;

    private void Awake()
    {
        // Only one instance of the GameManager can exist.
        if (Cur == null)
            Cur = this;
        else
            Destroy(gameObject);

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
        if (InputController == null)
            InputController = GetComponent<InputController>();

        // If open scene count is more than 1, unload all scenes and load the GameManager scene.
        #if UNITY_EDITOR
        if (SceneManager.sceneCount > 1)
        {
            Debug.Log("RESTARTING: Game Manager!");
            SceneManager.LoadScene(SceneController.gameManagerSceneData.BuildIndex, LoadSceneMode.Single);
            return;
        }
        #endif

        // Create save files` directory.
        Storage.CreateGameDirectories();
        // Load the saved data at the start of the game.
        Database.Load(Database.LevelsFile);
        Database.Load(Database.OptionsFile);
    }

    private void Start()
    {
        if (EditorModeOn)
            StartCoroutine(SceneController.LoadLevelScene(1, false)); // Load the Level1 scene at the start of the game.
        else
            StartCoroutine(SceneController.LoadMainMenuScene(false)); // Load the MainMenu scene at the start of the game.
    }

    public void SetSceneDatabase()
    {
        if (GameObject.FindGameObjectWithTag("Scene Database") != null)
        {
            SceneDatabase = GameObject.FindGameObjectWithTag("Scene Database").GetComponent<SceneDatabase>();
            Debug.Log("SCENE DATABASE: found");

            if (SceneDatabase.Canvas != null)
            {
                Canvas = SceneDatabase.Canvas;
            }
            if (SceneDatabase.CameraController != null)
            {
                CamController = SceneDatabase.CameraController;
                Debug.Log("CAMERA CONTROLLER: found");
            }
            if (SceneDatabase.Player != null)
            {
                Player = SceneDatabase.Player;
                Debug.Log("PLAYER: found");
            }
            if (SceneDatabase.Enemy != null)
            {
                Enemy = SceneDatabase.Enemy;
                Debug.Log("ENEMY: found");
            }


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
