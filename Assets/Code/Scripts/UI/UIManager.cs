using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Reference to game objects
    public GameObject spawnManager;
    public GameObject player;
    public GameObject hud;
    public GameObject gameOverUI;

    // References to the UI object for wave slider + text
    public Slider waveTimer;
    private float maxWaveTimer;
    public RectTransform waveTimerFillRect;
    public TextMeshProUGUI waveText;
    
    // Reference to the UI object for score text
    public TextMeshProUGUI scoreText;

    void Start()
    {
        // Init variables
        maxWaveTimer = spawnManager.GetComponent<SpawnManager>().waveTimer;

        // Subscribe to game manager events
        GameManager.Instance.OnGameRestart += SwapToGameUI;

        // Subscribe to SpawnManager events
        spawnManager.GetComponent<SpawnManager>().OnWaveTimerStart += StartWaveTimer;
        spawnManager.GetComponent<SpawnManager>().OnWaveTimerTick += TickWaveTimer;
        spawnManager.GetComponent<SpawnManager>().OnWaveTimerEnd += EndWaveTimer;

        player.GetComponent<PlayerController>().OnDeath += SwapToGameOver;
    }

    // Update is called once per frame
    void Update()
    {
        // Every frame, keep the wave text updated
        waveText.text = "Wave: " + GameManager.Instance.waveNum;
        scoreText.text = "Score: " + GameManager.Instance.score;
        // Wave timer, stays same size the whole time
        waveTimer.maxValue = maxWaveTimer;
    }

    void OnDisable()
    {
        // Unsubscribe to player reload timer
        if(player != null)
        {
            player.GetComponent<PlayerController>().OnDeath -= SwapToGameOver;  
        }
        
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

    void StartWaveTimer()
    {
        // Refresh max value
        maxWaveTimer = spawnManager.GetComponent<SpawnManager>().waveTimer;
        waveTimer.maxValue = maxWaveTimer;
    }


    void TickWaveTimer(float timeRemaining)
    {
        // Instead of counting down, shrink it by how big it is compared to its max
        float percent = timeRemaining / maxWaveTimer;

        waveTimerFillRect.localScale = new Vector3(percent,1,1);
    }

    void EndWaveTimer()
    {
        // set the percent to zero
        waveTimerFillRect.localScale = new Vector3(0,1,1);
    }



}
