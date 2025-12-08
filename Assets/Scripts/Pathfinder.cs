using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public static Pathfinder Instance { get; private set; }

    private List<Node> allNodes;
    private Dictionary<Vector2Int, Node> nodeGrid;

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
    
    private void Start()
{
    Debug.Log($"=== PATHFINDER START ===");
        Debug.Log($"Total nodes found: {allNodes?.Count ?? 0}");
        Debug.Log($"Grid entries: {nodeGrid?.Count ?? 0}");
    
    // Kiểm tra xem có tìm thấy Pac-Man không
    Ghost[] ghosts = FindObjectsOfType<Ghost>();
    foreach (Ghost ghost in ghosts)
    {
        if (ghost.target != null)
        {
            Debug.Log($"{ghost.name} target: {ghost.target.name}");
        }
    }
}

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void InitializeNodes()
    {
        allNodes = new List<Node>(FindObjectsOfType<Node>());
        nodeGrid = new Dictionary<Vector2Int, Node>();

        // THÊM DEBUG
        Debug.Log($"Found {allNodes.Count} nodes in scene");

        foreach (Node node in allNodes)
        {
            // SỬA: Dùng Vector2Int để tránh floating point errors
            Vector2Int gridPos = new Vector2Int(
                Mathf.RoundToInt(node.transform.position.x),
                Mathf.RoundToInt(node.transform.position.y)
            );
            
            if (!nodeGrid.ContainsKey(gridPos))
            {
                nodeGrid.Add(gridPos, node);
                Debug.Log($"Added node to grid: {gridPos} -> {node.name}");
            }
            else
            {
                Debug.LogWarning($"Duplicate node position at {gridPos}! Already has {nodeGrid[gridPos].name}");
            }
        }
    }

    /// <summary>
    /// Tìm đường từ vị trí start đến target sử dụng BFS
    /// </summary>
    /// <param name="start">Vị trí bắt đầu</param>
    /// <param name="target">Vị trí mục tiêu</param>
    /// <returns>Hướng đi đầu tiên cần thực hiện</returns>
    public Vector2 FindDirectionToTarget(Vector2 start, Vector2 target)
    {
        // SỬA: Làm tròn chính xác hơn
        Vector2Int startGrid = new Vector2Int(
            Mathf.RoundToInt(start.x),
            Mathf.RoundToInt(start.y)
        );
        Vector2Int targetGrid = new Vector2Int(
            Mathf.RoundToInt(target.x),
            Mathf.RoundToInt(target.y)
        );

        Debug.Log($"FindDirectionToTarget: {start} -> {target}");
        Debug.Log($"Grid positions: {startGrid} -> {targetGrid}");

        // Nếu đã ở cùng vị trí
        if (startGrid == targetGrid)
        {
            Debug.Log("Already at target position");
            return Vector2.zero;
        }

        // Tìm node
        Node startNode = GetNodeAtPosition(startGrid);
        Node targetNode = GetNodeAtPosition(targetGrid);

        if (startNode == null)
        {
            Debug.LogWarning($"No node found at start position {startGrid}");
            startNode = FindNearestNode(start);
        }
        
        if (targetNode == null)
        {
            Debug.LogWarning($"No node found at target position {targetGrid}");
            targetNode = FindNearestNode(target);
        }

        if (startNode == null || targetNode == null)
        {
            Debug.LogError($"Cannot find path: startNode={startNode}, targetNode={targetNode}");
            return Vector2.zero;
        }

        Debug.Log($"Start node: {startNode.name} at {startNode.transform.position}");
        Debug.Log($"Target node: {targetNode.name} at {targetNode.transform.position}");

        // BFS
        Queue<Node> queue = new Queue<Node>();
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, Vector2> firstMove = new Dictionary<Node, Vector2>();

        queue.Enqueue(startNode);
        cameFrom[startNode] = null;
        firstMove[startNode] = Vector2.zero;

        int iteration = 0;
        int maxIterations = 500;

        while (queue.Count > 0 && iteration < maxIterations)
        {
            iteration++;
            Node current = queue.Dequeue();

            // Tìm thấy target
            if (current == targetNode)
            {
                Vector2 direction = firstMove[current];
                Debug.Log($"Path found in {iteration} iterations! First move: {direction}");
                return direction;
            }

            // Duyệt neighbors
            if (current.availableDirections == null)
            {
                Debug.LogError($"Node {current.name} has null availableDirections!");
                continue;
            }

            foreach (Vector2 direction in current.availableDirections)
            {
                // Tính vị trí neighbor
                Vector2 neighborPos = (Vector2)current.transform.position + direction;
                Vector2Int neighborGrid = new Vector2Int(
                    Mathf.RoundToInt(neighborPos.x),
                    Mathf.RoundToInt(neighborPos.y)
                );

                if (nodeGrid.TryGetValue(neighborGrid, out Node neighbor))
                {
                    if (!cameFrom.ContainsKey(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        cameFrom[neighbor] = current;
                        
                        // Lưu hướng đầu tiên
                        if (firstMove[current] == Vector2.zero)
                        {
                            firstMove[neighbor] = direction;
                        }
                        else
                        {
                            firstMove[neighbor] = firstMove[current];
                        }
                        
                        Debug.Log($"  Added neighbor: {neighbor.name} at {neighborGrid}, first move: {firstMove[neighbor]}");
                    }
                }
                else
                {
                    Debug.LogWarning($"No node at neighbor position {neighborGrid} from {current.name}");
                }
            }
        }

        Debug.LogWarning($"No path found after {iteration} iterations");
        return Vector2.zero;
    }

    private Node GetNodeAtPosition(Vector2Int gridPos)
    {
        if (nodeGrid.TryGetValue(gridPos, out Node node))
        {
            return node;
        }
        return null;
    }

    private Node FindNearestNode(Vector2 position)
    {
        Node nearest = null;
        float minDistance = float.MaxValue;
        float searchRadius = 5.0f; // Tăng radius tìm kiếm

        foreach (Node node in allNodes)
        {
            float distance = Vector2.Distance(position, node.transform.position);
            if (distance < minDistance && distance <= searchRadius)
            {
                minDistance = distance;
                nearest = node;
            }
        }

        Debug.Log($"FindNearestNode({position}): found {nearest?.name} at distance {minDistance}");
        return nearest;
    }

    /// <summary>
    /// Tìm đường ngẫu nhiên để chạy trốn (cho chế độ frightened)
    /// </summary>
    public Vector2 FindRandomDirection(Vector2 start, Node currentNode)
    {
        if (currentNode == null || currentNode.availableDirections.Count == 0)
            return Vector2.zero;

        // Chọn hướng ngẫu nhiên, ưu tiên không quay đầu
        List<Vector2> validDirections = new List<Vector2>(currentNode.availableDirections);
        
        // Loại bỏ hướng quay đầu nếu có nhiều hướng
        if (validDirections.Count > 1)
        {
            Vector2 oppositeDirection = -GhostManager.Instance.GetGhostDirection(this.GetComponent<Ghost>());
            validDirections.Remove(oppositeDirection);
        }

        if (validDirections.Count == 0)
            validDirections = new List<Vector2>(currentNode.availableDirections);

        int randomIndex = Random.Range(0, validDirections.Count);
        return validDirections[randomIndex];
    }
}