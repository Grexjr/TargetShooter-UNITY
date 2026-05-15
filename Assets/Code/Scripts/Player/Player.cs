using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System.Collections;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerInputHandler))]
public class Player : MonoBehaviour
{

    // Player states
    private enum PlayerState
    {
        ALIVE, // just generic alive
        RELOADING, // when alive and reloading
        DEAD // when dead
    }

    // Broadcasts
    public System.Action OnReloadTimerStart; // broadcast to: HUDManager
    public System.Action<float> OnReloadTimerTick; // broadcast to: HUDManager
    public System.Action OnReloadTimerEnd; // broadcast to: HUDManager
    public System.Action OnDeath; // broadcast to: GameManager

    // Necessary player components
    private Health health;
    private PlayerInputHandler input;
    [SerializeField] private FirstPersonCamera playerCamera;

    // Necessary player variables
    private PlayerState state;

    public int maxAmmo, currentAmmo;

    // Bullet reference
    public GameObject bulletPrefab;


    // Timer variable for reload buffer, in seconds
    private float reloadBuffer = 1.5f;
    // CoRoutine for stopping the countdown if needed
    private Coroutine reloadCountdown;

    // Initialization, guaranteed to happen first
    void Awake()
    {
        // Basic initialization
        health = GetComponent<Health>();
        input = GetComponent<PlayerInputHandler>();
        state = PlayerState.ALIVE;

        maxAmmo = 5;
        currentAmmo = maxAmmo;

        // Subscribe to game manager broadcasts and call reset state when game is restarted
        GameManager.Instance.OnGameRestart += ResetStats;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Subscribe to input events
        input.onAttack += Shoot;
        input.onReload += DoReload;
    }

    // Update is called once per frame
    void Update()
    {
        // Get the look input vector
        Vector2 look = input.LookInput;
        
        // FIXME: This is part of the temporary fix; this pauses the game, in future we should change this to be a better
        // pausing system.
        if(Time.timeScale > 0)
        {
            Look(look.x,look.y);  
        }
    }

    void OnDisable()
    {
        GameManager.Instance.OnGameRestart -= ResetStats;
        input.onAttack -= Shoot;
        input.onReload -= DoReload;
    }

    public float GetReloadBuffer()
    {
        return reloadBuffer;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        // Reduce health
        health.TakeDamage(1);
        // Broadcast event that player has died if health == 0
        if(health.CheckDeath())
        {
            OnDeath?.Invoke();
        }
    }

    void ReduceAmmo()
    {
        currentAmmo -= 1;
    }

    void Reload()
    {
        currentAmmo = maxAmmo;
    }

    IEnumerator DoReloadCountdown()
    {
        // Send out event for HUDManager to listen to to enable the reload bar and count down on the UI
        OnReloadTimerStart?.Invoke();

        float timeRemaining = reloadBuffer;

        while(timeRemaining > 0)
        {
            // Subtract time passed since last frame
            timeRemaining -= Time.deltaTime;

            // Update UI
            // Passes in a variable to the broadcast to the event, that's really cool
            OnReloadTimerTick?.Invoke(timeRemaining);
       
            yield return null;
        }

        OnReloadTimerEnd?.Invoke();
        state = PlayerState.ALIVE;
        Reload();
    }

    void Shoot()
    {
        if(state != PlayerState.RELOADING) 
        {
            Instantiate(bulletPrefab,playerCamera.transform.position,playerCamera.transform.rotation);
            ReduceAmmo();
        }
    }

    void DoReload()
    {
        state = PlayerState.RELOADING;
        reloadCountdown = StartCoroutine(DoReloadCountdown());
    }

    void Look(float mouseDeltaX, float mouseDeltaY)
    {
        // Scale by the sensitivity
        float mouseX = mouseDeltaX * PersistentManager.Sensitivity;
        float mouseY = mouseDeltaY * PersistentManager.Sensitivity;

        // Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Update the camera
        playerCamera.UpdatePitchAndYaw(mouseX,mouseY);
    }

    void ResetStats()
    {
        state = PlayerState.ALIVE;

        health.ResetHealth();
        currentAmmo = maxAmmo;
        if(reloadCountdown != null)
        {
            // Stop and remov the coRoutine
            StopCoroutine(reloadCountdown);
            reloadCountdown = null;
            // Tell the UI to hide the bar
            OnReloadTimerEnd?.Invoke();
        }
        
    }
}
