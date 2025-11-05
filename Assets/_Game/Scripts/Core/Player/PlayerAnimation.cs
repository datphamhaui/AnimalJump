using UnityEngine;

/// <summary>
/// Quản lý animation của player dựa trên động vật đã chọn
/// Controller phải có: Idle state (default), Jump trigger
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    [Header("Animal Animator Controllers")]
    [Tooltip("Các Animator Controller cho từng loại động vật (1-4)")]
    [SerializeField] private RuntimeAnimatorController[] _animalAnimators = new RuntimeAnimatorController[4];

    [Header("Animation Settings")]
    [Tooltip("Tốc độ animation jump (1 = bình thường, 2 = nhanh gấp đôi)")]
    [SerializeField] private float _jumpAnimationSpeed = 1.5f;

    private Animator _anim;
    private float _defaultAnimSpeed = 1f;
    private bool _isInitialized = false;

    private static readonly int JUMP_TRIGGER = Animator.StringToHash("Jump");

    /// <summary>
    /// Khởi tạo PlayerAnimation - PHẢI được gọi từ PlayerBehaviour sau khi spawn model
    /// </summary>
    public void Initialize()
    {
        if (_isInitialized) return;

        // Tìm Animator trong children (model vừa spawn)
        // includeInactive = true để tìm cả khi model đang bị disable
        _anim = GetComponentInChildren<Animator>(true);

        if (_anim == null)
        {
            Debug.LogError("[PlayerAnimation] Animator not found in children! Make sure animal model has Animator component.");
            return;
        }

        // QUAN TRỌNG: Nếu tìm thấy Animator ở chính Player GameObject thì bỏ qua
        if (_anim.gameObject == gameObject)
        {
            Debug.LogError("[PlayerAnimation] Animator found on Player itself, not on model! Please remove Animator from Player GameObject.");
            _anim = null;
            return;
        }

        Debug.Log($"[PlayerAnimation] Found Animator in child: {_anim.gameObject.name}");

        SetupAnimatorController();
        _defaultAnimSpeed = _anim.speed;
        _isInitialized = true;
    }

    /// <summary>
    /// Gán Animator Controller dựa trên động vật đã chọn
    /// </summary>
    private void SetupAnimatorController()
    {
        int selectedIndex = AnimalSelectionManager.GetSelectedAnimalIndex();
        int arrayIndex = selectedIndex - 1; // Panel index: 1-4, array index: 0-3

        if (arrayIndex >= 0 && arrayIndex < _animalAnimators.Length && _animalAnimators[arrayIndex] != null)
        {
            _anim.runtimeAnimatorController = _animalAnimators[arrayIndex];
            Debug.Log($"[PlayerAnimation] Loaded animator controller: {_animalAnimators[arrayIndex].name}");
        }
        else
        {
            Debug.LogError($"[PlayerAnimation] Invalid animal animator index: {arrayIndex}");
        }
    }

    /// <summary>
    /// Trigger animation jump
    /// Sử dụng trigger thay vì bool để animation luôn play từ đầu
    /// </summary>
    public void Jump()
    {
        if (_anim == null)
        {
            Debug.LogWarning("[PlayerAnimation] Cannot jump - Animator not initialized!");
            return;
        }

        // Set tốc độ animation nhanh hơn để mượt mà với gameplay
        _anim.speed = _jumpAnimationSpeed;

        // Trigger jump animation
        _anim.SetTrigger(JUMP_TRIGGER);

        Debug.Log("[PlayerAnimation] Jump animation triggered");
    }

    /// <summary>
    /// Reset về idle animation (dùng khi revive hoặc game over)
    /// </summary>
    public void ResetToIdle()
    {
        if (_anim == null) return;

        _anim.speed = _defaultAnimSpeed;
        _anim.ResetTrigger(JUMP_TRIGGER);
        _anim.Play("Idle", 0, 0f); // Force play Idle từ đầu

        Debug.Log("[PlayerAnimation] Reset to Idle");
    }

    /// <summary>
    /// Dừng animation (dùng khi game over)
    /// </summary>
    public void StopAnimation()
    {
        if (_anim == null) return;
        _anim.enabled = false;
    }

    /// <summary>
    /// Bật lại animation (dùng khi revive)
    /// </summary>
    public void ResumeAnimation()
    {
        if (_anim == null) return;
        _anim.enabled = true;
        ResetToIdle();
    }
}