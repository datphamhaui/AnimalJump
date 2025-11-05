using UnityEngine;

/// <summary>
/// Xử lý va chạm của player với platform/base
/// </summary>
public class PlayerCollision : MonoBehaviour
{
    [field: SerializeField]
    public bool CanJump { get; set; }

    private PlayerMovement _playerMovement;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        CanJump = false; // Bắt đầu không thể jump (phải chờ rơi xuống base trước)
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[PlayerCollision] ========== COLLISION ENTER ==========");
        Debug.Log($"[PlayerCollision] Collided with: {collision.gameObject.name}");
        
        // Cho phép jump khi chạm platform
        CanJump = true;

        // Gắn player vào platform để di chuyển theo platform
        transform.parent = collision.transform;
        Debug.Log($"[PlayerCollision] 🔗 PARENTED to: {collision.transform.name}");

        // Thông báo cho PlayerMovement MỖI LẦN chạm platform để cập nhật base height
        if (_playerMovement != null)
        {
            _playerMovement.OnLandedOnBase();
        }

        Debug.Log($"[PlayerCollision] CanJump={CanJump}");
        Debug.Log($"[PlayerCollision] ====================================\n");
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log($"[PlayerCollision] ========== COLLISION EXIT ==========");
        Debug.Log($"[PlayerCollision] Exited from: {collision.gameObject.name}");
        Debug.Log($"[PlayerCollision] Current parent before unparent: {(transform.parent != null ? transform.parent.name : "NULL")}");

        // CHỈ tách player khỏi platform nếu parent đã null (đã được detach trong Jump())
        // Nếu vẫn còn parent = platform di chuyển gây exit, không phải player nhảy
        if (transform.parent == null)
        {
            // Player đã nhảy (parent đã bị xóa trong Jump())
            Debug.Log($"[PlayerCollision] ✂️ UNPARENTED (parent was already null)");
            CanJump = false; // Không cho phép jump khi đang trong không trung
        }
        else
        {
            // Platform di chuyển gây exit, không phải player nhảy
            Debug.Log($"[PlayerCollision] ⚠️ EXIT IGNORED - Player still on platform: {transform.parent.name}");
            // GIỮ NGUYÊN CanJump = true vì player vẫn đứng trên platform
        }

        Debug.Log($"[PlayerCollision] CanJump={CanJump}");
        Debug.Log($"[PlayerCollision] ====================================\n");
    }
}
