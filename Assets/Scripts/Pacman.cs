using UnityEngine;

// Yêu cầu phải có component Movement
[RequireComponent(typeof(Movement))]
public class Pacman : MonoBehaviour
{
    // Chúng ta sẽ cần tham chiếu đến Movement script
    private Movement movement;

    private void Awake()
    {
        // Lấy component Movement mà chúng ta đã gắn vào Pacman
        movement = GetComponent<Movement>();
    }

    private void Update()
    {
        // 1. Xử lý Input (Phần này ta chuyển từ Movement.cs sang đây)
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            movement.SetDirection(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            movement.SetDirection(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            movement.SetDirection(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            movement.SetDirection(Vector2.right);
        }

        // 2. Xử lý xoay Sprite
        // Lấy hướng di chuyển hiện tại từ script Movement
        float angle = Mathf.Atan2(movement.direction.y, movement.direction.x);
        // Xoay Pacman (transform của nó)
        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
    }

    // Hàm này sẽ dùng để hồi sinh Pacman sau này
    public void ResetState()
    {
        enabled = true; // Bật script Pacman.cs
        movement.ResetState(); // Gọi hàm ResetState của Movement.cs
        gameObject.SetActive(true); // Bật GameObject Pacman lên
    }
}