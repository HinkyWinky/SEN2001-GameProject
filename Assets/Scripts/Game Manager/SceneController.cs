using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public SceneData gameManagerSceneData;
    public SceneData mainMenuSceneData;
    public List<SceneData> levelsSceneData = new List<SceneData>();

    private void SetActiveScene(SceneData sceneData)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneData.BuildIndex));
        Debug.Log("ACTIVE SCENE SET: " + sceneData.name);
    }
    private IEnumerator LoadScene(SceneData sceneData)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneData.BuildIndex, LoadSceneMode.Single);
        while (!operation.isDone)
        {
            yield return null;
        }
        Debug.Log("SCENE LOADED: " + sceneData.name);
    }
    private IEnumerator LoadSceneAdditive(SceneData sceneData, bool setSceneActive, bool setSceneDatabase, bool unloadActiveScene)
    {
        GameManager.Cur.EventController.onSceneLoadStarted?.Invoke();

        Scene preActiveScene = SceneManager.GetActiveScene();
        SetActiveScene(gameManagerSceneData);

        if (unloadActiveScene)
        {
            if (preActiveScene.isLoaded)
            {
                string sceneName = preActiveScene.name;
                AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(preActiveScene.buildIndex);
                while (!unloadOperation.isDone)
                {
                    float unloadProgress = Mathf.Clamp01(unloadOperation.progress) * 0.5f / 0.9f;
                    GameManager.Cur.gameManagerCanvas.loadingPanel.loadingBar.bar.value = unloadProgress;
                    yield return null;
                }
                Debug.Log("SCENE UNLOADED: " + sceneName);
            }
        }

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneData.BuildIndex, LoadSceneMode.Additive);
        while (!loadOperation.isDone)
        {
            float loadProgress = Mathf.Clamp01(loadOperation.progress) * 0.5f / 0.9f + 0.5f;
            GameManager.Cur.gameManagerCanvas.loadingPanel.loadingBar.bar.value = loadProgress;
            yield return null;
        }
        Debug.Log("SCENE LOADED: " + sceneData.Name);

        if (setSceneActive)
            SetActiveScene(sceneData);

        if (setSceneDatabase)
            GameManager.Cur.SetSceneDatabase();

        GameManager.Cur.EventController.onSceneLoaded?.Invoke();
    }

    public IEnumerator LoadMainMenuScene(bool unloadActiveScene)
    {
        yield return StartCoroutine(LoadSceneAdditive(mainMenuSceneData, true, true, unloadActiveScene));
    }
    public IEnumerator LoadLevelScene(int levelNo, bool unloadActiveScene)
    {
        yield return StartCoroutine(LoadSceneAdditive(levelsSceneData[levelNo - 1], true, true, unloadActiveScene));
    }
}
