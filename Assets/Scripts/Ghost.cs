using UnityEngine;
using System.Collections;


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
    public enum GhostType { Blinky, Pinky, Inky, Clyde }
    [SerializeField] private GhostType ghostType = GhostType.Blinky;

    // Hành vi ban đầu khi game bắt đầu (ví dụ: ở trong nhà, hoặc đi lảng vảng)
    public GhostBehavior initialBehavior;
    [SerializeField] private float homeExitDelay = 2f;
    // Mục tiêu của Ghost (chính là Pacman)
    public Transform target;

    // Điểm thưởng khi bị ăn
    public int points = 200;
    private GhostManager ghostManager;
    private bool isLeavingHome = false;
    private void Awake()
    {
        // Lấy tất cả các component script trên cùng GameObject này
        movement = GetComponent<Movement>();
        home = GetComponent<GhostHome>();
        scatter = GetComponent<GhostScatter>();
        chase = GetComponent<GhostChase>();
        frightened = GetComponent<GhostFrightened>();

        ghostManager = FindObjectOfType<GhostManager>();
        if (ghostManager == null)
        {
            GameObject managerObj = new GameObject("GhostManager");
            ghostManager = managerObj.AddComponent<GhostManager>();
        }
    }

    private void Start()
    {
        // Đảm bảo target được gán
        if (target == null)
        {
            GameObject pacman = GameObject.FindGameObjectWithTag("Pacman");
            if (pacman != null)
            {
                target = pacman.transform;
                Debug.Log($"{name}: Auto-assigned target to {target.name}");
            }
            else
            {
                Debug.LogError($"{name}: Cannot find Pacman in scene!");
            }
        }
        ResetState();
    }

    public void ResetState()
    {
        gameObject.SetActive(true);
        movement.ResetState(); // Reset script di chuyển

        // Đặt vị trí về trong nhà nếu có
        if (home != null && home.inside != null)
        {
            transform.position = home.inside.position;
            Debug.Log($"{name} reset to home position");
        }

        // Tắt tất cả behaviors TRỪ home
        frightened.Disable();
        chase.Disable();
        scatter.Disable();

        // QUAN TRỌNG: KHÔNG gọi home.Disable() ở đây!
        // Thay vào đó, BẬT GhostHome và chờ để ra ngoài
        if (home != null)
        {
            // Bật GhostHome và bắt đầu quá trình ra khỏi nhà
            home.Enable();

            // Bắt đầu coroutine để ra khỏi nhà sau delay
            StartCoroutine(LeaveHomeRoutine());
        }
        else
        {
            Debug.LogError($"{name}: No GhostHome component found!");
            // Fallback: bật initialBehavior nếu không có home
            if (initialBehavior != null)
                initialBehavior.Enable();
        }
    }

    // Hàm này dùng để xử lý va chạm với Pacman
    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     // Kiểm tra xem có va chạm với Layer "Pacman" không
    //     if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
    //     {

    //         if (GameManager.Instance == null)
    //     {
    //         Debug.LogError("GameManager.Instance is null!");
    //         return;
    //     }

    //         // Nếu Ghost đang ở trạng thái "sợ hãi" (frightened)
    //         if (frightened.enabled)
    //         {
    //             // Ghost bị ăn -> Báo cho GameManager
    //             GameManager.Instance.GhostEaten(this);
    //         }
    //         else
    //         {
    //             // Nếu Ghost đang bình thường -> Pacman bị ăn
    //             GameManager.Instance.PacmanEaten();
    //         }
    //     }
    // }
    private void SetExitDelayByType()
    {
        switch (ghostType)
        {
            case GhostType.Blinky:
                homeExitDelay = 0f;     // Blinky ra đầu tiên (ngay lập tức)
                break;
            case GhostType.Pinky:
                homeExitDelay = 2f;     // Pinky ra sau 2 giây
                break;
            case GhostType.Inky:
                homeExitDelay = 4f;     // Inky ra sau 4 giây
                break;
            case GhostType.Clyde:
                homeExitDelay = 6f;     // Clyde ra sau 6 giây
                break;
        }
    }
    private IEnumerator LeaveHomeRoutine()
    {
        Debug.Log($"{name}: Waiting {homeExitDelay}s before leaving home");

        // Chờ thời gian delay riêng cho từng ghost
        yield return new WaitForSeconds(homeExitDelay);

        if (home != null && home.enabled)
        {
            Debug.Log($"{name}: Disabling GhostHome to exit");
            home.Disable();

            // Chờ cho ExitTransition hoàn thành
            yield return new WaitForSeconds(1f);

            if (initialBehavior != null)
            {
                Debug.Log($"{name}: Enabling initial behavior: {initialBehavior.GetType().Name}");
                initialBehavior.Enable();
            }
        }
    }


    // Thêm phương thức để gọi từ bên ngoài nếu cần
    public void ForceLeaveHome()
    {
        if (home != null && home.enabled)
        {
            StopAllCoroutines();
            home.Disable();

            if (initialBehavior != null)
                initialBehavior.Enable();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("GameManager.Instance is null!");
                return;
            }

            if (frightened.enabled)
            {
                GameManager.Instance.GhostEaten(this);
            }
            else
            {
                GameManager.Instance.PacmanEaten();
            }
        }
    }


}