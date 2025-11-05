using UnityEngine;

/// <summary>
/// ScriptableObject chứa config cho một level cụ thể
/// </summary>
[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data", order = 0)]
public class LevelDataSO : ScriptableObject
{
    [Header("Level Info")]
    [Tooltip("Số thứ tự level (1, 2, 3...)")]
    public int levelNumber = 1;

    [Tooltip("Tên level hiển thị")]
    public string levelName = "Level 1";

    [Header("Win Condition")]
    [Tooltip("Số điểm cần đạt để hoàn thành level")]
    public int targetScore = 50;

    [Header("Rewards")]
    [Tooltip("Số coin thưởng khi hoàn thành level")]
    public int coinReward = 100;

    [Header("Difficulty Settings")]
    [Tooltip("Tốc độ di chuyển của platform")]
    public float platformSpeed = 2f;

    [Tooltip("Khoảng cách giữa các platform (min-max)")]
    public Vector2 platformGapRange = new Vector2(0.5f, 1.5f);

    [Tooltip("Tỷ lệ vùng an toàn khi đáp (0-1). Nhỏ hơn = khó hơn")]
    [Range(0.5f, 0.9f)]
    public float safeLandingZoneRatio = 0.7f;

    [Header("Optional Settings")]
    [Tooltip("Có obstacle trong level này không?")]
    public bool hasObstacles = false;

    [Tooltip("Tốc độ tăng dần trong level (0 = không tăng)")]
    public float speedIncreaseRate = 0f;

    /// <summary>
    /// Validate dữ liệu khi tạo/edit trong Inspector
    /// </summary>
    private void OnValidate()
    {
        levelNumber = Mathf.Max(1, levelNumber);
        targetScore = Mathf.Max(1, targetScore);
        coinReward = Mathf.Max(0, coinReward);
        platformSpeed = Mathf.Max(0.5f, platformSpeed);
        platformGapRange.x = Mathf.Max(0.1f, platformGapRange.x);
        platformGapRange.y = Mathf.Max(platformGapRange.x, platformGapRange.y);
    }
}
