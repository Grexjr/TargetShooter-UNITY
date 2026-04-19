using System;
using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    // Signal that enemy has tied that other scripts listen to
    public System.Action OnEnemyDeath;


    // Enemy speed value
    public float baseEnemySpeed = 10.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(MoveIntoPosition());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Method to move the enemy upwards from spawn position into 'attack' range
    IEnumerator MoveIntoPosition()
    {
        // Moves up at half speed until it hits the y ceiling, then stops
        while(transform.position.y < 10.0f)
        {
            transform.Translate(Vector3.forward * (int)(baseEnemySpeed * 0.5) * Time.deltaTime);
            yield return null;
        }
    }

    // Method to run collision logic if it hits another collider
    void OnTriggerEnter(Collider other)
    {
        // If it hits a bullet, destroy the enemy
        if (other.CompareTag("Bullet"))
        {
            Destroy(gameObject);
            OnEnemyDeath?.Invoke(); // Tell anyone listening that the enemy die
        }
        else if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            print("Player has been hit!");
        }

    }
}
