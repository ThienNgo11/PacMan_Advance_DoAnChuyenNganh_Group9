using UnityEngine;

// Yêu cầu GameObject phải có Rigidbody2D
[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public float speed = 8f;
    public float speedMultiplier = 1f;
    public Vector2 initialDirection; // Hướng đi ban đầu (sẽ đặt trong Inspector)
    public LayerMask obstacleLayer;  // Layer của tường (sẽ đặt trong Inspector)

    public Rigidbody2D rb { get; private set; }
    public Vector2 direction { get; private set; }
    public Vector2 nextDirection { get; private set; } // Hướng đi "xếp hàng"
    public Vector3 startingPosition { get; private set; }

    private void Awake()
    {
        // Lấy component Rigidbody2D và lưu lại để dùng
        rb = GetComponent<Rigidbody2D>();
        // Lưu lại vị trí ban đầu để reset game
        startingPosition = transform.position;
    }

    private void Start()
    {
        ResetState();
    }

    public void ResetState()
    {
        speedMultiplier = 1f;
        direction = initialDirection;
        nextDirection = Vector2.zero;
        transform.position = startingPosition;

        // --- BẮT ĐẦU SỬA LỖI ---
        // 1. Dùng code MỚI để ép nó thành Dynamic (thay cho isKinematic = false)
        rb.bodyType = RigidbodyType2D.Dynamic;

        // 2. Tắt trọng lực (vì giờ nó là Dynamic)
        rb.gravityScale = 0f;
        // --- KẾT THÚC SỬA LỖI ---

        enabled = true; // Đảm bảo script này được bật
    }

    // Tạm thời thêm code input vào Update để kiểm tra
    // Sau này chúng ta sẽ chuyển nó sang script 'Pacman.cs'
    private void Update()
    {
        if (nextDirection != Vector2.zero)
        {
            SetDirection(nextDirection);
        }
    }

    // Di chuyển vật lý diễn ra ở FixedUpdate
    private void FixedUpdate()
    {
        Vector2 position = rb.position;
        Vector2 translation = speed * speedMultiplier * Time.fixedDeltaTime * direction;

        // Đây là hàm mấu chốt!
        // MovePosition sẽ di chuyển và TỰ ĐỘNG dừng lại khi gặp collider
        rb.MovePosition(position + translation);
    }

    public void SetDirection(Vector2 direction, bool forced = false)
    {
        if (forced || !Occupied(direction))
        {
            this.direction = direction;
            nextDirection = Vector2.zero;
        }
        else
        {
            // Nếu vướng tường, lưu vào 'nextDirection'
            nextDirection = direction;
        }
    }

    // Kiểm tra xem có tường ở hướng sắp đi không
    public bool Occupied(Vector2 direction)
    {
        // Dùng BoxCast (giống như bắn một cái hộp) để kiểm tra va chạm
        // Kích thước 0.75f nhỏ hơn 1 unit để dễ đi qua kẽ hở
        // Khoảng cách 1.5f để nó "nhìn" đủ xa
        // Bắn một "viên đạn" hình tròn (bán kính 0.25) đi một khoảng cách ngắn (0.75f)
        // để kiểm tra xem có tường không.
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.25f, direction, 0.75f, obstacleLayer);

        // Nếu hit.collider khác null, nghĩa là có vật cản
        return hit.collider != null;
    }
}