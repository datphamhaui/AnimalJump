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
    [Tooltip("Text hiển thị tên động vật (ở dưới)")]
    [SerializeField] private TMP_Text _animalNameText;
    
    [Header("Lock/Unlock UI")]
    [Tooltip("Image khóa hiển thị khi animal bị lock (overlay trên animal model)")]
    [SerializeField] private Image _lockImage;
    
    [Tooltip("Text hiển thị số coin cần unlock (hiển thị ở giữa màn hình khi locked)")]
    [SerializeField] private TMP_Text _unlockCostText;

    [Tooltip("Button 'Unlock' màu đỏ (góc dưới phải)")]
    [SerializeField] private Button _unlockButton;
    
    [Header("Optional - Advanced")]
    [Tooltip("GameObject chứa unlock UI (bao gồm cost text, có thể ẩn/hiện cả nhóm)")]
    [SerializeField] private GameObject _unlockUIGroup;

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

        if (_unlockButton != null)
        {
            _unlockButton.onClick.AddListener(OnUnlockButtonClicked);
        }

        // Listen to panel selection events
        if (_simpleScrollSnap != null)
        {
            _simpleScrollSnap.OnPanelSelected.AddListener(OnPanelSelected);
            _simpleScrollSnap.OnPanelSelecting.AddListener(OnPanelSelecting);
        }

        // Initial update (event sẽ không trigger ngay trong Start)
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
    /// Xử lý khi panel đang được selecting (drag)
    /// </summary>
    private void OnPanelSelecting(int panelIndex)
    {
        UpdateAnimalInfo();
    }

    /// <summary>
    /// Xử lý khi panel đã được selected (button click hoặc snap)
    /// </summary>
    private void OnPanelSelected(int panelIndex)
    {
        Debug.Log($"[OnPanelSelected] panelIndex from event: {panelIndex}, SelectedPanel: {_simpleScrollSnap.SelectedPanel}, CenteredPanel: {_simpleScrollSnap.CenteredPanel}");
        
        // Dùng CenteredPanel vì nó được cập nhật TRƯỚC khi event invoke
        UpdateAnimalInfo(_simpleScrollSnap.CenteredPanel);
    }

    /// <summary>
    /// Cập nhật thông tin động vật hiển thị
    /// </summary>
    private void UpdateAnimalInfo(int? overridePanelIndex = null)
    {
        if (_simpleScrollSnap == null || _ownershipManager == null) return;

        // Lấy animal type từ panel hiện tại (hoặc override từ event)
        int selectedPanel = overridePanelIndex ?? _simpleScrollSnap.SelectedPanel;
        int panelIndexForAnimal = selectedPanel + 1;
        AnimalType currentAnimal = AnimalTypeExtensions.FromPanelIndex(panelIndexForAnimal);
        
        Debug.Log($"[UpdateAnimalInfo] selectedPanel: {selectedPanel}, panelIndexForAnimal: {panelIndexForAnimal}");

        // Cập nhật tên động vật (luôn hiển thị)
        if (_animalNameText != null)
        {
            _animalNameText.text = currentAnimal.GetDisplayName();
        }

        // Check unlock status
        bool isUnlocked = _ownershipManager.IsAnimalUnlocked(currentAnimal);
        int price = _ownershipManager.GetAnimalPrice(currentAnimal);

        // Cập nhật Lock Image (hiển thị khi locked)
        if (_lockImage != null)
        {
            _lockImage.gameObject.SetActive(!isUnlocked);
        }

        // Cập nhật Unlock Cost Text (hiển thị giá khi locked)
        if (_unlockCostText != null)
        {
            if (!isUnlocked)
            {
                _unlockCostText.gameObject.SetActive(true);
                _unlockCostText.text = $"{price}";
                
                // Đổi màu text dựa trên affordability
                if (_currencyManager != null)
                {
                    _unlockCostText.color = _currencyManager.HasEnoughCoins(price) 
                        ? Color.white 
                        : Color.red;
                }
            }
            else
            {
                _unlockCostText.gameObject.SetActive(false);
            }
        }

        // Cập nhật Unlock Button (chỉ hiển thị khi locked)
        if (_unlockButton != null)
        {
            _unlockButton.gameObject.SetActive(!isUnlocked);

            // Enable/disable button dựa trên số coin
            if (_currencyManager != null && !isUnlocked)
            {
                _unlockButton.interactable = _currencyManager.HasEnoughCoins(price);
            }
        }
        
        // Cập nhật Unlock UI Group (nếu có)
        if (_unlockUIGroup != null)
        {
            _unlockUIGroup.SetActive(!isUnlocked);
        }

        // Cập nhật Select Button (chỉ hiển thị khi unlocked)
        if (_selectButton != null)
        {
            _selectButton.gameObject.SetActive(isUnlocked);
        }
        
        // Debug log để kiểm tra
        Debug.Log($"[AnimalSelector] Animal: {currentAnimal}, Unlocked: {isUnlocked}, Price: {price}, Has enough coins: {_currencyManager?.HasEnoughCoins(price)}");
    }

    /// <summary>
    /// Xử lý khi nhấn nút Unlock
    /// </summary>
    private void OnUnlockButtonClicked()
    {
        if (_simpleScrollSnap == null || _ownershipManager == null || _currencyManager == null)
            return;

        // Lấy động vật hiện tại
        int selectedPanel = _simpleScrollSnap.SelectedPanel + 1;
        AnimalType currentAnimal = AnimalTypeExtensions.FromPanelIndex(selectedPanel);

        // Kiểm tra xem đã unlock chưa
        if (_ownershipManager.IsAnimalUnlocked(currentAnimal))
        {
            Debug.Log($"{currentAnimal} đã được unlock rồi!");
            return;
        }

        // Thử mua động vật
        bool purchaseSuccess = _ownershipManager.PurchaseAnimal(currentAnimal);

        if (purchaseSuccess)
        {
            Debug.Log($"Successfully unlocked {currentAnimal}!");
            
            // Cập nhật UI để phản ánh trạng thái mới
            UpdateAnimalInfo();
            
            // TODO: Có thể thêm animation/effect mua thành công ở đây
        }
        else
        {
            Debug.Log($"Not enough coins to unlock {currentAnimal}!");
            
            // TODO: Có thể hiển thị popup "Not enough coins" ở đây
        }
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
            // Không nên xảy ra vì Select button chỉ hiển thị khi unlocked
            Debug.LogWarning($"[AnimalSelector] Cannot select {selectedAnimal} - not unlocked!");
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
