using System;
using UnityEngine;

/// <summary>
/// Quản lý hệ thống tiền tệ (Coin) trong game
/// Singleton pattern, persist qua các scene
/// </summary>
public class CurrencyManager : Singleton<CurrencyManager>
{
    private const string COIN_KEY = "PlayerCoins";
    private const int DEFAULT_COINS = 0;

    private int _currentCoins;

    // Events
    public static event Action<int> OnCoinsChanged; // Trigger khi số coin thay đổi
    public static event Action<int, int> OnCoinsAdded; // amount, newTotal
    public static event Action<int, int> OnCoinsSpent; // amount, newTotal

    protected override void Awake()
    {
        base.Awake();
        LoadCoins();
    }

    #region Coin Management

    /// <summary>
    /// Lấy số coin hiện tại
    /// </summary>
    public int Coins => _currentCoins;

    /// <summary>
    /// Thêm coin
    /// </summary>
    public void AddCoins(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"[CurrencyManager] Cannot add negative or zero coins: {amount}");
            return;
        }

        _currentCoins += amount;
        SaveCoins();

        Debug.Log($"[CurrencyManager] +{amount} coins → Total: {_currentCoins}");

        OnCoinsAdded?.Invoke(amount, _currentCoins);
        OnCoinsChanged?.Invoke(_currentCoins);
    }

    /// <summary>
    /// Trừ coin (mua item)
    /// </summary>
    public bool SpendCoins(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"[CurrencyManager] Cannot spend negative or zero coins: {amount}");
            return false;
        }

        if (_currentCoins < amount)
        {
            Debug.LogWarning($"[CurrencyManager] Not enough coins! Need: {amount}, Have: {_currentCoins}");
            return false;
        }

        _currentCoins -= amount;
        SaveCoins();

        Debug.Log($"[CurrencyManager] -{amount} coins → Remaining: {_currentCoins}");

        OnCoinsSpent?.Invoke(amount, _currentCoins);
        OnCoinsChanged?.Invoke(_currentCoins);

        return true;
    }

    /// <summary>
    /// Kiểm tra có đủ coin không
    /// </summary>
    public bool HasEnoughCoins(int amount)
    {
        return _currentCoins >= amount;
    }

    /// <summary>
    /// Set coin trực tiếp (dùng cho debug hoặc special cases)
    /// </summary>
    public void SetCoins(int amount)
    {
        _currentCoins = Mathf.Max(0, amount);
        SaveCoins();
        
        Debug.Log($"[CurrencyManager] Coins set to: {_currentCoins}");
        
        OnCoinsChanged?.Invoke(_currentCoins);
    }

    #endregion

    #region Save/Load

    private void LoadCoins()
    {
        _currentCoins = PlayerPrefs.GetInt(COIN_KEY, DEFAULT_COINS);
        Debug.Log($"[CurrencyManager] Loaded coins: {_currentCoins}");
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt(COIN_KEY, _currentCoins);
        PlayerPrefs.Save();
    }

    #endregion

    #region Debug

    /// <summary>
    /// Reset tất cả coin (debug)
    /// </summary>
    [ContextMenu("Debug: Reset Coins")]
    public void ResetCoins()
    {
        SetCoins(DEFAULT_COINS);
        Debug.Log("[CurrencyManager] Coins reset to 0!");
    }

    /// <summary>
    /// Add 1000 coins (debug)
    /// </summary>
    [ContextMenu("Debug: Add 1000 Coins")]
    public void DebugAdd1000Coins()
    {
        AddCoins(1000);
    }

    /// <summary>
    /// Add 10000 coins (debug)
    /// </summary>
    [ContextMenu("Debug: Add 10000 Coins")]
    public void DebugAdd10000Coins()
    {
        AddCoins(10000);
    }

    /// <summary>
    /// Log coin status
    /// </summary>
    [ContextMenu("Debug: Log Coins")]
    public void LogCoins()
    {
        Debug.Log($"[CurrencyManager] Current Coins: {_currentCoins}");
    }

    #endregion
}
