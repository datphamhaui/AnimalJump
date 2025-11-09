using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Menu Pause game
/// </summary>
public class PauseMenu : Menu
{
    [Header("Buttons")]
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _homeButton;

    private void Start()
    {
        OnButtonPressed(_resumeButton, ResumeButton);
        OnButtonPressed(_restartButton, RestartButton);
        OnButtonPressed(_homeButton, HomeButton);
    }

    public override void SetEnable()
    {
        base.SetEnable();

        // Pause game
        Time.timeScale = 0f;

        // Enable buttons
        if (_resumeButton) _resumeButton.interactable = true;
        if (_restartButton) _restartButton.interactable = true;
        if (_homeButton) _homeButton.interactable = true;

        Debug.Log("[PauseMenu] Game paused");
    }

    public override void SetDisable()
    {
        base.SetDisable();

        // Resume game
        Time.timeScale = 1f;

        Debug.Log("[PauseMenu] Game resumed");
    }

    /// <summary>
    /// Resume game
    /// </summary>
    private void ResumeButton()
    {
        if (_resumeButton) _resumeButton.interactable = false;

        MenuManager.GetInstance().CloseMenu(); // Close pause menu
        
        // Menu sẽ tự động resume game trong SetDisable()
    }

    /// <summary>
    /// Restart level
    /// </summary>
    private void RestartButton()
    {
        if (_restartButton) _restartButton.interactable = false;

        // Reset health về 3 hearts trước khi reload
        HealthManager healthManager = HealthManager.GetInstance();
        if (healthManager != null)
        {
            healthManager.ResetHealth();
        }

        // Đóng menu và resume time
        SetDisable();

        StartCoroutine(LevelLoader.ReloadLevelAsync(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }));
    }

    /// <summary>
    /// Về menu chính
    /// </summary>
    private void HomeButton()
    {
        if (_homeButton) _homeButton.interactable = false;

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
}
