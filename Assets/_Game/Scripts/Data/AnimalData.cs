using System;
using UnityEngine;

/// <summary>
/// Data class cho mỗi loại động vật
/// Chứa thông tin: tên, giá mua, icon, prefab
/// </summary>
[Serializable]
public class AnimalData
{
    [Header("Basic Info")]
    public AnimalType animalType;
    public string displayName;
    
    [Header("Shop Info")]
    [Tooltip("Giá mua động vật này (coin). 0 = free/default")]
    public int price;
    
    [Header("Visual (Optional)")]
    [Tooltip("Icon hiển thị trong shop")]
    public Sprite icon;
    
    [Tooltip("Prefab model của động vật (reference only)")]
    public GameObject modelPrefab;
}

/// <summary>
/// Extension methods cho AnimalType enum
/// </summary>
public static class AnimalTypeExtensions
{
    /// <summary>
    /// Lấy display name của động vật
    /// </summary>
    public static string GetDisplayName(this AnimalType type)
    {
        return type switch
        {
            AnimalType.Kangaroo => "Kangaroo",
            AnimalType.Elephant => "Elephant",
            AnimalType.Lion => "Lion",
            AnimalType.Bear => "Bear",
            _ => type.ToString()
        };
    }

    /// <summary>
    /// Lấy giá mặc định của động vật
    /// </summary>
    public static int GetDefaultPrice(this AnimalType type)
    {
        return type switch
        {
            AnimalType.Kangaroo => 0,      // Free (default)
            AnimalType.Elephant => 500,    // 500 coins
            AnimalType.Lion => 1000,       // 1000 coins
            AnimalType.Bear => 1500,       // 1500 coins
            _ => 0
        };
    }

    /// <summary>
    /// Lấy description của động vật
    /// </summary>
    public static string GetDescription(this AnimalType type)
    {
        return type switch
        {
            AnimalType.Kangaroo => "The default jumper!",
            AnimalType.Elephant => "Strong and steady.",
            AnimalType.Lion => "King of the jungle.",
            AnimalType.Bear => "Powerful and brave.",
            _ => ""
        };
    }

    /// <summary>
    /// Convert panel index (1-4) sang AnimalType
    /// </summary>
    public static AnimalType FromPanelIndex(int panelIndex)
    {
        return (AnimalType)(panelIndex - 1);
    }

    /// <summary>
    /// Convert AnimalType sang panel index (1-4)
    /// </summary>
    public static int ToPanelIndex(this AnimalType type)
    {
        return (int)type + 1;
    }
}
