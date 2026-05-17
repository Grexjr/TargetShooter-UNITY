using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    // References to canvasses for disabling and enabling for UI functions
    public GameObject mainMenuCanvas;
    public GameObject settingsCanvas;

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Loads next scene in the build settings list
    }

    public void QuitGame()
    {
        Debug.Log("Quit Pressed!");
        Application.Quit();
    }

    public void OpenSettings()
    {
        // Disable the main menu canvas, and enable the settings canvas
        // (background is separate game object so it persists between enables and disables)
        mainMenuCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
