using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelLoader
{
    public static void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(levelIndex);
        }
        else
        {
            Debug.LogWarning("LEVELLOADER LoadLevel Error: invalid scene specified");
        }
    }

    public static void ReloadLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public static IEnumerator ReloadLevelAsync(Action OnSceneLoaded = null)
    {
        yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        OnSceneLoaded?.Invoke();
    }
}