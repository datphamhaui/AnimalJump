using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Editor Script để xóa Animation Events từ các file Jump.anim
/// Sử dụng: Unity Menu → Tools → Remove Jump Animation Events
/// </summary>
public class RemoveAnimationEvents : EditorWindow
{
    private List<AnimationClip> _jumpAnimations = new List<AnimationClip>();
    private bool _hasScanned = false;

    [MenuItem("Tools/Remove Jump Animation Events")]
    public static void ShowWindow()
    {
        GetWindow<RemoveAnimationEvents>("Remove Anim Events");
    }

    private void OnGUI()
    {
        GUILayout.Label("Xóa Animation Events khỏi Jump Animations", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Quét tìm các file Jump.anim", GUILayout.Height(30)))
        {
            ScanForJumpAnimations();
        }

        GUILayout.Space(10);

        if (_hasScanned)
        {
            if (_jumpAnimations.Count > 0)
            {
                GUILayout.Label($"Tìm thấy {_jumpAnimations.Count} file Jump.anim:", EditorStyles.boldLabel);
                GUILayout.Space(5);

                foreach (var clip in _jumpAnimations)
                {
                    if (clip != null)
                    {
                        string path = AssetDatabase.GetAssetPath(clip);
                        EditorGUILayout.LabelField($"• {clip.name}", path);
                    }
                }

                GUILayout.Space(10);

                GUI.backgroundColor = Color.yellow;
                if (GUILayout.Button("XÓA TẤT CẢ ANIMATION EVENTS", GUILayout.Height(40)))
                {
                    if (EditorUtility.DisplayDialog(
                        "Xác nhận",
                        $"Bạn có chắc muốn xóa tất cả Animation Events từ {_jumpAnimations.Count} file?",
                        "Xóa", "Hủy"))
                    {
                        RemoveAllEvents();
                    }
                }
                GUI.backgroundColor = Color.white;
            }
            else
            {
                EditorGUILayout.HelpBox("Không tìm thấy file Jump.anim nào!", MessageType.Warning);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Nhấn nút 'Quét tìm' để tìm các file Jump.anim", MessageType.Info);
        }
    }

    private void ScanForJumpAnimations()
    {
        _jumpAnimations.Clear();

        // Tìm tất cả AnimationClip trong project
        string[] guids = AssetDatabase.FindAssets("t:AnimationClip Jump");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // Chỉ lấy các file trong thư mục _Game/Animations
            if (path.Contains("_Game/Animations") && path.Contains("Jump.anim"))
            {
                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                if (clip != null)
                {
                    _jumpAnimations.Add(clip);
                }
            }
        }

        _hasScanned = true;
        Debug.Log($"[RemoveAnimationEvents] Tìm thấy {_jumpAnimations.Count} file Jump.anim");
    }

    private void RemoveAllEvents()
    {
        int removedCount = 0;
        int totalEventsRemoved = 0;

        foreach (var clip in _jumpAnimations)
        {
            if (clip != null)
            {
                int eventCount = clip.events.Length;

                if (eventCount > 0)
                {
                    // Lưu path để log
                    string path = AssetDatabase.GetAssetPath(clip);

                    // Xóa tất cả events
                    AnimationUtility.SetAnimationEvents(clip, new AnimationEvent[0]);

                    // Lưu thay đổi
                    EditorUtility.SetDirty(clip);

                    removedCount++;
                    totalEventsRemoved += eventCount;

                    Debug.Log($"[RemoveAnimationEvents] Đã xóa {eventCount} events từ: {path}");
                }
            }
        }

        // Lưu tất cả assets
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string message = $"Hoàn thành!\n\n" +
                        $"Đã xóa {totalEventsRemoved} Animation Events từ {removedCount}/{_jumpAnimations.Count} file.";

        EditorUtility.DisplayDialog("Thành công", message, "OK");

        Debug.Log($"[RemoveAnimationEvents] Hoàn thành! Tổng: {totalEventsRemoved} events từ {removedCount} file");
    }
}
