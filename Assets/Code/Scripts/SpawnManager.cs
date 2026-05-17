using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;

public class SpawnManager : MonoBehaviour
{

    // Events
    public System.Action OnWaveTimerStart;
    public System.Action<float> OnWaveTimerTick;
    public System.Action OnWaveTimerEnd;
    public System.Action<int> OnWaveEnd;

    // Game state information (get this from game later, probably)
    public bool canSpawnWave = true;
    public float waveTimer = 10.0f;
    public float buffer = 15.0f;

    // Wave information for difficulty
    // Max waiting period if enemy has not spawned
    public float maxWaiting = 5.0f;
    // Cooldown between spawning enemies
    public float cooldown = 2.0f;
    // Random value to roll
    public int randMax = 50;
    // Reference to what time the most recent enemy spawned
    float lastSpawnTime = 0;
    int maxEnemiesPerWave = 2;
    int enemiesSpawnedThisWave = 0;

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

    // Coroutine reference
    Coroutine runningWave;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize variables
        canSpawnWave = true;
        enemiesAlive = 0;

        // Get references to half of the ground range -- distance from origin enemies can spawn in x and y directions
        xRange = ground.GetComponent<Renderer>().bounds.size.x / 2;
        zRange = ground.GetComponent<Renderer>().bounds.size.z / 2;
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
        if (canSpawnWave)
        {
            // Set can spawn wave to false; no spawning wave while wave is running
            canSpawnWave = false;
            // Update wave FIRST so OnWaveComplete runs properly
            GameManager.Instance.IncrementWave();
            runningWave = StartCoroutine(RunWave());
        }
    }

    void OnDisable()
    {
        // Clean up static subscriptions
        Enemy.OnEnemyDeath -= RemoveEnemy;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameRestart -= ResetState;
        }
    }

    IEnumerator RunWave()
    {
        // Invoke wave start
        OnWaveTimerStart?.Invoke();

        // Wavescore for now equals zero
        int waveScore = 0;
        int playerStartHealth = player.GetComponent<Health>().CurrentHealth;

        // Reset enemies spawned this wave
        enemiesSpawnedThisWave = 0;

        // How much time is left in the wave
        float timeRemaining = waveTimer;

        while (timeRemaining >= 0)
        {
            // Keep running counter of current time
            float currentTime = Time.time;

            // Code for running the wave; run chance to spawn enemy, increment enemies, when time is over we end the wave
            timeRemaining -= Time.deltaTime;

            //TODO: invoke timer tick that ui manager listens to for graphical timer logic
            OnWaveTimerTick?.Invoke(timeRemaining);

            //Check if we can spawn enemies (below maximum enemies)
            if (enemiesAlive < maxConcurrentEnemies && enemiesSpawnedThisWave < maxEnemiesPerWave)
            {

                float timeSinceLastSpawn = Time.time - lastSpawnTime;

                //Debug.Log(timeSinceLastSpawn);

                if (timeSinceLastSpawn >= cooldown)
                {
                    bool forceSpawn = timeSinceLastSpawn >= maxWaiting;
                    float rand = Random.Range(1, randMax);
                    float spawnChance = GameManager.Instance.waveNum * GameManager.Instance.difficultyScale;

                    Debug.Log(rand+"/"+spawnChance);

                    if (rand < spawnChance || forceSpawn)
                    {
                        SpawnEnemy(FindSpawnLocation());
                        enemiesAlive++;
                        enemiesSpawnedThisWave++;
                        lastSpawnTime = Time.time;
                    }
                }
            }

            yield return null;
        }

        // Wait until all enemies from this wave are done
        while(enemiesAlive > 0)
        {
            yield return null;
        }

        // TODO: end of wave logic
        OnWaveTimerEnd?.Invoke();
        if(maxWaiting > 0.5f)
        {
            maxWaiting -= 0.5f;
        }
        if(cooldown > 0.1f)
        {
            cooldown -= 0.01f;
        }
        if (randMax > 1)
        {
            randMax -= 1;
        }
        waveTimer = waveTimer + (GameManager.Instance.difficultyScale / 2);
        maxEnemiesPerWave++; //TODO: just increases by one, but later will have a formula

        // Calculate waveScore for now based on player health: 
        // TODO: do something else
        int playerEndHealth = player.GetComponent<Health>().CurrentHealth;
        if(playerStartHealth == playerEndHealth)
        {
            waveScore = 100; // TODO: move this into variable so it can be tweaked
        }

        // Now invoke on wave end
        OnWaveEnd?.Invoke(waveScore);

        canSpawnWave = true;
    }

    Vector3 FindSpawnLocation()
    {
        Vector3 spawnPos;
        int safety = 0; // prevents infinite loops breaking

        // Get random x and z, if within buffer distance of player, re-randomize
        do
        {
            float randomX = Random.Range(-xRange, xRange);
            float randomZ = Random.Range(-zRange, zRange);

            spawnPos = new Vector3(randomX, yRange, randomZ);

            //Prevents infinity if things break
            safety++;
            if (safety > 100) break;

        } while (Vector3.Distance(player.transform.position, spawnPos) <= buffer);
        // No randomized y; they all start one below the ground

        return spawnPos;
    }

    void SpawnEnemy(Vector3 spawnPos)
    {
        // Instantiates an enemy to be facing straight up at a randomized spawn position
        Instantiate(enemyPrefab, spawnPos, Quaternion.Euler(-90f, 0f, 0f));
    }

    void ResetState()
    {
        // Sets enemiesAlive to 0 so new wave can start
        enemiesAlive = 0;
        // Sets wave timer to end
        if (runningWave != null)
        {
            // Stop and remov the coRoutine
            StopCoroutine(runningWave);
            runningWave = null;
            canSpawnWave = true;
        }
        maxWaiting = 5.0f;
        cooldown = 2.0f;
        randMax = 50;
        lastSpawnTime = 0;
    }

    // does not need the integer parameter for points
    void RemoveEnemy(int _)
    {
        enemiesAlive = Mathf.Max(0, enemiesAlive - 1);
    }



}
