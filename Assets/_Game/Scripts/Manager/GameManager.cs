using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Object References :")]
    [SerializeField] Transform _base;
    [SerializeField] PlayerBehaviour _player;
    [SerializeField] PlayerRenderer  _playerRenderer;

    float _lastZpos   = 0;
    bool  _isGameOver = false;
    bool  _isRevive   = false;
    bool  _isGameWon  = false;

    MenuManager  _menuController;
    ScoreManager _scoreManager;
    LevelManager _levelManager;
    LevelProgressManager _levelProgressManager;
    HealthManager _healthManager;

    public static event Action      OnGameEnd;
    public static event Action      OnRevive;
    public static event Action<int> OnScoreUpdated;
    public static event Action<int> OnGameWin; // stars earned

    private void Awake()
    {
        _scoreManager = GetComponent<ScoreManager>();
        _levelManager = GetComponent<LevelManager>();

        // Nếu không có LevelManager trong cùng GameObject, tìm trong scene
        if (_levelManager == null)
        {
            _levelManager = FindFirstObjectByType<LevelManager>();
        }

        if (_levelManager == null)
        {
            Debug.LogError("[GameManager] LevelManager not found! Please add LevelManager component.");
        }

        // Lấy các managers
        _levelProgressManager = LevelProgressManager.GetInstance();
        _healthManager = HealthManager.GetInstance();
    }

    private void OnEnable()
    {
        Piece.OnGameOver      += HandleMiss; // Đáp lệch mép → mất health
        Piece.OnLastPieceExit += UpdateLastPos;
        Piece.OnGettingScore  += SetScore;

        PlayerBehaviour.OnPlayerDeath += GameEnd;
        PlayerBehaviour.OnFirstJump   += StartGameplay;
        
        if (_playerRenderer != null)
        {
            _playerRenderer.OnInvisible.AddListener(GameEnd);
        }
    }

    private void SetScore(int val)
    {
        _scoreManager.AddScore(val);
        OnScoreUpdated?.Invoke(_scoreManager.Score);

        // Check win condition
        CheckWinCondition();
    }

    /// <summary>
    /// Xử lý khi player đáp lệch mép (miss)
    /// Mất 1 heart, nếu hết heart → Game Over
    /// </summary>
    private void HandleMiss()
    {
        if (_healthManager == null) 
        {
            Debug.LogError("[GameManager] HealthManager not found!");
            GameEnd(); // Fallback: game over nếu không có health system
            return;
        }

        // Mất 1 heart
        bool stillAlive = _healthManager.LoseHealth(1);

        if (!stillAlive)
        {
            // Hết health → Game Over
            Debug.Log($"[GameManager] ☠️ No more hearts! Game Over!");
            GameEnd();
        }
        else
        {
            // Còn health → Tiếp tục chơi
            Debug.Log($"[GameManager] 💔 Lost 1 heart! Remaining: {_healthManager.CurrentHealth}/{_healthManager.MaxHealth}");
            
            // TODO: Có thể thêm visual feedback (shake camera, flash screen red, etc.)
        }
    }

    /// <summary>
    /// Kiểm tra điều kiện thắng
    /// </summary>
    private void CheckWinCondition()
    {
        if (_isGameWon || _isGameOver) return;

        LevelDataSO currentLevelData = _levelManager.GetCurrentLevelData();
        if (currentLevelData == null) return;

        // Kiểm tra nếu đạt đủ điểm target
        if (_scoreManager.Score >= currentLevelData.targetScore)
        {
            GameWin();
        }
    }

    private void Start() 
    { 
        _menuController = MenuManager.GetInstance(); 
        
        // Chuyển sang nhạc game khi vào scene game
        if (SoundController.GetInstance() != null)
        {
            SoundController.GetInstance().SwitchToGameMusic();
        }
    }

    public void GameEnd()
    {
        if (_isGameOver || _isGameWon) return;
        _isGameOver = true;

        OnGameEnd?.Invoke();

        _player.GameOver();

        // Show Lose menu instead of Revive menu
        _menuController.SwitchMenu(MenuType.Lose);

        SoundController.GetInstance().PlayAudio(AudioType.GAMEOVER);
        
        Debug.Log("[GameManager] 💀 GAME OVER - Showing Lose Menu");
    }

    /// <summary>
    /// Xử lý khi người chơi thắng level
    /// </summary>
    private void GameWin()
    {
        if (_isGameWon || _isGameOver) return;
        _isGameWon = true;

        Debug.Log("[GameManager] 🎉 LEVEL COMPLETED!");

        // Tính số sao
        int stars = CalculateStars();
        Debug.Log($"[GameManager] ⭐ Stars earned: {stars}");

        // Lưu tiến độ
        int currentLevel = _levelProgressManager.GetCurrentLevel();
        _levelProgressManager.CompleteLevel(currentLevel, stars);

        // TODO: Add coin reward khi implement currency system
        // LevelDataSO levelData = _levelManager.GetCurrentLevelData();
        // CurrencyManager.AddCoins(levelData.coinReward);

        // Dừng game
        _player.GameOver();
        OnGameEnd?.Invoke();

        // Trigger event
        OnGameWin?.Invoke(stars);

        // Hiện Win menu
        _menuController.SwitchMenu(MenuType.Win);

        SoundController.GetInstance().PlayAudio(AudioType.GAMEOVER); // TODO: Thay bằng WIN sound
    }

    /// <summary>
    /// Tính số sao dựa trên số heart đã dùng
    /// - 0 heart used = 3 sao (perfect)
    /// - 1 heart used = 2 sao
    /// - 2+ hearts used = 1 sao
    /// </summary>
    private int CalculateStars()
    {
        if (_healthManager == null) return 1;
        
        return _healthManager.CalculateStars();
    }

    public void Revive()
    {
        _isGameOver = false;
        _isGameWon = false;
        _isRevive = true;
        OnRevive?.Invoke();

        _menuController.SwitchMenu(MenuType.Gameplay);

        Vector3 revivePosition = Vector3.forward * _lastZpos;

        _base.position = revivePosition;
        _player.Revive(revivePosition + Vector3.up);
    }

    /// <summary>
    /// Reset game state khi bắt đầu level mới
    /// </summary>
    public void ResetGameState()
    {
        _isGameOver = false;
        _isGameWon = false;
        _isRevive = false;
        _lastZpos = 0;
        
        // Reset health về 3 hearts
        if (_healthManager != null)
        {
            _healthManager.ResetHealth();
        }
    }

    private void UpdateLastPos(Vector3 lastPos) { _lastZpos = lastPos.z; }

    public void StartGameplay()
    {
        if (_menuController.GetCurrentMenu != MenuType.Gameplay)
        {
            _menuController.SwitchMenu(MenuType.Gameplay);
        }
    }

    private void OnDisable()
    {
        Piece.OnGameOver      -= HandleMiss;
        Piece.OnLastPieceExit -= UpdateLastPos;
        Piece.OnGettingScore  -= SetScore;

        PlayerBehaviour.OnPlayerDeath -= GameEnd;
        PlayerBehaviour.OnFirstJump   -= StartGameplay;
        
        if (_playerRenderer != null)
        {
            _playerRenderer.OnInvisible.RemoveListener(GameEnd);
        }
    }
}