using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public static Pathfinder Instance { get; private set; }

    private List<Node> allNodes;
    private List<Vector3> debugPath = new List<Vector3>();
    private Dictionary<Vector2Int, Node> nodeGrid;

    [Header("Settings")]
    public float maxSearchDistance = 10f; // Khoảng cách tối đa để tìm Node tiếp theo

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            InitializeNodes();
        }
    }

    private void InitializeNodes()
    {
        allNodes = new List<Node>(FindObjectsOfType<Node>());
        nodeGrid = new Dictionary<Vector2Int, Node>();

        foreach (Node node in allNodes)
        {
            Vector2Int gridPos = ToGrid(node.transform.position);
            if (!nodeGrid.ContainsKey(gridPos))
            {
                nodeGrid.Add(gridPos, node);
            }
        }
        Debug.Log($"Pathfinder: Loaded {nodeGrid.Count} nodes into grid.");
    }

    public Vector2 FindDirectionToTarget(Vector2 start, Vector2 target)
    {
        Node startNode = GetNodeAtPosition(ToGrid(start)) ?? FindNearestNode(start);
        Node targetNode = GetNodeAtPosition(ToGrid(target)) ?? FindNearestNode(target);

        if (startNode == null || targetNode == null) return Vector2.zero;

        // Nếu Ghost và Pacman đang ở cùng một Node, hãy di chuyển thẳng tới vị trí thực tế của Pacman
        if (startNode == targetNode)
        {
            // Trả về hướng từ vị trí hiện tại tới vị trí đích (không phải vị trí Node)
            return (target - start).normalized;
        }
        // ----------------------------

        // BFS
        Queue<Node> queue = new Queue<Node>();
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();

        queue.Enqueue(startNode);
        cameFrom[startNode] = null;

        Node reachedTarget = null;

        while (queue.Count > 0)
        {
            Node current = queue.Dequeue();

            if (current == targetNode)
            {
                reachedTarget = current;
                break;
            }

            foreach (Vector2 dir in current.availableDirections)
            {
                // SỬ DỤNG HÀM THÔNG MINH ĐỂ TÌM HÀNG XÓM
                Node neighbor = GetNextNodeInDirection(current, dir);

                if (neighbor != null && !cameFrom.ContainsKey(neighbor))
                {
                    cameFrom[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }

        if (reachedTarget != null)
        {
            UpdateDebugPath(reachedTarget, cameFrom);

            // Tìm hướng đi đầu tiên
            Node step = reachedTarget;
            
            //logic truy ngược vết
            while (cameFrom[step] != null && cameFrom[step] != startNode)
            {
                step = cameFrom[step];
            }
            return (step.transform.position - startNode.transform.position).normalized;
        }

        debugPath.Clear();
        return Vector2.zero;
    }

    // HÀM THÔNG MINH: Dò tìm Node tiếp theo theo hướng chỉ định
    private Node GetNextNodeInDirection(Node startNode, Vector2 direction)
    {
        // Thử dò từ 1 đơn vị đến maxSearchDistance
        for (float d = 1; d <= maxSearchDistance; d++)
        {
            Vector2 checkPos = (Vector2)startNode.transform.position + (direction * d);
            Node found = GetNodeAtPosition(ToGrid(checkPos));

            if (found != null) return found;
        }
        return null;
    }

    private Vector2Int ToGrid(Vector2 pos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(pos.x),
            Mathf.FloorToInt(pos.y)
        );
    }

    private Node GetNodeAtPosition(Vector2Int gridPos)
    {
        return nodeGrid.TryGetValue(gridPos, out Node node) ? node : null;
    }

    private void UpdateDebugPath(Node endNode, Dictionary<Node, Node> cameFrom)
    {
        debugPath.Clear();
        Node current = endNode;
        while (current != null)
        {
            debugPath.Add(current.transform.position);
            current = cameFrom[current];
        }
        debugPath.Reverse();
    }

    private void OnDrawGizmos()
    {
        if (debugPath == null || debugPath.Count < 2) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < debugPath.Count - 1; i++)
        {
            Gizmos.DrawLine(debugPath[i], debugPath[i + 1]);
            Gizmos.DrawSphere(debugPath[i], 0.2f);
        }
    }

    private Node FindNearestNode(Vector2 position)
    {
        Node nearest = null;
        float minDistance = float.MaxValue;
        foreach (Node node in allNodes)
        {
            float distance = Vector2.Distance(position, node.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = node;
            }
        }
        return nearest;
    }
}