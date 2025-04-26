using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Defines actions that can be triggered from menu buttons.
/// </summary>
public class MenuActions : MonoBehaviour
{
    [SerializeField] private GameObject _MainMenu;
    [SerializeField] private GameObject _OptionsMenu;

    /// <summary>
    /// Starts the game by loading the SinglePlayerScene.
    /// </summary>
    public void SinglePlayerMode()
    {
        // Load the SinglePlayerScene  
    }

    /// <summary>
    /// Starts the game by loading the MultiPlayerScene.
    /// </summary>
    public void MultiplayerMode()
    {
        // Load the MultiPlayerScene
    }

    /// <summary>
    /// Opens the options menu.
    /// </summary>
    public void OpenOptions()
    {
        _MainMenu.SetActive(false);
        _OptionsMenu.SetActive(true);
        Debug.Log("Options Menu Opened.");
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Exited.");
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void Credits()
    {
        Debug.Log("Credits.");
    }

    /// <summary>
    /// Return to the main menu.
    /// </summary>
    public void Back()
    {
        _MainMenu.SetActive(true);
        _OptionsMenu.SetActive(false);

        Debug.Log("Back.");
    }
}
