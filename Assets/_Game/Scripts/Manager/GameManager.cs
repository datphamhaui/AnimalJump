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
        Piece.OnGameOver      += HandleMiss; // Đáp lệch mép → mất health
        Piece.OnLastPieceExit += UpdateLastPos;
        Piece.OnGettingScore  += SetScore;
        Piece.OnSafeLanding   += SetCheckpoint; // Set checkpoint khi player landed safe

        BoundaryWall.OnBoundaryHit += HandleBoundaryCollision; // Player chạm boundary wall

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
    /// Freeze platforms → Mất 1 heart → Revive về checkpoint hoặc Game Over
    /// </summary>
    private void HandleMiss()
    {
        if (_healthManager == null)
        {
            Debug.LogError("[GameManager] HealthManager not found!");
            GameEnd(); // Fallback: game over nếu không có health system
            return;
        }

        // Freeze platforms NGAY
        OnPlatformFreeze?.Invoke();
        Debug.Log("[GameManager] 🧊 Platforms FROZEN");

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
            // Còn health → Revive về checkpoint
            Debug.Log($"[GameManager] 💔 Lost 1 heart! Remaining: {_healthManager.CurrentHealth}/{_healthManager.MaxHealth}");
            ReviveToCheckpoint();
        }
    }

    /// <summary>
    /// Xử lý khi player chạm boundary wall (ra khỏi view)
    /// Freeze platforms → Mất 1 heart → Revive về center piece của platform hiện tại
    /// </summary>
    private void HandleBoundaryCollision(Transform platform)
    {
        if (_healthManager == null)
        {
            Debug.LogError("[GameManager] HealthManager not found!");
            GameEnd();
            return;
        }

        // Freeze platforms NGAY
        OnPlatformFreeze?.Invoke();
        Debug.Log("[GameManager] 🧊 Platforms FROZEN (Boundary hit)");

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
            // Còn health → Revive về center piece của platform
            Debug.Log($"[GameManager] 💔 Lost 1 heart (Boundary)! Remaining: {_healthManager.CurrentHealth}/{_healthManager.MaxHealth}");
            ReviveToCenterPiece(platform);
        }
    }

    /// <summary>
    /// Revive player về center piece của platform (khi chạm boundary wall)
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
    /// Revive player về checkpoint
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