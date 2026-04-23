using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{

    // Events
    public System.Action<float> OnWaveTimerTick;
    public System.Action OnWaveTimerEnd;

    // Game state information (get this from game later, probably)
    public bool canSpawnWave = true;
    public float waveTimer = 10.0f;
    public float buffer = 15.0f;

    // Reference to the ground to provide ranges of spawning
    public GameObject ground;
    // Reference to Enemy prefab for spawning
    public GameObject enemyPrefab;
    // Reference to player for buffer
    public GameObject player;

    // Number of enemies still alive
    public int enemiesAlive;

    // Private variables
    private float xRange;
    private float yRange;
    private float zRange;

    // Private variable for max concurrent enemies
    private int maxConcurrentEnemies = 3;

    // Health tracking for adding score
    private int playerHealth = 0;

    // Coroutine reference
    Coroutine runningWave;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize variables
        canSpawnWave = true;
        enemiesAlive = 0;

        // Get references to half of the ground range -- distance from origin enemies can spawn in x and y directions
        xRange = ground.GetComponent<Renderer>().bounds.size.x/2;
        zRange = ground.GetComponent<Renderer>().bounds.size.z/2;
        // One below the ground, so they rize up out of the ground
        yRange = ground.GetComponent<Renderer>().bounds.size.y - 1;

        // Subscribe to game on restart event with command to destroy all enemies
        GameManager.Instance.OnGameRestart += ResetState;
        Enemy.OnEnemyDeath += RemoveEnemy;
        // Subscribe to enemy on hit event to remove the enemy, need lambda because of point requirement
        Enemy.OnEnemyHit += () =>
        {
            RemoveEnemy(0);
        };
    }

    // Update is called once per frame
    void Update()
    {
        if(canSpawnWave)
        {
            // Update wave FIRST so OnWaveComplete runs properly
            GameManager.Instance.IncrementWave();
            runningWave = StartCoroutine(RunWave());
        }
    }

    void OnDisable()
    {
        // Clean up static subscriptions
        Enemy.OnEnemyDeath -= RemoveEnemy;
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnGameRestart -= ResetState;
        }
    }

    IEnumerator RunWave()
    {
        // Set can spawn wave to false; no spawning wave while wave is running
        canSpawnWave = false;

        float timeRemaining = waveTimer;
        // Time check, checks if enemy has spawned
        float timeCheck = 0;
        float maxWaiting = 5.0f;

        while(timeRemaining >= 0)
        {
            // Code for running the wave; run chance to spawn enemy, increment enemies, when time is over we end the wave
            timeRemaining -= Time.deltaTime;
            timeCheck += Time.deltaTime;

            //TODO: invoke timer tick that ui manager listens to for graphical timer logic
            OnWaveTimerTick?.Invoke(timeRemaining);

            // Get a random number between 1 and 50
            float rand = Random.Range(1,50);

            // If random number is less than waveNum * difficulty scaling and less than max nonconcurrent enemies alive
            if(rand < GameManager.Instance.waveNum * GameManager.Instance.difficultyScale && enemiesAlive < maxConcurrentEnemies)
            {
                // Then spawn enemy
                SpawnEnemy(FindSpawnLocation());
                // Set spawn timer to 0
                timeCheck = 0;
                // Increment enemies alive
                enemiesAlive++;
            }

            // If no enemy spawned in 5 seconds, spawn an enemy regardless if less than max concurrent enemies alive
            if(timeCheck >= maxWaiting && enemiesAlive <= maxConcurrentEnemies)
            {
                SpawnEnemy(FindSpawnLocation());
                timeCheck = 0;
                enemiesAlive++;
            }

            yield return null;
        }

        // TODO: end of wave logic
        OnWaveTimerEnd?.Invoke();

        // Set canSpawnWave back to true
        canSpawnWave = true;
    }



    Vector3 FindSpawnLocation()
    {
        Vector3 spawnPos;
        int safety = 0; // prevents infinite loops breaking

        // Get random x and z, if within buffer distance of player, re-randomize
        do
        {
            float randomX = Random.Range(-xRange,xRange);
            float randomZ = Random.Range(-zRange,zRange);

            spawnPos = new Vector3(randomX, yRange, randomZ);

            //Prevents infinity if things break
            safety++;
            if(safety > 100) break;

        } while (Vector3.Distance(player.transform.position,spawnPos) <= buffer);
        // No randomized y; they all start one below the ground
        
        return spawnPos;
    }

    void SpawnEnemy(Vector3 spawnPos)
    {
        // Instantiates an enemy to be facing straight up at a randomized spawn position
        Instantiate(enemyPrefab,spawnPos,Quaternion.Euler(-90f,0f,0f));
    }

    // Spawns enemies in the wave, then returns false to set canSpawnWave to false
    bool SpawnWave()
    {
        // Adds score per wave cleared (basically any wave number after 1)
        // Only adds 100 score if player health is same as it was, i.e. player took no damage that round
        // TODO: Use a broadcast of spawn wave, either with or without extra points
        //FIXME: move to game manager!!!!
        if(GameManager.Instance.waveNum > 1 && player.GetComponent<Player>().currentHealth == playerHealth)
        {
            GameManager.Instance.AddScore(100);
        }

        // For now spawns as many enemies as the wave number
        for(int i = 0; i < GameManager.Instance.waveNum; i++)
        {
            SpawnEnemy(FindSpawnLocation());
        }

        // Saves player score at start of wave
        playerHealth = player.GetComponent<Player>().currentHealth;

        return false;
    }

    // Checks if a new wave can spawn- if all enemies from last wave are dead
    bool CheckWaveSpawn(int enemyNum)
    {
        if(enemyNum == 0)
        {
            return true;
        }
        return false;
    }

    void ResetState()
    {
        // Sets enemiesAlive to 0 so new wave can start
        enemiesAlive = 0;
        // Sets wave timer to end
        if(runningWave != null)
        {
            // Stop and remov the coRoutine
            StopCoroutine(runningWave);
            runningWave = null;
            canSpawnWave = true;
        }
    }

    // does not need the integer parameter for points
    void RemoveEnemy(int _)
    {
        enemiesAlive = Mathf.Max(0,enemiesAlive-1);
    }



}
