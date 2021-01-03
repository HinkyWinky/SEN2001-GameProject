using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    [System.Serializable] public class UnityEventInt : UnityEvent<int> { }
    [System.Serializable] public class UnityEventIntInt : UnityEvent<int, int> { }
    [System.Serializable] public class UnityEventLevelResult : UnityEvent<LevelResults> { }

    public class EventCtrl : MonoBehaviour
    {
        private GameManager Mng => GameManager.Cur;

        [HideInInspector] public UnityEvent onSceneLoadStarted;
        [HideInInspector] public UnityEvent onSceneLoadEnded;
        [HideInInspector] public UnityEvent onMainMenuSceneLoadStarted;
        [HideInInspector] public UnityEventInt onLevelSceneLoadStarted;
        [HideInInspector] public UnityEventIntInt onPlayerHealthChange;
        [HideInInspector] public UnityEventIntInt onEnemyHealthChange;
        [HideInInspector] public UnityEventLevelResult onPlayerDie;
        [HideInInspector] public UnityEventLevelResult onEnemyDie;
        [HideInInspector] public UnityEvent onPausePanelOpened;
        [HideInInspector] public UnityEvent onPausePanelClosed;
        [HideInInspector] public UnityEventLevelResult onLevelEnd;

        private void Start()
        {
            onSceneLoadStarted.AddListener(Mng.OnSceneLoadStarted);
            onSceneLoadStarted.AddListener(OnSceneLoadStarted);
            onSceneLoadStarted.AddListener(Mng.InputCtrl.ResetAllInputs);
            onSceneLoadEnded.AddListener(OnSceneLoadEnded);
            onMainMenuSceneLoadStarted.AddListener(Mng.OnMainMenuSceneLoadStarted);
            onLevelSceneLoadStarted.AddListener(Mng.OnLevelSceneLoadStarted);
        }

        private void OnDestroy()
        {
            onSceneLoadStarted.RemoveAllListeners();
            onSceneLoadEnded.RemoveAllListeners();
            onMainMenuSceneLoadStarted.RemoveAllListeners();
            onLevelSceneLoadStarted.RemoveAllListeners();
        }

        // Called before opening new scene.
        public void OnSceneLoadStarted()
        {
            Debug.Log("EVENT: OnSceneLoadStarted");

            if (Mng.SceneCtrl.CompareSceneType(SceneTypes.LEVEL))
            {
                onPlayerHealthChange.RemoveAllListeners();
                onEnemyHealthChange.RemoveAllListeners();
                onPlayerDie.RemoveAllListeners();
                onEnemyDie.RemoveAllListeners();
                onPausePanelOpened.RemoveAllListeners();
                onPausePanelClosed.RemoveAllListeners();
                onLevelEnd.RemoveAllListeners();
            }
        }

        // Called after opening new scene.
        public void OnSceneLoadEnded()
        {
            Debug.Log("EVENT: OnSceneLoadEnded");

            if (Mng.SceneCtrl.CompareSceneType(SceneTypes.LEVEL))
            {
                onPlayerHealthChange.AddListener(Mng.LevelCanvas.inGamePanel.playerHealthBar.SetValue);
                onEnemyHealthChange.AddListener(Mng.LevelCanvas.inGamePanel.enemyHealthBar.SetValue);
                onPlayerDie.AddListener(Mng.OnLevelEnd);
                onEnemyDie.AddListener(Mng.OnLevelEnd);
                onPausePanelOpened.AddListener(Mng.OnPausePanelOpened);
                onPausePanelClosed.AddListener(Mng.OnPausePanelClosed);
                onLevelEnd.AddListener(Mng.LevelCanvas.OpenEndLevelPanel);
            }
        }
    }
}
