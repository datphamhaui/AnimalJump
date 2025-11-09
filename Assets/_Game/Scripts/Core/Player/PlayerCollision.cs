using UnityEngine;

/// <summary>
/// Xử lý va chạm của player với platform/base
/// Sử dụng Raycast để check ground chính xác, tránh spam jump
/// </summary>
public class PlayerCollision : MonoBehaviour
{
    [Header("Ground Check Settings")]
    [Tooltip("Khoảng cách ray để check ground")]
    [SerializeField] private float _rayDistance = 0.2f;

    [Tooltip("Layer của platform (để raycast chỉ check platform)")]
    [SerializeField] private LayerMask _groundLayer;

    [Tooltip("Hiển thị ray debug trong Scene view")]
    [SerializeField] private bool _debugRay = true;

    [field: SerializeField]
    public bool CanJump { get; set; }

    private PlayerMovement _playerMovement;
    private Transform      _animalTransform; // Animal collider để bắn ray từ vị trí này
    private bool           _jumpDisabledTemporarily = false; // Flag để disable jump tạm thời sau revival

    private void Awake() { _playerMovement = GetComponent<PlayerMovement>(); }

    private void Start()
    {
        CanJump = false; // Bắt đầu không thể jump (phải chờ rơi xuống base trước)
        // Tự động tìm animal collider trong children
        Collider animalCollider = GetComponentInChildren<Collider>();

        if (animalCollider != null)
        {
            _animalTransform = animalCollider.transform;
        }
        else
        {
            // Animal collider not found — ground check will not work.
        }
    }

    private void Update()
    {
        // Cập nhật CanJump mỗi frame bằng ground check
        CanJump = GroundCheck();
    }

    /// <summary>
    /// Kiểm tra xem player có đang chạm ground không bằng Raycast
    /// </summary>
    private bool GroundCheck()
    {
        // Nếu jump bị disable tạm thời (sau revival) → không cho jump
        if (_jumpDisabledTemporarily)
            return false;

        // Nếu chưa có animal transform thì không check được
        if (_animalTransform == null)
            return false;

        // Bắn ray từ animal position xuống dưới
        Vector3 rayOrigin    = _animalTransform.position;
        Vector3 rayDirection = Vector3.down;

        bool isGrounded = Physics.Raycast(rayOrigin, rayDirection, _rayDistance, _groundLayer);

        // Debug visualization
        if (_debugRay)
        {
            Color rayColor = isGrounded ? Color.green : Color.red;
            Debug.DrawRay(rayOrigin, rayDirection * _rayDistance, rayColor);
        }

        return isGrounded;
    }

    /// <summary>
    /// Disable jump tạm thời sau revival (1 giây)
    /// </summary>
    public void DisableJumpTemporarily(float duration = 1f)
    {
        StartCoroutine(DisableJumpCoroutine(duration));
    }

    private System.Collections.IEnumerator DisableJumpCoroutine(float duration)
    {
    _jumpDisabledTemporarily = true;

    yield return new WaitForSeconds(duration);

    _jumpDisabledTemporarily = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Gắn player vào platform để di chuyển theo platform
        transform.parent = collision.transform;

        // Thông báo cho PlayerMovement MỖI LẦN chạm platform để cập nhật base height
        if (_playerMovement != null)
        {
            _playerMovement.OnLandedOnBase();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // CHỈ tách player khỏi platform nếu parent đã null (đã được detach trong Jump())
        // Nếu vẫn còn parent = platform di chuyển gây exit, không phải player nhảy
        if (transform.parent == null)
        {
            // Player đã nhảy (parent đã bị xóa trong Jump())
        }
        else
        {
            // Platform di chuyển gây exit, không phải player nhảy
        }
    }
}