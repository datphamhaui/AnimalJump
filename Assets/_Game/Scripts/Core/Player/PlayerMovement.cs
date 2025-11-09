using UnityEngine;

/// <summary>
/// Quản lý chuyển động của player - nhảy qua các platform
/// Sử dụng Lerp để di chuyển mượt mà, không dùng physics forces
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Jump Settings")]
    [Tooltip("Thời gian để hoàn thành một cú nhảy (giây)")]
    [SerializeField] private float _jumpTime = 0.3f;

    [Tooltip("Khoảng cách nhảy (về phía trước)")]
    [SerializeField] private float _gap = 3f;

    [Tooltip("Độ cao của cú nhảy (arc height)")]
    [SerializeField] private float _jumpHeight = 2f;

    [Header("Jump Arc Curve")]
    [Tooltip("Curve điều chỉnh độ cao khi nhảy (X=0->1 là thời gian, Y=0->1 là độ cao). Phải bắt đầu và kết thúc tại 0!")]
    [SerializeField] private AnimationCurve _jumpCurve = new AnimationCurve(
        new Keyframe(0, 0),    // Bắt đầu: Y=0 (trên platform)
        new Keyframe(0.5f, 1), // Giữa chừng: Y=1 (cao nhất)
        new Keyframe(1, 0)     // Kết thúc: Y=0 (đáp xuống platform)
    );

    private Rigidbody _rb;
    private float _elapsedTime = 0;
    private float _startZ, _endZ;
    private float _baseHeight; // Độ cao của base/platform (Y position)
    private bool _isJumping = false;
    private bool _isGameOver = false;
    private bool _hasLanded = false; // Đã rơi xuống base chưa

    // Public property để Piece có thể check
    public bool IsJumping => _isJumping;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        // Setup Rigidbody
        _rb.isKinematic = false;
        _rb.useGravity = true; // BẬT gravity ban đầu để player rơi xuống base
        _rb.freezeRotation = true; // Freeze rotation để không bị lật

        // QUAN TRỌNG: Force reset curve to correct parabola
        _jumpCurve = new AnimationCurve(
            new Keyframe(0f, 0f),    // Bắt đầu: Y=0 (trên platform)
            new Keyframe(0.5f, 1f),  // Giữa chừng: Y=1 (cao nhất)
            new Keyframe(1f, 0f)     // Kết thúc: Y=0 (đáp xuống platform)
        );

        // Initialization complete (logs removed in production)
    }

    /// <summary>
    /// Được gọi từ PlayerCollision khi player chạm platform
    /// Cập nhật base height mỗi lần landed
    /// </summary>
    public void OnLandedOnBase()
    {
        // Lần đầu tiên landed - tắt gravity
        if (!_hasLanded)
        {
            _hasLanded = true;
            _rb.useGravity = false;
            // First landing: gravity disabled
        }

        // Lưu Y position TRƯỚC khi update
        float oldBaseHeight = _baseHeight;
        
        // Cập nhật base height từ world position hiện tại
        _baseHeight = transform.position.y;

    // Land info updated (debug logs removed)
    }

    private void Update()
    {
        if (_isJumping && !_isGameOver)
        {
            _elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(_elapsedTime / _jumpTime);

            MovePlayer(progress);

            // Kết thúc jump
            if (progress >= 1f)
            {
                _isJumping = false;
                
                // SAFETY CHECK: Nếu jump xong mà vẫn chưa landed = miss platform
                // Delay check một chút để cho collision system xử lý
                StartCoroutine(CheckMissedPlatform());
            }
        }
    }

    /// <summary>
    /// Check nếu player miss platform sau khi jump xong
    /// </summary>
    private System.Collections.IEnumerator CheckMissedPlatform()
    {
        // Đợi đủ lâu cho jump hoàn tất (_jumpTime + buffer)
        yield return new WaitForSeconds(_jumpTime + 0.2f);

        // Nếu sau khi jump xong mà vẫn không có parent = miss platform
        if (!_isGameOver && transform.parent == null && !_isJumping)
        {
            EnablePhysicsOnGameOver();
        }
    }

    /// <summary>
    /// Bắt đầu nhảy
    /// </summary>
    public void Jump()
    {
    // Jump called (debug logs removed)
        
        // QUAN TRỌNG: Tách khỏi platform trước khi nhảy để có thể control world position
        if (transform.parent != null)
        {
            transform.parent = null;
        }

        _elapsedTime = 0f;
        _startZ = transform.position.z;
        _endZ = _startZ + _gap;
        _isJumping = true;

    // Reset velocity để đảm bảo không có ảnh hưởng từ physics
    _rb.linearVelocity = Vector3.zero;
    _rb.angularVelocity = Vector3.zero;
    }

    /// <summary>
    /// Di chuyển player theo curve mượt mà
    /// </summary>
    private void MovePlayer(float progress)
    {
        // Tính vị trí Z (tiến về phía trước)
        float currentZ = Mathf.Lerp(_startZ, _endZ, progress);

        // Tính độ cao Y theo curve (tạo arc cho jump)
        // _baseHeight + arc height để player không rơi xuống dưới base
        float curveValue = _jumpCurve.Evaluate(progress);
        float jumpArc = curveValue * _jumpHeight;
        float currentY = _baseHeight + jumpArc;

        // Cập nhật vị trí (world position)
        Vector3 newPosition = new Vector3(transform.position.x, currentY, currentZ);
        transform.position = newPosition;

        // Detailed jump logs removed
        if (progress >= 0.95f)
        {
            // no-op: removed debug output
        }
    }

    /// <summary>
    /// Hồi sinh player tại vị trí mới
    /// </summary>
    public void Revive(Vector3 position)
    {
        _isJumping = false;
        _isGameOver = false;
        
        // QUAN TRỌNG: Set _hasLanded = FALSE để player rơi xuống base
        _hasLanded = false;

        // Reset physics và BẬT gravity để rơi xuống
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.isKinematic = false;
        _rb.useGravity = true; // BẬT gravity để rơi xuống base
        
        // Reset rotation constraints
        _rb.freezeRotation = true;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Reset rotation và position
        transform.eulerAngles = Vector3.zero;
        transform.position = position;
        
        // Tách khỏi parent nếu có
        transform.parent = null;

        // Reset base height (sẽ được update lại khi landed)
        _baseHeight = 0f;

        // Reset jump values
        _startZ = position.z;
        _endZ = position.z;
        _elapsedTime = 0f;

    // Revive performed (debug logs removed)
    }

    /// <summary>
    /// Kích hoạt physics khi game over (player sẽ rơi xuống)
    /// </summary>
    public void EnablePhysicsOnGameOver()
    {
        // _isGameOver = true;
        _isJumping = false;
        
        // Bật gravity và đảm bảo rigidbody không bị kinematic
        _rb.isKinematic = false;
        _rb.useGravity = true;
        
        // Unfreeze rotation để player có thể xoay khi rơi
        _rb.freezeRotation = false;
        
        // Reset constraints để player rơi tự do
        _rb.constraints = RigidbodyConstraints.None;

    // Physics enabled for Game Over (debug logs removed)
    }
}