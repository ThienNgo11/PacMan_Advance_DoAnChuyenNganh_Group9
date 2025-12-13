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
    }

    private void Start()
    {
        if (wallTilemap == null) return;

        availableDirections = new List<Vector2>();

        CheckAvailableDirection(Vector2.up);
        CheckAvailableDirection(Vector2.down);
        CheckAvailableDirection(Vector2.left);
        CheckAvailableDirection(Vector2.right);

        Debug.Log($"Node {name} at {transform.position} has {availableDirections.Count} directions");
    }

    private void CheckAvailableDirection(Vector2 direction)
    {
        Vector3Int currentCell = wallTilemap.WorldToCell(transform.position);
        Vector3Int targetCell = currentCell + new Vector3Int(
            Mathf.RoundToInt(direction.x),
            Mathf.RoundToInt(direction.y),
            0
        );

        if (!wallTilemap.HasTile(targetCell))
        {
            availableDirections.Add(direction);
        }
    }
}
