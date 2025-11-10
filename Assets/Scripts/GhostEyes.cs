using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GhostEyes : MonoBehaviour
{
    // Các sprite cặp mắt (hướng lên, xuống, trái, phải)
    public Sprite up;
    public Sprite down;
    public Sprite left;
    public Sprite right;

    private SpriteRenderer spriteRenderer;
    private Movement movement; // Lấy script Movement từ 'cha'

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // GetComponentInParent sẽ tìm component Movement ở GameObject cha
        movement = GetComponentInParent<Movement>();
    }

    private void Update()
    {
        // Dựa vào hướng đi của 'cha', thay đổi sprite mắt cho phù hợp
        if (movement.direction == Vector2.up)
        {
            spriteRenderer.sprite = up;
        }
        else if (movement.direction == Vector2.down)
        {
            spriteRenderer.sprite = down;
        }
        else if (movement.direction == Vector2.left)
        {
            spriteRenderer.sprite = left;
        }
        else if (movement.direction == Vector2.right)
        {
            spriteRenderer.sprite = right;
        }
    }
}