using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Handles the description display for the selected menu button.
/// </summary>
public class ScreenMessageControl : MonoBehaviour
{
    [SerializeField] private GameObject _descriptionPanel;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _CommingSoonText; // Text to display when the button is not implemented
    [SerializeField] private float _blinkInterval = 0.5f; // Interval for blinking in seconds

    [SerializeField] private Image _Pantalla1;
    [SerializeField] private Image _Pantalla2;
    [SerializeField] private RawImage _Daños;

    private Color Original;

    private string _baseDescription = "";
    private Coroutine _blinkCoroutine;

    void Start()
    {
        Original = _Pantalla1.color;
        _CommingSoonText.gameObject.SetActive(false);
        _Daños.gameObject.SetActive(false);
    }


    /// <summary>
    /// Called when a new button is selected.
    /// </summary>
    /// <param name="button">The currently selected button.</param>
    public void OnButtonSelected(Button button)
    {

        string buttonName = button.name;

        switch (buttonName)
        {
            case "SinglePlayer":
                _baseDescription = "\n.- Embarcate en una mision solitaria.\n\n" +
                                   "Sobrevive hordas infernales y demuestra que eres el verdadero marine.\n\n.-";
                _CommingSoonText.gameObject.SetActive(false);
                _Pantalla1.color = Original;
                _Pantalla2.color = Original;
                _Daños.gameObject.SetActive(false);
                break;

            case "MultiPlayer":
                _baseDescription = "\n.- ¡Reune a tus amigos!\n\n" +
                                   "Masacra demonios o compite contra ellos en un sangriento festin multijugador.\n\n.-";
                
                _Daños.gameObject.SetActive(true);
                _CommingSoonText.gameObject.SetActive(true);
                Color tempColor = _Pantalla1.color;
                tempColor.a = 1;
                _Pantalla1.color = tempColor;
                _Pantalla2.color = tempColor;
                break;

            case "Options":
                _baseDescription = "\n.- Ajusta tu arsenal.\n\n" +
                                   "Configura los controles, el sonido y otros detalles para perfeccionar tu masacre.\n\n.-";
                _CommingSoonText.gameObject.SetActive(false);
                _Pantalla1.color = Original;
                _Pantalla2.color = Original;
                _Daños.gameObject.SetActive(false);
                break;

            case "Credits":
                _baseDescription = "\n.- Honra a los arquitectos del caos.\n\n" +
                                   "Descubre a las mentes retorcidas detras de este festin infernal.\n\n.-";
                _CommingSoonText.gameObject.SetActive(false);
                _Pantalla1.color = Original;
                _Pantalla2.color = Original;
                _Daños.gameObject.SetActive(false);

                break;

            case "Exit":
                _baseDescription = "\n.- ¿Vas a huir, marine?\n\n" +
                                   "Abandona la batalla... por ahora.\n\n.-";
                _CommingSoonText.gameObject.SetActive(false);
                _Pantalla1.color = Original;
                _Pantalla2.color = Original;
                _Daños.gameObject.SetActive(false);

                break;

            default:
                _baseDescription = "";
                _CommingSoonText.gameObject.SetActive(false);
                _Pantalla1.color = Original;
                _Pantalla2.color = Original;
                _Daños.gameObject.SetActive(false);

                break;
        }

        if (!string.IsNullOrEmpty(_baseDescription) && buttonName != "Multiplayer")
        {
            _descriptionPanel.SetActive(true);

            if (_blinkCoroutine != null)
                StopCoroutine(_blinkCoroutine);

            _blinkCoroutine = StartCoroutine(BlinkDots());
        }
        else
        {
            //_descriptionPanel.SetActive(false);

            if (_blinkCoroutine != null)
                StopCoroutine(_blinkCoroutine);

            _descriptionText.text = "";
        }
    }

    /// <summary>
    /// Coroutine to make the dots blink repeatedly.
    /// </summary>
    private IEnumerator BlinkDots()
    {
        while (true)
        {
            _descriptionText.text = _baseDescription + " .";
            yield return new WaitForSeconds(_blinkInterval);
            _descriptionText.text = _descriptionText.text + " .";
            yield return new WaitForSeconds(_blinkInterval);
            _descriptionText.text = _descriptionText.text + " .";
            yield return new WaitForSeconds(_blinkInterval);
            _descriptionText.text = _baseDescription;
            yield return new WaitForSeconds(_blinkInterval);
        }
    }
}