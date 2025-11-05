using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hiển thị UI hearts của player (3 trái tim)
/// Tự động cập nhật khi health thay đổi
/// </summary>
public class HealthDisplay : MonoBehaviour
{
    [Header("Hearts UI")]
    [SerializeField] private List<Image> _heartImages; // 3 heart images
    
    [Header("Heart Sprites")]
    [SerializeField] private Sprite _fullHeartSprite;
    [SerializeField] private Sprite _emptyHeartSprite;

    [Header("Animation Settings")]
    [SerializeField] private float _loseHeartScale = 0.5f;
    [SerializeField] private float _gainHeartScale = 1.3f;

    private HealthManager _healthManager;

    private void Start()
    {
        _healthManager = HealthManager.GetInstance();
        
        if (_healthManager == null)
        {
            Debug.LogError("[HealthDisplay] HealthManager not found!");
            return;
        }

        // Initial display
        UpdateHealthDisplay(_healthManager.CurrentHealth);
    }

    private void OnEnable()
    {
        HealthManager.OnHealthChanged += UpdateHealthDisplay;
        HealthManager.OnHealthLost += OnHealthLost;
        HealthManager.OnHealthAdded += OnHealthAdded;
    }

    private void OnDisable()
    {
        HealthManager.OnHealthChanged -= UpdateHealthDisplay;
        HealthManager.OnHealthLost -= OnHealthLost;
        HealthManager.OnHealthAdded -= OnHealthAdded;
    }

    /// <summary>
    /// Cập nhật hiển thị hearts
    /// </summary>
    private void UpdateHealthDisplay(int currentHealth)
    {
        for (int i = 0; i < _heartImages.Count; i++)
        {
            if (i < currentHealth)
            {
                // Heart còn
                _heartImages[i].sprite = _fullHeartSprite;
                _heartImages[i].enabled = true;
            }
            else
            {
                // Heart mất
                _heartImages[i].sprite = _emptyHeartSprite;
                _heartImages[i].enabled = true;
            }
        }

        Debug.Log($"[HealthDisplay] Updated hearts: {currentHealth}/{_heartImages.Count}");
    }

    /// <summary>
    /// Animation khi mất heart
    /// </summary>
    private void OnHealthLost()
    {
        int currentHealth = _healthManager.CurrentHealth;
        
        // Animate heart vừa mất (index = currentHealth vì đã giảm rồi)
        if (currentHealth >= 0 && currentHealth < _heartImages.Count)
        {
            GameObject heartObj = _heartImages[currentHealth].gameObject;
            
            // Scale down animation
            LeanTween.cancel(heartObj);
            LeanTween.scale(heartObj, Vector3.one * _loseHeartScale, 0.3f)
                .setEase(LeanTweenType.easeInBack);
            
            // Shake effect
            Vector3 originalPos = heartObj.transform.localPosition;
            LeanTween.moveLocal(heartObj, originalPos + Vector3.right * 10f, 0.05f)
                .setLoopPingPong(4)
                .setOnComplete(() => 
                {
                    heartObj.transform.localPosition = originalPos;
                });
        }
    }

    /// <summary>
    /// Animation khi nhận heart
    /// </summary>
    private void OnHealthAdded()
    {
        int currentHealth = _healthManager.CurrentHealth;
        
        // Animate heart vừa được thêm (index = currentHealth - 1)
        if (currentHealth > 0 && currentHealth <= _heartImages.Count)
        {
            GameObject heartObj = _heartImages[currentHealth - 1].gameObject;
            
            // Scale up -> back to normal
            LeanTween.cancel(heartObj);
            heartObj.transform.localScale = Vector3.zero;
            LeanTween.scale(heartObj, Vector3.one * _gainHeartScale, 0.3f)
                .setEase(LeanTweenType.easeOutBack)
                .setOnComplete(() => 
                {
                    LeanTween.scale(heartObj, Vector3.one, 0.2f)
                        .setEase(LeanTweenType.easeInOutQuad);
                });
        }
    }
}
