using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

/// <summary>
/// Defines actions that can be triggered from menu buttons.
/// </summary>
public class MenuActions : MonoBehaviour
{
    [SerializeField] private GameObject _MainMenu;
    [SerializeField] private GameObject _OptionsMenu;

    [SerializeField] private GameObject _ExitMenu;
    [SerializeField] private Image _TransitionPanel;

    [SerializeField] private float _MenuSlideDuration = 0.2f; // Duración rápida
    [SerializeField] private Vector3 _MenuHiddenPosition = new Vector3(0, -500, 0); // Posición inicial fuera de pantalla
    [SerializeField] private Vector3 _MenuVisiblePosition = Vector3.zero; // Centro del canvas

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
        _ExitMenu.transform.localPosition = _MenuHiddenPosition;
        StartCoroutine(SlideMenuUp(_ExitMenu));

        SetPanelOpacityInstant(0.9f);
        Debug.Log("Game Exited.");
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

    public void QuitConfirm()
    {
        Application.Quit();
        Debug.Log("QuitConfirm.");
    }

    /// <summary>
    /// Sets the panel opacity instantly to a target value.
    /// </summary>
    /// <param name="targetAlpha">Target opacity value between 0 and 1.</param>
    private void SetPanelOpacityInstant(float targetAlpha)
    {
        if (_TransitionPanel != null)
        {
            _TransitionPanel.gameObject.SetActive(true);
            Color color = _TransitionPanel.color;
            color.a = targetAlpha;
            _TransitionPanel.color = color;
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
        if (_TransitionPanel != null)
        {
            _TransitionPanel.gameObject.SetActive(true);

            float fadeDuration = 1.0f;
            float elapsed = 0f;
            Color color = _TransitionPanel.color;
            float initialAlpha = color.a;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                color.a = Mathf.Lerp(initialAlpha, targetAlpha, elapsed / fadeDuration);
                _TransitionPanel.color = color;
                yield return null;
            }

            color.a = targetAlpha;
            _TransitionPanel.color = color;
        }
    }

    private IEnumerator SlideMenuUp(GameObject menu)
    {
        float elapsed = 0f;
        Vector3 start = _MenuHiddenPosition;
        Vector3 end = _MenuVisiblePosition;

        while (elapsed < _MenuSlideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            menu.transform.localPosition = Vector3.Lerp(start, end, elapsed / _MenuSlideDuration);
            yield return null;
        }

        menu.transform.localPosition = end;
    }

}
