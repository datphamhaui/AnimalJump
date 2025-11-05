using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReviveMenu : Menu
{
    [Header("UI References :")]
    [SerializeField] Button _continueButton;

    [SerializeField] Button _skipButton;

    [SerializeField] TMP_Text _timerText;
    [SerializeField] Image    _timerFill;

    private Timer _timer;

    protected override void Awake()
    {
        base.Awake();

        _timer = GetComponent<Timer>();
    }

    public override void SetEnable()
    {
        base.SetEnable();

        _continueButton.interactable = true;
        _skipButton.interactable     = true;

        // start timer
        _timer.PlayTimer(i => _timerText.text = i, j => _timerFill.fillAmount = j, GameOver);

        LeanTween.scale(_continueButton.gameObject, Vector2.one * 1.1f, .3f).setEase(LeanTweenType.easeOutQuad).setLoopPingPong();
    }

    private void Start()
    {
        OnButtonPressed(_continueButton, ContinueButtonPressed);
        OnButtonPressed(_skipButton, SkipButtonPressed);
    }

    private void SkipButtonPressed()
    {
        _skipButton.interactable = false;
        ResetWatchAdButton();

        _timer.StopTimer();
        MenuManager.GetInstance().SwitchMenu(MenuType.GameOver);
    }

    private void ContinueButtonPressed()
    {
        _continueButton.interactable = false;
        ResetWatchAdButton();

        _timer.StopTimer();
        
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.Revive();
        }
    }

    private void ResetWatchAdButton()
    {
        LeanTween.cancel(_continueButton.gameObject);
        _continueButton.transform.localScale = Vector3.one;
    }

    private void GameOver() { MenuManager.GetInstance().SwitchMenu(MenuType.GameOver); }
}