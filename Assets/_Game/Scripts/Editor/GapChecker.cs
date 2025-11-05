using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor tool để kiểm tra gap giữa PlayerMovement và PlatformSpawner
/// Menu: Tools → Check Gap Settings
/// </summary>
public class GapChecker : EditorWindow
{
    [MenuItem("Tools/Check Gap Settings")]
    public static void ShowWindow()
    {
        GetWindow<GapChecker>("Gap Checker");
    }

    private void OnGUI()
    {
        GUILayout.Label("Kiểm tra Gap Settings", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Kiểm tra Gap hiện tại", GUILayout.Height(40)))
        {
            CheckGaps();
        }

        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Tool này kiểm tra xem Gap trong PlayerMovement và PlatformSpawner có khớp nhau không.\n\n" +
            "Để game hoạt động đúng:\n" +
            "• PlayerMovement.Gap = PlatformSpawner.Gap\n" +
            "• Player position Z = 0\n" +
            "• Platform đầu tiên Z = Gap",
            MessageType.Info
        );
    }

    private void CheckGaps()
    {
        // Tìm PlayerMovement
        PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("[GapChecker] Không tìm thấy PlayerMovement trong scene!");
            EditorUtility.DisplayDialog("Lỗi", "Không tìm thấy PlayerMovement trong scene!", "OK");
            return;
        }

        // Tìm PlatformSpawner
        PlatformSpawner platformSpawner = FindFirstObjectByType<PlatformSpawner>();
        if (platformSpawner == null)
        {
            Debug.LogError("[GapChecker] Không tìm thấy PlatformSpawner trong scene!");
            EditorUtility.DisplayDialog("Lỗi", "Không tìm thấy PlatformSpawner trong scene!", "OK");
            return;
        }

        // Lấy giá trị gap từ serialized fields
        SerializedObject playerSO = new SerializedObject(playerMovement);
        SerializedProperty playerGapProp = playerSO.FindProperty("_gap");
        float playerGap = playerGapProp.floatValue;

        SerializedObject spawnerSO = new SerializedObject(platformSpawner);
        SerializedProperty spawnerGapProp = spawnerSO.FindProperty("_gap");
        float spawnerGap = spawnerGapProp.floatValue;

        // So sánh
        bool isMatching = Mathf.Approximately(playerGap, spawnerGap);

        string message = $"=== KẾT QUẢ KIỂM TRA ===\n\n";
        message += $"PlayerMovement.Gap: {playerGap}\n";
        message += $"PlatformSpawner.Gap: {spawnerGap}\n\n";

        if (isMatching)
        {
            message += "✓ Gap khớp nhau! Game sẽ hoạt động đúng.\n\n";
            Debug.Log($"[GapChecker] {message}");
            EditorUtility.DisplayDialog("Kiểm tra Gap", message, "OK");
        }
        else
        {
            message += "✗ Gap KHÔNG khớp nhau!\n\n";
            message += "Bạn có muốn đồng bộ giá trị không?\n";
            message += $"(Sẽ đặt cả 2 = {playerGap})";

            Debug.LogWarning($"[GapChecker] {message}");

            if (EditorUtility.DisplayDialog("Gap không khớp!", message, "Đồng bộ", "Hủy"))
            {
                // Đồng bộ gap
                spawnerGapProp.floatValue = playerGap;
                spawnerSO.ApplyModifiedProperties();

                Debug.Log($"[GapChecker] Đã đồng bộ gap = {playerGap}");
                EditorUtility.DisplayDialog("Thành công", $"Đã đồng bộ gap = {playerGap}", "OK");
            }
        }

        // Kiểm tra vị trí player
        Vector3 playerPos = playerMovement.transform.position;
        message = $"\n=== VỊ TRÍ PLAYER ===\n";
        message += $"Position: X={playerPos.x:F2}, Y={playerPos.y:F2}, Z={playerPos.z:F2}\n\n";

        if (Mathf.Approximately(playerPos.z, 0f))
        {
            message += "✓ Player đang ở Z = 0 (đúng vị trí ban đầu)\n";
        }
        else
        {
            message += $"⚠ Player Z = {playerPos.z:F2}, khuyến nghị Z = 0\n";
        }

        Debug.Log($"[GapChecker] {message}");
    }
}
