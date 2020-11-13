using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    public string MainMenuSceneName => mainMenuSceneName;

    public IEnumerator LoadScene(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
        while (!operation.isDone)
        {
            yield return null;
        }
        Debug.Log("SCENE LOADED: " + SceneManager.GetSceneByBuildIndex(sceneIndex).name);
    }

    public IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!operation.isDone)
        {
            yield return null;
        }
        Debug.Log("SCENE LOADED: " + sceneName);
    }

    public IEnumerator LoadSceneAdditive(int sceneIndex, bool setSceneActive, bool setSceneDatabase)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
        while (!operation.isDone)
        {
            yield return null;
        }
        Debug.Log("SCENE LOADED: " + SceneManager.GetSceneByBuildIndex(sceneIndex).name);

        if (setSceneActive)
            SetActiveScene(sceneIndex);

        if (setSceneDatabase)
            GameManager.Cur.SetSceneDatabase();
    }

    public IEnumerator LoadSceneAdditive(string sceneName, bool setSceneActive, bool setSceneDatabase)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!operation.isDone)
        {
            yield return null;
        }
        Debug.Log("SCENE LOADED: " + sceneName);

        if (setSceneActive)
            SetActiveScene(sceneName);

        if (setSceneDatabase)
            GameManager.Cur.SetSceneDatabase();
    }

    public void SetActiveScene(int sceneIndex)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneIndex));
        Debug.Log("ACTIVE SCENE SET: " + SceneManager.GetSceneByBuildIndex(sceneIndex).name);
    }

    public void SetActiveScene(string sceneName)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        Debug.Log("ACTIVE SCENE SET: " + sceneName);
    }
}
