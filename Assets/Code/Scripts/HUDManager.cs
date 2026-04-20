using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Reference to game objects
    public GameObject spawnManager;
    public GameObject player;

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
        player.GetComponent<Player>().OnReloadTimerStart += () =>
        {
            // Add temporary timer to HUD
            reloadTimer.gameObject.SetActive(true);
            reloadTimer.value = maxReload;
        };
        player.GetComponent<Player>().OnReloadTimerTick += (float timeRemaining) =>
        {
            // Count down with the value from the player functions
            reloadTimer.value = timeRemaining;
        };
        player.GetComponent<Player>().OnReloadTimerEnd += () =>
        {
            // Remove temporary timer from HUD
            reloadTimer.gameObject.SetActive(false);
        };
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


}
