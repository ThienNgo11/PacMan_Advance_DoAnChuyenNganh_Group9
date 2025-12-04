using UnityEngine;

public class PowerPellet : Pellet 
{

    public float duration = 8f;
    private void Start()
    {
        points = 50;
    }
    protected override void Eat()
    {
        GameManager.Instance.PowerPelletEaten(this);
    }
}