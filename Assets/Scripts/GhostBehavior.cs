using UnityEngine;

// Yêu cầu phải có component Ghost
[RequireComponent(typeof(Ghost))]
public abstract class GhostBehavior : MonoBehaviour
{
    // Tham chiếu đến script Ghost chính
    public Ghost ghost { get; private set; }

    // Thời lượng mặc định cho hành vi này
    public float duration;

    private void Awake()
    {
        // Lấy script Ghost trên cùng GameObject
        ghost = GetComponent<Ghost>();
    }

    // Bật hành vi (không có thời lượng, chạy mãi)
    public void Enable()
    {
        Enable(duration); // Gọi hàm bên dưới với thời lượng mặc định
    }

    // Bật hành vi (có thời lượng)
    public virtual void Enable(float duration)
    {
        // Bật component script này (ví dụ: bật script GhostChase.cs)
        enabled = true;

        // Hủy mọi lệnh "Disable" đang chờ
        CancelInvoke();

        // Hẹn giờ để tự động gọi hàm Disable() sau 'duration' giây
        // Nếu duration = 0, nó sẽ chạy mãi (giống như Invoke(..., 0))
        Invoke(nameof(Disable), duration);
    }

    // Tắt hành vi
    public virtual void Disable()
    {
        // Tắt component script này
        enabled = false;

        // Hủy mọi lệnh hẹn giờ (phòng trường hợp)
        CancelInvoke();
    }
}