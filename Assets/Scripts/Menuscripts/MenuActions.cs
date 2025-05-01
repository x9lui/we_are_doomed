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

    [SerializeField] private GameObject _ExitMenu;
    [SerializeField] private Image _TransitionPanel;

    [SerializeField] private GameObject _MessagePanel;
    [SerializeField] private GameObject _SettingPanel;

    [SerializeField] private float _MenuSlideDuration = 0.2f;
    [SerializeField] private Vector3 _MenuHiddenPosition = new Vector3(0, -500, 0);
    [SerializeField] private Vector3 _MenuVisiblePosition = Vector3.zero;

    private Vector3 _messagePanelOriginalPos;
    private Vector3 _settingPanelOriginalPos;

    void Awake()
    {
        // Guardar posiciones originales
        if (_MessagePanel != null) _messagePanelOriginalPos = _MessagePanel.transform.localPosition;
        if (_SettingPanel != null) _settingPanelOriginalPos = _SettingPanel.transform.localPosition;
    }

    public void SinglePlayerMode()
    {
        Debug.Log("Single Player Mode Started.");  
        StartCoroutine(FadeAndLoadCredits("LoadScene"));
    }

    public void MultiplayerMode()
    {
        // Implementar si es necesario
    }

    public void OpenOptions()
    {
        _MainMenu.SetActive(false);

        if (_MessagePanel != null)
            StartCoroutine(SlidePanel(_MessagePanel, _messagePanelOriginalPos + new Vector3(-800, 0, 0), _messagePanelOriginalPos));
        
        if (_SettingPanel != null)
            StartCoroutine(SlidePanel(_SettingPanel, _settingPanelOriginalPos + new Vector3(0, 600, 0), _settingPanelOriginalPos));

        Debug.Log("Options Menu Opened.");
    }

    public void QuitGame()
    {
        _ExitMenu.SetActive(true);
        _ExitMenu.transform.localPosition = _MenuHiddenPosition;
        StartCoroutine(SlideMenuUp(_ExitMenu));

        SetPanelOpacityInstant(0.9f);
        Debug.Log("Game Exited.");
    }

    public void Credits()
    {
        Debug.Log("Credits.");
        StartCoroutine(FadeAndLoadCredits("Credits"));
    }

    public void Back()
    {
        if (_MessagePanel != null)
            StartCoroutine(SlidePanel(_MessagePanel, _messagePanelOriginalPos, _messagePanelOriginalPos + new Vector3(-800, 0, 0)));
        
        if (_SettingPanel != null)
            StartCoroutine(SlidePanel(_SettingPanel, _settingPanelOriginalPos, _settingPanelOriginalPos + new Vector3(0, 600, 0)));

        StartCoroutine(SwitchToMainAfterDelay(_MenuSlideDuration));
        Debug.Log("Back.");
    }

    private IEnumerator SwitchToMainAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        _MainMenu.SetActive(true);
    }

    public void QuitConfirm()
    {
        Application.Quit();
        Debug.Log("QuitConfirm.");
    }

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

    private IEnumerator FadeAndLoadCredits(string name)
    {
        yield return StartCoroutine(FadeToOpacity(1f));
        SceneManager.LoadScene(name);
    }

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

    private IEnumerator SlidePanel(GameObject panel, Vector3 start, Vector3 end)
    {
        panel.SetActive(true);
        panel.transform.localPosition = start;

        float elapsed = 0f;

        while (elapsed < _MenuSlideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            panel.transform.localPosition = Vector3.Lerp(start, end, elapsed / _MenuSlideDuration);
            yield return null;
        }

        panel.transform.localPosition = end;
    }
}
