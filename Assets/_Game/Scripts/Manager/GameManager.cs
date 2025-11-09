using System;
using _Game.Scripts.Core;
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
    public static event Action      OnPlatformFreeze; // Freeze platform khi player miss
    public static event Action      OnPlatformResume; // Resume platform khi player landed checkpoint

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
        Piece.OnGameOver      += HandleMiss; // Đáp lệch mép → game over trực tiếp
        Piece.OnLastPieceExit += UpdateLastPos;
        Piece.OnGettingScore  += SetScore;
        Piece.OnSafeLanding   += SetCheckpoint; // Set checkpoint khi player landed safe

        BoundaryWall.OnBoundaryHit += HandleBoundaryCollision; // Player chạm boundary wall → game over trực tiếp
        FallDetector.OnPlayerFell += HandleMiss; // Player rơi xuống → game over trực tiếp

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
    /// → Game Over TRỰC TIẾP (không trừ health, không revive)
    /// </summary>
    private void HandleMiss()
    {
        Debug.Log("[GameManager] ☠️ Missed safe zone! Game Over!");
        GameEnd();
    }

    /// <summary>
    /// Xử lý khi player chạm boundary wall (ra khỏi view)
    /// → Game Over TRỰC TIẾP (không trừ health, không revive)
    /// </summary>
    private void HandleBoundaryCollision(Transform platform)
    {
        Debug.Log("[GameManager] ☠️ Hit boundary wall! Game Over!");
        GameEnd();
    }

    /// <summary>
    /// Xử lý khi player đạp vào bẫy (trap)
    /// Trừ 1 heart → Nếu hết heart thì Game Over
    /// Dùng cho các traps sẽ implement sau
    /// </summary>
    public void HandleTrapDamage()
    {
        if (_healthManager == null)
        {
            Debug.LogWarning("[GameManager] HealthManager not found! Skipping trap damage.");
            return;
        }

        // Trừ 1 heart
        bool stillAlive = _healthManager.LoseHealth(1);

        if (!stillAlive)
        {
            // Hết health → Game Over
            Debug.Log($"[GameManager] ☠️ No more hearts from traps! Game Over!");
            GameEnd();
        }
        else
        {
            Debug.Log($"[GameManager] 💔 Hit trap! Lost 1 heart. Remaining: {_healthManager.CurrentHealth}/{_healthManager.MaxHealth}");
            // Player tiếp tục chơi (không revive, chỉ mất health)
        }
    }

    /// <summary>
    /// [DEPRECATED] Revive player về center piece của platform (khi chạm boundary wall)
    /// Không còn dùng - giữ lại cho reference
    /// </summary>
    private void ReviveToCenterPiece(Transform platform)
    {
        if (platform == null)
        {
            Debug.LogError("[GameManager] ❌ Platform is null!");
            GameEnd();
            return;
        }

        // Lấy Platform component
        Platform platformScript = platform.GetComponent<Platform>();
        if (platformScript == null)
        {
            Debug.LogError("[GameManager] ❌ Platform component not found!");
            GameEnd();
            return;
        }

        // Lấy center piece
        Transform centerPiece = platformScript.GetCenterPiece();
        if (centerPiece == null)
        {
            Debug.LogError("[GameManager] ❌ Center piece not found!");
            GameEnd();
            return;
        }

        // Set center piece làm checkpoint mới
        CheckpointManager.GetInstance().SetCheckpoint(centerPiece);
        Debug.Log($"[GameManager] ✅ New checkpoint set to center piece: {centerPiece.name}");

        // Revive về center piece
        Vector3 centerPiecePos = centerPiece.position;
        Vector3 revivePos = centerPiecePos + Vector3.up * 1f; // Spawn 1 unit phía trên

        Debug.Log($"[GameManager] 🔄 Reviving to center piece at {revivePos}");

        // Set reviving flag để disable scoring
        Piece.IsReviving = true;

        // Revive player (sẽ rơi xuống center piece)
        _player.Revive(revivePos);

        // Platforms sẽ resume khi player landed (xử lý trong Piece.OnCollisionEnter)
    }

    /// <summary>
    /// Set checkpoint khi player landed safe
    /// </summary>
    private void SetCheckpoint(Transform piece)
    {
        CheckpointManager.GetInstance().SetCheckpoint(piece);
    }

    /// <summary>
    /// Resume platforms sau khi player landed checkpoint (được gọi từ Piece)
    /// </summary>
    public void ResumePlatformsFromRevival()
    {
        OnPlatformResume?.Invoke();
        Debug.Log("[GameManager] 🔓 Platforms RESUMED from revival");
    }

    /// <summary>
    /// [DEPRECATED] Revive player về checkpoint
    /// Không còn dùng - giữ lại cho reference
    /// </summary>
    private void ReviveToCheckpoint()
    {
        CheckpointManager checkpoint = CheckpointManager.GetInstance();

        if (!checkpoint.HasCheckpoint())
        {
            Debug.LogError("[GameManager] ❌ No checkpoint available!");
            GameEnd(); // Không có checkpoint → game over
            return;
        }

        Vector3 checkpointPos = checkpoint.GetCheckpointPosition();
        Vector3 revivePos = checkpointPos + Vector3.up * 1f; // Spawn 1 unit phía trên piece

        Debug.Log($"[GameManager] 🔄 Reviving to checkpoint at {revivePos}");

        // Set reviving flag để disable scoring
        Piece.IsReviving = true;

        // Revive player (sẽ rơi xuống piece)
        _player.Revive(revivePos);

        // Platforms sẽ resume khi player landed (xử lý trong Piece.OnCollisionEnter)
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

        // Set initial checkpoint to base piece
        Transform basePiece = _base.GetComponentInChildren<Piece>()?.transform;
        if (basePiece != null)
        {
            CheckpointManager.GetInstance().SetCheckpoint(basePiece);
            Debug.Log("[GameManager] ✅ Initial checkpoint set to base piece");
        }
        else
        {
            Debug.LogWarning("[GameManager] ⚠️ Base piece not found! No initial checkpoint.");
        }

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
        HealthManager.GetInstance()?.SetHealthToZero(); // Reset health để tránh mất heart khi restart
        OnGameEnd?.Invoke();

        if (this._player == null)
        {
            this._player = FindFirstObjectByType<PlayerBehaviour>();
        }
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

        SoundController.GetInstance().PlayAudio(AudioType.GAME_WIN); // TODO: Thay bằng WIN sound
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

        // Reset checkpoint
        CheckpointManager.GetInstance().ResetCheckpoint();

        // Reset reviving flag
        Piece.IsReviving = false;

        Debug.Log("[GameManager] 🔄 Game state reset");
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
        Piece.OnSafeLanding   -= SetCheckpoint;

        BoundaryWall.OnBoundaryHit -= HandleBoundaryCollision;

        PlayerBehaviour.OnPlayerDeath -= GameEnd;
        PlayerBehaviour.OnFirstJump   -= StartGameplay;

        if (_playerRenderer != null)
        {
            _playerRenderer.OnInvisible.RemoveListener(GameEnd);
        }
    }
}