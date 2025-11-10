using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Pellet : MonoBehaviour
{
    public int points = 10; // Điểm số khi ăn hạt này

    // Hàm ảo để PowerPellet có thể kế thừa và thay đổi hành vi
    protected virtual void Eat()
    {
        GameManager.Instance.PelletEaten(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem vật va chạm có phải là Layer "Pacman" không
        if (other.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            Eat();
        }
    }
}