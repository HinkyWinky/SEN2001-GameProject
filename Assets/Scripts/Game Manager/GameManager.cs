using Game.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Cur { get; private set; }

    [SerializeField] private bool editorModeOn = false;
    public bool EditorModeOn => editorModeOn;

    public GameDatabase GameDatabase { get; private set; }
    public EventCtrl EventCtrl { get; private set; }
    public AudioCtrl AudioCtrl { get; private set; }
    public StateCtrl StateCtrl { get; private set; }
    public SceneCtrl SceneCtrl { get; private set; }
    public InputCtrl InputCtrl { get; private set; }
    public SettingCtrl SettingCtrl { get; private set; }

    public Subscene Subscene { get; private set; }
    public CanvasX Canvas { get; private set; }
    public CamCtrl CamCtrl { get; private set; }
    public Player Player { get; private set; }
    public Enemy1 Enemy { get; private set; }

    public MainMenuCanvas MainMenuCanvas
    {
        get
        {
            if (Canvas as MainMenuCanvas != null)
                return Canvas as MainMenuCanvas;

            return null;
        }
    }
    public SceneCanvas SceneCanvas
    {
        get
        {
            if (Canvas as SceneCanvas != null)
                return Canvas as SceneCanvas;

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
        if (GameDatabase == null)
            GameDatabase = GetComponent<GameDatabase>();
        if (EventCtrl == null)
            EventCtrl = GetComponent<EventCtrl>();
        if (AudioCtrl == null)
            AudioCtrl = GetComponent<AudioCtrl>();
        if (StateCtrl == null)
            StateCtrl = GetComponent<StateCtrl>();
        if (SceneCtrl == null)
            SceneCtrl = GetComponent<SceneCtrl>();
        if (InputCtrl == null)
            InputCtrl = GetComponent<InputCtrl>();
        if (SettingCtrl == null)
            SettingCtrl = GetComponent<SettingCtrl>();

        // If open scene count is more than 1, unload all scenes and load the GameManager scene.
        #if UNITY_EDITOR
        if (UnityEngine.SceneManagement.SceneManager.sceneCount > 1)
        {
            Debug.Log("RESTARTING: Game Manager!");
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneCtrl.gameManagerSceneData.BuildIndex, LoadSceneMode.Single);
            return;
        }
        #endif

        GameManager.Cur.StateCtrl.ChangeGameState(GameState.LOADING);

        // Create save files` directory.
        Storage.CreateGameDirectories();
        // Load the saved data at the start of the game.
        GameDatabase.Load(GameDatabase.LevelsFile);
        GameDatabase.Load(GameDatabase.OptionsFile);
    }

    private void Start()
    {
        if (EditorModeOn)
            StartCoroutine(SceneCtrl.LoadLevelScene(1, false)); // Load the Level1 scene at the start of the game.
        else
            StartCoroutine(SceneCtrl.LoadMainMenuScene(false)); // Load the MainMenu scene at the start of the game.

        SettingCtrl.SetFrameRate();
    }

    public void OnSceneLoadStarted()
    {
        GameManager.Cur.StateCtrl.ChangeGameState(GameState.LOADING);

        gameManagerCanvas.Activate(true);
        gameManagerCamera.gameObject.SetActive(true);
    }
    public void OnSceneLoadEnded()
    {
        gameManagerCanvas.Activate(false);
        gameManagerCamera.gameObject.SetActive(false);

        switch (SceneCtrl.CurSceneType)
        {
            case SceneType.MAINMENU:
                GameManager.Cur.StateCtrl.ChangeGameState(GameState.MAINMENU);
                break;
            case SceneType.LEVEL:
                GameManager.Cur.StateCtrl.ChangeGameState(GameState.PLAY);
                break;
        }
    }

    public void SetSubscene()
    {
        if (GameObject.FindGameObjectWithTag("Subscene") != null)
        {
            Subscene = GameObject.FindGameObjectWithTag("Subscene").GetComponent<Subscene>();
            Debug.Log("SUBSCENE: found");

            if (Subscene.Canvas != null)
            {
                Canvas = Subscene.Canvas;
            }
            if (Subscene.CamCtrl != null)
            {
                CamCtrl = Subscene.CamCtrl;
                Debug.Log("CAM CTRL: found");
            }
            if (Subscene.Player != null)
            {
                Player = Subscene.Player;
                Debug.Log("PLAYER: found");
            }
            if (Subscene.Enemy != null)
            {
                Enemy = Subscene.Enemy;
                Debug.Log("ENEMY: found");
            }

            if (MainMenuCanvas != null)
                Debug.Log("MAIN MENU CANVAS: found");
            if (SceneCanvas != null)
                Debug.Log("LEVEL CANVAS: found");

            GameManager.Cur.EventCtrl.onSceneLoadEnded?.Invoke();
        }
        else
        {
            Debug.LogWarning("SUBSCENE: Not found");
        }
    }
}
