using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Component cho mỗi animal button trong animal selector/shop
/// Hiển thị: Lock/Unlock state, Price, Select button
/// </summary>
public class AnimalShopButton : MonoBehaviour
{
    [Header("Animal Info")]
    [SerializeField] private AnimalType _animalType;

    [Header("UI References")]
    [Tooltip("Button chính để select/buy")]
    [SerializeField] private Button _mainButton;
    
    [Tooltip("Text hiển thị tên động vật")]
    [SerializeField] private TMP_Text _nameText;
    
    [Tooltip("GameObject hiển thị khi locked")]
    [SerializeField] private GameObject _lockedState;
    
    [Tooltip("GameObject hiển thị khi unlocked")]
    [SerializeField] private GameObject _unlockedState;
    
    [Tooltip("Text hiển thị giá (trong locked state)")]
    [SerializeField] private TMP_Text _priceText;
    
    [Tooltip("Icon coin (optional)")]
    [SerializeField] private Image _coinIcon;

    [Header("Visual Feedback")]
    [Tooltip("Highlight khi được select")]
    [SerializeField] private GameObject _selectedHighlight;
    
    [Tooltip("Image của button (để đổi màu khi locked)")]
    [SerializeField] private Image _buttonImage;
    
    [SerializeField] private Color _unlockedColor = Color.white;
    [SerializeField] private Color _lockedColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    private AnimalOwnershipManager _ownershipManager;
    private CurrencyManager _currencyManager;
    private bool _isUnlocked;

    private void Start()
    {
        _ownershipManager = AnimalOwnershipManager.GetInstance();
        _currencyManager = CurrencyManager.GetInstance();

        if (_mainButton != null)
        {
            _mainButton.onClick.AddListener(OnButtonClicked);
        }

        UpdateDisplay();
    }

    private void OnEnable()
    {
        // Subscribe to events
        CurrencyManager.OnCoinsChanged += OnCoinsChanged;
        AnimalOwnershipManager.OnAnimalUnlocked += OnAnimalUnlocked;
        AnimalOwnershipManager.OnAnimalSelected += OnAnimalSelected;
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        CurrencyManager.OnCoinsChanged -= OnCoinsChanged;
        AnimalOwnershipManager.OnAnimalUnlocked -= OnAnimalUnlocked;
        AnimalOwnershipManager.OnAnimalSelected -= OnAnimalSelected;
    }

    /// <summary>
    /// Cập nhật UI display
    /// </summary>
    private void UpdateDisplay()
    {
        if (_ownershipManager == null) return;

        _isUnlocked = _ownershipManager.IsAnimalUnlocked(_animalType);

        // Name
        if (_nameText != null)
        {
            _nameText.text = _animalType.GetDisplayName();
        }

        // Lock/Unlock state
        if (_lockedState != null)
        {
            _lockedState.SetActive(!_isUnlocked);
        }

        if (_unlockedState != null)
        {
            _unlockedState.SetActive(_isUnlocked);
        }

        // Price (chỉ hiện khi locked)
        if (_priceText != null && !_isUnlocked)
        {
            int price = _ownershipManager.GetAnimalPrice(_animalType);
            _priceText.text = price.ToString();

            // Đổi màu text nếu không đủ coin
            if (_currencyManager != null)
            {
                _priceText.color = _currencyManager.HasEnoughCoins(price) 
                    ? Color.white 
                    : Color.red;
            }
        }

        // Button color
        if (_buttonImage != null)
        {
            _buttonImage.color = _isUnlocked ? _unlockedColor : _lockedColor;
        }

        // Selected highlight
        UpdateSelectedState();

        // Button interactable
        if (_mainButton != null)
        {
            _mainButton.interactable = true; // Luôn clickable (locked = mua, unlocked = chọn)
        }
    }

    /// <summary>
    /// Cập nhật trạng thái selected
    /// </summary>
    private void UpdateSelectedState()
    {
        if (_selectedHighlight == null || _ownershipManager == null) return;

        bool isSelected = _ownershipManager.GetSelectedAnimal() == _animalType;
        _selectedHighlight.SetActive(isSelected && _isUnlocked);
    }

    /// <summary>
    /// Xử lý khi click button
    /// </summary>
    private void OnButtonClicked()
    {
        if (_ownershipManager == null) return;

        if (_isUnlocked)
        {
            // Đã unlock → Select
            _ownershipManager.SelectAnimal(_animalType);
            Debug.Log($"[AnimalShopButton] Selected: {_animalType.GetDisplayName()}");

            // Play sound
            if (SoundController.GetInstance() != null)
            {
                SoundController.GetInstance().PlayAudio(AudioType.BUTTON_CLICK);
            }
        }
        else
        {
            // Chưa unlock → Mua
            bool success = _ownershipManager.PurchaseAnimal(_animalType);
            
            if (success)
            {
                Debug.Log($"[AnimalShopButton] Purchased: {_animalType.GetDisplayName()}");
                
                // Play sound
                if (SoundController.GetInstance() != null)
                {
                    SoundController.GetInstance().PlayAudio(AudioType.BUTTON_CLICK);
                }
            }
            else
            {
                Debug.LogWarning($"[AnimalShopButton] Cannot purchase: {_animalType.GetDisplayName()}");
                
                // TODO: Show "Not enough coins" popup
            }
        }
    }

    /// <summary>
    /// Event handler khi coin thay đổi
    /// </summary>
    private void OnCoinsChanged(int newAmount)
    {
        // Cập nhật màu text price nếu đang locked
        if (!_isUnlocked && _priceText != null && _ownershipManager != null)
        {
            int price = _ownershipManager.GetAnimalPrice(_animalType);
            _priceText.color = _currencyManager.HasEnoughCoins(price) 
                ? Color.white 
                : Color.red;
        }
    }

    /// <summary>
    /// Event handler khi có động vật được unlock
    /// </summary>
    private void OnAnimalUnlocked(AnimalType type)
    {
        if (type == _animalType)
        {
            UpdateDisplay();
        }
    }

    /// <summary>
    /// Event handler khi có động vật được chọn
    /// </summary>
    private void OnAnimalSelected(AnimalType type)
    {
        UpdateSelectedState();
    }

    /// <summary>
    /// Set animal type (dùng khi tạo button runtime)
    /// </summary>
    public void SetAnimalType(AnimalType type)
    {
        _animalType = type;
        UpdateDisplay();
    }

    /// <summary>
    /// Public method để refresh display
    /// </summary>
    public void RefreshDisplay()
    {
        UpdateDisplay();
    }
}
