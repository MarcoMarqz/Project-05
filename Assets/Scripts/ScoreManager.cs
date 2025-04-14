using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int score = 0;
    public int coinsToAdvance = 10;
    public Text scoreText; // 🔥 Assign this in the Inspector

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int value)
    {
        score += value;

        // Update UI
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        // Check for scene transition
        if (score >= coinsToAdvance)
        {
            AdvanceScene();
        }
    }

    void AdvanceScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Maze")
        {
            score = 0;
            SceneManager.LoadScene("Maze2");
        }
        else if (currentScene == "Maze2")
        {
            SceneManager.LoadScene("End Screen");
        }
    }
}
