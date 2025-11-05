using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine.SceneManagement;
using TMPro;

public class AnimalSelector : MonoBehaviour
{
    [SerializeField] private SimpleScrollSnap _simpleScrollSnap;
    [SerializeField] private Button _selectButton;
    
    [Header("Animal Info Display")]
    [Tooltip("Text hiển thị tên động vật")]
    [SerializeField] private TMP_Text _animalNameText;
    
    [Tooltip("Text hiển thị trạng thái (Locked/Unlocked)")]
    [SerializeField] private TMP_Text _statusText;
    
    [Tooltip("Text hiển thị giá (khi locked)")]
    [SerializeField] private TMP_Text _priceText;
    
    [Tooltip("GameObject chứa price display (ẩn khi unlocked)")]
    [SerializeField] private GameObject _priceDisplay;

    private AnimalOwnershipManager _ownershipManager;
    private CurrencyManager _currencyManager;

    private void Start()
    {
        _ownershipManager = AnimalOwnershipManager.GetInstance();
        _currencyManager = CurrencyManager.GetInstance();

        if (_selectButton != null)
        {
            _selectButton.onClick.AddListener(OnSelectButtonClicked);
        }

        // Listen to panel change
        if (_simpleScrollSnap != null)
        {
            _simpleScrollSnap.onPanelChanged.AddListener(OnPanelChanged);
        }

        // Update display for initial panel
        UpdateAnimalInfo();
    }

    private void OnEnable()
    {
        AnimalOwnershipManager.OnAnimalUnlocked += OnAnimalUnlocked;
        CurrencyManager.OnCoinsChanged += OnCoinsChanged;
    }

    private void OnDisable()
    {
        AnimalOwnershipManager.OnAnimalUnlocked -= OnAnimalUnlocked;
        CurrencyManager.OnCoinsChanged -= OnCoinsChanged;
    }

    /// <summary>
    /// Xử lý khi panel thay đổi
    /// </summary>
    private void OnPanelChanged()
    {
        UpdateAnimalInfo();
    }

    /// <summary>
    /// Cập nhật thông tin động vật hiển thị
    /// </summary>
    private void UpdateAnimalInfo()
    {
        if (_simpleScrollSnap == null || _ownershipManager == null) return;

        // Lấy animal type từ panel hiện tại
        int selectedPanel = _simpleScrollSnap.SelectedPanel + 1;
        AnimalType currentAnimal = AnimalTypeExtensions.FromPanelIndex(selectedPanel);

        // Cập nhật tên
        if (_animalNameText != null)
        {
            _animalNameText.text = currentAnimal.GetDisplayName();
        }

        // Check unlock status
        bool isUnlocked = _ownershipManager.IsAnimalUnlocked(currentAnimal);

        // Cập nhật status text
        if (_statusText != null)
        {
            if (isUnlocked)
            {
                _statusText.text = "Unlocked";
                _statusText.color = Color.green;
            }
            else
            {
                _statusText.text = "Locked";
                _statusText.color = Color.red;
            }
        }

        // Cập nhật price display
        if (_priceDisplay != null)
        {
            _priceDisplay.SetActive(!isUnlocked);
        }

        if (_priceText != null && !isUnlocked)
        {
            int price = _ownershipManager.GetAnimalPrice(currentAnimal);
            _priceText.text = $"{price}";

            // Đổi màu nếu không đủ coin
            if (_currencyManager != null)
            {
                _priceText.color = _currencyManager.HasEnoughCoins(price) 
                    ? Color.white 
                    : Color.red;
            }
        }

        // Cập nhật button text
        UpdateSelectButton(currentAnimal, isUnlocked);
    }

    /// <summary>
    /// Cập nhật text của button Select
    /// </summary>
    private void UpdateSelectButton(AnimalType animal, bool isUnlocked)
    {
        if (_selectButton == null) return;

        TMP_Text buttonText = _selectButton.GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            if (isUnlocked)
            {
                buttonText.text = "SELECT";
            }
            else
            {
                int price = _ownershipManager.GetAnimalPrice(animal);
                buttonText.text = $"BUY ({price} COINS)";
            }
        }

        // Button luôn interactable (locked = mua, unlocked = chọn)
        _selectButton.interactable = true;
    }

    /// <summary>
    /// Xử lý khi nhấn nút Select/Buy
    /// </summary>
    private void OnSelectButtonClicked()
    {
        if (_simpleScrollSnap == null || _ownershipManager == null) return;

        int selectedPanel = _simpleScrollSnap.SelectedPanel + 1;
        AnimalType selectedAnimal = AnimalTypeExtensions.FromPanelIndex(selectedPanel);

        bool isUnlocked = _ownershipManager.IsAnimalUnlocked(selectedAnimal);

        if (isUnlocked)
        {
            // Đã unlock → Select và chuyển scene
            _ownershipManager.SelectAnimal(selectedAnimal);
            Debug.Log($"[AnimalSelector] Selected {selectedAnimal.GetDisplayName()}");
            
            // Play sound
            if (SoundController.GetInstance() != null)
            {
                SoundController.GetInstance().PlayAudio(AudioType.BUTTON_CLICK);
            }

            SceneManager.LoadScene("Level");
        }
        else
        {
            // Chưa unlock → Mua
            bool success = _ownershipManager.PurchaseAnimal(selectedAnimal);
            
            if (success)
            {
                Debug.Log($"[AnimalSelector] Purchased {selectedAnimal.GetDisplayName()}!");
                
                // Play sound
                if (SoundController.GetInstance() != null)
                {
                    SoundController.GetInstance().PlayAudio(AudioType.BUTTON_CLICK);
                }

                // Cập nhật UI
                UpdateAnimalInfo();
            }
            else
            {
                Debug.LogWarning($"[AnimalSelector] Cannot purchase {selectedAnimal.GetDisplayName()}");
                
                // TODO: Show "Not enough coins" popup
            }
        }
    }

    /// <summary>
    /// Event handler khi unlock động vật
    /// </summary>
    private void OnAnimalUnlocked(AnimalType type)
    {
        UpdateAnimalInfo();
    }

    /// <summary>
    /// Event handler khi coin thay đổi
    /// </summary>
    private void OnCoinsChanged(int newAmount)
    {
        UpdateAnimalInfo();
    }
}
