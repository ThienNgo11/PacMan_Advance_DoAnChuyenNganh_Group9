using UnityEngine;

public class GhostChase : GhostBehavior
{
    // Khi trạng thái Chase bị TẮT (Disable), tự động BẬT (Enable) trạng thái Scatter
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
            float minDistance = float.MaxValue; // Đặt khoảng cách nhỏ nhất là vô cực

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
}