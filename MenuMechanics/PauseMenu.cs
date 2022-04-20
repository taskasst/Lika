using UnityEngine;
using UnityEngine.SceneManagement; // include so we can load new scenes

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseCanvas;

    public void Resume()
    {
        // Resume button was pressed
        Time.timeScale = 1;
        pauseCanvas.SetActive(false);
    }
    
    public void MainMenu(string level)
    {
        // Main menu button was pressed
        Time.timeScale = 1;
        SceneManager.LoadScene(level);
    }
}
