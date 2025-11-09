using UnityEngine;

/// <summary>
/// Script điều khiển trap (bẫy): xoay tròn và nhấp nhô
/// Khi player đạp vào → Trừ 1 heart → Nếu hết heart = Game Over
/// </summary>
public class Trap : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float _rotationSpeed = 120f;
    [SerializeField] private Vector3 _rotationAxis = Vector3.up;

    [Header("Bobbing Settings")]
    [SerializeField] private float _bobbingSpeed = 3f;
    [SerializeField] private float _bobbingHeight = 0.2f;

    private Vector3 _startPosition;
    private bool _isTriggered = false;

    private void Start()
    {
        _startPosition = transform.localPosition;
    }

    private void Update()
    {
        if (_isTriggered) return;

        // Xoay tròn trap
        transform.Rotate(_rotationAxis, _rotationSpeed * Time.deltaTime, Space.World);

        // Nhấp nhô lên xuống
        float newY = _startPosition.y + Mathf.Sin(Time.time * _bobbingSpeed) * _bobbingHeight;
        transform.localPosition = new Vector3(_startPosition.x, newY, _startPosition.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem có phải player không
        if (other.CompareTag("Player") && !_isTriggered)
        {
            Debug.Log("[Trap] Player hit trap!");
            TriggerTrap();
        }
    }

    private void TriggerTrap()
    {
        _isTriggered = true;

        // Gọi GameManager để xử lý damage
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.HandleTrapDamage();
            Debug.Log("[Trap] Player triggered trap!");
        }
        else
        {
            Debug.LogError("[Trap] GameManager not found!");
        }

        // Play sound effect
        if (SoundController.GetInstance() != null)
        {
            SoundController.GetInstance().PlayAudio(AudioType.GAMEOVER); // TODO: Thêm AudioType.TRAP_HIT nếu có
        }

        // Animation trigger trap (scale up + move up + fade out)
        LeanTween.scale(gameObject, Vector3.one * 1.5f, 0.3f).setEase(LeanTweenType.easeOutQuad);
        LeanTween.moveLocalY(gameObject, transform.localPosition.y + 1.5f, 0.4f).setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
            {
                Destroy(gameObject);
            });

        // Fade out
        LeanTween.alpha(gameObject, 0f, 0.4f).setDelay(0.1f);
    }

    /// <summary>
    /// Hiển thị trap (gọi từ Piece khi spawn)
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        _isTriggered = false;

        // Animation spawn (scale from 0 to 1)
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, Vector3.one, 0.3f).setEase(LeanTweenType.easeOutBack).setDelay(0.1f);
    }

    /// <summary>
    /// Ẩn trap
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
