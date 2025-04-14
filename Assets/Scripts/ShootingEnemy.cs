using UnityEngine;

public class ShootingEnemy : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootInterval = 0.2f; // 🔫 Shoot fast!
    public float rotateSpeed = 100f;   // 🔁 Spin speed (degrees per second)

    void Start()
    {
        InvokeRepeating(nameof(Shoot), shootInterval, shootInterval);
    }

    void Update()
    {
        if (firePoint != null)
        {
            // Spin around the Y axis forever
            firePoint.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
        }
    }

    void Shoot()
    {
        if (firePoint == null || bulletPrefab == null) return;

        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
