using UnityEngine;
using System.Collections;

public enum AudioType
{
    JUMP,
    LANDING,
    GAMEOVER,
    BUTTON_CLICK,
    GAME_WIN,
}

public enum MusicType
{
    LOADING,
    GAME
}

public class SoundController : Singleton<SoundController>
{
    bool _musicEnable = true;
    bool _fxEnable = true;

    [Space(10)]
    [Range(0, 1)] [SerializeField] float _musicVolume = 0.6f;
    [Range(0, 1)] [SerializeField] float _fxVolume = 0.8f;

    [Header("Background Music")]
    [SerializeField] AudioSource _bgMusicSource;
    [SerializeField] AudioClip _loadingMusicClip;
    [SerializeField] AudioClip _gameMusicClip;

    [Header("Sound Effect Clip :")]
    [SerializeField] AudioClip _jump;
    [SerializeField] AudioClip _landing;
    [SerializeField] AudioClip _gameover;
    [SerializeField] AudioClip _buttonClick;
    [SerializeField] AudioClip _gameWin;

    [Header("Fade Settings")]
    [SerializeField] float _fadeDuration = 1f;

    GameObject oneShotGameObject;
    AudioSource oneShotAudioSource;
    
    private MusicType _currentMusicType = MusicType.LOADING;
    private Coroutine _fadeCoroutine;

    protected override void Awake()
    {
        base.Awake();
        
        // Persist qua các scene
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _fxEnable = PlayerPrefs.GetInt("sfxState") == 0;
        _musicEnable = PlayerPrefs.GetInt("musicState") == 0;

        // Bắt đầu với nhạc Loading
        if (_musicEnable) PlayBackgroundMusic(MusicType.LOADING, false);
    }

    public void PlayAudio(AudioType type)
    {
        // return if audio fx is disable
        if (!_fxEnable) return;

        if (oneShotGameObject == null)
        {
            oneShotGameObject = new GameObject("Sound");
            oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
        }

        AudioClip clip = GetClip(type);

        oneShotAudioSource.volume = _fxVolume;
        oneShotAudioSource.PlayOneShot(clip);
    }

    public void ToggleMusic(ref bool state)
    {
        _musicEnable = !_musicEnable;
        UpdateMusic();
        state = _musicEnable;
        PlayerPrefs.SetInt("musicState", _musicEnable ? 0 : 1);
        PlayerPrefs.Save();
        Debug.Log($"[SoundController] Music state changed: {(_musicEnable ? "ON" : "OFF")} (PlayerPrefs: {PlayerPrefs.GetInt("musicState")})");
    }

    public void ToggleFX(ref bool state)
    {
        _fxEnable = !_fxEnable;
        state = _fxEnable;
        PlayerPrefs.SetInt("sfxState", _fxEnable ? 0 : 1);
        PlayerPrefs.Save();
        Debug.Log($"[SoundController] SFX state changed: {(_fxEnable ? "ON" : "OFF")} (PlayerPrefs: {PlayerPrefs.GetInt("sfxState")})");
    }
    
    /// <summary>
    /// Set trạng thái Music (không toggle)
    /// </summary>
    public void SetMusicState(bool enabled)
    {
        if (_musicEnable != enabled)
        {
            _musicEnable = enabled;
            UpdateMusic();
            PlayerPrefs.SetInt("musicState", _musicEnable ? 0 : 1);
            PlayerPrefs.Save();
            Debug.Log($"[SoundController] Music set to: {(_musicEnable ? "ON" : "OFF")}");
        }
    }
    
    /// <summary>
    /// Set trạng thái Sound FX (không toggle)
    /// </summary>
    public void SetFXState(bool enabled)
    {
        if (_fxEnable != enabled)
        {
            _fxEnable = enabled;
            PlayerPrefs.SetInt("sfxState", _fxEnable ? 0 : 1);
            PlayerPrefs.Save();
            Debug.Log($"[SoundController] SFX set to: {(_fxEnable ? "ON" : "OFF")}");
        }
    }

    /// <summary>
    /// Chuyển đổi nhạc nền theo loại (Loading hoặc Game)
    /// </summary>
    public void PlayBackgroundMusic(MusicType musicType, bool useFade = true)
    {
        if (!_musicEnable) return;

        _currentMusicType = musicType;
        AudioClip clip = GetMusicClip(musicType);

        if (clip == null)
        {
            Debug.LogWarning($"[SoundController] No music clip for {musicType}");
            return;
        }

        // Nếu đang phát cùng clip thì không cần chuyển
        if (_bgMusicSource.clip == clip && _bgMusicSource.isPlaying)
        {
            Debug.Log($"[SoundController] Already playing {musicType} music");
            return;
        }

        if (useFade)
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
            _fadeCoroutine = StartCoroutine(FadeMusicTransition(clip));
        }
        else
        {
            PlayMusicImmediate(clip);
        }

        Debug.Log($"[SoundController] 🎵 Playing {musicType} music: {clip.name}");
    }

    /// <summary>
    /// Chuyển nhạc loading sang nhạc game (gọi khi vào Game scene)
    /// </summary>
    public void SwitchToGameMusic()
    {
        PlayBackgroundMusic(MusicType.GAME, true);
    }

    /// <summary>
    /// Chuyển nhạc game sang nhạc loading (gọi khi về Loading scene)
    /// </summary>
    public void SwitchToLoadingMusic()
    {
        PlayBackgroundMusic(MusicType.LOADING, true);
    }

    void UpdateMusic()
    {
        if (!_musicEnable)
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }
            _bgMusicSource.Stop();
        }
        else
        {
            // Phát lại nhạc hiện tại
            PlayBackgroundMusic(_currentMusicType, false);
        }
    }

    private AudioClip GetClip(AudioType audioType)
    {
        switch (audioType)
        {
            case AudioType.JUMP:
                return _jump;
            case AudioType.LANDING:
                return _landing;
            case AudioType.GAMEOVER:
                return _gameover;
            case AudioType.BUTTON_CLICK:
                return _buttonClick;
            case AudioType.GAME_WIN:
                return _gameWin;
            
            default:
                return null;
        }
    }

    private AudioClip GetMusicClip(MusicType musicType)
    {
        switch (musicType)
        {
            case MusicType.LOADING:
                return _loadingMusicClip;
            case MusicType.GAME:
                return _gameMusicClip;
            default:
                return null;
        }
    }

    private void PlayMusicImmediate(AudioClip clip)
    {
        _bgMusicSource.Stop();
        _bgMusicSource.clip = clip;
        _bgMusicSource.volume = _musicVolume;
        _bgMusicSource.loop = true;
        _bgMusicSource.Play();
    }

    private IEnumerator FadeMusicTransition(AudioClip newClip)
    {
        // Fade out nhạc cũ
        float startVolume = _bgMusicSource.volume;
        float elapsed = 0f;

        while (elapsed < _fadeDuration / 2f)
        {
            elapsed += Time.deltaTime;
            _bgMusicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / (_fadeDuration / 2f));
            yield return null;
        }

        // Chuyển sang nhạc mới
        _bgMusicSource.Stop();
        _bgMusicSource.clip = newClip;
        _bgMusicSource.loop = true;
        _bgMusicSource.Play();

        // Fade in nhạc mới
        elapsed = 0f;
        while (elapsed < _fadeDuration / 2f)
        {
            elapsed += Time.deltaTime;
            _bgMusicSource.volume = Mathf.Lerp(0f, _musicVolume, elapsed / (_fadeDuration / 2f));
            yield return null;
        }

        _bgMusicSource.volume = _musicVolume;
        _fadeCoroutine = null;
    }
}
