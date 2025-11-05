using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component tự động phát sound khi button được click
/// Attach vào Button GameObject để tự động có sound
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonSoundPlayer : MonoBehaviour
{
    [Header("Sound Settings")]
    [Tooltip("Loại audio sẽ phát khi click button")]
    [SerializeField] private AudioType _audioType = AudioType.BUTTON_CLICK;
    
    [Tooltip("Có phát sound hay không")]
    [SerializeField] private bool _playSound = true;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (_button != null)
        {
            _button.onClick.AddListener(PlayButtonSound);
        }
    }

    private void OnDisable()
    {
        if (_button != null)
        {
            _button.onClick.RemoveListener(PlayButtonSound);
        }
    }

    private void PlayButtonSound()
    {
        if (!_playSound) return;

        var soundController = SoundController.GetInstance();
        if (soundController != null)
        {
            soundController.PlayAudio(_audioType);
        }
        else
        {
            Debug.LogWarning($"[ButtonSoundPlayer] SoundController not found on button: {gameObject.name}");
        }
    }

    /// <summary>
    /// Bật/tắt sound cho button này
    /// </summary>
    public void SetSoundEnabled(bool enabled)
    {
        _playSound = enabled;
    }
}
