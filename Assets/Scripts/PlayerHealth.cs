using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHits = 5;
    private int currentHits = 0;

    public void TakeDamage()
    {
        currentHits++;
        Debug.Log("Player hit: " + currentHits);

        if (currentHits >= maxHits)
        {
            SceneManager.LoadScene("End Screen");
        }
    }
}

