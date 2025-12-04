using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Ghost[] ghosts;
    [SerializeField] private Pacman pacman;
    [SerializeField] private Transform pellets; // (Cái này để kiểm tra thắng)

    public int score { get; private set; }
    public int lives { get; private set; }

    private int ghostMultiplier = 1;
    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        SetScore(0);
        SetLives(3);
        NewRound();
        AudioManager.Instance.PlayIntro();
    }

    private void NewRound()
    {
        foreach (Transform pellet in pellets)
        {
            pellet.gameObject.SetActive(true);
        }

        ResetState();
        AudioManager.Instance.PlayIntro();
    }

    private void ResetState()
    {
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].ResetState();
        }
        pacman.ResetState();
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER!"); 

        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].gameObject.SetActive(false);
        }

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

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);
        SetScore(score + pellet.points);

        AudioManager.Instance.PlayMunch();

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
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].frightened.Enable(pellet.duration);
        }

        PelletEaten(pellet);
        ghostMultiplier = 1;
    }

    public void GhostEaten(Ghost ghost)
    {
        int points = ghost.points * ghostMultiplier;
        SetScore(score + points);
        ghostMultiplier++;
        AudioManager.Instance.PlayEatGhost();
        Debug.Log("Ăn ma! Điểm: " + points + " (Hệ số: x" + (ghostMultiplier - 1) + ")");
    }

    public void PacmanEaten()
    {
        // Tắt Pacman đi
        pacman.gameObject.SetActive(false); // (Sau này sẽ thay bằng animation chết)
        AudioManager.Instance.PlayDeath();
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