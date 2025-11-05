using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Xử lý input từ người dùng - cả PC (chuột) và Mobile (touch)
/// Khi người dùng tap/click màn hình, sẽ trigger OnJump event
/// </summary>
public class PlayerInput : MonoBehaviour
{
    [Header("Jump Event")]
    [Tooltip("Event được gọi khi người dùng tap màn hình. Kết nối với PlayerBehaviour.OnJump()")]
    [field: SerializeField]
    public UnityEvent OnJump { get; set; }

    private void Update()
    {
        // Kiểm tra input tùy theo platform
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#elif UNITY_ANDROID || UNITY_IOS
        HandleTouchInput();
#else
        HandleMouseInput();
        HandleTouchInput();
#endif
    }

    /// <summary>
    /// Xử lý input chuột (PC/Editor)
    /// </summary>
    private void HandleMouseInput()
    {
        // Chỉ xử lý khi không click lên UI
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            OnJump?.Invoke();
            Debug.Log("[PlayerInput] Mouse click detected - Jump!");
        }
    }

    /// <summary>
    /// Xử lý input touch (Mobile)
    /// </summary>
    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];

            // Chỉ xử lý lần đầu chạm và không chạm vào UI
            if (touch.phase == TouchPhase.Began && !IsPointerOverUI(touch.fingerId))
            {
                OnJump?.Invoke();
                Debug.Log("[PlayerInput] Touch detected - Jump!");
            }
        }
    }

    /// <summary>
    /// Kiểm tra xem có đang click/touch vào UI không
    /// </summary>
    private bool IsPointerOverUI(int touchId = -1)
    {
        if (EventSystem.current == null)
            return false;

        if (touchId >= 0)
            return EventSystem.current.IsPointerOverGameObject(touchId);
        else
            return EventSystem.current.IsPointerOverGameObject();
    }
}