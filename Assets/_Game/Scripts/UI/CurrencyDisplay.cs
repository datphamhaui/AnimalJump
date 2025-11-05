using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Component hiển thị số coin hiện tại
/// Tự động update khi coin thay đổi
/// </summary>
public class CurrencyDisplay : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Text hiển thị số coin")]
    [SerializeField] private TMP_Text _coinText;
    
    [Tooltip("Icon coin (optional)")]
    [SerializeField] private Image _coinIcon;

    [Header("Animation")]
    [Tooltip("Có animate khi coin thay đổi không")]
    [SerializeField] private bool _useAnimation = true;
    
    [Tooltip("Thời gian animation (giây)")]
    [SerializeField] private float _animationDuration = 0.5f;

    private CurrencyManager _currencyManager;
    private int _displayedCoins = 0;

    private void Start()
    {
        _currencyManager = CurrencyManager.GetInstance();
        
        if (_currencyManager != null)
        {
            _displayedCoins = _currencyManager.Coins;
            UpdateDisplay(_displayedCoins);
        }
    }

    private void OnEnable()
    {
        CurrencyManager.OnCoinsChanged += OnCoinsChanged;
        
        // Refresh display khi enable
        if (_currencyManager != null)
        {
            UpdateDisplay(_currencyManager.Coins);
        }
    }

    private void OnDisable()
    {
        CurrencyManager.OnCoinsChanged -= OnCoinsChanged;
    }

    /// <summary>
    /// Event handler khi coin thay đổi
    /// </summary>
    private void OnCoinsChanged(int newAmount)
    {
        if (_useAnimation)
        {
            AnimateCoins(_displayedCoins, newAmount);
        }
        else
        {
            _displayedCoins = newAmount;
            UpdateDisplay(newAmount);
        }
    }

    /// <summary>
    /// Cập nhật text hiển thị
    /// </summary>
    private void UpdateDisplay(int amount)
    {
        if (_coinText != null)
        {
            _coinText.text = amount.ToString("N0"); // Format với dấu phẩy: 1,000
        }
    }

    /// <summary>
    /// Animate số coin từ old value sang new value
    /// </summary>
    private void AnimateCoins(int oldValue, int newValue)
    {
        // Cancel tween cũ nếu có
        LeanTween.cancel(gameObject);

        // Tween số từ old -> new
        LeanTween.value(gameObject, oldValue, newValue, _animationDuration)
            .setOnUpdate((float val) =>
            {
                _displayedCoins = Mathf.RoundToInt(val);
                UpdateDisplay(_displayedCoins);
            })
            .setEase(LeanTweenType.easeOutQuad);

        // Scale animation cho feedback
        if (_coinText != null)
        {
            Transform textTransform = _coinText.transform;
            LeanTween.scale(textTransform.gameObject, Vector3.one * 1.2f, _animationDuration * 0.3f)
                .setEase(LeanTweenType.easeOutQuad)
                .setLoopPingPong(1);
        }
    }

    /// <summary>
    /// Public method để force refresh
    /// </summary>
    public void RefreshDisplay()
    {
        if (_currencyManager != null)
        {
            _displayedCoins = _currencyManager.Coins;
            UpdateDisplay(_displayedCoins);
        }
    }
}
