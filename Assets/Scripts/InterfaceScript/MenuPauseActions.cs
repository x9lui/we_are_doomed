using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.LowLevelPhysics;

public class MenuPauseActions : MonoBehaviour
{
    
    [SerializeField] private GameObject _OptionPanel;
    [SerializeField] private GameObject _DescripcionText;
    [SerializeField] private Image _TransitionPanel;
    public TextMeshProUGUI _ContadorDeMuertes;
    private int muertes = 0;

    [Header("Menus")]
    
    [SerializeField] private GameObject _OptionsMenuDisplay;
    [SerializeField] private GameObject _MenuPrincipalDisplay;
    [SerializeField] private GameObject _ConfirmationExit;

    [SerializeField] private GameObject deathPanelUI;

    [Header("Scripts")]
    [SerializeField] private UIElementMover _mover;
    [SerializeField] private MouseLook _mouseLookScript;

    [Header("Sounds")]

    [SerializeField] private AudioClip ClickDeBoton;
    [SerializeField] private AudioClip _MetalSound;

    [SerializeField] private AudioClip _Death;

    [Header("Modo")]

    [SerializeField] private bool UnJugador = true;

    [Header("Jugador")]
    [SerializeField] private GameObject player;

    private Vector3 posicionInicialJugador;

    // Variables para controlar la posición
    private Vector3 _optionsOriginal;
    private Vector3 _menuOriginal;
    private Vector3 _confirmationOriginal;
    private Vector3 _menuUp;
    private Vector3 _optionUp;
    private Vector3 ConfirmationDown;

    // Variables para controlar el estado de los botones
    private bool pulsadoOpciones = false;
    private bool pulsadoAbandonar = false;
     private bool pulsadoReanudar = false;

    private void Start()
    {
        // Guardamos las posiciones originales
        _optionsOriginal = _OptionsMenuDisplay.transform.localPosition;
        _menuOriginal = _MenuPrincipalDisplay.transform.localPosition;
        _confirmationOriginal = _ConfirmationExit.transform.localPosition;

        // Posicion Inicial del Jugador 
        if (player != null)
        {
            posicionInicialJugador = player.transform.position;
            Debug.Log(posicionInicialJugador);
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Reanudar();
        }
    }

    public void Reanudar()
    {
        _menuUp = _menuOriginal + new Vector3(0, 566, 0);
        _optionUp = _optionsOriginal + new Vector3(0, 567, 0);

        if(!pulsadoReanudar){
            Cursor.lockState = CursorLockMode.None;     // Libera Cursor
            Cursor.visible = true;                      // Ver
            if (UnJugador)
            {
                Time.timeScale = 0f;
            }

            _mouseLookScript.enabled = false;          // Desactiva el script de movimiento del ratón

            pulsadoReanudar = true;

            _OptionPanel.SetActive(true);
            _MenuPrincipalDisplay.SetActive(true);
            _OptionsMenuDisplay.SetActive(true);

            _mover.Move(_MenuPrincipalDisplay, _menuOriginal, _menuUp);
            _mover.Move(_OptionsMenuDisplay, _optionsOriginal, _optionUp);
        }else{
            Cursor.lockState = CursorLockMode.Locked;     // Bloquea el cursor en el centro de la pantalla
            Cursor.visible = false;                      // Ocutar el cursor
            if (UnJugador)
            {
                Time.timeScale = 1f;
            }
            _mouseLookScript.enabled = true;           // Activa el script de movimiento del ratón

            pulsadoReanudar = false;
            _OptionPanel.SetActive(false);

            _mover.Move(_MenuPrincipalDisplay, _menuOriginal, _menuOriginal);
            _mover.Move(_OptionsMenuDisplay, _optionsOriginal, _optionsOriginal);

        }

        AudioManager.Instance.ReproducirInterfaz(_MetalSound);
        pulsadoOpciones = false;
        pulsadoAbandonar = false;
    }

    public void Reaparecer()
    {
        muertes++;

        player.GetComponent<PlayerHealth>().HealPlayer(100);
        player.GetComponent<PlayerHealth>().ArmorPlayer(120);
        player.GetComponent<InventoryScript>().ResetInventory();
        player.GetComponent<PlayerMove>().enabled = false;

        // Recargar munición de todas las armas automáticamente
        string[] weaponNames = {
            "Bfg", "Chainsaw", "EnergyGun", "Fist", "MachineGun", "Pistol2",
            "PlasmaGun", "ShotgunNormal", "SemiPistol"
        };

        foreach (string weaponName in weaponNames)
        {
            GameObject weaponGO = GameObject.Find($"Player/Canvas/HUD/{weaponName}");
            if (weaponGO != null)
            {
                Gun gun = weaponGO.GetComponent<Gun>();
                if (gun != null)
                {
                    gun.ammo = gun.maxAmmo;
                }
                else
                {
                    Debug.LogWarning($"{weaponName} no tiene un componente Gun.");
                }
            }
            else
            {
                Debug.LogWarning($"{weaponName} no encontrado en HUD.");
            }
        }

        
        if (UnJugador)
        {
            Time.timeScale = 1f;
        }
        Debug.Log("Reapareciendo");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _mouseLookScript.enabled = true;
        player.GetComponent<PlayerMove>().UpdateFistVisibility();
        if (player != null)
        {
            player.transform.position = posicionInicialJugador;
            Debug.Log(posicionInicialJugador);
        }

        if (deathPanelUI != null && deathPanelUI.activeSelf)
        {
            deathPanelUI.SetActive(false);
            AudioManager.Instance.ReproducirInterfaz(_Death);
        }
        else
        {
            _OptionPanel.SetActive(false);
        }

        
        _ContadorDeMuertes.text = muertes.ToString();
        pulsadoReanudar = false;
        StartCoroutine(ActivarScriptDespuesDeTiempo(1));
    }

    IEnumerator ActivarScriptDespuesDeTiempo(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        player.GetComponent<PlayerMove>().enabled = true;
    }


    public void Opciones()
    {
        Vector3 menuLeft = _menuUp + new Vector3(-80, 0, 0);
        Vector3 optionsRight = _optionUp + new Vector3(150, 0, 0);

        if (!pulsadoOpciones)
        {
            pulsadoOpciones = true;

            _mover.Move(_MenuPrincipalDisplay, _menuUp, menuLeft);
            _mover.Move(_OptionsMenuDisplay, _optionUp, optionsRight);
        }
        else
        {
            pulsadoOpciones = false;
            _mover.Move(_MenuPrincipalDisplay, menuLeft, _menuUp);
            _mover.Move(_OptionsMenuDisplay, optionsRight, _optionUp);
        }

        AudioManager.Instance.ReproducirInterfaz(_MetalSound);

    }

    public void Salir()
    {
        ConfirmationDown = _confirmationOriginal + new Vector3(0, -86, 0);

        if (!pulsadoAbandonar)
        {
            pulsadoAbandonar = true;
            _DescripcionText.gameObject.SetActive(true);

            _mover.Move(_ConfirmationExit, _confirmationOriginal, ConfirmationDown);
        }
        else
        {
            pulsadoAbandonar = false;
            _DescripcionText.gameObject.SetActive(false);

            _mover.Move(_ConfirmationExit, ConfirmationDown, _confirmationOriginal);
        }

        if (deathPanelUI != null && deathPanelUI.activeSelf)
        {
            AudioManager.Instance.ReproducirInterfaz(_Death);
        }
        else
        {
            AudioManager.Instance.ReproducirInterfaz(ClickDeBoton);
            AudioManager.Instance.ReproducirInterfaz(_MetalSound);
        }

        Debug.Log(pulsadoAbandonar ? "Menú de confirmación cerrado." : "Menú de confirmación abierto.");
    }

        

    public void SalirSI()
    {
        Debug.Log("Saliendo del juego...");
        AudioManager.Instance.ReproducirInterfaz(ClickDeBoton);
        SceneManager.LoadScene("MainMenu");

    }

    public void SalirNO()
    {
        pulsadoAbandonar = false;
        _mover.Move(_ConfirmationExit, ConfirmationDown, _confirmationOriginal);
        _DescripcionText.gameObject.SetActive(false);

        AudioManager.Instance.ReproducirInterfaz(ClickDeBoton);

        Debug.Log("Menú de confirmación cerrado.");
    }

    public void SonidoBotonInterfaz(AudioClip clip)
    {
        AudioManager.Instance.ReproducirInterfaz(clip);
    }
}
