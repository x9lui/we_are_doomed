using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class LoaderScene : MonoBehaviour
{
    [Header("Duración entre imágenes (segundos)")]
    [SerializeField] private float _timeBetweenImages = 5f;

    [Header("Duración del fundido (segundos)")]
    [SerializeField] private float _fadeDuration = 1f;

    [Header("Componentes UI")]
    [SerializeField] private Slider _loadingBar;
    [SerializeField] private GameObject _SceneLoader;
    [SerializeField] private Image _displayImage;
    [SerializeField] private Sprite[] _loadingImages;
    [SerializeField] private Image _fadeOverlay;

    public event Action OnLoadingComplete;

    private int _currentImageIndex = 0;
    private Coroutine _imageCycleCoroutine;

    private void Start()
    {
        if (_fadeOverlay != null)
        {
            _fadeOverlay.color = new Color(0, 0, 0, 0);
            _fadeOverlay.gameObject.SetActive(true);
        }

        if (_loadingBar != null)
        {
            _loadingBar.interactable = false;
            _loadingBar.value = 0f;
        }

        if (_displayImage != null && _loadingImages.Length > 0)
            _displayImage.sprite = _loadingImages[0];
    }

    public void LoaderSceneOn()
    {
        _SceneLoader.SetActive(true);

        if (_loadingBar != null)
            _loadingBar.value = 0f;

        if (_displayImage != null && _loadingImages.Length > 0)
            _displayImage.sprite = _loadingImages[0];

        if (_imageCycleCoroutine == null)
            _imageCycleCoroutine = StartCoroutine(ImageCycleCoroutine());
    }

    public void LoaderSceneOff()
    {
        _SceneLoader.SetActive(false);

        if (_imageCycleCoroutine != null)
        {
            StopCoroutine(_imageCycleCoroutine);
            _imageCycleCoroutine = null;
        }

        OnLoadingComplete?.Invoke();
    }

    public void LoaderSceneProgress(float value)
    {
        if (_loadingBar != null)
            _loadingBar.value = Mathf.Clamp01(value);
    }

    private IEnumerator ImageCycleCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_timeBetweenImages);

            if (!_SceneLoader.activeSelf) continue;

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
