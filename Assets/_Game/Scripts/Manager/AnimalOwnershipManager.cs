using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý việc sở hữu (unlock/lock) các động vật
/// Singleton pattern, persist qua các scene
/// </summary>
public class AnimalOwnershipManager : Singleton<AnimalOwnershipManager>
{
    [Header("Animal Database")]
    [SerializeField] private AnimalData[] _animalDatabase = new AnimalData[4];

    private const string ANIMAL_UNLOCK_KEY = "AnimalUnlock_";
    
    // Events
    public static event Action<AnimalType> OnAnimalUnlocked;
    public static event Action<AnimalType> OnAnimalSelected;

    protected override void Awake()
    {
        base.Awake();
        InitializeDefaultUnlocks();
    }

    #region Initialization

    /// <summary>
    /// Khởi tạo unlock mặc định (Kangaroo)
    /// </summary>
    private void InitializeDefaultUnlocks()
    {
        // Đảm bảo Kangaroo luôn được unlock
        if (!IsAnimalUnlocked(AnimalType.Kangaroo))
        {
            UnlockAnimal(AnimalType.Kangaroo, false); // false = không trigger event
            Debug.Log("[AnimalOwnershipManager] Kangaroo unlocked by default");
        }
    }

    #endregion

    #region Animal Unlock/Lock

    /// <summary>
    /// Kiểm tra động vật đã unlock chưa
    /// </summary>
    public bool IsAnimalUnlocked(AnimalType type)
    {
        return PlayerPrefs.GetInt(ANIMAL_UNLOCK_KEY + type.ToString(), 0) == 1;
    }

    /// <summary>
    /// Unlock một động vật (dùng khi mua hoặc reward)
    /// </summary>
    public void UnlockAnimal(AnimalType type, bool triggerEvent = true)
    {
        if (IsAnimalUnlocked(type))
        {
            Debug.LogWarning($"[AnimalOwnershipManager] {type} is already unlocked!");
            return;
        }

        PlayerPrefs.SetInt(ANIMAL_UNLOCK_KEY + type.ToString(), 1);
        PlayerPrefs.Save();

        Debug.Log($"[AnimalOwnershipManager] ✅ Unlocked: {type.GetDisplayName()}");

        if (triggerEvent)
        {
            OnAnimalUnlocked?.Invoke(type);
        }
    }

    /// <summary>
    /// Mua động vật bằng coin
    /// </summary>
    public bool PurchaseAnimal(AnimalType type)
    {
        // Check đã unlock chưa
        if (IsAnimalUnlocked(type))
        {
            Debug.LogWarning($"[AnimalOwnershipManager] {type} is already unlocked!");
            return false;
        }

        // Lấy giá
        int price = GetAnimalPrice(type);

        // Check coin
        CurrencyManager currencyManager = CurrencyManager.GetInstance();
        if (currencyManager == null)
        {
            Debug.LogError("[AnimalOwnershipManager] CurrencyManager not found!");
            return false;
        }

        if (!currencyManager.HasEnoughCoins(price))
        {
            Debug.LogWarning($"[AnimalOwnershipManager] Not enough coins to buy {type}! Need: {price}, Have: {currencyManager.Coins}");
            return false;
        }

        // Trừ coin
        if (!currencyManager.SpendCoins(price))
        {
            return false;
        }

        // Unlock động vật
        UnlockAnimal(type, true);

        Debug.Log($"[AnimalOwnershipManager] 🎉 Purchased {type.GetDisplayName()} for {price} coins!");
        
        return true;
    }

    #endregion

    #region Animal Data

    /// <summary>
    /// Lấy giá của động vật
    /// </summary>
    public int GetAnimalPrice(AnimalType type)
    {
        // Tìm trong database trước
        AnimalData data = GetAnimalData(type);
        if (data != null)
        {
            return data.price;
        }

        // Fallback: dùng default price
        return type.GetDefaultPrice();
    }

    /// <summary>
    /// Lấy AnimalData từ database
    /// </summary>
    public AnimalData GetAnimalData(AnimalType type)
    {
        if (_animalDatabase == null || _animalDatabase.Length == 0)
        {
            return null;
        }

        foreach (var data in _animalDatabase)
        {
            if (data != null && data.animalType == type)
            {
                return data;
            }
        }

        return null;
    }

    /// <summary>
    /// Lấy danh sách tất cả động vật
    /// </summary>
    public AnimalType[] GetAllAnimals()
    {
        return (AnimalType[])Enum.GetValues(typeof(AnimalType));
    }

    /// <summary>
    /// Lấy danh sách động vật đã unlock
    /// </summary>
    public List<AnimalType> GetUnlockedAnimals()
    {
        List<AnimalType> unlockedAnimals = new List<AnimalType>();
        
        foreach (AnimalType type in GetAllAnimals())
        {
            if (IsAnimalUnlocked(type))
            {
                unlockedAnimals.Add(type);
            }
        }

        return unlockedAnimals;
    }

    /// <summary>
    /// Lấy danh sách động vật chưa unlock
    /// </summary>
    public List<AnimalType> GetLockedAnimals()
    {
        List<AnimalType> lockedAnimals = new List<AnimalType>();
        
        foreach (AnimalType type in GetAllAnimals())
        {
            if (!IsAnimalUnlocked(type))
            {
                lockedAnimals.Add(type);
            }
        }

        return lockedAnimals;
    }

    #endregion

    #region Selection

    /// <summary>
    /// Chọn động vật (phải đã unlock)
    /// </summary>
    public bool SelectAnimal(AnimalType type)
    {
        if (!IsAnimalUnlocked(type))
        {
            Debug.LogWarning($"[AnimalOwnershipManager] Cannot select locked animal: {type}");
            return false;
        }

        int panelIndex = type.ToPanelIndex();
        AnimalSelectionManager.SaveSelectedAnimal(panelIndex);

        Debug.Log($"[AnimalOwnershipManager] Selected: {type.GetDisplayName()}");

        OnAnimalSelected?.Invoke(type);
        
        return true;
    }

    /// <summary>
    /// Lấy động vật hiện tại đã chọn
    /// </summary>
    public AnimalType GetSelectedAnimal()
    {
        return AnimalSelectionManager.GetSelectedAnimal();
    }

    #endregion

    #region Debug

    /// <summary>
    /// Unlock tất cả động vật (debug)
    /// </summary>
    [ContextMenu("Debug: Unlock All Animals")]
    public void DebugUnlockAllAnimals()
    {
        foreach (AnimalType type in GetAllAnimals())
        {
            UnlockAnimal(type, false);
        }
        Debug.Log("[AnimalOwnershipManager] ✅ All animals unlocked!");
    }

    /// <summary>
    /// Lock tất cả động vật trừ Kangaroo (debug)
    /// </summary>
    [ContextMenu("Debug: Reset All Animals")]
    public void DebugResetAllAnimals()
    {
        foreach (AnimalType type in GetAllAnimals())
        {
            PlayerPrefs.DeleteKey(ANIMAL_UNLOCK_KEY + type.ToString());
        }
        PlayerPrefs.Save();

        // Re-unlock Kangaroo
        InitializeDefaultUnlocks();

        Debug.Log("[AnimalOwnershipManager] 🔄 All animals reset (only Kangaroo unlocked)!");
    }

    /// <summary>
    /// Log trạng thái unlock (debug)
    /// </summary>
    [ContextMenu("Debug: Log Animal Status")]
    public void DebugLogAnimalStatus()
    {
        Debug.Log("=== ANIMAL OWNERSHIP STATUS ===");
        foreach (AnimalType type in GetAllAnimals())
        {
            bool unlocked = IsAnimalUnlocked(type);
            int price = GetAnimalPrice(type);
            Debug.Log($"{type.GetDisplayName()}: {(unlocked ? "✅ Unlocked" : $"🔒 Locked ({price} coins)")}");
        }
        Debug.Log("==============================");
    }

    #endregion
}
