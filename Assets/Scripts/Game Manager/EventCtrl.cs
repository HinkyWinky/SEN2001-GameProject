using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityEventIntInt : UnityEvent<int, int> { }

public class EventCtrl : MonoBehaviour
{
    private GameManager Mng => GameManager.Cur;

    [HideInInspector] public UnityEvent onSceneLoadStarted;
    [HideInInspector] public UnityEvent onSceneLoadEnded;
    [HideInInspector] public UnityEventIntInt onPlayerHealthChange;
    [HideInInspector] public UnityEventIntInt onEnemyHealthChange;
    [HideInInspector] public UnityEvent onPlayerDie;
    [HideInInspector] public UnityEvent onEnemyDie;
    [HideInInspector] public UnityEvent onPausePanelOpened;
    [HideInInspector] public UnityEvent onPausePanelClosed;
    [HideInInspector] public UnityEvent onEndLevelPanelOpened;

    private void Start()
    {
        onSceneLoadStarted.AddListener(Mng.OnSceneLoadStarted);
        onSceneLoadStarted.AddListener(OnSceneLoadStarted);
        onSceneLoadStarted.AddListener(Mng.InputCtrl.ResetAllInputs);
        onSceneLoadEnded.AddListener(OnSceneLoadEnded);
    }

    private void OnDestroy()
    {
        onSceneLoadStarted.RemoveAllListeners();
        onSceneLoadEnded.RemoveAllListeners();
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
            onEndLevelPanelOpened.RemoveAllListeners();
        }
    }

    // Called after opening new scene.
    public void OnSceneLoadEnded()
    {
        Debug.Log("EVENT: OnSceneLoadEnded");

        if (Mng.SceneCtrl.CompareSceneType(SceneTypes.LEVEL))
        {
            onPlayerHealthChange.AddListener(Mng.SceneCanvas.inGamePanel.playerHealthBar.SetValue);
            onEnemyHealthChange.AddListener(Mng.SceneCanvas.inGamePanel.enemyHealthBar.SetValue);
            onPlayerDie.AddListener(Mng.SceneCanvas.OpenEndLevelPanelOnPlayerDie);
            onEnemyDie.AddListener(Mng.SceneCanvas.OpenEndLevelPanelOnEnemyDie);
            onPausePanelOpened.AddListener(Mng.OnPausePanelOpened);
            onPausePanelClosed.AddListener(Mng.OnPausePanelClosed);
            onEndLevelPanelOpened.AddListener(Mng.OnEndLevelPanelOpened);
        }
    }
}
