using UnityEngine;

/// <summary>
/// Script điều khiển heart pickup: xoay tròn và nhấp nhô lên xuống
/// </summary>
public class Heart : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float _rotationSpeed = 80f;
    [SerializeField] private Vector3 _rotationAxis = Vector3.up;

    [Header("Bobbing Settings")]
    [SerializeField] private float _bobbingSpeed = 2.5f;
    [SerializeField] private float _bobbingHeight = 0.25f;

    private Vector3 _startPosition;
    private bool _isCollected = false;

    private void Start()
    {
        _startPosition = transform.localPosition;
    }

    private void Update()
    {
        if (_isCollected) return;

        // Xoay tròn heart
        transform.Rotate(_rotationAxis, _rotationSpeed * Time.deltaTime, Space.World);

        // Nhấp nhô lên xuống
        float newY = _startPosition.y + Mathf.Sin(Time.time * _bobbingSpeed) * _bobbingHeight;
        transform.localPosition = new Vector3(_startPosition.x, newY, _startPosition.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem có phải player không
        if (other.CompareTag("Player") && !_isCollected)
        {
            CollectHeart();
        }
    }

    private void CollectHeart()
    {
        _isCollected = true;

        // Thêm 1 heart cho player
        HealthManager healthManager = HealthManager.GetInstance();
        if (healthManager != null)
        {
            bool added = healthManager.AddHealth(1);
            
            if (added)
            {
                Debug.Log("[Heart] Player collected 1 heart!");
                
                // Play sound effect
                if (SoundController.GetInstance() != null)
                {
                    SoundController.GetInstance().PlayAudio(AudioType.BUTTON_CLICK); // Có thể thêm AudioType.HEART_COLLECT
                }

                // Animation thu thập heart (scale up + move up + fade)
                LeanTween.scale(gameObject, Vector3.one * 1.5f, 0.2f).setEase(LeanTweenType.easeOutQuad);
                LeanTween.moveLocalY(gameObject, transform.localPosition.y + 2f, 0.5f).setEase(LeanTweenType.easeOutQuad)
                    .setOnComplete(() => 
                    {
                        Destroy(gameObject);
                    });
                
                // Fade out
                LeanTween.alpha(gameObject, 0f, 0.5f).setDelay(0.2f);
            }
            else
            {
                Debug.Log("[Heart] Player already at max health - heart not collected");
                _isCollected = false; // Cho phép collect lại nếu health chưa đầy
            }
        }
    }

    /// <summary>
    /// Hiển thị heart (gọi từ Piece khi spawn)
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        _isCollected = false;

        // Animation spawn (scale from 0 to 1 với bounce)
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, Vector3.one, 0.4f).setEase(LeanTweenType.easeOutBack).setDelay(0.1f);
    }

    /// <summary>
    /// Ẩn heart
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
