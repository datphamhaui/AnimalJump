using UnityEngine;
using System.Collections;

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

    public void TriggerTrap()
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

        // BEAUTIFUL TRAP EXPLOSION EFFECT - Multi-phase animation sequence
        StartCoroutine(TrapExplosionSequence());
    }

    /// <summary>
    /// Hiệu ứng nổ đẹp mắt với nhiều giai đoạn
    /// </summary>
    private System.Collections.IEnumerator TrapExplosionSequence()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 originalPosition = transform.localPosition;
        
        // === PHASE 1: FREEZE & GLOW (0.1s) ===
        // Dừng tất cả animation hiện tại
        LeanTween.cancel(gameObject);
        
        // Freeze effect - scale nhỏ lại một chút
        LeanTween.scale(gameObject, originalScale * 0.8f, 0.1f).setEase(LeanTweenType.easeInQuart);
        yield return new WaitForSeconds(0.1f);
        
        // === PHASE 2: INTENSE SHAKE (0.2s) ===
        // Rung lắc mạnh với nhiều hướng
        for (int i = 0; i < 8; i++)
        {
            Vector3 shakeOffset = new Vector3(
                UnityEngine.Random.Range(-0.2f, 0.2f),
                UnityEngine.Random.Range(-0.1f, 0.1f),
                UnityEngine.Random.Range(-0.2f, 0.2f)
            );
            transform.localPosition = originalPosition + shakeOffset;
            yield return new WaitForSeconds(0.025f);
        }
        
        // === PHASE 3: EXPLOSIVE GROWTH (0.3s) ===
        transform.localPosition = originalPosition;
        
        // Nổ to ra với multiple scales
        LeanTween.scale(gameObject, originalScale * 2.5f, 0.15f).setEase(LeanTweenType.easeOutElastic);
        
        // Xoay cuồng phong với tăng tốc
        LeanTween.rotateLocal(gameObject, Vector3.up * 1080f, 0.3f).setEase(LeanTweenType.easeOutQuart);
        
        // Nhấp nhô mạnh lên xuống
        LeanTween.moveLocalY(gameObject, originalPosition.y + 1.5f, 0.15f).setEase(LeanTweenType.easeOutQuart)
            .setOnComplete(() => {
                LeanTween.moveLocalY(gameObject, originalPosition.y - 2f, 0.15f).setEase(LeanTweenType.easeInQuart);
            });
        
        yield return new WaitForSeconds(0.15f);
        
        // === PHASE 4: PARTICLE-LIKE FRAGMENTS (0.3s) ===
        // Tạo hiệu ứng như vỡ thành nhiều mảnh
        
        // Scale nhỏ dần theo dạng pulse
        for (int i = 0; i < 3; i++)
        {
            LeanTween.scale(gameObject, originalScale * (2f - i * 0.3f), 0.1f).setEase(LeanTweenType.easeInOutSine);
            yield return new WaitForSeconds(0.1f);
        }
        
        // === PHASE 5: DRAMATIC FINALE (0.2s) ===
        // Scale về 0 với hiệu ứng implosion
        LeanTween.scale(gameObject, Vector3.zero, 0.2f).setEase(LeanTweenType.easeInBack);
        
        // Xoay nhanh cuối cùng
        LeanTween.rotateLocal(gameObject, Vector3.up * 1800f, 0.2f).setEase(LeanTweenType.easeInExpo);
        
        // Fade out dramatic
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.material.color;
            LeanTween.value(gameObject, 1f, 0f, 0.2f)
                .setOnUpdate((float val) => {
                    if (renderer != null && renderer.material != null)
                    {
                        Color newColor = originalColor;
                        newColor.a = val;
                        renderer.material.color = newColor;
                    }
                });
        }
        
        yield return new WaitForSeconds(0.2f);
        
        // Final cleanup
        Destroy(gameObject);
    }

    /// <summary>
    /// Hiển thị trap (gọi từ Piece khi spawn) - Beautiful emergence effect
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        _isTriggered = false;

        StartCoroutine(TrapSpawnSequence());
    }

    /// <summary>
    /// Hiệu ứng spawn đẹp mắt cho trap
    /// </summary>
    private System.Collections.IEnumerator TrapSpawnSequence()
    {
        // Bắt đầu từ scale 0
        transform.localScale = Vector3.zero;
        Vector3 targetScale = Vector3.one;
        
        // === PHASE 1: MYSTERIOUS EMERGENCE (0.2s) ===
        // Xuất hiện từ từ với rotation
        LeanTween.scale(gameObject, targetScale * 0.3f, 0.2f).setEase(LeanTweenType.easeOutSine);
        LeanTween.rotateLocal(gameObject, Vector3.up * 180f, 0.2f).setEase(LeanTweenType.easeOutSine);
        
        yield return new WaitForSeconds(0.2f);
        
        // === PHASE 2: DRAMATIC REVEAL (0.3s) ===
        // Tăng scale với elastic bounce
        LeanTween.scale(gameObject, targetScale * 1.2f, 0.2f).setEase(LeanTweenType.easeOutElastic);
        
        // Thêm pulse effect
        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForSeconds(0.1f);
            LeanTween.scale(gameObject, targetScale * (1.2f - i * 0.1f), 0.05f).setEase(LeanTweenType.easeInOutSine);
        }
        
        // === PHASE 3: SETTLE & WARNING (0.2s) ===
        // Về scale bình thường
        LeanTween.scale(gameObject, targetScale, 0.1f).setEase(LeanTweenType.easeOutQuart);
        
        yield return new WaitForSeconds(0.1f);
        
        // === PHASE 4: DANGER INDICATION (0.3s) ===
        // Nhấp nháy cảnh báo nhẹ
        for (int i = 0; i < 3; i++)
        {
            LeanTween.rotateLocal(gameObject, Vector3.forward * (i % 2 == 0 ? 5f : -5f), 0.05f).setEase(LeanTweenType.easeInOutSine);
            yield return new WaitForSeconds(0.1f);
        }
        
        // Reset rotation về 0
        LeanTween.rotateLocal(gameObject, Vector3.zero, 0.1f).setEase(LeanTweenType.easeOutSine);
    }

    /// <summary>
    /// Ẩn trap
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
