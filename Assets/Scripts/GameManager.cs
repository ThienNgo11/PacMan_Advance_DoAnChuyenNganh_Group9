using UnityEngine;

// Đặt thứ tự thực thi của script này là -100
// Đảm bảo nó chạy TRƯỚC HẦU HẾT các script khác
[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    // Đây là "Singleton Pattern" - một cách để mọi script khác
    // có thể truy cập GameManager một cách dễ dàng thông qua "GameManager.Instance"
    public static GameManager Instance { get; private set; }

    [SerializeField] private Ghost[] ghosts;
    [SerializeField] private Pacman pacman;
    [SerializeField] private Transform pellets; // (Cái này để kiểm tra thắng)

    public int score { get; private set; }
    public int lives { get; private set; }

    // Hàm Awake chạy trước cả hàm Start
    private void Awake()
    {
        // Thiết lập Singleton
        if (Instance != null)
        {
            // Nếu đã có một GameManager khác -> phá hủy cái mới này
            DestroyImmediate(gameObject);
        }
        else
        {
            // Nếu chưa có -> gán "Instance" là chính nó (this)
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        // Khi scene bị hủy, gán Instance về null
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Start chạy khi game bắt đầu
    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        SetScore(0);
        SetLives(3); // Giả sử bắt đầu với 3 mạng
        NewRound();
    }

    private void NewRound()
    {
        // Kích hoạt lại tất cả các hạt pellet
        foreach (Transform pellet in pellets)
        {
            pellet.gameObject.SetActive(true);
        }

        ResetState();
    }

    // Hàm reset vị trí Pacman và Ghost
    private void ResetState()
    {
        // Reset tất cả Ghost
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].ResetState();
        }

        // Reset Pacman
        pacman.ResetState();
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER!"); // Tạm thời

        // Tắt tất cả Ghost
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].gameObject.SetActive(false);
        }

        // Tắt Pacman
        pacman.gameObject.SetActive(false);
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        Debug.Log("Lives: " + this.lives); // Tạm thời
    }

    private void SetScore(int score)
    {
        this.score = score;
        Debug.Log("Score: " + this.score); // Tạm thời debug

        // (Code cập nhật UI Text sẽ ở đây)
    }

    // Đây là hàm mà Pellet sẽ gọi
    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);
        SetScore(score + pellet.points);

        // KIỂM TRA THẮNG:
        if (!HasRemainingPellets())
        {
            pacman.gameObject.SetActive(false); // Tắt Pacman đi
            Invoke(nameof(NewRound), 3f); // Bắt đầu màn mới sau 3 giây
        }
    }

    // Hàm private để kiểm tra xem còn hạt nào không
    private bool HasRemainingPellets()
    {
        foreach (Transform pellet in pellets)
        {
            if (pellet.gameObject.activeSelf)
            {
                return true; // Vẫn còn
            }
        }

        return false; // Hết rồi!
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        // ----- Code cho Power Pellet -----
        // (Sau này chúng ta sẽ thêm code làm Ghost sợ hãi ở đây)
        Debug.Log("ĂN HẠT SỨC MẠNH!");
        // ---------------------------------

        // Gọi hàm PelletEaten cơ bản để cộng điểm và làm biến mất hạt
        PelletEaten(pellet);

        // (Sau này chúng ta sẽ thêm code reset bộ đếm ăn ma ở đây)
    }

    // THÊM HÀM NÀY ĐỂ SỬA LỖI CS1061:
    public void GhostEaten(Ghost ghost)
    {
        // (Sau này sẽ thêm logic nhân điểm)
        int points = ghost.points;
        SetScore(score + points);

        Debug.Log("Ăn ma, được " + points + " điểm!");
    }

    // THÊM HÀM NÀY ĐỂ SỬA LỖI CÒN LẠI:
    public void PacmanEaten()
    {
        // Tắt Pacman đi
        pacman.gameObject.SetActive(false); // (Sau này sẽ thay bằng animation chết)

        SetLives(lives - 1);

        if (lives > 0)
        {
            // Nếu còn mạng, gọi ResetState sau 3 giây
            Invoke(nameof(ResetState), 3f);
        }
        else
        {
            // Nếu hết mạng, gọi Game Over
            GameOver();
        }
    }
}