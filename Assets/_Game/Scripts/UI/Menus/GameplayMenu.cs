using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayMenu : Menu
{
    [Header("UI References :")]
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _targetScoreText; // Hiển thị điểm mục tiêu
    [SerializeField] private TMP_Text _levelText; // Hiển thị level hiện tại
    
    [Header("Pause Button")]
    [SerializeField] private Button _pauseButton;

    private void Start()
    {
        if (_pauseButton != null)
        {
            _pauseButton.onClick.AddListener(OnPauseButtonClicked);
        }
    }

    public override void SetEnable()
    {
        base.SetEnable();

        // Update score
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            UpdateScore(scoreManager.Score);
        }

        // Update level info
        UpdateLevelInfo();
    }

    /// <summary>
    /// Cập nhật thông tin level hiện tại
    /// </summary>
    private void UpdateLevelInfo()
    {
        LevelProgressManager progressManager = LevelProgressManager.GetInstance();
        LevelManager levelManager = FindFirstObjectByType<LevelManager>();

        if (progressManager != null && levelManager != null)
        {
            int currentLevel = progressManager.GetCurrentLevel();
            LevelDataSO levelData = levelManager.GetCurrentLevelData();

            // Hiển thị level
            if (_levelText != null)
            {
                _levelText.text = $"Level {currentLevel}";
            }

            // Hiển thị target score
            if (_targetScoreText != null && levelData != null)
            {
                _targetScoreText.text = $"Target: {levelData.targetScore}";
            }
        }
    }

    private void UpdateScore(int currentScore) 
    { 
        if (_scoreText != null)
        {
            _scoreText.text = currentScore.ToString(); 
        }
    }

    /// <summary>
    /// Xử lý khi nhấn nút Pause
    /// </summary>
    private void OnPauseButtonClicked()
    {
        MenuManager.GetInstance().OpenMenu(MenuType.Pause);
        
        Debug.Log("[GameplayMenu] Pause button clicked");
    }

    private void OnEnable() { GameManager.OnScoreUpdated += UpdateScore; }

    private void OnDisable() { GameManager.OnScoreUpdated -= UpdateScore; }
}