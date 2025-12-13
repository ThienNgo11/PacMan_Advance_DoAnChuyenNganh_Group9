using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text totalScoreText;
    public TMP_Text timePlayText;

    private void Awake()
    {
        gameObject.SetActive(false); // Ẩn mặc định
    }
    public void Show()
{
    Debug.Log("GameOverUI.Show() called");
    
    // THÊM: Kiểm tra GameObject
    Debug.Log($"GameObject active before: {gameObject.activeSelf}");
    Debug.Log($"GameObject name: {gameObject.name}");
    Debug.Log($"GameObject parent: {gameObject.transform.parent?.name}");
    
    gameObject.SetActive(true);
    
    // THÊM: Kiểm tra components
    if (totalScoreText == null)
        Debug.LogError("totalScoreText is not assigned!");
    else
        Debug.Log($"totalScoreText assigned: {totalScoreText.gameObject.name}");
    
    if (timePlayText == null)
        Debug.LogError("timePlayText is not assigned!");
    else
        Debug.Log($"timePlayText assigned: {timePlayText.gameObject.name}");
    
    if (GameManager.Instance != null)
    {
        Debug.Log($"Score: {GameManager.Instance.score}, Time: {GameManager.Instance.GetFormattedTime()}");
        if (totalScoreText != null)
            totalScoreText.text = "Total Score: " + GameManager.Instance.score.ToString("D6");
        if (timePlayText != null)
            timePlayText.text = "Time Play: " + GameManager.Instance.GetFormattedTime();
    }
    else
    {
        Debug.LogError("GameManager.Instance is null!");
    }

    Time.timeScale = 0f;
    Debug.Log("Game paused (Time.timeScale = 0)");
    
    // THÊM: Kiểm tra RectTransform
    RectTransform rect = GetComponent<RectTransform>();
    if (rect != null)
    {
        Debug.Log($"RectTransform position: {rect.anchoredPosition}, size: {rect.sizeDelta}");
    }
}

    // ===== BUTTON =====
    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }
}
