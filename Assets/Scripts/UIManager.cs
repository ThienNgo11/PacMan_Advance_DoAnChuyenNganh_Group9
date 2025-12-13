using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // ===== TÊN SCENE (PHẢI TRÙNG 100%) =====
    private const string MAIN_MENU_SCENE = "Mainmenu";
    private const string MODE_SCENE_NAME = "Mode";
    private const string SETTING_SCENE_NAME = "Setting";
    private const string MAP_1_SCENE = "Pacman";
    private const string MAP_2_SCENE = "Map2";

    /// ===== MAIN MENU =====
    public void OpenModeScene()
    {
        Debug.Log("Chuyển sang Scene Mode");
        SceneManager.LoadScene(MODE_SCENE_NAME);
    }

    public void OpenSettingScene()
    {
        Debug.Log("Chuyển sang Scene Setting");
        SceneManager.LoadScene(SETTING_SCENE_NAME);
    }

    public void QuitGame()
    {
        Debug.Log("Thoát khỏi trò chơi");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // ===== MODE SCENE =====
    public void SelectMap1()
    {
        Debug.Log("Chọn Map 1 → Pacman");
        SceneManager.LoadScene(MAP_1_SCENE);
    }

    public void SelectMap2()
    {
        Debug.Log("Chọn Map 2 → Map2");
        SceneManager.LoadScene(MAP_2_SCENE);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Debug.Log("Quay về Mainmenu");
        SceneManager.LoadScene(MAIN_MENU_SCENE);
    }
}
