using System;
using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    // Signal that enemy has died that other scripts listen to
    public System.Action OnEnemyDeath;
    // Signal that enemy has hit player that other scripts listen to
    public System.Action OnEnemyHit;

    // Reference to player object and its transform
    public GameObject player;
    private Transform playerTransform;


    // Enemy speed value
    public float baseEnemySpeed = 5.0f;

    // Enemy rotate speed value (hidden)
    private float rotateSpeed = 10.0f;

    // private variable for enemy setting up stage
    private bool isReadyToAttack = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get a reference to the player actually in the game and their transform
        player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)
        {
            playerTransform = player.transform;
        } else
        {
            print("ERROR FINDING PLAYER OBJECT, TRANSFORM SET TO (0,0,0)");
            playerTransform.position = new Vector3(0,0,0);
        }
        StartCoroutine(MoveIntoPosition());
    }

    // Update is called once per frame
    void Update()
    {
        // If not ready to attack, wait until ready to attack
        if (!isReadyToAttack)
        {
            return;
        }    

        // Makes enemy always look at player, smoothly through Quaternion Slerp
        // Solution from: 
        // https://discussions.unity.com/t/transform-lookat-smooth-transition-instead-of-instant-snapping/695061/5
        var rotation = Quaternion.LookRotation(playerTransform.position - transform.position);
        float step = rotateSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation,rotation,step);


        // Always move towards the player once ready--uses Vector3.MoveTowards
        transform.position = Vector3.MoveTowards(transform.position,playerTransform.position,baseEnemySpeed * Time.deltaTime);
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

        isReadyToAttack = true;
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
            OnEnemyHit?.Invoke(); // Tell anyone listening that the enemy hit the player
        }

    }
}
