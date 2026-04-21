using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System.Collections;

public class Player : MonoBehaviour
{

    // Broadcasts
    public System.Action OnReloadTimerStart; // broadcast to: HUDManager
    public System.Action<float> OnReloadTimerTick; // broadcast to: HUDManager
    public System.Action OnReloadTimerEnd; // broadcast to: HUDManager
    public System.Action OnDeath; // broadcast to: GameManager

    // References to other necessary objects
    public Transform cameraTransform;

    // Basic necessary player stats: health and ammo
    public int maxHealth, currentHealth;
    public int maxAmmo, currentAmmo;
    
    // Controls variables
    public float sensitivity = 0.5f;

    // Bullet reference
    public GameObject bulletPrefab;
    
    // Input maps
    private InputAction attackAction;
    private InputAction lookAction;
    private InputAction reloadAction;

    // Rotation values for looking around
    private float xRotation = 0.0f;

    // Timer variable for reload buffer, in seconds
    private float reloadBuffer = 5.0f;
    // Boolean for if reload is available
    private bool canReload = true;


    // Initialization, guaranteed to happen first
    void Awake()
    {
        // Initialize player values
        maxHealth = 10;
        maxAmmo = 5;
        currentHealth = maxHealth;
        currentAmmo = maxAmmo;

        // Subscribe to game manager broadcasts and call reset state when game is restarted
        GameManager.Instance.GetComponent<GameManager>().OnGameRestart += ResetStats;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Save reference to the input system's binding of the attack action
        attackAction = InputSystem.actions.FindActionMap("Player").FindAction("Attack");
        lookAction = InputSystem.actions.FindActionMap("Player").FindAction("Look");
        reloadAction = InputSystem.actions.FindActionMap("Player").FindAction("Reload");
        
        // MUST enable the action (uses inline null check with ? operator)
        attackAction?.Enable(); 
        lookAction?.Enable();
        reloadAction?.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        // FIXME: This is part of the temporary fix; this pauses the game, in future we should change this to be a better
        // pausing system.
        if(Time.timeScale > 0)
        {
            Look();
            CheckShooting();   
        }        
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
        currentHealth -= 1;
        currentHealth = Math.Max(0,currentHealth);
        // Broadcast event that player has died if health == 0
        if(currentHealth == 0)
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
        canReload = true;
    }
    
    // Check if the player is shooting
    void CheckShooting() {
        // New input manager, returns bool if key pressed this frame
        if(attackAction.WasPressedThisFrame()) {
            // Checks if the player has ammo above zero
            if(currentAmmo > 0)
            {
                // If so, Creates a bullt prefab at the fire point (player body) position with offset, default rotation (no rotation)
                Instantiate(bulletPrefab,cameraTransform.position,cameraTransform.rotation);
                ReduceAmmo();
            }

            // TODO: some indication that player is out of ammo
                   
        }
        else if(reloadAction.WasPressedThisFrame() && canReload)
        {
            Reload();
            canReload = false;
            StartCoroutine(DoReloadCountdown()); // TODO: get reference so you can set to null and check to ensure it doesn't start twice and game over
        }
    }

    void Look()
    {
        // Get the look input vector
        Vector2 look = lookAction.ReadValue<Vector2>();

        // Scale by the sensitivity
        float mouseX = look.x * sensitivity;
        float mouseY = look.y * sensitivity;

        // VERTICAL
        // Modifly private float to clamp it
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation,-90f,90f);

        // Rotate camera locally
        cameraTransform.localRotation = Quaternion.Euler(xRotation,0f,0f);

        // HORIZONTAL
        transform.Rotate(Vector3.up * mouseX);
    }

    void ResetStats()
    {
        currentHealth = maxHealth;
        currentAmmo = maxAmmo;
        canReload = true; // TODO: make sure this timer is interrupted if it is going
    }
    
}
