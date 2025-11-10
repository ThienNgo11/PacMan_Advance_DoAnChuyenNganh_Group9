using UnityEngine;

public class GhostFrightened : GhostBehavior
{
    // Tham chiếu đến các SpriteRenderer con (sẽ gán trong Prefab)
    public SpriteRenderer body;
    public SpriteRenderer eyes;
    public SpriteRenderer blue;
    public SpriteRenderer white;

    private bool eaten; // Đánh dấu xem đã bị ăn chưa

    // Ghi đè hàm Enable (vì nó cần làm nhiều thứ hơn là chỉ bật)
    public override void Enable(float duration)
    {
        base.Enable(duration); // Gọi hàm Enable của lớp cha (để chạy Invoke)

        // Tắt sprite thân và mắt bình thường
        body.enabled = false;
        eyes.enabled = false;
        // Bật sprite màu xanh
        blue.enabled = true;
        white.enabled = false;

        // Hẹn giờ để gọi hàm Flash (nhấp nháy) sau một nửa thời gian
        Invoke(nameof(Flash), duration / 2f);
    }

    // Ghi đè hàm Disable
    public override void Disable()
    {
        base.Disable(); // Gọi hàm Disable của lớp cha

        // Khôi phục lại sprite thân và mắt bình thường
        body.enabled = true;
        eyes.enabled = true;
        blue.enabled = false;
        white.enabled = false;
    }

    // Hàm này được gọi khi Ghost bị Pacman ăn
    private void Eaten()
    {
        eaten = true;

        // Dịch chuyển Ghost về vị trí "trong chuồng"
        // (Chúng ta cần tạo các điểm này trong Scene đã)
        // Tạm thời bạn cứ code, nếu 'ghost.home.inside' báo lỗi thì kệ nó
        Vector3 position = ghost.home.inside.position;
        position.z = ghost.transform.position.z;
        ghost.transform.position = position;

        // Kích hoạt trạng thái "về nhà" (sẽ được code sau)
        ghost.home.Enable(duration); // Tái sử dụng 'duration' để về nhà

        // Chỉ hiển thị mắt
        body.enabled = false;
        eyes.enabled = true;
        blue.enabled = false;
        white.enabled = false;
    }

    // Hàm đổi sang sprite nhấp nháy
    private void Flash()
    {
        if (!eaten) // Nếu chưa bị ăn
        {
            blue.enabled = false;
            white.enabled = true;
            // Khởi động lại animation nhấp nháy
            white.GetComponent<AnimatedSprite>().Restart();
        }
    }

    // Khi script này được bật (OnEnable), giảm tốc độ Ghost
    private void OnEnable()
    {
        blue.GetComponent<AnimatedSprite>().Restart();
        ghost.movement.speedMultiplier = 0.5f; // Chạy chậm bằng nửa
        eaten = false;
    }

    // Khi script này bị tắt (OnDisable), khôi phục tốc độ
    private void OnDisable()
    {
        ghost.movement.speedMultiplier = 1f; // Tốc độ bình thường
        eaten = false;
    }

    // Logic chọn đường (ngược với Chase)
    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();

        if (node != null && enabled)
        {
            Vector2 direction = Vector2.zero;
            float maxDistance = float.MinValue; // Tìm khoảng cách XA NHẤT

            // Lặp qua các hướng đi
            foreach (Vector2 availableDirection in node.availableDirections)
            {
                Vector3 newPosition = transform.position + new Vector3(availableDirection.x, availableDirection.y);
                float distance = (ghost.target.position - newPosition).sqrMagnitude;

                // Nếu khoảng cách này là LỚN NHẤT
                if (distance > maxDistance)
                {
                    direction = availableDirection; // Chọn hướng này
                    maxDistance = distance;
                }
            }

            ghost.movement.SetDirection(direction);
        }
    }

    // Ghi đè hàm OnCollisionEnter2D (vì GhostFrightened tự xử lý va chạm)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            if (enabled)
            { // Nếu đang ở trạng thái sợ hãi
                Eaten(); // Bị ăn
            }
        }
    }
}