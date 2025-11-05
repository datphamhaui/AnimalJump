using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Quản lý tiến độ của người chơi qua các level
/// Lưu trữ: level đã unlock, số sao đạt được mỗi level
/// </summary>
public class LevelProgressManager : Singleton<LevelProgressManager>
{
    [Header("Level Configuration")]
    [Tooltip("Danh sách tất cả các level trong game (theo thứ tự)")]
    [SerializeField] private LevelDataSO[] _allLevels;

    [SerializeField] private Button _backButton;

    private const string LEVEL_UNLOCK_KEY  = "LevelUnlock_";
    private const string LEVEL_STARS_KEY   = "LevelStars_";
    private const string CURRENT_LEVEL_KEY = "CurrentLevel";

    public static event Action<int, int> OnLevelCompleted; // levelNumber, stars

    protected override void Awake()
    {
        base.Awake();

        // Level 1 luôn unlock
        if (!IsLevelUnlocked(1))
        {
            UnlockLevel(1);
        }

        _backButton.onClick.AddListener(this.BackButtonOnClick);
    }

    #region Level Access

    /// <summary>
    /// Lấy data của level theo số thứ tự
    /// </summary>
    public LevelDataSO GetLevelData(int levelNumber)
    {
        if (levelNumber < 1 || levelNumber > _allLevels.Length)
        {
            Debug.LogError($"Level {levelNumber} không tồn tại!");

            return null;
        }

        return _allLevels[levelNumber - 1];
    }

    /// <summary>
    /// Tổng số level trong game
    /// </summary>
    public int TotalLevels => _allLevels.Length;

    #endregion

    #region Level Progress

    /// <summary>
    /// Kiểm tra level đã unlock chưa
    /// </summary>
    public bool IsLevelUnlocked(int levelNumber) { return PlayerPrefs.GetInt(LEVEL_UNLOCK_KEY + levelNumber, 0) == 1; }

    /// <summary>
    /// Unlock một level
    /// </summary>
    public void UnlockLevel(int levelNumber)
    {
        PlayerPrefs.SetInt(LEVEL_UNLOCK_KEY + levelNumber, 1);
        PlayerPrefs.Save();
        Debug.Log($"Level {levelNumber} đã được unlock!");
    }

    /// <summary>
    /// Lấy số sao của một level (0-3)
    /// </summary>
    public int GetLevelStars(int levelNumber) { return PlayerPrefs.GetInt(LEVEL_STARS_KEY + levelNumber, 0); }

    /// <summary>
    /// Lưu số sao của một level (chỉ lưu nếu cao hơn record cũ)
    /// </summary>
    public void SaveLevelStars(int levelNumber, int stars)
    {
        int currentStars = GetLevelStars(levelNumber);

        if (stars > currentStars)
        {
            PlayerPrefs.SetInt(LEVEL_STARS_KEY + levelNumber, stars);
            PlayerPrefs.Save();
            Debug.Log($"Level {levelNumber} đạt {stars} sao (mới)!");
        }
        else
        {
            Debug.Log($"Level {levelNumber} đạt {stars} sao (cũ: {currentStars})");
        }
    }

    /// <summary>
    /// Hoàn thành một level
    /// </summary>
    public void CompleteLevel(int levelNumber, int stars)
    {
        // Lưu số sao
        SaveLevelStars(levelNumber, stars);

        // Unlock level tiếp theo (nếu có)
        if (levelNumber < _allLevels.Length)
        {
            UnlockLevel(levelNumber + 1);
        }

        // Trigger event
        OnLevelCompleted?.Invoke(levelNumber, stars);

        Debug.Log($"✅ Hoàn thành Level {levelNumber} với {stars} sao!");
    }

    #endregion

    #region Current Level

    /// <summary>
    /// Lưu level hiện tại đang chơi
    /// </summary>
    public void SetCurrentLevel(int levelNumber)
    {
        PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, levelNumber);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Lấy level hiện tại đang chơi
    /// </summary>
    public int GetCurrentLevel() { return PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 1); }

    #endregion

    #region Debug & Reset

    /// <summary>
    /// Reset toàn bộ tiến độ (dùng cho debug)
    /// </summary>
    public void ResetAllProgress()
    {
        for (int i = 1; i <= _allLevels.Length; i++)
        {
            PlayerPrefs.DeleteKey(LEVEL_UNLOCK_KEY + i);
            PlayerPrefs.DeleteKey(LEVEL_STARS_KEY + i);
        }

        PlayerPrefs.DeleteKey(CURRENT_LEVEL_KEY);
        PlayerPrefs.Save();

        // Unlock level 1 lại
        UnlockLevel(1);

        Debug.Log("🔄 Reset toàn bộ tiến độ!");
    }

    /// <summary>
    /// Log tất cả tiến độ (debug)
    /// </summary>
    public void LogProgress()
    {
        Debug.Log("=== LEVEL PROGRESS ===");

        for (int i = 1; i <= _allLevels.Length; i++)
        {
            bool unlocked = IsLevelUnlocked(i);
            int  stars    = GetLevelStars(i);
            Debug.Log($"Level {i}: {(unlocked ? "Unlocked" : "Locked")} - Stars: {stars}");
        }

        Debug.Log("=====================");
    }

    #endregion

    private void BackButtonOnClick() { SceneManager.LoadScene("SelectAnimalScene"); }
}