using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the fade-out of a black panel at the start of the scene
/// and then smoothly fades in a group of UI elements.
/// </summary>
public class SceneTransition : MonoBehaviour
{
    /// <summary>
    /// The black panel that will fade out.
    /// </summary>
    [Header("Fade Panel Settings")]
    public Image fadePanel;

    /// <summary>
    /// Duration of the fade-out effect in seconds.
    /// </summary>
    public float fadeDuration = 2f;

    /// <summary>
    /// The group of UI elements to appear after the fade.
    /// </summary>
    [Header("UI Content Settings")]
    public GameObject contentGroup;
    public MenuSelector _MenuSelector;

    /// <summary>
    /// Delay before starting the UI content fade-in after panel fade-out.
    /// </summary>
    public float appearDelay = 0.5f;

    /// <summary>
    /// Duration of the UI content fade-in effect in seconds.
    /// </summary>
    public float contentFadeDuration = 1.5f;

    private CanvasGroup contentCanvasGroup;

    /// <summary>
    /// Called on the frame when the script is enabled, before any Update methods.
    /// </summary>
    private void Start()
    {
        // Ensure contentGroup has a CanvasGroup
        contentCanvasGroup = contentGroup.GetComponent<CanvasGroup>();
        if (contentCanvasGroup == null)
        {
            contentCanvasGroup = contentGroup.AddComponent<CanvasGroup>();
        }

        // Initially hide the UI content
        contentGroup.SetActive(false);
        _MenuSelector.enabled = false;
        contentCanvasGroup.alpha = 0f;

        // Start the fade-out sequence
        StartCoroutine(FadeOutPanel());
    }

    /// <summary>
    /// Coroutine that handles fading out the panel and then fading in the content group.
    /// </summary>
    private IEnumerator FadeOutPanel()
    {
        float elapsed = 0f;
        Color panelColor = fadePanel.color;

        // Gradually reduce panel alpha from 1 to 0
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            panelColor.a = alpha;
            fadePanel.color = panelColor;
            yield return null;
        }

        // Ensure the panel is fully transparent
        panelColor.a = 0f;
        fadePanel.color = panelColor;

        // Deactivate the panel
        fadePanel.gameObject.SetActive(false);

        // Wait before showing the UI content
        yield return new WaitForSeconds(appearDelay);

        // Activate the UI content
        contentGroup.SetActive(true);
        _MenuSelector.enabled = true;

        // Start fading in the content
        StartCoroutine(FadeInContent());
    }

    /// <summary>
    /// Coroutine that handles the fade-in of the content group.
    /// </summary>
    private IEnumerator FadeInContent()
    {
        float elapsed = 0f;

        // Gradually increase content alpha from 0 to 1
        while (elapsed < contentFadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / contentFadeDuration);
            contentCanvasGroup.alpha = alpha;
            yield return null;
        }

        // Ensure content is fully opaque
        contentCanvasGroup.alpha = 1f;
    }
}
