using UnityEngine;

// Ghost cũng cần di chuyển, nên nó cũng dùng script Movement
[RequireComponent(typeof(Movement))]
public class Ghost : MonoBehaviour
{
    // Tham chiếu đến các component quan trọng
    public Movement movement { get; private set; }

    // Tham chiếu đến TẤT CẢ các script hành vi (State)
    // Chúng ta sẽ tạo các script này ở bước sau, nhưng ta khai báo trước
    public GhostHome home { get; private set; }
    public GhostScatter scatter { get; private set; }
    public GhostChase chase { get; private set; }
    public GhostFrightened frightened { get; private set; }

    // Hành vi ban đầu khi game bắt đầu (ví dụ: ở trong nhà, hoặc đi lảng vảng)
    public GhostBehavior initialBehavior;

    // Mục tiêu của Ghost (chính là Pacman)
    public Transform target;

    // Điểm thưởng khi bị ăn
    public int points = 200;

    private void Awake()
    {
        // Lấy tất cả các component script trên cùng GameObject này
        movement = GetComponent<Movement>();
        home = GetComponent<GhostHome>();
        scatter = GetComponent<GhostScatter>();
        chase = GetComponent<GhostChase>();
        frightened = GetComponent<GhostFrightened>();
    }

    private void Start()
    {
        ResetState();
    }

    public void ResetState()
    {
        gameObject.SetActive(true);
        movement.ResetState(); // Reset script di chuyển

        // Tắt hết mọi trạng thái
        frightened.Disable();
        chase.Disable();
        scatter.Disable();
        home.Disable();

        // Bật trạng thái ban đầu
        if (initialBehavior != null)
        {
            initialBehavior.Enable();
        }
    }

    // Hàm này dùng để xử lý va chạm với Pacman
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra xem có va chạm với Layer "Pacman" không
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            // Nếu Ghost đang ở trạng thái "sợ hãi" (frightened)
            if (frightened.enabled)
            {
                // Ghost bị ăn -> Báo cho GameManager
                GameManager.Instance.GhostEaten(this);
            }
            else
            {
                // Nếu Ghost đang bình thường -> Pacman bị ăn
                GameManager.Instance.PacmanEaten();
            }
        }
    }
}