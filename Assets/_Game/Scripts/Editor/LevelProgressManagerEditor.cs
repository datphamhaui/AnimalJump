using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom Inspector cho LevelProgressManager để dễ debug và test
/// </summary>
#if UNITY_EDITOR
[CustomEditor(typeof(LevelProgressManager))]
public class LevelProgressManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelProgressManager manager = (LevelProgressManager)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Debug Tools", EditorStyles.boldLabel);

        // Button: Log Progress
        if (GUILayout.Button("📊 Log Progress"))
        {
            manager.LogProgress();
        }

        // Button: Reset Progress
        EditorGUILayout.Space(5);
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("🔄 Reset All Progress"))
        {
            if (EditorUtility.DisplayDialog("Reset Progress", 
                "Bạn có chắc muốn reset toàn bộ tiến độ?\n(Sẽ unlock lại level 1, xóa hết stars)", 
                "Yes", "Cancel"))
            {
                manager.ResetAllProgress();
                Debug.Log("✅ Progress đã được reset!");
            }
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Quick Level Unlock", EditorStyles.boldLabel);

        // Unlock specific level
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Unlock Level 1-5"))
        {
            for (int i = 1; i <= 5; i++) manager.UnlockLevel(i);
            Debug.Log("✅ Unlocked levels 1-5");
        }
        if (GUILayout.Button("Unlock Level 6-10"))
        {
            for (int i = 6; i <= 10; i++) manager.UnlockLevel(i);
            Debug.Log("✅ Unlocked levels 6-10");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Quick Star Test", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("⭐ Level 1 - 1 Star"))
        {
            manager.SaveLevelStars(1, 1);
        }
        if (GUILayout.Button("⭐⭐ Level 1 - 2 Stars"))
        {
            manager.SaveLevelStars(1, 2);
        }
        if (GUILayout.Button("⭐⭐⭐ Level 1 - 3 Stars"))
        {
            manager.SaveLevelStars(1, 3);
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
