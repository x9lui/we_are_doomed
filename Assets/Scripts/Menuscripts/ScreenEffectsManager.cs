using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Manages visual effects on multiple UI screens to simulate an old CRT monitor.
/// </summary>
public class ScreenEffectManager : MonoBehaviour
{
    [System.Serializable]
    public class TextPanelPair
    {
        public RectTransform _screenPanel;
        public TextMeshProUGUI _screenText;
    }

    [SerializeField]
    private List<TextPanelPair> _screenPairs;

    [SerializeField]
    private float _shakeMagnitude = 2f;

    [SerializeField]
    private float _shakeSpeed = 5f;

    [SerializeField]
    private float _flickerIntensity = 0.1f;

    [SerializeField]
    private float _flickerSpeed = 5f;

    private List<Vector3> _initialPositions = new List<Vector3>();
    private List<Color> _initialColors = new List<Color>();
    private Vector3 _maxShakeOffset;

    private void Start()
    {
        if (_screenPairs != null && _screenPairs.Count > 0)
        {
            foreach (var pair in _screenPairs)
            {
                if (pair._screenPanel != null)
                {
                    _initialPositions.Add(pair._screenPanel.localPosition);
                }

                if (pair._screenText != null)
                {
                    _initialColors.Add(pair._screenText.color);
                }
            }

            _maxShakeOffset = new Vector3(_shakeMagnitude, _shakeMagnitude, 0f);
        }
    }

    private void Update()
    {
        this.ApplyScreenEffects();
    }

    /// <summary>
    /// Applies shaking and flickering effects to simulate a CRT screen.
    /// </summary>
    private void ApplyScreenEffects()
    {
        for (int i = 0; i < _screenPairs.Count; i++)
        {
            if (_screenPairs[i]._screenPanel != null)
            {
                ApplyShake(i);
            }
            if (_screenPairs[i]._screenText != null)
            {
                ApplyFlicker(i);
            }
        }
    }

    /// <summary>
    /// Applies a small shake to the screen panel while keeping it within the allowed bounds.
    /// </summary>
    private void ApplyShake(int index)
    {
        Vector3 randomOffset = new Vector3(
            Mathf.PerlinNoise(Time.time * _shakeSpeed, 0f) - 0.5f,
            Mathf.PerlinNoise(0f, Time.time * _shakeSpeed) - 0.5f,
            0f
        ) * _shakeMagnitude;

        Vector3 newPosition = _initialPositions[index] + randomOffset;

        // Restringir el desplazamiento a los límites establecidos
        newPosition.x = Mathf.Clamp(newPosition.x, _initialPositions[index].x - _maxShakeOffset.x, _initialPositions[index].x + _maxShakeOffset.x);
        newPosition.y = Mathf.Clamp(newPosition.y, _initialPositions[index].y - _maxShakeOffset.y, _initialPositions[index].y + _maxShakeOffset.y);

        _screenPairs[index]._screenPanel.localPosition = newPosition;
    }

    /// <summary>
    /// Applies a flicker effect to the screen text.
    /// </summary>
    private void ApplyFlicker(int index)
    {
        float flicker = Mathf.PerlinNoise(Time.time * _flickerSpeed, 0f) * _flickerIntensity;
        _screenPairs[index]._screenText.color = new Color(
            _initialColors[index].r,
            _initialColors[index].g,
            _initialColors[index].b,
            1f - flicker
        );
    }
}
