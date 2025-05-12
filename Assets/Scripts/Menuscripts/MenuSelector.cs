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
        Vector3 newPosition = buttonTransform.position + new Vector3(_offset.x, _offset.y, 0f);

        if (_selectedIndex == 3 || _selectedIndex == 4)
        {
            newPosition.x += 90f;
        }

        _skullImage.position = newPosition;

        // Notify the description handler
        if (_descriptionHandler != null)
        {
            _descriptionHandler.OnButtonSelected(_buttons[_selectedIndex]);
        }

        this.UpdateButtonVisuals();

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