using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    // Layer của tường (chúng ta sẽ gán nó trong Prefab)
    public LayerMask obstacleLayer;

    // Danh sách các hướng đi có thể từ Node này (không vướng tường)
    public readonly List<Vector2> availableDirections = new List<Vector2>();

    private void Start()
    {
        availableDirections.Clear();

        // Chúng ta kiểm tra 4 hướng
        // Dùng CircleCast (giống như "mắt tròn" của Pacman) để xem có vướng tường không
        CheckAvailableDirection(Vector2.up);
        CheckAvailableDirection(Vector2.down);
        CheckAvailableDirection(Vector2.left);
        CheckAvailableDirection(Vector2.right);
    }

    private void CheckAvailableDirection(Vector2 direction)
    {
        // Bắn một CircleCast với bán kính 0.25f, đi một đoạn ngắn 1f
        // và chỉ kiểm tra va chạm với Layer "Obstacle"
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.25f, direction, 1f, obstacleLayer);

        // Nếu hit.collider là null (không bắn trúng gì), nghĩa là đường đó trống
        if (hit.collider == null)
        {
            availableDirections.Add(direction);
        }
    }
}