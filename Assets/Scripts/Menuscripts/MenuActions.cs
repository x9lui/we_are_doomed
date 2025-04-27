using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Defines actions that can be triggered from menu buttons.
/// </summary>
public class MenuActions : MonoBehaviour
{
    [SerializeField] private GameObject _MainMenu;
    [SerializeField] private GameObject _OptionsMenu;

    [SerializeField] private GameObject _ExitMenu;
    [SerializeField] private Image _transitionPanel;

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
        _ExitMenu.SetActive(true);
        _MainMenu.SetActive(false);
        SetPanelOpacityInstant(0.7f); // Set to 50% instantly
        Debug.Log("Game Exited.");
        Application.Quit();
    }

    /// <summary>
    /// Opens the credits scene with a fade transition.
    /// </summary>
    public void Credits()
    {
        Debug.Log("Credits.");
        StartCoroutine(FadeAndLoadCredits());
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

    /// <summary>
    /// Sets the panel opacity instantly to a target value.
    /// </summary>
    /// <param name="targetAlpha">Target opacity value between 0 and 1.</param>
    private void SetPanelOpacityInstant(float targetAlpha)
    {
        if (_transitionPanel != null)
        {
            _transitionPanel.gameObject.SetActive(true);
            Color color = _transitionPanel.color;
            color.a = targetAlpha;
            _transitionPanel.color = color;
        }
    }

    /// <summary>
    /// Coroutine to fade the transition panel to full opacity and load Credits scene.
    /// </summary>
    private IEnumerator FadeAndLoadCredits()
    {
        yield return StartCoroutine(FadeToOpacity(1f)); // Fade to full black (opacity 1.0)
        SceneManager.LoadScene("Credits");
    }

    /// <summary>
    /// Coroutine to fade the panel smoothly to a target opacity.
    /// </summary>
    /// <param name="targetAlpha">Target opacity value between 0 and 1.</param>
    private IEnumerator FadeToOpacity(float targetAlpha)
    {
        if (_transitionPanel != null)
        {
            _transitionPanel.gameObject.SetActive(true);

            float fadeDuration = 1.0f;
            float elapsed = 0f;
            Color color = _transitionPanel.color;
            float initialAlpha = color.a;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                color.a = Mathf.Lerp(initialAlpha, targetAlpha, elapsed / fadeDuration);
                _transitionPanel.color = color;
                yield return null;
            }

            color.a = targetAlpha;
            _transitionPanel.color = color;
        }
    }
}
