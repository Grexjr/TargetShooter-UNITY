using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Reference to game objects
    public GameObject spawnManager;
    public GameObject player;
    public GameObject hud;
    public GameObject gameOverUI;

    // Reference to the UI object for wave text
    public TextMeshProUGUI waveText;
    
    // Reference to the UI object for score text
    public TextMeshProUGUI scoreText;

    // Reference to the UI object for slider value and text value
    public Slider healthBar;
    public TextMeshProUGUI healthText;

    // Reference to the UI object for ammo value
    public TextMeshProUGUI ammoText;

    // Reference to the reload timer
    public Slider reloadTimer;
    private float maxReload = 5.0f;

    void Start()
    {
        // Subscribe to player reload timer
        player.GetComponent<Player>().OnReloadTimerStart += StartReloadTimer;
        player.GetComponent<Player>().OnReloadTimerTick += TickReloadTimer;
        player.GetComponent<Player>().OnReloadTimerEnd += RemoveReloadTimer;
        player.GetComponent<Player>().OnDeath += SwapToGameOver;
        // Subscribe to game manager events
        GameManager.Instance.OnGameRestart += SwapToGameUI;
    }

    // Update is called once per frame
    void Update()
    {
        // Every frame, keep the wave text updated
        waveText.text = "Wave: " + GameManager.Instance.waveNum;
        scoreText.text = "Score: " + GameManager.Instance.score;
        healthBar.value = player.GetComponent<Player>().currentHealth;
        healthBar.maxValue = player.GetComponent<Player>().maxHealth;
        healthText.text = player.GetComponent<Player>().currentHealth + "/" + player.GetComponent<Player>().maxHealth;
        ammoText.text = player.GetComponent<Player>().currentAmmo + "/" + player.GetComponent<Player>().maxAmmo;
        // Set reload timer max every frame, but its current value is handled by the countdown co-routine in player class
        reloadTimer.maxValue = maxReload;
    }

    void OnDisable()
    {
        // Unsubscribe to player reload timer
        player.GetComponent<Player>().OnReloadTimerStart -= StartReloadTimer;
        player.GetComponent<Player>().OnReloadTimerTick -= TickReloadTimer;
        player.GetComponent<Player>().OnReloadTimerEnd -= RemoveReloadTimer;
        player.GetComponent<Player>().OnDeath -= SwapToGameOver;
        // Unsubscribe to game manager events
        GameManager.Instance.OnGameRestart -= SwapToGameUI;
    }

    void SwapToGameOver()
    {
        // First set the graphics raycasting of the game UI to false so it does not show up and set game over to true
        hud.GetComponent<GraphicRaycaster>().enabled = false;
        hud.gameObject.SetActive(false);
        gameOverUI.gameObject.SetActive(true);
        gameOverUI.GetComponent<GraphicRaycaster>().enabled = true;
    }

    void SwapToGameUI()
    {
        gameOverUI.gameObject.SetActive(false);
        gameOverUI.GetComponent<GraphicRaycaster>().enabled = false;
        hud.gameObject.SetActive(true);
        hud.GetComponent<GraphicRaycaster>().enabled = true;
    }

    void StartReloadTimer()
    {
        // Add temporary timer to HUD
        reloadTimer.gameObject.SetActive(true);
        reloadTimer.value = maxReload;
    }

    void TickReloadTimer(float timeRemaining)
    {
        // Count down with the value from the player functions
        reloadTimer.value = timeRemaining;
    }

    void RemoveReloadTimer()
    {
        // Remove temporary timer from HUD
        reloadTimer.gameObject.SetActive(false);
    }




}
