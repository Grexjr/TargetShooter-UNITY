using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    //TODO: GENERAL>>ADD ONDISABLE TO OBJECTS
    // Static Instance
    // instance that everyone can see
    public static GameManager Instance;

    // Essential game objects
    public GameObject player;
    public GameObject spawnManager; // TODO: Refactor this

    // Game state variables
    public int score = 0;
    public int waveNum = 0;
    public int difficultyScale = 5;
    public bool isPaused = false;
    public bool isGameOver = false;

    // Input Maps
    private InputAction pauseAction;

    // Systme Actions that others listen to
    public System.Action OnGameRestart; // broadcast to: all lower entities (player, spawn manager, enemy)

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

        // Enable if pause action is not null, otherwise it does nothing
        pauseAction?.Enable();
    }

    void Start()
    {
        if(player != null)
        {
            // Subscribe to the on death event of the player
            player.GetComponent<Player>().OnDeath += EndGame;
        } 
        else
        {
            Debug.Log("ERROR: No player found!");
        }

        // Subscribe to static enemy broadcast
        // NOTE: parameters are passed implicitly by the event--cool!
        Enemy.OnEnemyDeath += AddScore;
        spawnManager.GetComponent<SpawnManager>().OnWaveEnd += (int score) => {
            AddScore(score);
        };
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

    // Unsubscribe from all events when disabled
    void OnDisable()
    {
        if(player != null)
        {
            player.GetComponent<Player>().OnDeath -= EndGame;
        }
        Enemy.OnEnemyDeath -= AddScore;
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
        // TODO: find a better way to do this, this is temporary and works with your basic logic
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
        // Set all variables back to zero
        score = 0;
        waveNum = 0; // set to 1 so enemies start spawning again
        isGameOver = false;

        // Broadcast that game is restarting so other objects set their states to default
        OnGameRestart?.Invoke();

        // Change cursor mode back
        Cursor.lockState = CursorLockMode.Locked;

        // Set time scale back to normal
        Time.timeScale = 1.0f;
    }

}
