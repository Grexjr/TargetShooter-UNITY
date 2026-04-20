using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    public System.Action OnTakeDamage;

    // References to other necessary objects
    public Transform cameraTransform;

    // Basic necessary player stats: health and ammo
    public int maxHealth, currentHealth;
    public int maxAmmo, currentAmmo;
    
    // Controls variables
    public float sensitivity = 1.0f;

    // Bullet reference
    public GameObject bulletPrefab;
    
    // Input maps
    private InputAction attackAction;
    private InputAction lookAction;

    // Rotation values for looking around
    private float xRotation = 0.0f;


    // Initialization, guaranteed to happen first
    void Awake()
    {
        // Initialize player values
        maxHealth = 10;
        maxAmmo = 5;
        currentHealth = maxHealth;
        currentAmmo = maxAmmo;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Save reference to the input system's binding of the attack action
        attackAction = InputSystem.actions.FindActionMap("Player").FindAction("Attack");
        lookAction = InputSystem.actions.FindActionMap("Player").FindAction("Look");
        
        // MUST enable the action
        if(attackAction != null) {
            attackAction.Enable();
        } 
        if(lookAction != null)
        {
            lookAction.Enable();
        }
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

    public void TakeDamage()
    {
        // Reduce health
        currentHealth -= 1;
        // Broadcast that player was hit
        OnTakeDamage?.Invoke();
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
            }

            // TODO: some indication that player is out of ammo
                   
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
    
}
