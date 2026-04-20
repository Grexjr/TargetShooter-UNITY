using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Reference to game objects
    public GameObject spawnManager;

    // Reference to the UI object for wave text
    public TextMeshProUGUI waveText;
    
    // Reference to the UI object for score text
    public TextMeshProUGUI scoreText;
    

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
    }


}
