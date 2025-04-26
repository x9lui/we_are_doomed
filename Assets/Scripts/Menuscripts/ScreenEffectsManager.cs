using UnityEngine;
using TMPro;

/// <summary>
/// Manages visual effects on a UI screen to simulate an old CRT monitor.
/// </summary>
public class ScreenEffectManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform _screenPanel;

    [SerializeField]
    private TextMeshProUGUI _screenText;

    [SerializeField]
    private float _shakeMagnitude = 2f;

    [SerializeField]
    private float _shakeSpeed = 5f;

    [SerializeField]
    private float _flickerIntensity = 0.1f;

    [SerializeField]
    private float _flickerSpeed = 5f;

    private Vector3 _initialPosition;
    private Color _initialColor;

    private void Start()
    {
        if (this._screenPanel != null)
        {
            this._initialPosition = this._screenPanel.localPosition;
        }

        if (this._screenText != null)
        {
            this._initialColor = this._screenText.color;
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
        this.ApplyShake();
        this.ApplyFlicker();
    }

    /// <summary>
    /// Applies a small shake to the screen panel.
    /// </summary>
    private void ApplyShake()
    {
        if (this._screenPanel == null)
        {
            return;
        }

        Vector3 randomOffset = new Vector3(
            Mathf.PerlinNoise(Time.time * this._shakeSpeed, 0f) - 0.5f,
            Mathf.PerlinNoise(0f, Time.time * this._shakeSpeed) - 0.5f,
            0f
        ) * this._shakeMagnitude;

        this._screenPanel.localPosition = this._initialPosition + randomOffset;
    }

    /// <summary>
    /// Applies a flicker effect to the screen text.
    /// </summary>
    private void ApplyFlicker()
    {
        if (this._screenText == null)
        {
            return;
        }

        float flicker = Mathf.PerlinNoise(Time.time * this._flickerSpeed, 0f) * this._flickerIntensity;
        this._screenText.color = new Color(
            this._initialColor.r,
            this._initialColor.g,
            this._initialColor.b,
            1f - flicker
        );
    }
}

