using System;
using UnityEngine;
using UnityEngine.Events;

public class EventController : MonoBehaviour
{
    private GameManager Mng => GameManager.Cur;

    public UnityEvent onSceneLoadStarted;
    public UnityEvent onSceneLoaded;

    private void Start()
    {
        onSceneLoadStarted.AddListener(OnSceneLoadStarted);
        onSceneLoaded.AddListener(OnSceneLoaded);
        onSceneLoadStarted.AddListener(Mng.OnSceneLoadStarted);
        onSceneLoaded.AddListener(Mng.OnSceneLoaded);
    }
    private void OnDestroy()
    {
        onSceneLoadStarted.RemoveAllListeners();
        onSceneLoaded.RemoveAllListeners();
    }

    public void OnSceneLoadStarted()
    {
        Debug.Log("EVENT: OnSceneLoadStarted");
    }

    public void OnSceneLoaded()
    {
        Debug.Log("EVENT: OnSceneLoaded");
    }
}
