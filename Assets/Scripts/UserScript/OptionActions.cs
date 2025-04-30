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
            // Si se está mostrando el menú, reseteamos cualquier confirmación previa
            _ConfirmationExit.SetActive(false);
            _ConfirmationExitYes.SetActive(false);
            _ConfirmationExitNo.SetActive(false);
            _DescripcionText.SetActive(false);
        }
    }

    public void Reanudar()
    {
        _OptionsMenu.SetActive(false);
    }

    public void Reaparecer()
    {
        // Aquí iría la lógica de reaparecer al jugador
    }

    public void Salir()
    {
        if(_ConfirmationExit.activeSelf)
        {
            _ConfirmationExit.SetActive(false);
            _ConfirmationExitYes.SetActive(false);
            _ConfirmationExitNo.SetActive(false);
            _DescripcionText.SetActive(false);
        }else{
            _ConfirmationExit.SetActive(true);
            _ConfirmationExitYes.SetActive(true);
            _ConfirmationExitNo.SetActive(true);
            _DescripcionText.SetActive(true);

            Debug.Log("Menú de confirmación abierto.");
        }
        
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
