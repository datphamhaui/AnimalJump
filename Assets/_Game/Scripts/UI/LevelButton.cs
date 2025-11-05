using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Component cho mỗi level button trong màn chọn level
/// Hiển thị số level, trạng thái lock/unlock, số sao
/// 
/// UI Structure (dựa trên hình):
/// Level Button
/// ├─ Radial Shine (hiệu ứng sáng khi unlock)
/// ├─ Shadow (bóng)
/// ├─ Lines (hiệu ứng đường khi unlock)
/// ├─ UnLock (GameObject chứa UI khi unlock)
/// ├─ Lock (GameObject icon khóa khi locked)
/// ├─ Text (TMP) (số level)
/// └─ Stars (GameObject chứa 3 sao)
/// </summary>
public class LevelButton : MonoBehaviour
{
    [Header("Level Info")]
    [SerializeField] private int _levelNumber;
    [SerializeField] private Button _button;
    
    [Header("Text")]
    [SerializeField] private TMP_Text _levelText; // Text (TMP)
    
    [Header("Lock/Unlock GameObjects")]
    [Tooltip("GameObject 'Lock' - hiển thị khi level bị khóa")]
    [SerializeField] private GameObject _lock;
    
    [Tooltip("GameObject 'UnLock' - hiển thị khi level đã mở")]
    [SerializeField] private GameObject _unlock;
    
    [Header("Visual Effects (Optional)")]
    [Tooltip("GameObject 'Radial Shine' - hiệu ứng sáng")]
    [SerializeField] private GameObject _radialShine;
    
    [Tooltip("GameObject 'Lines' - hiệu ứng đường")]
    [SerializeField] private GameObject _lines;
    
    [Header("Stars")]
    [Tooltip("GameObject 'Stars' chứa 3 sao con")]
    [SerializeField] private GameObject _starsContainer;
    
    [Tooltip("3 sao con trong Stars container (Star1, Star2, Star3) - phải có Image component")]
    [SerializeField] private Image[] _starImages = new Image[3];
    
    [Header("Star Sprites")]
    [Tooltip("Sprite sao vàng (khi đạt được sao)")]
    [SerializeField] private Sprite _yellowStarSprite;
    
    [Tooltip("Sprite sao xám (khi chưa đạt được sao)")]
    [SerializeField] private Sprite _grayStarSprite;

    [Header("Settings")]
    [SerializeField] private string _gameSceneName = "Game";

    private LevelProgressManager _progressManager;
    private bool _isUnlocked;

    private void Start()
    {
        _progressManager = LevelProgressManager.GetInstance();
        
        if (_button != null)
        {
            _button.onClick.AddListener(OnLevelButtonClicked);
        }

        UpdateDisplay();
    }

    /// <summary>
    /// Cập nhật hiển thị button
    /// </summary>
    private void UpdateDisplay()
    {
        if (_progressManager == null) return;

        // Check unlock status
        _isUnlocked = _progressManager.IsLevelUnlocked(_levelNumber);

        // Level number text (luôn hiển thị)
        if (_levelText != null)
        {
            _levelText.text = _levelNumber.ToString();
        }

        // Lock/Unlock UI - Toggle giữa Lock và UnLock GameObject
        if (_lock != null)
        {
            _lock.SetActive(!_isUnlocked); // Lock active khi level bị khóa
        }

        if (_unlock != null)
        {
            _unlock.SetActive(_isUnlocked); // UnLock active khi level mở
        }

        // Visual effects (chỉ hiện khi unlock)
        if (_radialShine != null)
        {
            _radialShine.SetActive(_isUnlocked);
        }

        if (_lines != null)
        {
            _lines.SetActive(_isUnlocked);
        }

        // Stars container (luôn hiện khi unlock để thấy gray stars)
        if (_starsContainer != null)
        {
            _starsContainer.SetActive(_isUnlocked); // Chỉ hiện stars khi unlock
        }

        // Update số sao
        if (_isUnlocked)
        {
            int stars = _progressManager.GetLevelStars(_levelNumber);
            UpdateStarsDisplay(stars);
        }
        else
        {
            // Khi locked, không hiển thị stars container
            // UpdateStarsDisplay(0) sẽ được gọi khi unlock
        }

        // Button interactable
        if (_button != null)
        {
            _button.interactable = _isUnlocked;
        }
    }

    /// <summary>
    /// Hiển thị số sao bằng cách thay đổi sprite
    /// Yellow star = đạt được sao, Gray star = chưa đạt được sao
    /// Ví dụ: 2 sao → Star[0]=yellow, Star[1]=yellow, Star[2]=gray
    /// </summary>
    private void UpdateStarsDisplay(int stars)
    {
        // Clamp stars trong khoảng 0-3
        stars = Mathf.Clamp(stars, 0, 3);

        for (int i = 0; i < _starImages.Length && i < 3; i++)
        {
            if (_starImages[i] != null)
            {
                // Luôn active tất cả star images
                _starImages[i].gameObject.SetActive(true);
                
                // Đổi sprite: yellow nếu đạt được, gray nếu chưa đạt
                if (i < stars)
                {
                    // Đạt được sao → Yellow star
                    _starImages[i].sprite = _yellowStarSprite;
                }
                else
                {
                    // Chưa đạt được sao → Gray star
                    _starImages[i].sprite = _grayStarSprite;
                }
            }
        }

        Debug.Log($"[LevelButton {_levelNumber}] Stars: {stars}/3 (Yellow: {stars}, Gray: {3-stars})");
    }

    /// <summary>
    /// Xử lý khi click vào level button
    /// </summary>
    private void OnLevelButtonClicked()
    {
        if (!_isUnlocked)
        {
            Debug.Log($"Level {_levelNumber} is locked!");
            // TODO: Show popup "Level chưa mở khóa"
            return;
        }

        Debug.Log($"Loading Level {_levelNumber}...");

        // Set current level
        _progressManager.SetCurrentLevel(_levelNumber);

        // Load game scene
        SceneManager.LoadScene(_gameSceneName);

        // Play sound
        if (SoundController.GetInstance() != null)
        {
            SoundController.GetInstance().PlayAudio(AudioType.BUTTON_CLICK);
        }
    }

    /// <summary>
    /// Set level number (dùng khi tạo button runtime)
    /// </summary>
    public void SetLevelNumber(int levelNumber)
    {
        _levelNumber = levelNumber;
        UpdateDisplay();
    }

    /// <summary>
    /// Validate sprites khi assign trong Inspector
    /// </summary>
    private void OnValidate()
    {
        // Cảnh báo nếu thiếu sprites
        if (_yellowStarSprite == null)
        {
            Debug.LogWarning($"[LevelButton {_levelNumber}] Yellow Star Sprite chưa được assign!", this);
        }
        
        if (_grayStarSprite == null)
        {
            Debug.LogWarning($"[LevelButton {_levelNumber}] Gray Star Sprite chưa được assign!", this);
        }
        
        // Check star images có Image component không
        for (int i = 0; i < _starImages.Length; i++)
        {
            if (_starImages[i] == null)
            {
                Debug.LogWarning($"[LevelButton {_levelNumber}] Star Image {i+1} chưa được assign!", this);
            }
        }
    }

    /// <summary>
    /// Public method để refresh display (gọi từ LevelSelectionManager)
    /// </summary>
    public void RefreshDisplay()
    {
        UpdateDisplay();
    }

    /// <summary>
    /// Lấy level number của button này
    /// </summary>
    public int LevelNumber => _levelNumber;

    /// <summary>
    /// Debug: Force unlock level này
    /// </summary>
    [ContextMenu("Debug: Unlock This Level")]
    private void DebugUnlockLevel()
    {
        if (_progressManager != null)
        {
            _progressManager.UnlockLevel(_levelNumber);
            UpdateDisplay();
            Debug.Log($"Level {_levelNumber} unlocked!");
        }
    }

    /// <summary>
    /// Debug: Set 3 stars
    /// </summary>
    [ContextMenu("Debug: Set 3 Stars")]
    private void DebugSet3Stars()
    {
        if (_progressManager != null)
        {
            _progressManager.SaveLevelStars(_levelNumber, 3);
            UpdateDisplay();
            Debug.Log($"Level {_levelNumber} set to 3 stars!");
        }
    }
}
