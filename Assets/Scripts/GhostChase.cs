using UnityEngine;
using System.Collections.Generic;

public class GhostChase : GhostBehavior
{
    
    private void OnDisable()
    {
        if (ghost != null && ghost.scatter != null)
        {
            ghost.scatter.Enable();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();

        // Nếu chạm đúng Node VÀ script này đang được bật VÀ Ghost không bị sợ hãi
        if (node != null && enabled && ghost != null && !ghost.frightened.enabled)
        {
            // KIỂM TRA TARGET TRƯỚC
            if (ghost.target == null)
            {
                Debug.LogWarning($"{ghost.name}: Target is null! Cannot chase.");
                return;
            }

            Vector2 direction = GetChaseDirection(node);

            // // Lặp qua tất cả các hướng đi có thể
            // foreach (Vector2 availableDirection in node.availableDirections)
            // {
            //     // Tính vị trí mới NẾU đi theo hướng này
            //     Vector3 newPosition = transform.position + new Vector3(availableDirection.x, availableDirection.y);

            //     // Tính khoảng cách (bình phương) từ vị trí đó đến mục tiêu (Pacman)
            //     float distance = (ghost.target.position - newPosition).sqrMagnitude;

            //     // Nếu khoảng cách này là nhỏ nhất (tốt nhất) từ trước đến giờ
            //     if (distance < minDistance)
            //     {
            //         direction = availableDirection; // Chọn hướng này
            //         minDistance = distance;
            //     }
            // }

            // Ra lệnh cho script Movement đi theo hướng tốt nhất đã tìm được
            if (ghost.movement != null && direction != Vector2.zero)
            {
                ghost.movement.SetDirection(direction);
            }
        }
    }
    private void OnEnable()
    {
        if (ghost == null)
        {
            Debug.LogError("ghost null");
            return;
        }
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

    private Vector2 GetChaseDirection(Node currentNode)
    {

        if (ghost == null)
        {
            Debug.LogError("Ghost is null in GetChaseDirection!");
            return Vector2.zero;
        }
        
        if (ghost.target == null)
        {
            Debug.LogWarning($"{ghost.name}: Target is null, using random direction");
            return GetRandomDirection(currentNode);
        }
        
        if (currentNode == null)
        {
            Debug.LogError("CurrentNode is null!");
            return Vector2.zero;
        }
        
        // Sử dụng BFS để tìm đường ngắn nhất đến Pac-Man
        if (Pathfinder.Instance != null && ghost.target != null)
        {
            Vector2 ghostPos = ghost.transform.position;
            Vector2 targetPos = ghost.target.position;
            
            // DEBUG THÊM
            Debug.Log($"{ghost.name}: Finding path from {ghostPos} to {targetPos}");
            
            Vector2 bfsDirection = Pathfinder.Instance.FindDirectionToTarget(ghostPos, targetPos);
            
            // Nếu BFS tìm được hướng
            if (bfsDirection != Vector2.zero)
            {
                Debug.Log($"{ghost.name}: BFS found direction {bfsDirection}");
                return bfsDirection;
            }
            else
            {
                Debug.LogWarning($"{ghost.name}: BFS returned zero direction");
            }
        }
        else
        {
            Debug.LogWarning("Pathfinder.Instance is null!");
        }

         // Fallback: dùng thuật toán greedy
        Debug.Log($"{ghost.name}: Using greedy fallback");
        return GetGreedyDirection(currentNode);
    }
    private bool IsValidDirection(Vector2 direction, Node node)
    {
        // Kiểm tra xem hướng này có trong danh sách availableDirections không
        foreach (Vector2 availableDir in node.availableDirections)
        {
            if (Vector2.Dot(direction.normalized, availableDir.normalized) > 0.9f)
            {
                return true;
            }
        }
        return false;
    }

    private Vector2 GetGreedyDirection(Node node)
    {
        if (node == null || node.availableDirections == null || ghost == null || ghost.target == null)
        {
            Debug.LogError("Cannot get greedy direction - missing references");
            return GetRandomDirection(node);
        }
        
        Vector2 direction = Vector2.zero;
        float minDistance = float.MaxValue;

        foreach (Vector2 availableDirection in node.availableDirections)
        {
            // Tránh quay đầu trừ khi bắt buộc
            if (availableDirection == -ghost.movement.direction && node.availableDirections.Count > 1)
                continue;

            Vector3 newPosition = transform.position + new Vector3(availableDirection.x, availableDirection.y);
            float distance = (ghost.target.position - newPosition).sqrMagnitude;

            if (distance < minDistance)
            {
                direction = availableDirection;
                minDistance = distance;
            }
        }

        // Nếu không tìm được hướng
        if (direction == Vector2.zero && node.availableDirections.Count > 0)
        {
            direction = node.availableDirections[0];
        }

        return direction;
    }
    private Vector2 GetRandomDirection(Node node)
    {
        if (node == null || node.availableDirections == null || node.availableDirections.Count == 0)
            return Vector2.zero;
        
        // Loại bỏ hướng ngược lại nếu có thể
        List<Vector2> validDirections = new List<Vector2>(node.availableDirections);
        
        if (ghost != null && ghost.movement != null && validDirections.Count > 1)
        {
            validDirections.Remove(-ghost.movement.direction);
        }
        
        if (validDirections.Count == 0)
            validDirections = new List<Vector2>(node.availableDirections);
        
        int randomIndex = Random.Range(0, validDirections.Count);
        return validDirections[randomIndex];
    }
}