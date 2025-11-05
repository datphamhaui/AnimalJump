using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component cho Pause button trong gameplay UI
/// Attach script này vào button Pause và nó sẽ tự động mở PauseMenu khi click
/// </summary>
[RequireComponent(typeof(Button))]
public class PauseButton : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        
        // Add listener to button
        _button.onClick.AddListener(OnPauseButtonClicked);
    }

    /// <summary>
    /// Callback khi nhấn pause button
    /// </summary>
    private void OnPauseButtonClicked()
    {
        // Disable button để tránh click nhiều lần
        _button.interactable = false;
        
        // Mở Pause menu
        MenuManager menuManager = MenuManager.GetInstance();
        if (menuManager != null)
        {
            menuManager.OpenMenu(MenuType.Pause);
            Debug.Log("[PauseButton] Opening Pause Menu");
            
            // Start coroutine để check khi menu đóng
            StartCoroutine(WaitForMenuClose(menuManager));
        }
        else
        {
            Debug.LogError("[PauseButton] MenuManager not found!");
            // Re-enable nếu có lỗi
            _button.interactable = true;
        }
    }
    
    /// <summary>
    /// Coroutine để wait cho menu đóng và re-enable button
    /// </summary>
    private System.Collections.IEnumerator WaitForMenuClose(MenuManager menuManager)
    {
        // Đợi frame tiếp theo để menu mở xong
        yield return null;
        
        // Check liên tục cho đến khi menu không còn là Pause nữa
        while (menuManager != null && menuManager.GetCurrentMenu == MenuType.Pause)
        {
            yield return null;
        }
        
        // Menu đã đóng, re-enable button
        if (_button != null)
        {
            _button.interactable = true;
            Debug.Log("[PauseButton] Re-enabled after menu closed");
        }
    }

    private void OnEnable()
    {
        // Re-enable button khi active lại
        if (_button != null)
        {
            _button.interactable = true;
        }
        
        // Subscribe to game events để re-enable button
        GameManager.OnRevive += EnableButton;
    }
    
    private void OnDisable()
    {
        // Unsubscribe
        GameManager.OnRevive -= EnableButton;
    }
    
    /// <summary>
    /// Enable button sau khi resume/revive game
    /// </summary>
    private void EnableButton()
    {
        if (_button != null)
        {
            _button.interactable = true;
            Debug.Log("[PauseButton] Re-enabled");
        }
    }

    /// <summary>
    /// Optional: Hỗ trợ ESC key để pause (nếu muốn)
    /// </summary>
    private void Update()
    {
        // Chỉ xử lý ESC key khi đang trong gameplay
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuManager menuManager = MenuManager.GetInstance();
            if (menuManager != null && menuManager.GetCurrentMenu == MenuType.Gameplay)
            {
                OnPauseButtonClicked();
            }
        }
    }

    private void OnDestroy()
    {
        // Cleanup
        if (_button != null)
        {
            _button.onClick.RemoveListener(OnPauseButtonClicked);
        }
    }
}
