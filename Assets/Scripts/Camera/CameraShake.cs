using System.Collections;
using UnityEngine;

/// <summary>
/// Handles camera shake effects to simulate explosions or impacts.
/// The script shakes the camera for a specified duration after a delay
/// and plays an explosion sound effect.
/// </summary>
public class CameraShake : MonoBehaviour
{
    /// <summary>
    /// The main camera to apply the shake effect to.
    /// </summary>
    public Camera mainCamera;

    /// <summary>
    /// The audio source used to play sound effects.
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// The audio clip to play during the shake effect.
    /// </summary>
    public AudioClip explosionSound;

    /// <summary>
    /// Cached transform of the main camera for optimization.
    /// </summary>
    private Transform _cameraTransform;

    /// <summary>
    /// Original position of the camera to reset after shaking.
    /// </summary>
    private Vector3 _originalPosition;

    /// <summary>
    /// Initializes the camera shake effect by setting up the camera transform
    /// and original position. Starts the shake with a delay.
    /// </summary>
    private void Awake()
    {
        // Ensure the main camera is assigned
        if (mainCamera != null)
        {
            _cameraTransform = mainCamera.transform; // Cache the camera's transform
            _originalPosition = _cameraTransform.localPosition; // Store the original position
            StartDelayedShake(11f, 300f, 1f); // Start the shake effect with parameters
        }
        else
        {
            Debug.LogError("Main Camera is not assigned."); // Log an error if camera is not set
        }
    }

    /// <summary>
    /// Starts the shake effect after a specified delay.
    /// </summary>
    /// <param name="delay">Time in seconds to wait before starting the shake.</param>
    /// <param name="duration">Total duration of the shake effect in seconds.</param>
    /// <param name="magnitude">The intensity of the shake effect.</param>
    private void StartDelayedShake(float delay, float duration, float magnitude)
    {
        StartCoroutine(DelayedShakeCoroutine(delay, duration, magnitude)); // Start the coroutine for the shake effect
    }

    /// <summary>
    /// Coroutine to handle the shaking of the camera over time.
    /// </summary>
    /// <param name="delay">Time in seconds to wait before starting the shake.</param>
    /// <param name="duration">Total duration of the shake effect in seconds.</param>
    /// <param name="magnitude">The initial intensity of the shake effect.</param>
    /// <returns>Yield instruction to control the coroutine's timing.</returns>
    private IEnumerator DelayedShakeCoroutine(float delay, float duration, float magnitude)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay

        float elapsed = 0.0f; // Track elapsed time

        audioSource.PlayOneShot(explosionSound); // Play the explosion sound

        // Perform the shake effect over the specified duration
        while (elapsed < duration)
        {
            // Calculate random offsets for shaking
            var x = Random.Range(-1f, 1f) * magnitude;
            var y = Random.Range(-1f, 1f) * magnitude;

            // Update the camera's local position to create the shake effect
            _cameraTransform.localPosition = new Vector3(x, y, _originalPosition.z);

            elapsed += Time.deltaTime; // Increment elapsed time

            // Gradually decrease the magnitude of shaking
            magnitude = Mathf.Lerp(magnitude, 0, elapsed / duration);
            yield return null; // Wait for the next frame
        }

        // Reset the camera's position to its original state
        _cameraTransform.localPosition = _originalPosition;
    }
}
