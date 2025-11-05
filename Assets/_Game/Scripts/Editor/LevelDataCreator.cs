using UnityEngine;
using UnityEditor;

/// <summary>
/// Menu helper để tạo Level Data nhanh từ Unity Editor
/// </summary>
#if UNITY_EDITOR
public class LevelDataCreator
{
    [MenuItem("Tools/Game/Create Level Data Assets")]
    public static void CreateLevelDataAssets()
    {
        // Tạo 10 level mẫu
        for (int i = 1; i <= 10; i++)
        {
            CreateLevelData(i);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ Đã tạo 10 Level Data trong Assets/_Game/Data/Levels/");
    }

    private static void CreateLevelData(int levelNumber)
    {
        string folderPath = "Assets/_Game/Data/Levels";
        
        // Tạo folder nếu chưa có
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            string parentFolder = "Assets/_Game/Data";
            if (!AssetDatabase.IsValidFolder(parentFolder))
            {
                AssetDatabase.CreateFolder("Assets/_Game", "Data");
            }
            AssetDatabase.CreateFolder(parentFolder, "Levels");
        }

        // Tạo LevelDataSO
        LevelDataSO levelData = ScriptableObject.CreateInstance<LevelDataSO>();
        
        // Config dựa trên level number (độ khó tăng dần)
        levelData.levelNumber = levelNumber;
        levelData.levelName = $"Level {levelNumber}";
        
        // Target score tăng dần
        levelData.targetScore = 20 + (levelNumber - 1) * 15;
        
        // Coin reward tăng dần theo level
        levelData.coinReward = 50 + (levelNumber - 1) * 25;
        
        // Tốc độ tăng dần
        levelData.platformSpeed = 2f + (levelNumber - 1) * 0.3f;
        
        // Gap tăng dần
        float gapMin = 0.5f + (levelNumber - 1) * 0.05f;
        float gapMax = 1.5f + (levelNumber - 1) * 0.1f;
        levelData.platformGapRange = new Vector2(gapMin, gapMax);
        
        // Safe zone giảm dần (khó hơn)
        levelData.safeLandingZoneRatio = Mathf.Max(0.5f, 0.75f - (levelNumber - 1) * 0.02f);
        
        // Obstacle từ level 5 trở đi
        levelData.hasObstacles = levelNumber >= 5;
        
        // Speed increase từ level 3 trở đi
        levelData.speedIncreaseRate = levelNumber >= 3 ? 0.05f : 0f;

        // Save asset
        string assetPath = $"{folderPath}/Level{levelNumber:D2}_Data.asset";
        AssetDatabase.CreateAsset(levelData, assetPath);
        
        Debug.Log($"Created: {assetPath}");
    }

    [MenuItem("Tools/Game/Open Level Data Folder")]
    public static void OpenLevelDataFolder()
    {
        string path = "Assets/_Game/Data/Levels";
        
        // Tạo folder nếu chưa có
        if (!AssetDatabase.IsValidFolder(path))
        {
            Debug.LogWarning("Folder chưa tồn tại. Chạy 'Create Level Data Assets' trước!");
            return;
        }

        // Open folder
        Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
        Selection.activeObject = obj;
        EditorGUIUtility.PingObject(obj);
    }
}
#endif
