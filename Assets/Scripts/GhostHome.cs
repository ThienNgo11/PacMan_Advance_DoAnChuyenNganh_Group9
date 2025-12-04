using System.Collections;
using UnityEngine;

public class GhostHome : GhostBehavior
{
    public Transform inside;
    public Transform outside;

    private void OnEnable()
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(ExitTransition());
        }
    }


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
        ghost.movement.SetDirection(Vector2.up, true); // Hướng lên cửa
        ghost.movement.rb.isKinematic = true;
        ghost.movement.enabled = false;

        Vector3 position = transform.position;

        float duration = 0.5f; // Thời gian đi từ điểm A -> B
        float elapsed = 0f;

        // 2. Di chuyển từ vị trí hiện tại đến điểm "inside" (giữa chuồng)
        while (elapsed < duration)
        {
            Vector3 newPos = Vector3.Lerp(position, inside.position, elapsed / duration);
            newPos.z = position.z;
            transform.position = newPos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;

        // 3. Di chuyển từ điểm "inside" đến điểm "outside" (ra khỏi cửa)
        while (elapsed < duration)
        {
            Vector3 newPos = Vector3.Lerp(inside.position, outside.position, elapsed / duration);
            newPos.z = position.z;
            transform.position = newPos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 4. BẬT LẠI script Movement, chọn hướng đi ngẫu nhiên (trái hoặc phải)
        ghost.movement.SetDirection(new Vector2(Random.value < 0.5f ? -1f : 1f, 0f), true);
        ghost.movement.rb.isKinematic = false; // Bật lại vật lý
        ghost.movement.enabled = true;
        ghost.chase.Enable();
    }
}