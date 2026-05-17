using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    // PRIVATE CONSTANTS
    private static readonly String AMMO_DIVIDER = "/";

    // AMMO UI VARIABLES
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Slider reloadTimer;
    // TODO: Create a weapon manager so you just need to reference that script rather than player controller itself
    [SerializeField] private PlayerController playerControl;
    private float maxReload;

    void Start()
    {
        // Init variables
        maxReload = playerControl.GetReloadTime();

        // Init slider
        reloadTimer.maxValue = maxReload;

        int current = playerControl.GetCurrentAmmo();
        int max = playerControl.GetMaxAmmo();
        
        UpdateAmmoDisplay(current,max);
    }

    void OnEnable()
    {
        // Subscribe to player reload time events
        if(playerControl != null)
        {
            playerControl.OnReloadTimerStart += StartReloadTimer;
            playerControl.OnReloadTimerTick += TickReloadTimer;
            playerControl.OnReloadTimerEnd += EndReloadTimer;
            playerControl.onAmmoChanged += UpdateAmmoDisplay;   
        }
    }

    void OnDisable()
    {
        if(playerControl != null)
        {
            playerControl.OnReloadTimerStart -= StartReloadTimer;
            playerControl.OnReloadTimerTick -= TickReloadTimer;
            playerControl.OnReloadTimerEnd -= EndReloadTimer;   
        }
    }


    private void StartReloadTimer()
    {   
        // Enable the timer object itself
        reloadTimer.gameObject.SetActive(true);
        reloadTimer.value = maxReload;
    }

    private void TickReloadTimer(float timeRemaining)
    {
        reloadTimer.value = timeRemaining;
    }

    private void EndReloadTimer()
    {
        // Disable the timer object itself
        reloadTimer.gameObject.SetActive(false);
    }

    private void UpdateAmmoDisplay(int current, int max)
    {
        ammoText.text = current + AMMO_DIVIDER + max;
    }


}
