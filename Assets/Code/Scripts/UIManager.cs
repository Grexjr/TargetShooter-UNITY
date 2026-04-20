using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Reference to game objects
    public GameObject spawnManager;

    // Reference to the UI object for wave text
    public TextMeshProUGUI waveText;
    private int wave = 0;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // initialize references
        wave = spawnManager.GetComponent<SpawnManager>().GetWave();

        // subscribe to wave update event
        spawnManager.GetComponent<SpawnManager>().OnWaveComplete += () =>
        {
            // Update the wave value
            wave = spawnManager.GetComponent<SpawnManager>().GetWave();
        };
    }

    // Update is called once per frame
    void Update()
    {
        // Every frame, keep the wave text updated
        waveText.text = "Wave: " + wave;
    }


}
