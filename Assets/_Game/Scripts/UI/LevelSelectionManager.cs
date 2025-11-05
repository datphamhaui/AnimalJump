using UnityEngine;

/// <summary>
/// Manager cho màn Level Selection
/// Quản lý tất cả level buttons
/// </summary>
public class LevelSelectionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelButton[] _levelButtons;

    private LevelProgressManager _progressManager;

    private void Start()
    {
        _progressManager = LevelProgressManager.GetInstance();

        // Auto tìm tất cả LevelButton nếu chưa assign
        if (_levelButtons == null || _levelButtons.Length == 0)
        {
            _levelButtons = FindObjectsByType<LevelButton>(FindObjectsSortMode.None);
        }

        RefreshAllLevels();
    }

    /// <summary>
    /// Refresh tất cả level buttons
    /// </summary>
    public void RefreshAllLevels()
    {
        foreach (var levelButton in _levelButtons)
        {
            if (levelButton != null)
            {
                levelButton.RefreshDisplay();
            }
        }

        Debug.Log($"[LevelSelectionManager] Refreshed {_levelButtons.Length} level buttons");
    }

    /// <summary>
    /// Debug: Unlock all levels
    /// </summary>
    [ContextMenu("Debug: Unlock All Levels")]
    private void DebugUnlockAllLevels()
    {
        if (_progressManager == null) return;

        for (int i = 1; i <= _progressManager.TotalLevels; i++)
        {
            _progressManager.UnlockLevel(i);
        }

        RefreshAllLevels();
        Debug.Log("✅ All levels unlocked!");
    }

    /// <summary>
    /// Debug: Set all levels to 3 stars
    /// </summary>
    [ContextMenu("Debug: 3 Stars All Levels")]
    private void DebugSetAllLevels3Stars()
    {
        if (_progressManager == null) return;

        for (int i = 1; i <= _progressManager.TotalLevels; i++)
        {
            _progressManager.UnlockLevel(i);
            _progressManager.SaveLevelStars(i, 3);
        }

        RefreshAllLevels();
        Debug.Log("✅ All levels set to 3 stars!");
    }

    /// <summary>
    /// Debug: Reset progress
    /// </summary>
    [ContextMenu("Debug: Reset Progress")]
    private void DebugResetProgress()
    {
        if (_progressManager != null)
        {
            _progressManager.ResetAllProgress();
            RefreshAllLevels();
            Debug.Log("✅ Progress reset!");
        }
    }
}
