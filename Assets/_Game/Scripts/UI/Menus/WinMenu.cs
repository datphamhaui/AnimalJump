using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Menu hiển thị khi người chơi hoàn thành level
/// </summary>
public class WinMenu : Menu
{
    [Header("UI Text References")]
    [SerializeField] private TMP_Text _celebrationText; // "Yay!" text
    [SerializeField] private TMP_Text _winTitleText; // "You win!" text
    [SerializeField] private TMP_Text _levelScoreText; // "Level # score:"
    [SerializeField] private TMP_Text _scoreValueText; // Score number
    [SerializeField] private TMP_Text _coinValueText; // Coin reward amount
    
    [Header("Stars Display")]
    [SerializeField] private Image[] _starImages; // 3 star images (not GameObjects)
    [SerializeField] private Sprite _yellowStarSprite; // Active star sprite
    [SerializeField] private Sprite _grayStarSprite; // Inactive star sprite
    
    [Header("Buttons")]
    [SerializeField] private Button _closeButton; // X button
    [SerializeField] private Button _homeButton; // Purple home button
    [SerializeField] private Button _retryButton; // Blue retry button  
    [SerializeField] private Button _nextLevelButton; // Green next button

    private int _earnedStars = 0;
    private int _earnedCoins = 0;
    private LevelProgressManager _levelProgressManager;
    private CurrencyManager _currencyManager;

    private void Start()
    {
        _levelProgressManager = LevelProgressManager.GetInstance();
        _currencyManager = CurrencyManager.GetInstance();

        // Setup button callbacks
        OnButtonPressed(_closeButton, CloseButton);
        OnButtonPressed(_homeButton, HomeButton);
        OnButtonPressed(_retryButton, RetryButton);
        OnButtonPressed(_nextLevelButton, NextLevelButton);
    }

    public override void SetEnable()
    {
        base.SetEnable();

        // Enable tất cả buttons
        SetButtonInteractable(_closeButton, true);
        SetButtonInteractable(_homeButton, true);
        SetButtonInteractable(_retryButton, true);
        SetButtonInteractable(_nextLevelButton, true);

        UpdateDisplay();
    }

    /// <summary>
    /// Helper method to safely set button interactable
    /// </summary>
    private void SetButtonInteractable(Button button, bool interactable)
    {
        if (button != null) button.interactable = interactable;
    }

    private void OnEnable()
    {
        GameManager.OnGameWin += OnGameWin;
    }

    private void OnDisable()
    {
        GameManager.OnGameWin -= OnGameWin;
    }

    /// <summary>
    /// Nhận event khi win game
    /// </summary>
    private void OnGameWin(int stars)
    {
        _earnedStars = stars;
        
        // Calculate coin reward based on level data and stars
        CalculateCoinReward();
        
        // Cộng coin vào CurrencyManager
        if (_currencyManager != null)
        {
            _currencyManager.AddCoins(_earnedCoins);
            Debug.Log($"[WinMenu] Added {_earnedCoins} coins to player!");
        }
        
        Debug.Log($"[WinMenu] Game won with {stars} stars, earned {_earnedCoins} coins!");
    }

    /// <summary>
    /// Tính toán coin reward dựa trên level data và số sao
    /// </summary>
    private void CalculateCoinReward()
    {
        int currentLevel = _levelProgressManager.GetCurrentLevel();
        LevelDataSO currentLevelData = _levelProgressManager.GetLevelData(currentLevel);
        
        if (currentLevelData != null)
        {
            // Base coins from level data
            int baseCoinReward = currentLevelData.coinReward;
            
            // Bonus based on stars: 1 star = 1x, 2 stars = 1.5x, 3 stars = 2x
            float starMultiplier = _earnedStars switch
            {
                1 => 1.0f,
                2 => 1.5f, 
                3 => 2.0f,
                _ => 1.0f
            };
            
            _earnedCoins = Mathf.RoundToInt(baseCoinReward * starMultiplier);
            
            Debug.Log($"[WinMenu] Level {currentLevel}: Base coins = {baseCoinReward}, Stars = {_earnedStars}, Multiplier = {starMultiplier}x, Final coins = {_earnedCoins}");
        }
        else
        {
            // Fallback default values
            _earnedCoins = _earnedStars * 100; // 100 coins per star
            Debug.LogWarning($"[WinMenu] No level data found for level {currentLevel}, using fallback: {_earnedCoins} coins");
        }
    }

    /// <summary>
    /// Cập nhật UI display
    /// </summary>
    private void UpdateDisplay()
    {
        // Celebration text
        if (_celebrationText) _celebrationText.text = "Yay!";
        
        // Win title
        if (_winTitleText) _winTitleText.text = "You win!";

        // Level score text
        int currentLevel = _levelProgressManager.GetCurrentLevel();
        if (_levelScoreText) _levelScoreText.text = $"Level {currentLevel} score:";

        // Score value
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        if (_scoreValueText && scoreManager != null) 
        {
            _scoreValueText.text = scoreManager.Score.ToString("N0"); // Format with comma separators
        }

        // Coin reward
        if (_coinValueText) 
        {
            _coinValueText.text = _earnedCoins.ToString();
        }

        // Stars display
        UpdateStarsDisplay(_earnedStars);

        // Check if next level is available
        bool hasNextLevel = currentLevel < _levelProgressManager.TotalLevels;
        if (_nextLevelButton) 
        {
            _nextLevelButton.gameObject.SetActive(hasNextLevel);
        }
    }

    /// <summary>
    /// Hiển thị số sao đạt được với sprite system
    /// </summary>
    private void UpdateStarsDisplay(int stars)
    {
        if (_starImages == null || _starImages.Length < 3) 
        {
            Debug.LogWarning("[WinMenu] Star images array is null or has less than 3 elements!");
            return;
        }

        for (int i = 0; i < _starImages.Length; i++)
        {
            if (_starImages[i] != null)
            {
                // Set sprite based on whether star is earned
                if (i < stars && _yellowStarSprite != null)
                {
                    _starImages[i].sprite = _yellowStarSprite;
                    _starImages[i].color = Color.white; // Full opacity for earned stars
                }
                else if (_grayStarSprite != null)
                {
                    _starImages[i].sprite = _grayStarSprite;
                    _starImages[i].color = Color.white; // Keep visible but gray
                }
                
                // Make sure the star image is always visible
                _starImages[i].gameObject.SetActive(true);
            }
        }

        Debug.Log($"[WinMenu] Displaying {stars} stars with sprite system");
    }

    /// <summary>
    /// Validation in Inspector
    /// </summary>
    private void OnValidate()
    {
        if (_starImages != null && _starImages.Length != 3)
        {
            Debug.LogWarning("[WinMenu] Star images array should have exactly 3 elements!");
        }
        
        if (_yellowStarSprite == null)
        {
            Debug.LogWarning("[WinMenu] Yellow star sprite is not assigned!");
        }
        
        if (_grayStarSprite == null)
        {
            Debug.LogWarning("[WinMenu] Gray star sprite is not assigned!");
        }
    }

    /// <summary>
    /// Đóng menu (X button)
    /// </summary>
    private void CloseButton()
    {
        SetButtonInteractable(_closeButton, false);
        
        // Đóng menu và disable tất cả menus trước khi chuyển scene
        SetDisable();
        DisableAllMenus();
        
        // Về scene chọn level
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
    /// <summary>
    /// Chơi level tiếp theo (Next button - Green)
    /// </summary>
    private void NextLevelButton()
    {
        SetButtonInteractable(_nextLevelButton, false);
        
        // Đóng menu trước
        SetDisable();

        int nextLevel = _levelProgressManager.GetCurrentLevel() + 1;
        _levelProgressManager.SetCurrentLevel(nextLevel);

        StartCoroutine(LevelLoader.ReloadLevelAsync(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }));
    }
}
