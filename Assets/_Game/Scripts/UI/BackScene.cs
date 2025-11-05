using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackScene : MonoBehaviour
{
    private Button _backButton;
    public  string sceneNameText;

    private void Awake() { _backButton = GetComponent<Button>(); }

    private void OnEnable()
    {
        if (_backButton != null)
        {
            _backButton.onClick.AddListener(OnBackButtonClicked);
        }
    }

    private void OnDisable()
    {
        if (_backButton != null)
        {
            _backButton.onClick.RemoveListener(OnBackButtonClicked);
        }
    }

    private void OnBackButtonClicked() { SceneManager.LoadScene(this.sceneNameText); }
}