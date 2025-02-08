using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // This function is called when the Start button is clicked
    public void StartGame()
    {
        // Load the main game scene, replace "MainGameScene" with the name of your scene
        SceneManager.LoadScene("Map1");
    }

    // This function is called when the Options button is clicked
    public void OpenOptions()
    {
        // Open the Options menu, here you can add code to show the Options UI
        Debug.Log("Options menu opened");
    }

    // This function is called when the Quit button is clicked
    public void QuitGame()
    {
        // Quit the application
        Application.Quit();
        Debug.Log("Game is exiting...");
    }
}
