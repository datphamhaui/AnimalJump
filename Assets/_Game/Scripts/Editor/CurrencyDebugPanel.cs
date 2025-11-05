using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Debug panel để test Currency & Animal Ownership system
/// Thêm vào scene để dễ dàng test các tính năng
/// </summary>
public class CurrencyDebugPanel : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] private TMP_Text _infoText;

    [Header("Buttons")]
    [SerializeField] private Button _add100Button;
    [SerializeField] private Button _add1000Button;
    [SerializeField] private Button _resetCoinsButton;
    [SerializeField] private Button _unlockAllAnimalsButton;
    [SerializeField] private Button _resetAnimalsButton;
    [SerializeField] private Button _logStatusButton;

    private CurrencyManager _currencyManager;
    private AnimalOwnershipManager _ownershipManager;

    private void Start()
    {
        _currencyManager = CurrencyManager.GetInstance();
        _ownershipManager = AnimalOwnershipManager.GetInstance();

        // Setup buttons
        if (_add100Button) _add100Button.onClick.AddListener(() => AddCoins(100));
        if (_add1000Button) _add1000Button.onClick.AddListener(() => AddCoins(1000));
        if (_resetCoinsButton) _resetCoinsButton.onClick.AddListener(ResetCoins);
        if (_unlockAllAnimalsButton) _unlockAllAnimalsButton.onClick.AddListener(UnlockAllAnimals);
        if (_resetAnimalsButton) _resetAnimalsButton.onClick.AddListener(ResetAnimals);
        if (_logStatusButton) _logStatusButton.onClick.AddListener(LogStatus);

        UpdateInfo();
    }

    private void OnEnable()
    {
        CurrencyManager.OnCoinsChanged += OnCoinsChanged;
        AnimalOwnershipManager.OnAnimalUnlocked += OnAnimalUnlocked;
    }

    private void OnDisable()
    {
        CurrencyManager.OnCoinsChanged -= OnCoinsChanged;
        AnimalOwnershipManager.OnAnimalUnlocked -= OnAnimalUnlocked;
    }

    private void AddCoins(int amount)
    {
        if (_currencyManager != null)
        {
            _currencyManager.AddCoins(amount);
            Debug.Log($"[Debug] Added {amount} coins");
        }
    }

    private void ResetCoins()
    {
        if (_currencyManager != null)
        {
            _currencyManager.ResetCoins();
            Debug.Log("[Debug] Coins reset to 0");
        }
    }

    private void UnlockAllAnimals()
    {
        if (_ownershipManager != null)
        {
            _ownershipManager.DebugUnlockAllAnimals();
            Debug.Log("[Debug] All animals unlocked");
        }
    }

    private void ResetAnimals()
    {
        if (_ownershipManager != null)
        {
            _ownershipManager.DebugResetAllAnimals();
            Debug.Log("[Debug] Animals reset (only Kangaroo unlocked)");
        }
    }

    private void LogStatus()
    {
        if (_currencyManager != null)
        {
            _currencyManager.LogCoins();
        }

        if (_ownershipManager != null)
        {
            _ownershipManager.DebugLogAnimalStatus();
        }
    }

    private void OnCoinsChanged(int newAmount)
    {
        UpdateInfo();
    }

    private void OnAnimalUnlocked(AnimalType type)
    {
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        if (_infoText == null) return;

        string info = "=== CURRENCY & ANIMALS ===\n\n";

        // Coins
        if (_currencyManager != null)
        {
            info += $"💰 Coins: {_currencyManager.Coins}\n\n";
        }

        // Animals
        if (_ownershipManager != null)
        {
            info += "🦘 ANIMALS:\n";
            foreach (AnimalType type in _ownershipManager.GetAllAnimals())
            {
                bool unlocked = _ownershipManager.IsAnimalUnlocked(type);
                int price = _ownershipManager.GetAnimalPrice(type);
                string status = unlocked ? "✅ Unlocked" : $"🔒 Locked ({price} coins)";
                info += $"  {type.GetDisplayName()}: {status}\n";
            }

            // Selected animal
            AnimalType selected = _ownershipManager.GetSelectedAnimal();
            info += $"\n🎯 Selected: {selected.GetDisplayName()}";
        }

        _infoText.text = info;
    }

    /// <summary>
    /// Update thông tin mỗi giây (để theo dõi real-time)
    /// </summary>
    private void Update()
    {
        // Update mỗi 1 giây
        if (Time.frameCount % 60 == 0)
        {
            UpdateInfo();
        }
    }
}
