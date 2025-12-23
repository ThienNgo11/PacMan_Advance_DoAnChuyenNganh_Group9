using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
    [Header("References")]
    public Tilemap wallTilemap;

    public List<Vector2> availableDirections { get; private set; }
    public bool isActive = true;

    private void Awake()
    {
        // TỰ ĐỘNG TÌM WALL TILEMAP NẾU CHƯA GÁN
        if (wallTilemap == null)
        {
            GameObject walls = GameObject.Find("Walls");
            if (walls != null)
            {
                wallTilemap = walls.GetComponent<Tilemap>();
            }
        }

        if (wallTilemap == null)
        {
            Debug.LogError($"Node {name}: wallTilemap NOT FOUND!");
        }
        if (wallTilemap == null) return;

        availableDirections = new List<Vector2>();

        CheckAvailableDirection(Vector2.up);
        CheckAvailableDirection(Vector2.down);
        CheckAvailableDirection(Vector2.left);
        CheckAvailableDirection(Vector2.right);

        Debug.Log($"Node {name} at {transform.position} has {availableDirections.Count} directions");
    }

    // private void Start()
    // {
        
    // }

    private void CheckAvailableDirection(Vector2 direction)
    {
        Vector3 targetWorldPos = transform.position + (Vector3)direction; 
        Vector3Int targetCell = wallTilemap.WorldToCell(targetWorldPos);

    if (!wallTilemap.HasTile(targetCell))
    {
        availableDirections.Add(direction);
    }
    }
}
