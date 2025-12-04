using UnityEngine;

public class GhostChase : GhostBehavior
{
    private void OnDisable()
    {
        ghost.scatter.Enable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();

        // Nếu chạm đúng Node VÀ script này đang được bật VÀ Ghost không bị sợ hãi
        if (node != null && enabled && !ghost.frightened.enabled)
        {
            Vector2 direction = Vector2.zero;
            float minDistance = float.MaxValue;

            // Lặp qua tất cả các hướng đi có thể
            foreach (Vector2 availableDirection in node.availableDirections)
            {
                // Tính vị trí mới NẾU đi theo hướng này
                Vector3 newPosition = transform.position + new Vector3(availableDirection.x, availableDirection.y);

                // Tính khoảng cách (bình phương) từ vị trí đó đến mục tiêu (Pacman)
                float distance = (ghost.target.position - newPosition).sqrMagnitude;

                // Nếu khoảng cách này là nhỏ nhất (tốt nhất) từ trước đến giờ
                if (distance < minDistance)
                {
                    direction = availableDirection; // Chọn hướng này
                    minDistance = distance;
                }
            }

            // Ra lệnh cho script Movement đi theo hướng tốt nhất đã tìm được
            ghost.movement.SetDirection(direction);
        }
    }
    private void OnEnable()
    {
        // Ngay khi bật chế độ đuổi, kiểm tra xem có đang đứng trên Node nào không
        // Nếu có, gọi hàm OnTriggerEnter2D thủ công để chọn hướng đi ngay lập tức

        // Dùng OverlapCircle để tìm Node tại vị trí hiện tại (bán kính nhỏ 0.2f)
        Collider2D nodeCollider = Physics2D.OverlapCircle(transform.position, 0.2f, LayerMask.GetMask("Node"));

        if (nodeCollider != null)
        {
            // Tìm thấy Node! Giả lập sự kiện "đi vào" Node
            OnTriggerEnter2D(nodeCollider);
        }
    }
}