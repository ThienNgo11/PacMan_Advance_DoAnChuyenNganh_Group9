using UnityEngine;
using TMPro;
using System.Collections;

public class GameUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text timeText;
    public TMP_Text scoreText;
    public TMP_Text livesText; // Optional

    void Start()
    {
        // Kiểm tra nếu các text được gán
        if (timeText == null)
            Debug.LogError("TimeText not assigned to InGameUI!");
        if (scoreText == null)
            Debug.LogError("ScoreText not assigned to InGameUI!");
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
    if (!GameManager.Instance.IsGameActive()) return;
        // Cập nhật UI mỗi frame
        UpdateUI();
    }

    void UpdateUI()
    {
        // Cập nhật thời gian
        if (timeText != null && GameManager.Instance != null)
        {
            timeText.text = GameManager.Instance.GetFormattedTime();
        }
        
        // Cập nhật điểm số
        if (scoreText != null && GameManager.Instance != null)
        {
            scoreText.text =GameManager.Instance.score.ToString("D6"); // 6 chữ số
        }
        
        // Cập nhật số mạng (nếu có)
        if (livesText != null && GameManager.Instance != null)
        {
            livesText.text = "MẠNG: " + GameManager.Instance.lives;
        }
    }
    
    // Hàm để update từ bên ngoài (nếu cần)
    public void RefreshUI()
    {
        UpdateUI();
    }
    
}