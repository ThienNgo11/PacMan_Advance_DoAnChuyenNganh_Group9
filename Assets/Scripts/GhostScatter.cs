using UnityEngine;

public class GhostScatter : GhostBehavior
{
    // Khi trạng thái Scatter bị TẮT (Disable), tự động BẬT (Enable) trạng thái Chase
    private void OnDisable()
    {
        ghost.chase.Enable();
    }

    // Hàm này được gọi khi Ghost (có CircleCollider) chạm vào một Node (có BoxCollider "Is Trigger")
    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();

        // Nếu chạm đúng Node VÀ script này đang được bật (enabled) VÀ Ghost không bị sợ hãi
        if (node != null && enabled && !ghost.frightened.enabled)
        {
            // 1. Chọn một hướng ngẫu nhiên từ danh sách các hướng đi có thể của Node
            int index = Random.Range(0, node.availableDirections.Count);

            // 2. Ưu tiên không quay đầu (Nếu hướng ngẫu nhiên là quay ngược lại)
            if (node.availableDirections.Count > 1 && node.availableDirections[index] == -ghost.movement.direction)
            {
                index++; // Chọn hướng tiếp theo trong danh sách

                // Nếu chọn lố danh sách, quay về 0
                if (index >= node.availableDirections.Count)
                {
                    index = 0;
                }
            }

            // 3. Ra lệnh cho script Movement đi theo hướng đã chọn
            ghost.movement.SetDirection(node.availableDirections[index]);
        }
    }
}