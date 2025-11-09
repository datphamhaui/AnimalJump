using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Menu hiển thị khi người chơi thua (Game Over)
/// </summary>
public class LoseMenu : Menu
{
    [Header("UI Text References")]
    [SerializeField] private TMP_Text _sadText; // "Oh.." text
    [SerializeField] private TMP_Text _loseTitleText; // "You lose.." text
    [SerializeField] private TMP_Text _levelScoreText; // "Level # score:"
    [SerializeField] private TMP_Text _scoreValueText; // Score number
    [SerializeField] private TMP_Text _coinValueText; // Coin amount (0 for lose)
    
    [Header("Buttons")]
    [SerializeField] private Button _closeButton; // X button
    [SerializeField] private Button _homeButton; // Purple home button
    [SerializeField] private Button _retryButton; // Blue retry button

    private LevelProgressManager _levelProgressManager;

    private void Start()
    {
        _levelProgressManager = LevelProgressManager.GetInstance();

        // Setup button callbacks
        OnButtonPressed(_closeButton, CloseButton);
        OnButtonPressed(_homeButton, HomeButton);
        OnButtonPressed(_retryButton, RetryButton);
    }

    public override void SetEnable()
    {
        base.SetEnable();

        // Enable all buttons
        SetButtonInteractable(_closeButton, true);
        SetButtonInteractable(_homeButton, true);
        SetButtonInteractable(_retryButton, true);

        UpdateDisplay();
    }

    /// <summary>
    /// Helper method to safely set button interactable
    /// </summary>
    private void SetButtonInteractable(Button button, bool interactable)
    {
        if (button != null) button.interactable = interactable;
    }

    /// <summary>
    /// Cập nhật UI display
    /// </summary>
    private void UpdateDisplay()
    {
        // Sad text
        if (_sadText) _sadText.text = "Oh..";
        
        // Lose title
        if (_loseTitleText) _loseTitleText.text = "You lose..";

        // Level score text
        int currentLevel = _levelProgressManager.GetCurrentLevel();
        if (_levelScoreText) _levelScoreText.text = $"Level {currentLevel} score:";

        // Score value
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        if (_scoreValueText && scoreManager != null) 
        {
            _scoreValueText.text = scoreManager.Score.ToString("N0"); // Format with comma separators
        }

        // Coin value - 0 for losing
        if (_coinValueText) 
        {
            _coinValueText.text = "0";
        }

        Debug.Log($"[LoseMenu] Displayed for Level {currentLevel}");
    }

    /// <summary>
    /// Đóng menu (X button)
    /// </summary>
    private void CloseButton()
    {
        SetButtonInteractable(_closeButton, false);

        // Reset health về 3 hearts trước khi về Level scene
        HealthManager healthManager = HealthManager.GetInstance();
        if (healthManager != null)
        {
            healthManager.ResetHealth();
        }

        // Đóng menu và disable tất cả menus trước khi chuyển scene
        SetDisable();
        DisableAllMenus();

        StartCoroutine(LevelLoader.ReloadLevelAsync(() =>
        {
            SceneManager.LoadScene("Level");
        }));
    }

    /// <summary>
    /// Về menu chính (Home button - Purple)
    /// </summary>
    private void HomeButton()
    {
        SetButtonInteractable(_homeButton, false);

        // Reset health về 3 hearts trước khi về Level scene
        HealthManager healthManager = HealthManager.GetInstance();
        if (healthManager != null)
        {
            healthManager.ResetHealth();
        }

        // Đóng menu và disable tất cả menus trước khi chuyển scene
        SetDisable();
        DisableAllMenus();

        StartCoroutine(LevelLoader.ReloadLevelAsync(() =>
        {
            SceneManager.LoadScene("Level");
        }));
    }

    /// <summary>
    /// Disable tất cả các menus trước khi chuyển scene
    /// </summary>
    private void DisableAllMenus()
    {
        MenuManager menuManager = MenuManager.GetInstance();
        if (menuManager != null)
        {
            // Clear tất cả menus trong stack
            menuManager.ClearAllMenus();
        }
    }

    /// <summary>
    /// Chơi lại level hiện tại (Retry button - Blue)
    /// </summary>
    private void RetryButton()
    {
        SetButtonInteractable(_retryButton, false);
        
        // Reset health về 3 hearts trước khi reload
        HealthManager healthManager = HealthManager.GetInstance();
        if (healthManager != null)
        {
            healthManager.ResetHealth();
        }
        
        // Đóng menu trước
        SetDisable();

        StartCoroutine(LevelLoader.ReloadLevelAsync(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }));
    }
}
