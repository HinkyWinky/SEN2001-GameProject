using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCtrl : MonoBehaviour
{
    private SceneType curSceneType;
    [ShowInInspector, ReadOnly] public SceneType CurSceneType => curSceneType;

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
    private IEnumerator LoadSceneAdditive(SceneData sceneData, bool setSceneActive, bool unloadActiveScene)
    {
        GameManager.Cur.EventCtrl.onSceneLoadStarted?.Invoke();

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
                    if (GameManager.Cur.GameManagerCanvas != null)
                    {
                        float unloadProgress = Mathf.Clamp01(unloadOperation.progress) * 0.5f / 0.9f;
                        GameManager.Cur.GameManagerCanvas.loadingPanel.loadingBar.bar.value = unloadProgress;
                    }
                    yield return null;
                }
                Debug.Log("SCENE UNLOADED: " + sceneName);
            }
        }

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneData.BuildIndex, LoadSceneMode.Additive);
        loadOperation.allowSceneActivation = false;
        while (!loadOperation.isDone)
        {
            if (GameManager.Cur.GameManagerCanvas != null)
            {
                float loadProgress = Mathf.Clamp01(loadOperation.progress) * 0.5f / 0.9f + 0.5f;
                GameManager.Cur.GameManagerCanvas.loadingPanel.loadingBar.bar.value = loadProgress;
            }

            if (loadOperation.progress >= 0.9f)
            {
                Debug.Log("SCENE LOADED: " + sceneData.Name);
                loadOperation.allowSceneActivation = true;
            }
            yield return null;
        }

        if (setSceneActive)
            SetActiveScene(sceneData);
    }

    public IEnumerator LoadMainMenuScene(bool unloadActiveScene)
    {
        curSceneType = SceneType.MAINMENU;
        yield return StartCoroutine(LoadSceneAdditive(mainMenuSceneData, true, unloadActiveScene));
    }
    public IEnumerator LoadLevelScene(int levelNo, bool unloadActiveScene)
    {
        curSceneType = SceneType.LEVEL;
        yield return StartCoroutine(LoadSceneAdditive(levelsSceneData[levelNo - 1], true, unloadActiveScene));
    }
}
