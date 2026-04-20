using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Static Instance
    // instance that everyone can see
    public static GameManager Instance;

    public int score = 0;
    public int waveNum = 0;
    public bool isPaused = false;
    public bool isGameOver = false;

    void Awake()
    {
        // Set the singleton
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
    }

    public void IncrementWave()
    {
        waveNum += 1;
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void UnPause()
    {
        isPaused = false;
    }

    public void EndGame()
    {
        isGameOver = true;
    }

    public void RestartGame()
    {
        isGameOver = false;
    }

}
