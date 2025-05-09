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
    

    [Header("Menus")]
    [SerializeField] private GameObject _MainMenu;
    [SerializeField] private GameObject _ExitMenu;
    [SerializeField] private GameObject _OptionMenu;

    [Header("Panels")]
    [SerializeField] private Image _TransitionPanel;
    [SerializeField] private GameObject _MessagePanel;
    [SerializeField] private GameObject _SettingPanel;

    [Header("Sounds")]
    [SerializeField] private AudioClip ClickDeBoton;
    [SerializeField] private AudioClip _MetalSound;
    [SerializeField] private float _MenuSlideDuration = 1f;
    [SerializeField] private Vector3 _MenuHiddenPosition = new Vector3(0, -500, 0);
    [SerializeField] private Vector3 _MenuVisiblePosition = Vector3.zero;

    // Variables para controlar la posición de los paneles
    private Vector3 _messagePanelOriginalPos;
    private Vector3 _settingPanelOriginalPos;
    private Vector3 _confirmationOriginalPos;
    private Vector3 _OptionLeft;
    private Vector3 _OptionDown;
    private Vector3 _ConfirmationUp;


    [Header("Scripts")]
    [SerializeField] private MenuSelector _MenuSelector;
    [SerializeField] private UIElementMover _mover;

    void Awake()
    {
        // Guardar posiciones originales
        _messagePanelOriginalPos = _MessagePanel.transform.localPosition;
        _settingPanelOriginalPos = _SettingPanel.transform.localPosition;
        _confirmationOriginalPos = _ExitMenu.transform.localPosition;
    }

    public void SinglePlayerMode()
    {
        StartCoroutine(FadeAndLoadCredits("LoadScene"));

        // Sonido de botón
        AudioManager.Instance.Reproducir(ClickDeBoton);

        // Mensaje terminal
        Debug.Log("Single Player Mode Started.");  
    }

    public void MultiplayerMode()
    {
        // Implementar si es necesario
    }

    public void OpenOptions()
    {
        _OptionMenu.SetActive(true);

        // Desactivar el menú principal y el selector de menú
        _MainMenu.SetActive(false);
        _MenuSelector.enabled = false;

        // Nuevas posiciones para los paneles
        _OptionLeft = _messagePanelOriginalPos + new Vector3(652, 0, 0);
        _OptionDown = _settingPanelOriginalPos + new Vector3(0, -550, 0);

        // Movemos los paneles a sus nuevas posiciones
        _mover.Move(_MessagePanel, _messagePanelOriginalPos, _OptionLeft);
        _mover.Move(_SettingPanel, _settingPanelOriginalPos, _OptionDown);


        // Sonido de botón
        AudioManager.Instance.Reproducir(_MetalSound);

        // Mensaje terminal
        Debug.Log("Options Menu Opened.");
    }

    public void QuitGame()
    {
        _ExitMenu.SetActive(true);

        //Desactivar el selector de menú
        _MenuSelector.enabled = false;

        // Nuevas posiciones para los paneles
        _ConfirmationUp = _confirmationOriginalPos + new Vector3(0, 726, 0);

        // Movemos los paneles a sus nuevas posiciones
        _mover.Move(_ExitMenu, _confirmationOriginalPos, _ConfirmationUp);

        // Opacidad del panel de transición
        SetPanelOpacityInstant(0.9f);

        // Sonido de botón
        AudioManager.Instance.Reproducir(_MetalSound);

        // Mensaje terminal
        Debug.Log("Game Exited.");
    }

    public void Credits()
    {
        
        StartCoroutine(FadeAndLoadCredits("Credits"));
        
        // Sonido de botón
        AudioManager.Instance.Reproducir(ClickDeBoton);

        // Mensaje terminal
        Debug.Log("Credits.");
    }

    public void Back()
    {
        // Activar el menú principal y el selector de menú
        StartCoroutine(SwitchToMainAfterDelay(_MenuSlideDuration, _MainMenu, true));
        _MenuSelector.enabled = true;

        // Movemos los paneles a sus nuevas posiciones
        _mover.Move(_MessagePanel, _OptionLeft, _messagePanelOriginalPos);
        _mover.Move(_SettingPanel, _OptionDown, _settingPanelOriginalPos);

        // Sonido de botón
        AudioManager.Instance.Reproducir(_MetalSound);

        StartCoroutine(SwitchToMainAfterDelay(_MenuSlideDuration, _OptionMenu, false));

        // Mensaje terminal
        Debug.Log("Back.");
    }

    public void QuitConfirm()
    {
        Application.Quit();

        // Sonido de botón
        AudioManager.Instance.Reproducir(ClickDeBoton);

        // Mensaje terminal
        Debug.Log("QuitConfirm.");
    }

    public void QuitCancel()
    {
        // Activar el selector de menú
        _MenuSelector.enabled = true;

        // Movemos los paneles a sus nuevas posiciones
        _mover.Move(_ExitMenu, _ConfirmationUp, _confirmationOriginalPos);

        // Sonido de botón
        AudioManager.Instance.Reproducir(_MetalSound);

        // Opacidad del panel de transición
        _TransitionPanel.gameObject.SetActive(false);

        // Desactivar el menú de salida
        StartCoroutine(SwitchToMainAfterDelay(_MenuSlideDuration, _ExitMenu, false));

        // Mensaje terminal
        Debug.Log("Quit Cancel.");
    }


    private IEnumerator SwitchToMainAfterDelay(float delay, GameObject menu, bool isActive){
        yield return new WaitForSecondsRealtime(delay);
        menu.SetActive(isActive);
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
}
