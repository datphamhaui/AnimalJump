using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Slider          loadingSlider; // Kéo thả Slider từ UI vào đây trong Inspector
    public string          nextSceneName = "NextScene"; // Đặt tên scene tiếp theo trong Inspector hoặc code
    public TextMeshProUGUI progressText; // Kéo thả TMP_Text vào đây trong Inspector

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (loadingSlider != null)
        {
            StartCoroutine(FakeLoading());
        }
    }

    System.Collections.IEnumerator FakeLoading()
    {
        loadingSlider.value = 1f; // Start from 1
        if (progressText != null) progressText.text = "1%";
        float duration                              = 2f; // thời gian loading giả lập (giây)
        float elapsed                               = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            loadingSlider.value = Mathf.Lerp(1f, 100f, progress); // Scale from 1 to 100
            if (progressText != null) progressText.text = Mathf.RoundToInt(loadingSlider.value) + "%";

            yield return null;
        }

        loadingSlider.value = 100f; // End at 100
        if (progressText != null) progressText.text = "100%";
        SceneManager.LoadScene(nextSceneName);
    }

    // Update is called once per frame
    void Update() { }

    [ContextMenu("Clear PlayerPrefs")]
    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs cleared.");
    }
}