using System;
using UnityEngine;

/// <summary>
/// Boundary wall detection - Khi player chạm vào boundary wall (ra khỏi view)
/// Trigger event để revive về center piece của platform hiện tại
/// </summary>
public class BoundaryWall : MonoBehaviour
{
    /// <summary>
    /// Event trigger khi player chạm boundary wall
    /// Truyền platform mà player đang đứng
    /// </summary>
    public static event Action<Transform> OnBoundaryHit;

    private bool _hasTriggered = false; // Flag để tránh spam trigger
    private float _cooldownDuration = 2f; // Cooldown 2 giây

    private void OnTriggerEnter(Collider other)
    {
        // Check nếu đã trigger gần đây (cooldown)
        if (_hasTriggered)
        {
            Debug.Log("[BoundaryWall] ⏳ Cooldown active, ignoring trigger");
            return;
        }

        // Check nếu là player
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[BoundaryWall] ⚠️ Player hit boundary wall!");

            // Set cooldown
            _hasTriggered = true;
            StartCoroutine(ResetCooldown());

            // Lấy platform từ checkpoint piece (đơn giản hơn traverse từ player collider)
            CheckpointManager checkpointManager = CheckpointManager.GetInstance();
            if (checkpointManager == null || !checkpointManager.HasCheckpoint())
            {
                Debug.LogError("[BoundaryWall] ❌ No checkpoint found!");
                return;
            }

            Transform checkpointPiece = checkpointManager.CheckpointPiece;
            if (checkpointPiece == null)
            {
                Debug.LogError("[BoundaryWall] ❌ Checkpoint piece is null!");
                return;
            }

            // Hierarchy từ Piece: Piece → Piece Container → Platform
            // Level 1: Piece Container
            if (checkpointPiece.parent == null)
            {
                Debug.LogError("[BoundaryWall] ❌ Piece has no parent (Container)!");
                return;
            }
            Transform pieceContainer = checkpointPiece.parent;

            // Level 2: Platform
            if (pieceContainer.parent == null)
            {
                Debug.LogError("[BoundaryWall] ❌ Container has no parent (Platform)!");
                return;
            }
            Transform platform = pieceContainer.parent;

            Debug.Log($"[BoundaryWall] Platform found from checkpoint: {platform.name}");

            // Check xem có Platform component không
            Platform platformScript = platform.GetComponent<Platform>();
            if (platformScript != null)
            {
                Debug.Log($"[BoundaryWall] ✅ Found Platform component! Triggering event...");
                OnBoundaryHit?.Invoke(platform);
            }
            else
            {
                Debug.LogError($"[BoundaryWall] ❌ No Platform component on: {platform.name}");
            }
        }
    }

    /// <summary>
    /// Reset cooldown sau một khoảng thời gian
    /// </summary>
    private System.Collections.IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(_cooldownDuration);
        _hasTriggered = false;
        Debug.Log("[BoundaryWall] ✅ Cooldown reset");
    }

    private void OnDrawGizmos()
    {
        // Vẽ boundary wall màu đỏ trong Scene view
        Gizmos.color = Color.red;
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        if (boxCollider != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
    }
}
