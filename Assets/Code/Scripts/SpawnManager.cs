using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // Events that the UI listens for
    public System.Action OnWaveComplete;


    // Game state information (get this from game later, probably)
    public bool canSpawnWave = true;
    public float buffer = 15.0f;

    // Reference to the ground to provide ranges of spawning
    public GameObject ground;
    // Reference to Enemy prefab for spawning
    public GameObject enemyPrefab;
    // Reference to player for buffer
    public GameObject player;

    // Reference to list of enemies
    public List<GameObject> enemies;

    // Private variables
    private float xRange;
    private float yRange;
    private float zRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize variables
        canSpawnWave = true;
        enemies = new List<GameObject>();

        // Get references to half of the ground range -- distance from origin enemies can spawn in x and y directions
        xRange = ground.GetComponent<Renderer>().bounds.size.x/2;
        zRange = ground.GetComponent<Renderer>().bounds.size.z/2;
        // One below the ground, so they rize up out of the ground
        yRange = ground.GetComponent<Renderer>().bounds.size.y - 1;


    }

    // Update is called once per frame
    void Update()
    {
        if(canSpawnWave)
        {
            // Update wave FIRST so OnWaveComplete runs properly
            GameManager.Instance.IncrementWave();
            // Invokes the event that the wave is complete
            OnWaveComplete?.Invoke();
            
            canSpawnWave = SpawnWave();
        }
        canSpawnWave = CheckWaveSpawn(enemies.Count);
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

            safety++;
            if(safety > 100) break;

        } while (Vector3.Distance(player.transform.position,spawnPos) <= buffer);
        // No randomized y; they all start one below the ground
        
        return spawnPos;
    }

    void SpawnEnemy(Vector3 spawnPos)
    {
        // Instantiates an enemy to be facing straight up
        // This then calls the enemy scripting
        GameObject toAdd = Instantiate(enemyPrefab,spawnPos,Quaternion.Euler(-90f,0f,0f));
        enemies.Add(toAdd);
        
        // Subscribe to death event
        toAdd.GetComponent<Enemy>().OnEnemyDeath += () => {
            enemies.Remove(toAdd);
            print("Enemy is dead!");
        };
        toAdd.GetComponent<Enemy>().OnEnemyHit += () =>
        {
            print("Enemy has hit player!");
            enemies.Remove(toAdd);
        };
    }

    // Spawns enemies in the wave, then returns false to set canSpawnWave to false
    bool SpawnWave()
    {
        // For now spawns as many enemies as the wave number
        for(int i = 0; i < GameManager.Instance.waveNum; i++)
        {
            SpawnEnemy(FindSpawnLocation());
        }

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



}
