using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    /// <summary>
    /// Reference to the Image component used for the fade effect.
    /// </summary>
    [SerializeField] private Image _fadeImage;

    /// <summary>
    /// Duration of the fade transition.
    /// </summary>
    [SerializeField] private float _fadeDuration = 1f;

    /// <summary>
    /// Initializes the fade transition by setting the fade image based on its current alpha value.
    /// </summary>
    void Start()
    {
        // If the _fadeImage is transparent, ensure it is fully transparent
        if (_fadeImage.color.a == 0)
        {
            _fadeImage.color = new Color(0, 0, 0, 0); // Transparent
        }
        else
        {
            _fadeImage.color = new Color(0, 0, 0, 1); // Black
        }
    }

    /// <summary>
    /// Starts the fade transition to the specified scene.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load after the fade transition.</param>
    public void FadeToScene(string sceneName)
    {
        if (_fadeImage.color.a == 0)
        {
            StartCoroutine(FadeOutToBlack(sceneName));
        }
        else
        {
            StartCoroutine(FadeBlackToWhite(sceneName));
        }
    }

    /// <summary>
    /// Coroutine that handles fading from transparent to black and then loading the target scene.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load after fade out.</param>
    private IEnumerator FadeOutToBlack(string sceneName)
    {
        float timeElapsed = 0f;

        // Fade to black
        while (timeElapsed < _fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(timeElapsed / _fadeDuration);
            _fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Load the new scene
        SceneManager.LoadScene(sceneName);

        // Optionally fade from black to transparent after loading
        StartCoroutine(FadeInFromBlack());
    }

    /// <summary>
    /// Coroutine that handles fading from black to transparent after loading a scene.
    /// </summary>
    private IEnumerator FadeInFromBlack()
    {
        float timeElapsed = 0f;

        while (timeElapsed < _fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (timeElapsed / _fadeDuration));
            _fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        _fadeImage.color = new Color(0, 0, 0, 0);
    }

    /// <summary>
    /// Coroutine that handles fading from black to white and then loading the target scene.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load after fade transition.</param>
    private IEnumerator FadeBlackToWhite(string sceneName)
    {
        float timeElapsed = 0f;

        // Start from black
        Color startColor = new Color(0, 0, 0, 1);
        Color endColor = new Color(1, 1, 1, 1);

        while (timeElapsed < _fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            _fadeImage.color = Color.Lerp(startColor, endColor, Mathf.Clamp01(timeElapsed / _fadeDuration));
            yield return null;
        }

        // Ensure it is completely white at the end
        _fadeImage.color = endColor;

        // Load the new scene
        SceneManager.LoadScene(sceneName);
    }
}
