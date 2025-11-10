using System.Collections;
using UnityEngine;

// Kế thừa từ GhostBehavior
public class GhostHome : GhostBehavior
{
    // Chúng ta cần 2 điểm tham chiếu: 1 điểm ở TRONG chuồng và 1 điểm NGAY NGOÀI cửa
    public Transform inside;
    public Transform outside;

    private void OnEnable()
    {
        // Dừng mọi Coroutine cũ (nếu có) khi được bật
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        // Khi hành vi này bị TẮT (Disable), nó sẽ tự động bắt đầu quá trình đi ra
        // (chỉ khi GameObject vẫn còn active)
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(ExitTransition());
        }
    }

    // Khi ở trong chuồng, nếu va vào tường thì đổi hướng (tạo hiệu ứng "nhảy" qua lại)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (enabled && collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            ghost.movement.SetDirection(-ghost.movement.direction);
        }
    }

    // Đây là hàm xử lý chính, chạy tuần tự từng bước một
    private IEnumerator ExitTransition()
    {
        // 1. TẮT script Movement để ta có thể điều khiển vị trí bằng tay
        ghost.movement.SetDirection(Vector2.up, true); // Hướng lên cửa
        ghost.movement.rb.isKinematic = true; // Tắt vật lý tạm thời
        ghost.movement.enabled = false;

        Vector3 position = transform.position;

        float duration = 0.5f; // Thời gian đi từ điểm A -> B
        float elapsed = 0f;

        // 2. Di chuyển từ vị trí hiện tại đến điểm "inside" (giữa chuồng)
        while (elapsed < duration)
        {
            // Lerp = di chuyển mượt mà
            Vector3 newPos = Vector3.Lerp(position, inside.position, elapsed / duration);
            newPos.z = position.z; // Giữ nguyên trục Z
            transform.position = newPos;

            elapsed += Time.deltaTime;
            yield return null; // Chờ đến frame tiếp theo
        }

        elapsed = 0f; // Reset đồng hồ

        // 3. Di chuyển từ điểm "inside" đến điểm "outside" (ra khỏi cửa)
        while (elapsed < duration)
        {
            Vector3 newPos = Vector3.Lerp(inside.position, outside.position, elapsed / duration);
            newPos.z = position.z; // Giữ nguyên trục Z
            transform.position = newPos;

            elapsed += Time.deltaTime;
            yield return null; // Chờ đến frame tiếp theo
        }

        // 4. BẬT LẠI script Movement, chọn hướng đi ngẫu nhiên (trái hoặc phải)
        ghost.movement.SetDirection(new Vector2(Random.value < 0.5f ? -1f : 1f, 0f), true);
        ghost.movement.rb.isKinematic = false; // Bật lại vật lý
        ghost.movement.enabled = true;
    }
}