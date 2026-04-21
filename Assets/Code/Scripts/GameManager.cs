using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // Static Instance
    // instance that everyone can see
    public static GameManager Instance;

    // Essential game objects
    public GameObject player;

    // Game state variables
    public int score = 0;
    public int waveNum = 0;
    public bool isPaused = false;
    public bool isGameOver = false;

    // Input Maps
    private InputAction pauseAction;

    // Systme Actions that others listen to
    public System.Action OnGameRestart;

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

    void Start()
    {
        if(player != null)
        {
            player.GetComponent<Player>().OnDeath += () =>
            {
                EndGame();
            };
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
        Time.timeScale = 0.0f;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void RestartGame()
    {
        //TODO: make the game manager and player less tightly coupled
        // TODO: Put this in player and have player listen to the on game restart routine by game manager
        // Set all variables back to zero
        score = 0;
        waveNum = 0; // set to 1 so enemies start spawning again
        player.GetComponent<Player>().currentHealth = player.GetComponent<Player>().maxHealth;
        player.GetComponent<Player>().currentAmmo = player.GetComponent<Player>().maxAmmo;
        // TODO: way to reset reload timer if needed

        isGameOver = false;

        // Broadcast that game is restarting
        OnGameRestart?.Invoke();

        // Change cursor mode back
        Cursor.lockState = CursorLockMode.Locked;

        // Set time scale back to normal
        Time.timeScale = 1.0f;
    }

}
