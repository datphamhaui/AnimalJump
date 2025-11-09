using UnityEngine;

/// <summary>
/// Quản lý checkpoint system - lưu piece mà player landed an toàn
/// Khi player miss và còn heart, sẽ revive về checkpoint này
/// </summary>
public class CheckpointManager : Singleton<CheckpointManager>
{
    private Transform _checkpointPiece;

    /// <summary>
    /// Set checkpoint piece (được gọi khi player landed safe)
    /// </summary>
    public void SetCheckpoint(Transform piece)
    {
        if (piece == null)
        {
            Debug.LogWarning("[CheckpointManager] Attempted to set null checkpoint!");
            return;
        }

        _checkpointPiece = piece;
        Debug.Log($"[CheckpointManager] ✅ Checkpoint set to: {piece.name}");
    }

    /// <summary>
    /// Lấy vị trí checkpoint (world position của piece)
    /// </summary>
    public Vector3 GetCheckpointPosition()
    {
        if (_checkpointPiece != null)
        {
            return _checkpointPiece.position;
        }

        Debug.LogWarning("[CheckpointManager] No checkpoint set!");
        return Vector3.zero;
    }

    /// <summary>
    /// Check có checkpoint không
    /// </summary>
    public bool HasCheckpoint()
    {
        return _checkpointPiece != null;
    }

    /// <summary>
    /// Reset checkpoint (khi restart game)
    /// </summary>
    public void ResetCheckpoint()
    {
        _checkpointPiece = null;
        Debug.Log("[CheckpointManager] 🔄 Checkpoint reset");
    }
    
    public Transform CheckpointPiece { get { return _checkpointPiece; } }
}
