using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : Menu
{
    [Header("UI References :")]
    [SerializeField] Button _adsButton;

    [SerializeField] Button _privacyButton;
    [SerializeField] Button _backButton;
    [SerializeField] Button _musicButton;
    [SerializeField] Button _sfxButton;

    [Space]
    [SerializeField] private RectTransform _panel;

    [Header("Image References Toggle")]
    [SerializeField] Image _sfxImage;

    [SerializeField] Image _MusicImage;

    [Header("Icon Toggle")]
    [SerializeField] Sprite _iconTrue;

    [SerializeField] Sprite _iconFalse;

    private bool _sfxState;
    private bool _musicState;

    public override void SetEnable()
    {
        base.SetEnable();

        _backButton.interactable    = true;
        _adsButton.interactable     = true;
        _privacyButton.interactable = true;
    }

    private void Start()
    {
        SetButtonIconToggle();

        OnButtonPressed(_musicButton, MusicButtonPressed);
        OnButtonPressed(_sfxButton, SfxButtonPressed);
        OnButtonPressed(_adsButton, AdsButtonPressed);
        OnButtonPressed(_privacyButton, PrivacyPolicyButtonPressed);
        OnButtonPressed(_backButton, BackButtonPressed);
    }

    private void SetButtonIconToggle()
    {
        _musicState = PlayerPrefs.GetInt("musicState", 0) == 0;
        _sfxState   = PlayerPrefs.GetInt("sfxState", 0) == 0;

        ToggleIconSFX();
        ToggleIconMusic();
    }

    private void ToggleIconSFX()
    {
        if (!_sfxImage || !_iconTrue || !_iconFalse) return;

        _sfxImage.sprite = _sfxState ? _iconTrue : _iconFalse;
    }

    private void ToggleIconMusic()
    {
        if (!_MusicImage || !_iconTrue || !_iconFalse) return;

        _MusicImage.sprite = _musicState ? _iconTrue : _iconFalse;
    }

    private void SfxButtonPressed()
    {
        SoundController.GetInstance().ToggleFX(ref _sfxState);
        ToggleIconSFX();
    }

    private void MusicButtonPressed()
    {
        SoundController.GetInstance().ToggleMusic(ref _musicState);
        ToggleIconMusic();
    }

    private void BackButtonPressed()
    {
        _backButton.interactable = false;
        MenuManager.GetInstance().CloseMenu();
    }

    private void PrivacyPolicyButtonPressed() { _privacyButton.interactable = true; }

    private void AdsButtonPressed()
    {
        _adsButton.interactable = false;
        PlayerPrefs.SetInt("npa", -1);

        SoundController.GetInstance().DestroyObject();

        //load gdpr scene
        StartCoroutine(LoadGDPRAsyncScene());
    }

    IEnumerator LoadGDPRAsyncScene()
    {
        yield return SceneManager.LoadSceneAsync(0);
        MenuManager.GetInstance().DestroyObject();
    }
}