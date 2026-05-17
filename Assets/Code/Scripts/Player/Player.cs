using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System.Collections;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerInputHandler))]
public class Player : MonoBehaviour
{

    // PLAYER CHARACTERISTICS
    private Health health;
    private PlayerInputHandler input;
    [SerializeField] private FirstPersonCamera playerCamera;
    [SerializeField] private Weapon currentWeapon;

    // PLAYER EVENTS
    public System.Action OnReloadTimerStart; // broadcast to: HUDManager
    public System.Action<float> OnReloadTimerTick; // broadcast to: HUDManager
    public System.Action OnReloadTimerEnd; // broadcast to: HUDManager
    public System.Action OnDeath; // broadcast to: GameManager

    // PLAYER VARIABLES
    public int maxAmmo, currentAmmo;

    // Timer variable for reload buffer, in seconds
    private float reloadBuffer = 1.5f;

    // Initialization, guaranteed to happen first
    void Awake()
    {
        // Basic initialization
        health = GetComponent<Health>();
        input = GetComponent<PlayerInputHandler>();

        // Subscribe to game manager broadcasts and call reset state when game is restarted
        GameManager.Instance.OnGameRestart += ResetState;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Subscribe to input events
        input.onAttack += Shoot;
        input.onReload += Reload;

        // Initialize weapon if it exists
        if(currentWeapon != null)
        {
            // Subscribe to weapon events
            currentWeapon.onReloadStart += BubbleReloadStart;
            currentWeapon.onReloadTick += BubbleReloadTick;
            currentWeapon.onReloadEnd += BubbleReloadEnd;
            // Initialize weapon information
            maxAmmo = currentWeapon.GetMaxAmmo();
            currentAmmo = currentWeapon.GetCurrentAmmo();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Get the look input vector
        Vector2 look = input.LookInput;
        
        // TODO: This is part of the temporary fix; this pauses the game, in future we should change this to be a better
        // pausing system.
        if(Time.timeScale > 0)
        {
            Look(look.x,look.y);  
        }
    }

    void OnDisable()
    {
        GameManager.Instance.OnGameRestart -= ResetState;

        input.onAttack -= Shoot;
        input.onReload -= Reload;

        if(currentWeapon != null)
        {
            currentWeapon.onReloadStart -= BubbleReloadStart;
            currentWeapon.onReloadTick -= BubbleReloadTick;
            currentWeapon.onReloadEnd -= BubbleReloadEnd;
        }
    }

    public float GetReloadBuffer()
    {
        return reloadBuffer;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        // Reduce health
        health.TakeDamage(damage);
        // Broadcast event that player has died if health == 0
        if(health.CheckDeath())
        {
            OnDeath?.Invoke();
        }
    }

    void Reload()
    {
        if(currentWeapon != null)
        {
            currentWeapon.Reload();
        }
    }

    void Shoot()
    {
        // Get camera transform
        Transform camTransform = playerCamera.transform;
        Vector3 targetPoint;

        // If gun does not exist, just return and do nothing
        if(currentWeapon == null) return;

        // Cast a ray from your eyes straight through crosshair
        if(Physics.Raycast(camTransform.position,camTransform.forward, out RaycastHit hit, 100f, ~LayerMask.GetMask("Player")))
        {
            // Hit enemy or wall
            targetPoint = hit.point;
        }
        else
        {
            // Otherwise choose point 100 meters away
            targetPoint = camTransform.position + camTransform.forward * 10f;
        }

        // Pass target point and camera transform to gun's fire method
        currentWeapon.Fire(playerCamera.transform,targetPoint);
        currentAmmo = currentWeapon.GetCurrentAmmo();   
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

    void ResetState()
    {
        // Reset health
        health.ResetHealth();
        
        // Reset weapon
        if(currentWeapon != null)
        {
            currentWeapon.Reset();
        }
    }

    private void BubbleReloadStart()
    {
        OnReloadTimerStart?.Invoke();
    }

    private void BubbleReloadTick(float timeRemaining)
    {
        OnReloadTimerTick?.Invoke(timeRemaining);
    }

    private void BubbleReloadEnd()
    {
        OnReloadTimerEnd?.Invoke();
        //TODO Change to a single function update ammo/update weapon
        currentAmmo = currentWeapon.GetCurrentAmmo();
    }


}
