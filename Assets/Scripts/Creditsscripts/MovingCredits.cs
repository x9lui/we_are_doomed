using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Moves the GameObject upwards continuously.
/// After 2 minutes, a click or the Enter key will load the "MainMenu" scene.
/// </summary>
public class MoveUpwards : MonoBehaviour
{
    /// <summary>
    /// Speed at which the GameObject moves upwards.
    /// </summary>
    public float _speed = 2.0f;

    /// <summary>
    /// Timer to track elapsed time.
    /// </summary>
    private float _timer = 0.0f;

    /// <summary>
    /// Time in seconds before automatically returning to the MainMenu.
    /// </summary>
    public float timeToReturn = 120f;

    /// <summary>
    /// Called once per frame. Handles movement and input checking.
    /// </summary>
    void Update()
    {
        // Move the GameObject upwards
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        // Update the _timer
        _timer += Time.deltaTime;

        // If 2 minutes have passed, load the MainMenu scene
        if (_timer >= timeToReturn)
        {
            LoadMainMenu();
        }

        // If the Enter key or mouse click is pressed, load the MainMenu scene immediately
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            LoadMainMenu();
        }
    }

    /// <summary>
    /// Loads the "MainMenu" scene.
    /// </summary>
    void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
