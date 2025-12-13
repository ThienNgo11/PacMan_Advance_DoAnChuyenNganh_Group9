using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        pauseMenuUI.SetActive(true);

        Time.timeScale = 0f; // DỪNG GAME
        AudioListener.pause = true;
    }

    public void Resume()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);

        Time.timeScale = 1f; // CHẠY LẠI GAME
        AudioListener.pause = false;
    }
}
