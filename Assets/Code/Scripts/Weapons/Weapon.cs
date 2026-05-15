using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    // BULLET VARIABLES
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float reloadTime = 5.0f;

    // BULLET EVENTS
    //TODO: Event for reload that player can call on to do ui reload stuff
    //TODO: Event for firing in case we want to add sound, etc

    private float nextFireTime;
    private int maxAmmo = 10;
    private int currentAmmo;

    void Awake()
    {
        RefillAmmo(); 
    }

    // Method to fire gun
    void Fire()
    {
        // If time has not passed enough for next fire time, return early (no shoot)
        if(Time.time < nextFireTime) return;
        
        // If no ammunition, don't fire
        if(!HasAmmo()) return;

        // TODO: Add state here for reloading; if reloading, do not fire


        // Otherwise, fire
        ExecuteShot();
    }

    // TODO: Reload function
    void Reload()
    {
        // Invoke event

        // Do own internal reload timer (same way as player and UI interact)

        // Finish the reload same way as in player
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

    private void ExecuteShot()
    {
        // Reduce ammo, instantiate a bullet at the muzzle position and then increment next fire time for next cooldown
        currentAmmo--;

        nextFireTime = Time.time + fireRate;

        Instantiate(bulletPrefab,muzzle.position,muzzle.rotation);
    }

}
