using UnityEngine;

/// <summary>
/// Spawn platform tự động khi player tiến về phía trước
/// Tốc độ platform được quản lý bởi LevelManager (dựa trên high score)
/// </summary>
public class PlatformSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Khoảng cách từ platform cuối đến target để spawn platform mới")]
    [SerializeField] private float _gapFromLastPlatformToTarget = 30f;

    [Tooltip("Khoảng cách giữa các platform")]
    [SerializeField] private float _gap = 10f;

    [Header("References")]
    [SerializeField] private Platform _platformPrefab;
    [SerializeField] private Transform _target; // Player transform

    private bool _invertPlatform;
    private Vector3 _lastPlatformPosition;
    private int _platformCount = 0;

    private void Start()
    {
        _lastPlatformPosition = Vector3.forward * _gap;
    }

    private void Update()
    {
        // Kiểm tra nếu target bị destroy (khi restart game)
        if (_target == null)
        {
            return;
        }

        // Kiểm tra nếu player gần đến platform cuối thì spawn platform mới
        float sqrDistance = Vector3.SqrMagnitude(_lastPlatformPosition - _target.position);
        float sqrThreshold = _gapFromLastPlatformToTarget * _gapFromLastPlatformToTarget;

        if (sqrDistance < sqrThreshold)
        {
            SpawnPlatform(_lastPlatformPosition, _platformPrefab);
        }
    }

    /// <summary>
    /// Spawn một platform mới
    /// </summary>
    private void SpawnPlatform(Vector3 position, Platform prefab)
    {
        Platform newPlatform = Instantiate(prefab, transform);
        newPlatform.transform.position = position;

        // Đảo ngược platform (luân phiên trái/phải)
        newPlatform.InvertPos = _invertPlatform;
        newPlatform.gameObject.SetActive(true);

        // Cập nhật vị trí platform cuối
        _lastPlatformPosition.z += _gap;

        // Đảo ngược cho platform tiếp theo
        _invertPlatform = !_invertPlatform;

        _platformCount++;

        Debug.Log($"[PlatformSpawner] Spawned platform #{_platformCount} at Z={position.z}");
    }

    /// <summary>
    /// Reset spawner (dùng khi restart game)
    /// </summary>
    public void ResetSpawner()
    {
        _platformCount = 0;
        _lastPlatformPosition = Vector3.forward * _gap;
        _invertPlatform = false;

        // Xóa tất cả platform đã spawn
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Debug.Log("[PlatformSpawner] Reset spawner");
    }
}
