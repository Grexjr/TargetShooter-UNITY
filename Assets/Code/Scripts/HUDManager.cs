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
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
    }


}
