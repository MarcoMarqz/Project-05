using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{


    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletspeed = 10;

    public AudioClip shootSound; // Public so you can assign it in the Inspector
    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Add one if not present
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletspeed;

            if (shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }
        }
    }
}
