using UnityEngine;

public class SimpleEnemyShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public Transform player;
    public float shootingRange = 15f;
    public float fireRate = 1f;
    public float bulletForce = 20f;

    private float nextTimeToFire = 0f;

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= shootingRange && Time.time >= nextTimeToFire)
        {
            Shoot();
            nextTimeToFire = Time.time + 1f / fireRate;
        }

        // Optional: Look at player
        transform.LookAt(player);
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(shootPoint.forward * bulletForce, ForceMode.Impulse);
        }
    }
}
