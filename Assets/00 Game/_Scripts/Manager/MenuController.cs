using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void StartGame()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }

    public void RestartGame()
    {
        // Reload the current scene
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1; // Reset time scale to normal
        Debug.Log("Game restarted");
    }
    public void OpenOptions()
    {
        Debug.Log("Options menu opened");
    }

    public void QuitGame()
    {
        // Quit the application
        Application.Quit();
        Debug.Log("Game is exiting...");
    }
}
