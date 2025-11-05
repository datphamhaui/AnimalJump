using System;
using UnityEngine;

/// <summary>
/// Quản lý health/heart của player
/// Max 3 hearts, mất heart khi đáp lệch mép
/// </summary>
public class HealthManager : Singleton<HealthManager>
{
    [Header("Health Settings")]
    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private int _startingHealth = 3;

    private int _currentHealth;
    private int _healthUsed = 0; // Số heart đã mất trong lần chơi này (dùng để tính sao)

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    public int HealthUsed => _healthUsed;

    public static event Action<int> OnHealthChanged; // current health
    public static event Action OnHealthAdded;
    public static event Action OnHealthLost;
    public static event Action OnPlayerDied; // Hết health

    protected override void Awake()
    {
        base.Awake();
        _currentHealth = _startingHealth;
    }

    private void OnEnable()
    {
        GameManager.OnRevive += ResetHealth;
    }

    private void OnDisable()
    {
        GameManager.OnRevive -= ResetHealth;
    }

    /// <summary>
    /// Thêm health (từ heart pickup)
    /// </summary>
    /// <returns>True nếu thêm thành công, False nếu đã max</returns>
    public bool AddHealth(int amount)
    {
        if (_currentHealth >= _maxHealth)
        {
            Debug.Log($"[HealthManager] Already at max health ({_maxHealth})");
            return false;
        }

        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
        
        Debug.Log($"[HealthManager] ❤️ +{amount} health! Current: {_currentHealth}/{_maxHealth}");
        
        OnHealthAdded?.Invoke();
        OnHealthChanged?.Invoke(_currentHealth);
        
        return true;
    }

    /// <summary>
    /// Mất health (khi đáp lệch mép)
    /// </summary>
    /// <returns>True nếu player còn sống, False nếu đã hết health</returns>
    public bool LoseHealth(int amount)
    {
        _currentHealth = Mathf.Max(_currentHealth - amount, 0);
        _healthUsed += amount;

        Debug.Log($"[HealthManager] 💔 -{amount} health! Current: {_currentHealth}/{_maxHealth} (Used: {_healthUsed})");

        OnHealthLost?.Invoke();
        OnHealthChanged?.Invoke(_currentHealth);

        if (_currentHealth <= 0)
        {
            Debug.Log($"[HealthManager] ☠️ Player died! Total hearts used: {_healthUsed}");
            OnPlayerDied?.Invoke();
            return false; // Player chết
        }

        return true; // Player còn sống
    }

    /// <summary>
    /// Reset health về giá trị ban đầu (khi revive hoặc restart)
    /// </summary>
    public void ResetHealth()
    {
        _currentHealth = _startingHealth;
        _healthUsed = 0;
        
        Debug.Log($"[HealthManager] 🔄 Health reset to {_currentHealth}/{_maxHealth}");
        
        OnHealthChanged?.Invoke(_currentHealth);
    }

    /// <summary>
    /// Tính số sao dựa trên số heart đã dùng
    /// - 0 heart used = 3 sao (perfect)
    /// - 1 heart used = 2 sao
    /// - 2+ hearts used = 1 sao
    /// </summary>
    public int CalculateStars()
    {
        if (_healthUsed == 0) return 3;
        if (_healthUsed == 1) return 2;
        return 1;
    }

    #region Debug Methods

    [ContextMenu("Add 1 Heart")]
    private void Debug_AddHeart()
    {
        AddHealth(1);
    }

    [ContextMenu("Lose 1 Heart")]
    private void Debug_LoseHeart()
    {
        LoseHealth(1);
    }

    [ContextMenu("Reset Health")]
    private void Debug_ResetHealth()
    {
        ResetHealth();
    }

    [ContextMenu("Log Health Status")]
    private void Debug_LogStatus()
    {
        Debug.Log($"=== HEALTH STATUS ===");
        Debug.Log($"Current: {_currentHealth}/{_maxHealth}");
        Debug.Log($"Health Used: {_healthUsed}");
        Debug.Log($"Stars: {CalculateStars()}");
        Debug.Log($"====================");
    }

    #endregion
}
