using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Controls the movement of the skull image and highlights the selected menu button.
/// Supports selection by keyboard arrows and mouse hover.
/// Displays the description of the selected option when the skull is over a button.
/// </summary>
public class MenuSelector : MonoBehaviour{
    [SerializeField] private RectTransform _skullImage;
    [SerializeField] private Button[] _buttons;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private ScreenMessageControl _descriptionHandler;
    [SerializeField] private AudioClip _bottonSound;

    private int _selectedIndex = 0;
    private bool _isMouseOverButton = false;

    /// <summary>
    /// Initializes the button events and sets the initial position of the skull.
    /// </summary>
    private void Start()
    {
        AssignButtonEvents();
        UpdateSkullPosition();
    }

    /// <summary>
    /// Handles keyboard input for moving the skull.
    /// </summary>
    private void Update()
    {
        if (_isMouseOverButton) return;  // Block keyboard movement if mouse is over a button

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _selectedIndex = (_selectedIndex - 1 + _buttons.Length) % _buttons.Length;
            UpdateSkullPosition();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _selectedIndex = (_selectedIndex + 1) % _buttons.Length;
            UpdateSkullPosition();
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            _buttons[_selectedIndex].onClick.Invoke();
            UpdateButtonVisuals();
        }
    }

    /// <summary>
    /// Updates the skull position and informs the description handler.
    /// </summary>
    private void UpdateSkullPosition()
    {
        // Sonido de botón
        AudioManager.Instance.ReproducirInterfaz(_bottonSound);
    
        RectTransform buttonTransform = _buttons[_selectedIndex].GetComponent<RectTransform>();
    
        // Obtener la esquina inferior izquierda (borde izquierdo del botón)
        Vector3[] buttonCorners = new Vector3[4];
        buttonTransform.GetWorldCorners(buttonCorners);
        Vector3 buttonLeftEdge = buttonCorners[0]; // esquina inferior izquierda del botón
    
        // Obtener el ancho del cráneo
        float skullWidth = _skullImage.rect.width * _skullImage.lossyScale.x; // tener en cuenta el escalado
    
        // Calcular la nueva posición: borde derecho de la calavera alineado al borde izquierdo del botón
        Vector3 newPosition = new Vector3(
            buttonLeftEdge.x - skullWidth + _offset.x,
            buttonTransform.position.y + _offset.y,
            buttonTransform.position.z
        );
    
        _skullImage.position = newPosition;
    
        // Notificar al manejador de descripciones
        if (_descriptionHandler != null)
        {
            _descriptionHandler.OnButtonSelected(_buttons[_selectedIndex]);
        }
    
        UpdateButtonVisuals();
    }

    /// <summary>
    /// Assigns mouse hover events to all buttons.
    /// </summary>
    private void AssignButtonEvents()
    {
        for (int i = 0; i < _buttons.Length; i++)
        {
            int index = i;
            EventTrigger trigger = _buttons[i].gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry enterEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            enterEntry.callback.AddListener((eventData) => OnButtonHover(index));
            trigger.triggers.Add(enterEntry);

            EventTrigger.Entry exitEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            exitEntry.callback.AddListener((eventData) => OnButtonExit());
            trigger.triggers.Add(exitEntry);
        }
    }

    /// <summary>
    /// Updates the button colors depending on whether they are selected or not.
    /// </summary>
    private void UpdateButtonVisuals()
    {
        for (int i = 0; i < this._buttons.Length; i++)
        {
            ColorBlock colors = this._buttons[i].colors;

            if (i == this._selectedIndex)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
                {
                    this._buttons[i].GetComponent<Image>().color = colors.selectedColor;
                }
                else
                {
                    this._buttons[i].GetComponent<Image>().color = colors.highlightedColor;
                }
            }
            else
            {
                this._buttons[i].GetComponent<Image>().color = colors.normalColor;
            }
        }
    }

    /// <summary>
    /// Called when the mouse hovers over a button.
    /// </summary>
    /// <param name="index">The index of the hovered button.</param>
    private void OnButtonHover(int index)
    {
        _isMouseOverButton = true;
        _selectedIndex = index;
        UpdateSkullPosition();
    }

    /// <summary>
    /// Called when the mouse exits a button.
    /// </summary>
    private void OnButtonExit()
    {
        _isMouseOverButton = false;
    }
}