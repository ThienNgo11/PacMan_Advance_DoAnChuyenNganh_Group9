using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public LayerMask obstacleLayer;
    public List<Vector2> availableDirections { get; private set; }

    private void Start()
    {
        availableDirections = new List<Vector2>();
        
        // THÊM DEBUG ĐỂ KIỂM TRA
        Debug.Log($"Node {name} at {transform.position} starting direction check");
        
        CheckAvailableDirection(Vector2.up);
        CheckAvailableDirection(Vector2.down);
        CheckAvailableDirection(Vector2.left);
        CheckAvailableDirection(Vector2.right);
        
        // DEBUG: In kết quả
        Debug.Log($"Node {name} at {transform.position} has {availableDirections.Count} directions");
        foreach (Vector2 dir in availableDirections)
        {
            Debug.Log($"  -> {dir}");
        }
    }

    private void CheckAvailableDirection(Vector2 direction)
    {
        // SỬA: Dùng Linecast thay vì CircleCast cho chính xác
        Vector2 start = transform.position;
        Vector2 end = start + direction; // Chỉ cần 1 unit vì maze grid là 1x1
        
        RaycastHit2D hit = Physics2D.Linecast(start, end, obstacleLayer);
        
        // DEBUG CHI TIẾT
        if (hit.collider != null)
        {
            Debug.Log($"  Node {name}: {direction} BLOCKED by {hit.collider.name}");
        }
        else
        {
            availableDirections.Add(direction);
            Debug.Log($"  Node {name}: {direction} is FREE");
        }
    }

    // THÊM: Visual debug trong Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.1f);
        
        if (availableDirections != null)
        {
            Gizmos.color = Color.yellow;
            foreach (Vector2 direction in availableDirections)
            {
                Gizmos.DrawRay(transform.position, direction * 0.5f);
            }
        }
    }
}