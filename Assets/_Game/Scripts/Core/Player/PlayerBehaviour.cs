using System;
using UnityEngine;

/// <summary>
/// Quản lý hành vi chính của player - nhảy khi người dùng tap màn hình
/// </summary>
public class PlayerBehaviour : MonoBehaviour
{
    [Header("Animal Model Prefabs")]
    [Tooltip("Các model động vật, chọn dựa trên AnimalSelectionManager")]
    [SerializeField] private GameObject[] _animalModelPrefabs = new GameObject[4];

    private GameObject _spawnedModel;

    // Components
    private PlayerMovement  _playerMovement;
    private PlayerAnimation _playerAnimation;
    private PlayerCollision _playerCollision;

    // States
    private bool _isPlayerDeath = false;
    private bool _isFirstJump   = true;

    // Events
    public static event Action OnFirstJump;
    public static event Action OnPlayerDeath;

    private void Awake()
    {
        _playerMovement  = GetComponent<PlayerMovement>();
        _playerAnimation = GetComponent<PlayerAnimation>();
        _playerCollision = GetComponent<PlayerCollision>();

        SetupAnimalModel();

        // QUAN TRỌNG: Khởi tạo PlayerAnimation SAU KHI spawn model
        if (_playerAnimation != null)
        {
            _playerAnimation.Initialize();
        }
    }

    /// <summary>
    /// Spawn model động vật đã chọn làm con của Player
    /// </summary>
    private void SetupAnimalModel()
    {
        int selectedIndex = AnimalSelectionManager.GetSelectedAnimalIndex();

        // Panel index: 1-4, array index: 0-3
        int arrayIndex = selectedIndex - 1;

        if (arrayIndex >= 0 && arrayIndex < _animalModelPrefabs.Length && _animalModelPrefabs[arrayIndex] != null)
        {
            _spawnedModel                         = Instantiate(_animalModelPrefabs[arrayIndex], transform);
            _spawnedModel.transform.localPosition = Vector3.zero;
            _spawnedModel.transform.localRotation = Quaternion.identity;

            Debug.Log($"[PlayerBehaviour] Animal model spawned: {_animalModelPrefabs[arrayIndex].name}");
        }
        else
        {
            Debug.LogError($"[PlayerBehaviour] Invalid animal index: {arrayIndex}");
        }
    }

    /// <summary>
    /// Hồi sinh player tại vị trí mới
    /// </summary>
    public void Revive(Vector3 position)
    {
        Debug.Log($"[PlayerBehaviour] 🔄 Revive called at: {position}");

        // Reset movement (sẽ bật gravity để rơi xuống)
        _playerMovement.Revive(position);

        // KHÔNG cho phép jump ngay - delay 1 giây sau khi landed
        _playerCollision.DisableJumpTemporarily(1f);

        // Reset death state
        _isPlayerDeath = false;

        // Resume animation nếu bị dừng
        if (_playerAnimation != null)
        {
            _playerAnimation.ResumeAnimation();
        }

        Debug.Log("[PlayerBehaviour] ✅ Revived! Waiting for landing...");
    }

    /// <summary>
    /// Xử lý khi người dùng tap màn hình để nhảy
    /// </summary>
    public void OnJump()
    {
        // Không cho nhảy nếu đã chết hoặc không thể nhảy (đang trong không trung)
        if (_isPlayerDeath || !_playerCollision.CanJump)
        {
            Debug.Log("[Player] Cannot jump - Death or already jumping");

            return;
        }

        // Lần nhảy đầu tiên = bắt đầu game
        if (_isFirstJump)
        {
            _isFirstJump = false;
            OnFirstJump?.Invoke();
            Debug.Log("[Player] First jump - Game started!");
        }

        // Thực hiện nhảy
        _playerMovement.Jump();
        _playerAnimation.Jump();

        SoundController.GetInstance().PlayAudio(AudioType.JUMP);
    }

    /// <summary>
    /// Kết thúc game khi player chết
    /// </summary>
    public void GameOver()
    {
        if (_isPlayerDeath) return;

        _isPlayerDeath = true;

        // Bật physics để player rơi xuống
        if (_playerMovement == null)
        {
            _playerMovement = GetComponent<PlayerMovement>();
        }

        _playerMovement.EnablePhysicsOnGameOver();

        // Dừng animation hoặc chuyển sang animation chết
        // _playerAnimation.StopAnimation(); // Uncomment nếu muốn dừng animation

        OnPlayerDeath?.Invoke();

        Debug.Log("[Player] Game Over!");
    }
}