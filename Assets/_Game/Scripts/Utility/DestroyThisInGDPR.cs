using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyThisInGDPR : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene s, LoadSceneMode arg)
    {
        if (s.buildIndex == 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }
}
