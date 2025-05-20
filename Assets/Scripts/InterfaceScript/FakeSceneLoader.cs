using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class FakeSceneLoader : MonoBehaviour
{
    [Header("Duración total de la carga (segundos)")]
    [SerializeField] private float _loadingDuration = 120f;

    [Header("Barra de carga (solo visual)")]
    [SerializeField] private Slider _loadingBar;
    [SerializeField] private GameObject _loadingScreen;

    [SerializeField] private GameObject _SceneLoader;

    [Header("Imagen principal que se va a cambiar")]
    [SerializeField] private Image _displayImage;

    [Header("Sprites a alternar")]
    [SerializeField] private Sprite[] _loadingImages;

    [Header("Overlay para fundido a negro")]
    [SerializeField] private Image _fadeOverlay;

    [Header("Duración entre imágenes (segundos)")]
    [SerializeField] private float _timeBetweenImages = 5f;

    [Header("Duración del fundido (segundos)")]
    [SerializeField] private float _fadeDuration = 1f;

    public event Action OnLoadingComplete;


    private int _currentImageIndex = 0;

    public void CargarPantalla(float tiempo)
    {
        _SceneLoader.SetActive(true);
        _loadingDuration = tiempo;
        // Asegurar que el Slider no sea interactivo
        if (_loadingBar != null)
        {
            _loadingBar.interactable = false;
            _loadingBar.value = 0f;
            _loadingBar.gameObject.SetActive(true);
        }

        if (_displayImage != null) _displayImage.gameObject.SetActive(true);

        if (_fadeOverlay != null)
        {
            _fadeOverlay.gameObject.SetActive(true);
            _fadeOverlay.color = new Color(0, 0, 0, 0);
        }

        if (_loadingImages.Length > 0 && _displayImage != null)
            _displayImage.sprite = _loadingImages[0];

        // Lanzar ambas coroutines
        StartCoroutine(FakeLoadCoroutine());
        StartCoroutine(ImageCycleCoroutine());
    }

    private IEnumerator FakeLoadCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < _loadingDuration)
        {
            elapsed += Time.deltaTime;

            if (_loadingBar != null)
                _loadingBar.value = Mathf.Clamp01(elapsed / _loadingDuration);

            yield return null;
        }

        // Asegurar que la barra se llena al final
        if (_loadingBar != null)
            _loadingBar.value = 1f;

        Debug.Log("Carga falsa completada.");

        // Desactivar elementos
        _SceneLoader.SetActive(false);

        OnLoadingComplete?.Invoke();
    }

    private IEnumerator ImageCycleCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_timeBetweenImages);

            yield return StartCoroutine(FadeToBlack());

            _currentImageIndex = (_currentImageIndex + 1) % _loadingImages.Length;

            if (_displayImage != null && _loadingImages.Length > 0)
                _displayImage.sprite = _loadingImages[_currentImageIndex];

            yield return StartCoroutine(FadeFromBlack());
        }
    }

    private IEnumerator FadeToBlack()
    {
        float elapsed = 0f;
        Color color = _fadeOverlay.color;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / _fadeDuration);
            _fadeOverlay.color = color;
            yield return null;
        }
        _fadeOverlay.color = new Color(0, 0, 0, 1);
    }

    private IEnumerator FadeFromBlack()
    {
        float elapsed = 0f;
        Color color = _fadeOverlay.color;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = 1f - Mathf.Clamp01(elapsed / _fadeDuration);
            _fadeOverlay.color = color;
            yield return null;
        }
        _fadeOverlay.color = new Color(0, 0, 0, 0);
    }
}
