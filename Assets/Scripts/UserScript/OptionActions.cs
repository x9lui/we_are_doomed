using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class OptionActions : MonoBehaviour
{
    [Header("Referencias del Menú")]
    [SerializeField] private GameObject _OptionsMenu;
    [SerializeField] private GameObject _ConfirmationExit;
    [SerializeField] private GameObject _ConfirmationExitYes;
    [SerializeField] private GameObject _ConfirmationExitNo;
    [SerializeField] private GameObject _DescripcionText;
    [SerializeField] private Image _TransitionPanel;

    [SerializeField] private GameObject _OptionsMenuDisplay;
    [SerializeField] private GameObject _MenuPrincipalDisplay;

    [Header("Mover")]
    [SerializeField] private UIElementMover _mover;

    private Vector3 _optionsOriginal;
    private Vector3 _menuOriginal;

    private bool pulsado = false;

    private void Start()
    {
        // Guardamos las posiciones originales
        _optionsOriginal = _OptionsMenuDisplay.transform.localPosition;
        _menuOriginal = _MenuPrincipalDisplay.transform.localPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOptionsMenu();
        }
    }

    private void ToggleOptionsMenu()
    {
        bool isActive = _OptionsMenu.activeSelf;
        _OptionsMenu.SetActive(!isActive);

        if (!isActive)
        {
            _ConfirmationExit.SetActive(false);
            _ConfirmationExitYes.SetActive(false);
            _ConfirmationExitNo.SetActive(false);
            _DescripcionText.SetActive(false);
        }
    }

    public void Reanudar()
    {
        // Ambos bajan desde su posición actual
        Vector3 downOffset = new Vector3(0, -600, 0);
        _mover.Move(_OptionsMenuDisplay, _OptionsMenuDisplay.transform.localPosition, _optionsOriginal + downOffset);
        _mover.Move(_MenuPrincipalDisplay, _MenuPrincipalDisplay.transform.localPosition, _menuOriginal + downOffset);
        pulsado = false;
        _OptionsMenu.SetActive(false);
    }

    public void Reaparecer()
    {
        // Aquí iría la lógica de reaparecer al jugador
    }

    public void Opciones()
    {
        // MenuPrincipal se mueve un poco a la izquierda
        // OptionsMenuDisplay se mueve un poco a la derecha
        Vector3 menuLeft = _menuOriginal + new Vector3(-138, 0, 0);
        Vector3 optionsRight = _optionsOriginal + new Vector3(214, 0, 0);

        if(!pulsado)
        {
            pulsado = true;
    
            _mover.Move(_MenuPrincipalDisplay, _MenuPrincipalDisplay.transform.localPosition, menuLeft);
            _mover.Move(_OptionsMenuDisplay, _OptionsMenuDisplay.transform.localPosition, optionsRight);
        }
        else
        {
            pulsado = false;
            _mover.Move(_MenuPrincipalDisplay, _MenuPrincipalDisplay.transform.localPosition, _menuOriginal);
            _mover.Move(_OptionsMenuDisplay, _OptionsMenuDisplay.transform.localPosition, _optionsOriginal);
        }

    }

    public void Salir()
    {
        bool active = _ConfirmationExit.activeSelf;

        _ConfirmationExit.SetActive(!active);
        _ConfirmationExitYes.SetActive(!active);
        _ConfirmationExitNo.SetActive(!active);
        _DescripcionText.SetActive(!active);

        Debug.Log(active ? "Menú de confirmación cerrado." : "Menú de confirmación abierto.");
    }

    public void SalirSI()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SalirNO()
    {
        _ConfirmationExit.SetActive(false);
        _ConfirmationExitYes.SetActive(false);
        _ConfirmationExitNo.SetActive(false);
        _DescripcionText.SetActive(false);
    }
}
