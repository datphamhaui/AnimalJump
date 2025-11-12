using UnityEngine;

/// <summary>
/// Script điều khiển coin: xoay tròn và nhấp nhô lên xuống
/// </summary>
public class Coin : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float _rotationSpeed = 100f;
    [SerializeField] private Vector3 _rotationAxis = Vector3.up;

    [Header("Bobbing Settings")]
    [SerializeField] private float _bobbingSpeed = 2f;
    [SerializeField] private float _bobbingHeight = 0.3f;

    private Vector3 _startPosition;
    private bool _isCollected = false;

    private void Start()
    {
        _startPosition = transform.localPosition;
    }

    private void Update()
    {
        if (_isCollected) return;

        // Xoay tròn coin
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
            Debug.Log("coin collected");
            // CollectCoin();
        }
    }

    public void CollectCoin()
    {
        _isCollected = true;

        // Thêm 1 coin cho player
        CurrencyManager currencyManager = CurrencyManager.GetInstance();
        if (currencyManager != null)
        {
            currencyManager.AddCoins(1);
            Debug.Log("[Coin] Player collected 1 coin!");
        }

        // Play sound effect
        if (SoundController.GetInstance() != null)
        {
            SoundController.GetInstance().PlayAudio(AudioType.BUTTON_CLICK); // Có thể thêm AudioType.COIN_COLLECT nếu có
        }

        // Animation thu thập coin (scale down + move up)
        LeanTween.scale(gameObject, Vector3.zero, 0.3f).setEase(LeanTweenType.easeInBack);
        LeanTween.moveLocalY(gameObject, transform.localPosition.y + 1f, 0.3f).setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() => 
            {
                Destroy(gameObject);
            });
    }

    /// <summary>
    /// Hiển thị coin (gọi từ Piece khi spawn)
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        _isCollected = false;

        // Animation spawn (scale from 0 to 1)
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, Vector3.one, 0.3f).setEase(LeanTweenType.easeOutBack).setDelay(0.1f);
    }

    /// <summary>
    /// Ẩn coin
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
