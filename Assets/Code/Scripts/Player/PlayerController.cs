using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System.Collections;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviour
{

    // PLAYER CHARACTERISTICS
    [SerializeField] private Weapon currentWeapon;
    private Health health;
    private PlayerInputHandler input;
    private FirstPersonCamera playerCamera;

    // PLAYER EVENTS

    // Weapon events
    public System.Action OnReloadTimerStart;
    public System.Action<float> OnReloadTimerTick;
    public System.Action OnReloadTimerEnd;
    public System.Action<int, int> onAmmoChanged;

    // Health events
    public System.Action onHealthChanged;
    public System.Action OnDeath;

    // PLAYER VARIABLES
    public int maxAmmo, currentAmmo;

    // Initialization, guaranteed to happen first
    void Awake()
    {
        // Basic initialization
        health = GetComponent<Health>();
        input = GetComponent<PlayerInputHandler>();
        playerCamera = GetComponentInChildren<FirstPersonCamera>();

        // Subscribe to game manager broadcasts and call reset state when game is restarted
        GameManager.Instance.OnGameRestart += ResetState;

        // Initialize weapon if it exists
        if(currentWeapon != null)
        {
            // Initialize weapon information
            maxAmmo = currentWeapon.GetMaxAmmo();
            currentAmmo = currentWeapon.GetCurrentAmmo();
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Subscribe to input events
        input.onAttack += Shoot;
        input.onReload += Reload;
    }

    void OnEnable()
    {
        // Subscribe to weapon events if it exists
        if(currentWeapon != null)
        {
            currentWeapon.onReloadStart += BubbleReloadStart;
            currentWeapon.onReloadTick += BubbleReloadTick;
            currentWeapon.onReloadEnd += BubbleReloadEnd;
            currentWeapon.onAmmoChanged += BubbleAmmoChanged;  
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
            currentWeapon.onAmmoChanged -= BubbleAmmoChanged;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    // GETTERS
    public float GetReloadTime()
    {
        return currentWeapon.GetReloadTime();
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public int GetMaxAmmo()
    {
        return maxAmmo;
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
        currentWeapon.Fire(targetPoint);
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
    }

    private void BubbleAmmoChanged(int current, int max)
    {
        // Code we need to do at this step: set variables
        currentAmmo = current;
        maxAmmo = max;

        // Bubble up the event
        onAmmoChanged?.Invoke(current,max);
    }


}
