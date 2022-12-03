using InexperiencedDeveloper.Core;
using InexperiencedDeveloper.Utils;
using InexperiencedDeveloper.Utils.Log;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager> 
{
    public const int LOGIN_LEVEL = 0;
    public const int LOADING_LEVEL = 1;
    public const int PERSISTENT_LEVEL = 2;
    public const int STARTING_AREA = 3;

    private static int m_LevelToLoad = -1;
    public static bool Loaded = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == LOADING_LEVEL)
        {
            //LOAD LEVEL TO LOAD ASYNC
            StartCoroutine(LoadLevelRoutine());
        }
    }

    private IEnumerator LoadLevelRoutine()
    {
        IDLogger.Log($"Loading Level Character Name: {GameManager.CharacterName}, Email: {GameManager.Email}, and Character");
        RequestSpawnPlayer();
        while (!Loaded && m_LevelToLoad < 0)
        {
            yield return null;
        }
        var levelAsync = SceneManager.LoadSceneAsync(m_LevelToLoad, LoadSceneMode.Additive);
        levelAsync.allowSceneActivation = true;
        while (!levelAsync.isDone)
        {
            IDLogger.Log($"Loading {levelAsync.progress}");
            yield return null;
        }
        var loadingAsync = SceneManager.UnloadSceneAsync(LOADING_LEVEL);
        PlayerManager.Instance.MovePlayersToScene(m_LevelToLoad);
        loadingAsync.allowSceneActivation = true;
        m_LevelToLoad = -1;
        Loaded = false;
    }

    public void RequestSpawnPlayer()
    {
        PlayerManager.Instance.RequestSpawnPlayer(GameManager.Email, GameManager.CharacterName);
    }

    public void LoadLevel()
    {
        IDLogger.Log($"Starting Load Level Character Name: {GameManager.CharacterName}, Email: {GameManager.Email}, and Character");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(PERSISTENT_LEVEL, LoadSceneMode.Single);
        SceneManager.LoadScene(LOADING_LEVEL, LoadSceneMode.Additive);
    }

    public void SetLevelToLoad(int level)
    {
        m_LevelToLoad = level;
    }

    public void MoveGameObjectToScene(int targetLevel, GameObject obj)
    {
        SceneManager.MoveGameObjectToScene(obj, SceneManager.GetSceneByBuildIndex(targetLevel));
    }

    public void MoveGameObjectToScene(int targetLevel, Player obj)
    {
        SceneManager.MoveGameObjectToScene(obj.gameObject, SceneManager.GetSceneByBuildIndex(targetLevel));
    }
}
