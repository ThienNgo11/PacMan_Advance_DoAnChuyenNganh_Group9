using UnityEngine;

// Yêu cầu GameObject phải có SpriteRenderer
[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedSprite : MonoBehaviour
{
    public Sprite[] sprites = new Sprite[0]; // Mảng chứa các khung hình
    public float animationTime = 0.25f;      // Thời gian giữa các khung hình
    public bool loop = true;                 // Có lặp lại animation không?

    private SpriteRenderer spriteRenderer;
    private int animationFrame; // Khung hình hiện tại

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Khi script được BẬT
    private void OnEnable()
    {
        spriteRenderer.enabled = true;
    }

    // Khi script bị TẮT
    private void OnDisable()
    {
        spriteRenderer.enabled = false;
    }

    private void Start()
    {
        // Gọi hàm Advance lặp đi lặp lại
        // Bắt đầu gọi sau 'animationTime' giây, và lặp lại sau mỗi 'animationTime' giây
        InvokeRepeating(nameof(Advance), animationTime, animationTime);
    }

    private void Advance()
    {
        // Nếu SpriteRenderer bị tắt thì không làm gì cả
        if (!spriteRenderer.enabled)
        {
            return;
        }

        animationFrame++; // Chuyển sang khung hình tiếp theo

        // Nếu hết mảng sprite VÀ có bật loop
        if (animationFrame >= sprites.Length && loop)
        {
            animationFrame = 0; // Quay về khung hình đầu tiên
        }

        // Chỉ cập nhật sprite nếu khung hình còn nằm trong mảng
        if (animationFrame >= 0 && animationFrame < sprites.Length)
        {
            spriteRenderer.sprite = sprites[animationFrame];
        }
    }

    // Hàm này dùng để chạy lại animation từ đầu
    public void Restart()
    {
        animationFrame = -1; // Đặt là -1
        Advance(); // Gọi Advance 1 lần (lúc này frame sẽ tăng lên 0 và hiển thị sprite đầu tiên)
    }
}