using UnityEngine;

/// <summary>
/// Quản lý Loading Scene - đảm bảo nhạc Loading được phát
/// </summary>
public class LoadingSceneManager : MonoBehaviour
{
    private void Start()
    {
        // Đảm bảo nhạc loading được phát khi vào scene loading
        if (SoundController.GetInstance() != null)
        {
            SoundController.GetInstance().SwitchToLoadingMusic();
            Debug.Log("[LoadingSceneManager] 🎵 Switched to Loading music");
        }
        else
        {
            Debug.LogWarning("[LoadingSceneManager] SoundController not found!");
        }
    }
}
