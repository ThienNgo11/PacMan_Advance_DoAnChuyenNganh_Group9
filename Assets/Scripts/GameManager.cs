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

        //Thêm debug
        Debug.Log("GameManager Awake - Checking references:");
        Debug.Log("Pacman: " + (pacman != null ? "Assigned" : "NULL - PLEASE ASSIGN IN INSPECTOR!"));
        Debug.Log("Pellets: " + (pellets != null ? "Assigned" : "NULL - PLEASE ASSIGN IN INSPECTOR!"));
        Debug.Log("Ghosts count: " + (ghosts != null ? ghosts.Length.ToString() : "0"));
    
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
        //Kiểm tra trước khi chơi
        if (pacman == null)
        {
            Debug.LogError("PACMAN IS NULL! Game cannot start. Please assign Pacman in GameManager Inspector.");
            return; // Dừng lại nếu pacman null
        }
        if (pellets == null)
        {
            Debug.LogError("PELLETS IS NULL! Game cannot start. Please assign Pellets Transform in GameManager Inspector.");
            return;
        }
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

        if (pacman != null)
        {
            pacman.gameObject.SetActive(true); // Thêm dòng này
            pacman.ResetState();
        }
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
            if (ghosts[i] != null && ghosts[i].frightened != null)
        {
            // THÊM KIỂM TRA TRƯỚC KHI ENABLE
            GhostFrightened frightened = ghosts[i].frightened;
            
            // Kiểm tra nếu SpriteRenderers đã được gán
            if (frightened.body == null || frightened.eyes == null || 
                frightened.blue == null || frightened.white == null)
            {
                Debug.LogError($"Cannot enable frightened for {ghosts[i].name}: SpriteRenderers not assigned!");
                continue; // Bỏ qua ghost này
            }
            
            frightened.Enable(pellet.duration);
        }
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
        

        pacman.gameObject.SetActive(false);
        AudioManager.Instance.PlayDeath();
        SetLives(lives - 1);

        if (lives > 0)
        {
            Invoke(nameof(ResetState), 3f);
        }
        else
        {
            GameOver();
        }
    }

}