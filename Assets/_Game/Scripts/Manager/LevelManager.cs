using System;
using UnityEngine;

/// <summary>
/// Quản lý level hiện tại - load config từ LevelDataSO
/// Apply settings vào game (tốc độ, độ khó, etc.)
/// </summary>
public class LevelManager : MonoBehaviour
{
    private LevelProgressManager _levelProgressManager;
    private LevelDataSO _currentLevelData;

    public LevelDataSO GetCurrentLevelData() => _currentLevelData;
    public float CurrentSpeed => _currentLevelData != null ? _currentLevelData.platformSpeed : 2f;

    public static event Action<int, float> OnLevelChanged; // level, speed

    private void Awake()
    {
        _levelProgressManager = LevelProgressManager.GetInstance();
    }

    private void Start()
    {
        LoadCurrentLevel();
    }

    /// <summary>
    /// Load level hiện tại từ LevelProgressManager
    /// </summary>
    private void LoadCurrentLevel()
    {
        int levelNumber = _levelProgressManager.GetCurrentLevel();
        _currentLevelData = _levelProgressManager.GetLevelData(levelNumber);

        if (_currentLevelData == null)
        {
            Debug.LogError($"[LevelManager] Không tìm thấy data cho Level {levelNumber}!");
            return;
        }

        Debug.Log($"[LevelManager] Loaded Level {levelNumber}: {_currentLevelData.levelName}");
        Debug.Log($"[LevelManager] - Target Score: {_currentLevelData.targetScore}");
        Debug.Log($"[LevelManager] - Platform Speed: {_currentLevelData.platformSpeed}");
        Debug.Log($"[LevelManager] - Safe Zone: {_currentLevelData.safeLandingZoneRatio}");

        // Trigger event để các system khác update
        OnLevelChanged?.Invoke(levelNumber, _currentLevelData.platformSpeed);
    }

    /// <summary>
    /// Lấy tốc độ hiện tại cho platform
    /// </summary>
    public float GetCurrentSpeed()
    {
        return CurrentSpeed;
    }

    /// <summary>
    /// Lấy gap range của level hiện tại
    /// </summary>
    public Vector2 GetPlatformGapRange()
    {
        return _currentLevelData != null ? _currentLevelData.platformGapRange : new Vector2(0.5f, 1.5f);
    }

    /// <summary>
    /// Lấy safe landing zone ratio
    /// </summary>
    public float GetSafeLandingZoneRatio()
    {
        return _currentLevelData != null ? _currentLevelData.safeLandingZoneRatio : 0.7f;
    }

    /// <summary>
    /// Reset level (dùng khi restart)
    /// </summary>
    public void ResetLevel()
    {
        LoadCurrentLevel();
    }
}
