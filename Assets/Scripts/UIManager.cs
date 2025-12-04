using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // --- Tham chiếu các Panel (Gán trong Inspector) ---
    // Gán GameObject của các Panel chính/phụ vào đây
    public GameObject mainPanel;      // Panel chứa TUTORIAL, MODE, SETTING, QUIT
    public GameObject modePanel;      // Panel chứa Casual, Survival, Power
    public GameObject settingPanel;   // Panel chứa cài đặt Âm thanh, Trang phục

    // --- Cấu hình Scene (Thay đổi tên Scene của bạn ở đây) ---
    private const string TUTORIAL_SCENE_NAME = "Pacman";
    private const string GAME_SCENE_NAME = "Map2"; 

    private void Start()
    {
        // Đảm bảo chỉ Panel chính được bật khi Scene Menu khởi động
        OpenMainPanel();
    }

    

    // Hàm gắn vào Button TUTORIAL
    public void PlayTutorialMode()
    {
        Debug.Log("Bắt đầu chế độ Hướng dẫn...");
        // Tải Scene hướng dẫn
        SceneManager.LoadScene(TUTORIAL_SCENE_NAME); 
    }

        // Hàm gắn vào các Button trong Panel MODE (Casual, Survival, Power)
    public void StartGameMode(string modeName)
    {
        Debug.Log($"Bắt đầu game với chế độ: {modeName}");
        
        // Lưu chế độ chơi đã chọn (để đọc lại trong Scene Game)
        PlayerPrefs.SetString("GameMode", modeName);
        
        // Tải Scene chơi chính
        SceneManager.LoadScene(GAME_SCENE_NAME); 
    }

    // Hàm gắn vào Button QUIT
    public void QuitGame()
    {
        Debug.Log("Thoát khỏi trò chơi!");
        Application.Quit();

        // Đoạn code này chỉ dùng để dừng chơi khi đang chạy trong Unity Editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    

    // Hàm gắn vào Button MODE
    public void OpenModePanel()
    {
        mainPanel.SetActive(false);
        modePanel.SetActive(true);
        settingPanel.SetActive(false);
    }

    // Hàm gắn vào Button SETTING
    public void OpenSettingPanel()
    {
        mainPanel.SetActive(false);
        modePanel.SetActive(false);
        settingPanel.SetActive(true);
    }

    // Hàm gắn vào các Button BACK (Quay lại Menu Chính)
    public void OpenMainPanel()
    {
        mainPanel.SetActive(true);
        modePanel.SetActive(false);
        settingPanel.SetActive(false);
    }
}