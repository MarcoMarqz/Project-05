using UnityEngine;

public class Coin : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLAYER"))
        {
            ScoreManager.instance.AddScore(1);
            Destroy(gameObject);
        }
    }
}
