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

    private void Start()
    {
        onSceneLoadStarted.AddListener(Mng.OnSceneLoadStarted);
        onSceneLoadStarted.AddListener(OnSceneLoadStarted);

        onSceneLoadEnded.AddListener(OnSceneLoadEnded);
    }

    private void OnDestroy()
    {
        onSceneLoadStarted.RemoveAllListeners();
        onSceneLoadEnded.RemoveAllListeners();
    }

    public void OnSceneLoadStarted()
    {
        Debug.Log("EVENT: OnSceneLoadStarted");

        if (Mng.SceneCtrl.CompareSceneType(SceneType.LEVEL))
        {
            onPlayerHealthChange.RemoveAllListeners();
            onEnemyHealthChange.RemoveAllListeners();
        }
    }

    public void OnSceneLoadEnded()
    {
        Debug.Log("EVENT: OnSceneLoadEnded");

        if (Mng.SceneCtrl.CompareSceneType(SceneType.LEVEL))
        {
            onPlayerHealthChange.AddListener(Mng.SceneCanvas.inGamePanel.playerHealthBar.SetValue);
            onEnemyHealthChange.AddListener(Mng.SceneCanvas.inGamePanel.enemyHealthBar.SetValue);
        }
    }
}
