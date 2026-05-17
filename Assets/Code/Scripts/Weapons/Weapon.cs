using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    private enum WeaponState
    {
        READY,
        RELOADING
    }

    // WEAPON CHARACTERISTICS
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float reloadTime = 0.5f;
    [SerializeField] private int maxAmmo = 10;
    [SerializeField] private WeaponState weaponState = WeaponState.READY;

    // WEAPON EVENTS
    public System.Action onReloadStart;
    public System.Action<float> onReloadTick;
    public System.Action onReloadEnd;
    //TODO: Event for firing in case we want to add sound, etc

    // RELOAD VARIABLES
    private float nextFireTime;
    private int currentAmmo;
    // Variable for reference to coRoutine in case reset is called
    private Coroutine reloadRoutine;

    // Getters
    public Coroutine GetReloadRoutine()
    {
        return reloadRoutine;
    }

    public int GetMaxAmmo()
    {
        return maxAmmo;
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    // Method to fire gun
    public void Fire(Transform cameraTransform, Vector3 targetPoint)
    {
        // If time has not passed enough for next fire time, return early (no shoot)
        if(Time.time < nextFireTime) return;
        
        // If no ammunition, don't fire
        if(!HasAmmo()) return;

        // If reloading, then don't fire
        if(weaponState == WeaponState.RELOADING) return;

        // Otherwise, fire
        ExecuteShot(cameraTransform, targetPoint);
    }

    // TODO: Reload function
    public void Reload()
    {
        // Set state to reloading
        weaponState = WeaponState.RELOADING;

        // Invoke event
        onReloadStart?.Invoke();

        // Start the reloading routine
        reloadRoutine = StartCoroutine(CountdownReload());
    }

    public void Reset()
    {
        RefillAmmo();
        if(reloadRoutine != null)
        {
            // Stop and remov the coRoutine
            StopCoroutine(reloadRoutine);
            reloadRoutine = null;
            // Tell the UI to hide the bar
            onReloadEnd?.Invoke();
        }
    }

    private IEnumerator CountdownReload()
    {
        float timeRemaining = reloadTime;

        while(timeRemaining > 0)
        {
            // Decrement time remaining by time difference
            timeRemaining -= Time.deltaTime;

            // Send out event that the reload timer is ticking (for UI purposes)
            onReloadTick?.Invoke(timeRemaining);

            yield return null;
        }

        // Finish the reload same way as in player: refill ammo then broadcast that reload is done
        RefillAmmo();
        weaponState = WeaponState.READY;
        onReloadEnd?.Invoke();
    }


    private void RefillAmmo()
    {
        currentAmmo = maxAmmo;
    }

    /// <summary>
    /// Checks if the gun has ammo or not
    /// </summary>
    /// <returns> False if current ammo is at or below 0, true if otherwise. </returns>
    private Boolean HasAmmo()
    {
        if(currentAmmo <= 0) return false;
        return true;
    }

    private void ExecuteShot(Transform cameraTransform, Vector3 targetPoint)
    {
        // Reduce ammo, instantiate a bullet at the muzzle position and then increment next fire time for next cooldown
        currentAmmo--;

        nextFireTime = Time.time + fireRate;

        Vector3 directionToTarget = targetPoint - muzzle.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        Instantiate(bulletPrefab,muzzle.position,targetRotation);
    }

    void Awake()
    {
        RefillAmmo(); 
    }

}
