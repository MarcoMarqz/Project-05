using UnityEngine;

public class Coin : MonoBehaviour
{
    public AudioClip collectSound; // 🎵 Assign this in the Inspector

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLAYER"))
        {
            // Play the sound at the coin's position
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            ScoreManager.instance.AddScore(1);
            Destroy(gameObject);
        }
    }
}
