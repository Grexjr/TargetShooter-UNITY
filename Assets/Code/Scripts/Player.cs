using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // TODO - here for now but should change, lock cursor into screen
        Cursor.lockState = CursorLockMode.Locked;

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
        Look();
        CheckShooting();
    }
    
    // Check if the player is shooting
    void CheckShooting() {
        // New input manager, returns bool if key pressed this frame
        if(attackAction.WasPressedThisFrame()) {
            // Creates a bullt prefab at the fire point (player body) position with offset,
            // Creates a bullet prefab with default rotation (no rotation)
            Instantiate(bulletPrefab,cameraTransform.position,cameraTransform.rotation);
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
