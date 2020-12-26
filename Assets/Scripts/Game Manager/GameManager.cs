using Game.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Cur { get; private set; }

    [ShowInInspector, ReadOnly] private bool isFrameRateRunning;

    [SerializeField] private bool editorModeOn = false;
    [SerializeField] private GameManagerCanvas gameManagerCanvas = default;
    [SerializeField] private Camera gameManagerCamera = default;
   
    public Database Database { get; private set; }
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

    public GameManagerCanvas GameManagerCanvas => gameManagerCanvas;
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
    
    public bool EditorModeOn => editorModeOn;
    public bool IsFrameRateRunning => isFrameRateRunning;

    #region Mono
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
        if (SceneManager.sceneCount > 1)
        {
            Debug.Log("RESTARTING: Game Manager!");
            SceneManager.LoadScene(SceneCtrl.gameManagerSceneData.BuildIndex, LoadSceneMode.Single);
            return;
        }
        #endif

        PauseFrameRate();
        StateCtrl.ChangeGameState(GameState.LOADING);

        // Create levels dictionary to write saved file data in it. 
        Database.CreateLevelsDictionary();

        // Create save files` directory.
        Storage.CreateGameDirectories();
        // Load the saved data at the start of the game.
        Database.Load(Database.LevelsFile);
        Database.Load(Database.OptionsFile);
    }
    private void Start()
    {
        if (EditorModeOn)
            StartCoroutine(SceneCtrl.LoadLevelScene(1, false)); // Load the Level1 scene at the start of the game.
        else
            StartCoroutine(SceneCtrl.LoadMainMenuScene(false)); // Load the MainMenu scene at the start of the game.

        SettingCtrl.SetFrameRate();
    }
    #endregion

    public void SetSubscene(SceneTypes curSceneType, Subscene subscene)
    {
        Subscene = subscene;
        if (Subscene != null)
            Debug.Log("SUBSCENE: found");
        else
            Debug.LogWarning("SUBSCENE: NULL!!!!!!!");

        SceneCtrl.curSceneType = curSceneType;

        gameManagerCanvas.Activate(false);
        gameManagerCamera.gameObject.SetActive(false);

        switch (SceneCtrl.CurSceneType)
        {
            case SceneTypes.MAINMENU:
                GameManager.Cur.StateCtrl.ChangeGameState(GameState.MAINMENU);
                if (Subscene.Canvas != null)
                    Canvas = Subscene.Canvas;
                PauseFrameRate();
                break;
            case SceneTypes.LEVEL:
                GameManager.Cur.StateCtrl.ChangeGameState(GameState.PLAYLEVEL);
                if (Subscene.Canvas != null)
                    Canvas = Subscene.Canvas;
                if (Subscene.CamCtrl != null)
                    CamCtrl = Subscene.CamCtrl;
                if (Subscene.Player != null)
                    Player = Subscene.Player;
                if (Subscene.Enemy != null)
                    Enemy = Subscene.Enemy;
                UnpauseFrameRate();
                break;
        }

        if (MainMenuCanvas != null)
            Debug.Log("MAIN MENU CANVAS: found");
        if (LevelCanvas != null)
            Debug.Log("LEVEL CANVAS: found");

        GameManager.Cur.EventCtrl.onSceneLoadEnded?.Invoke();
    }

    public void PauseFrameRate()
    {
        isFrameRateRunning = false;
        Time.timeScale = 0f;
    }
    public void UnpauseFrameRate()
    {
        isFrameRateRunning = true;
        Time.timeScale = 1f;
    }

    public void OnSceneLoadStarted()
    {
        StateCtrl.ChangeGameState(GameState.LOADING);

        gameManagerCanvas.Activate(true);
        gameManagerCamera.gameObject.SetActive(true);
    }

    public void OnMainMenuSceneLoadStarted()
    {
        Database.Load(Database.LevelsFile);
    }

    public void OnLevelSceneLoadStarted(int levelNo)
    {
        Database.Load(Database.LevelsFile);
        if (levelNo > 1)
        {
            if (!Database.levelsCompletionStatue[levelNo - 1])
            {
                Debug.LogError("Level No " + levelNo + " is not unlocked!");
                Application.Quit();
            }
        }
    }

    public void OnPausePanelOpened()
    {
        StateCtrl.ChangeGameState(GameState.PAUSEMENU);
        PauseFrameRate();
    }
    public void OnPausePanelClosed()
    {
        StateCtrl.ChangeGameState(GameState.PLAYLEVEL);
        UnpauseFrameRate();
    }

    public void OnEndLevelPanelOpened(LevelResults result)
    {
        StateCtrl.ChangeGameState(GameState.ENDMENU);
        if (result == LevelResults.VICTORY)
            Database.levelsCompletionStatue[SceneCtrl.CurrentLevelNo] = true; // complete current level, unlock next level.
        Database.Save(Database.LevelsFile);
    }
}
