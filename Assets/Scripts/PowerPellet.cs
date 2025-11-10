using UnityEngine;

public class PowerPellet : Pellet // <-- Lưu ý: Kế thừa từ Pellet
{
    // Thêm 2 biến public mới
    public float duration = 8f; // Thời gian "sợ hãi" cho Ghost

    // (Project gốc đặt điểm cho power pellet là 50)
    // Chúng ta có thể gán đè giá trị 'points' từ lớp cha
    private void Start()
    {
        points = 50;
    }

    // "override" nghĩa là "viết đè" lên hàm Eat() của lớp cha (Pellet.cs)
    protected override void Eat()
    {
        // Khi bị ăn, nó sẽ gọi hàm PowerPelletEaten trong GameManager
        // thay vì hàm PelletEaten thông thường.
        GameManager.Instance.PowerPelletEaten(this);
    }
}