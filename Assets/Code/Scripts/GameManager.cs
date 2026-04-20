using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // Static Instance
    // instance that everyone can see
    public static GameManager Instance;

    // Game state variables
    public int score = 0;
    public int waveNum = 0;
    public bool isPaused = false;
    public bool isGameOver = false;

    // Input Maps
    private InputAction pauseAction;

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

        // Set cursor to be locked into the screen
        Cursor.lockState = CursorLockMode.Locked;

        // Set up the action map
        pauseAction = InputSystem.actions.FindActionMap("Player").FindAction("Pause");

        if(pauseAction != null)
        {
            pauseAction.Enable();
        } 
    }

    void Update()
    {
        // Check every frame if pause button pressed
        if (pauseAction.WasPressedThisFrame() && !isPaused)
        {
            Pause();
        }
        else if(pauseAction.WasPressedThisFrame() && isPaused)
        {
            UnPause();
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
        // FIXME: find a better way to do this, this is temporary and works with your basic logic
        // IDEA: use a game object that calls all the update steps necessary
        Time.timeScale = 0.0f;
        // TODO: Also add wraps for if(Time.timeScale > 0) for all timers, like shooting countdown
        // Allows cursor movement
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void UnPause()
    {
        isPaused = false;
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
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
