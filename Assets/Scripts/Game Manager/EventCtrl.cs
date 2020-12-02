using UnityEngine;
using UnityEngine.Events;

public class EventCtrl : MonoBehaviour
{
    private GameManager Mng => GameManager.Cur;

    [HideInInspector] public UnityEvent onSceneLoadStarted;
    [HideInInspector] public UnityEvent onSceneLoadEnded;

    private void Start()
    {
        onSceneLoadStarted.AddListener(OnSceneLoadStarted);
        onSceneLoadEnded.AddListener(OnSceneLoadEnded);
        onSceneLoadStarted.AddListener(Mng.OnSceneLoadStarted);
        onSceneLoadEnded.AddListener(Mng.OnSceneLoadEnded);
    }
    private void OnDestroy()
    {
        onSceneLoadStarted.RemoveAllListeners();
        onSceneLoadEnded.RemoveAllListeners();
    }

    public void OnSceneLoadStarted()
    {
        Debug.Log("EVENT: OnSceneLoadStarted");
    }

    public void OnSceneLoadEnded()
    {
        Debug.Log("EVENT: OnSceneLoadEnded");
    }
}
