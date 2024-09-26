using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// Manages the cutscene sequence by controlling the cutscene camera's movement
/// along a spline path for a specified duration, after which it switches back
/// to the player camera and re-enables player input.
/// </summary>
public class CutsceneController : MonoBehaviour
{
    /// <summary>
    /// The main player camera used during regular gameplay.
    /// </summary>
    private Camera _playerCamera;

    /// <summary>
    /// The parent GameObject containing the spline that defines the cutscene camera's path.
    /// </summary>
    public Transform splineParent;

    /// <summary>
    /// The speed at which the cutscene camera moves along the spline.
    /// </summary>
    public float speed = 1.0f;

    /// <summary>
    /// The dedicated camera used during the cutscene sequence.
    /// </summary>
    private Camera _cutsceneCamera;

    /// <summary>
    /// The SplineContainer component that holds the spline data for the cutscene path.
    /// </summary>
    private SplineContainer _splineContainer;

    /// <summary>
    /// The total duration of the cutscene in seconds.
    /// </summary>
    private const float Duration = 8.0f;

    /// <summary>
    /// The time elapsed since the cutscene started.
    /// </summary>
    private float _elapsedTime;

    /// <summary>
    /// Initializes the cutscene by disabling the player camera, enabling the cutscene camera,
    /// and setting up the spline container. Also validates the existence of the spline.
    /// </summary>
    private void Start()
    {
        // Assign the main player camera (assumes it's tagged as MainCamera)
        _playerCamera = Camera.main;

        // Create a new cutscene camera dynamically and activate it
        _cutsceneCamera = new GameObject("Cutscene Camera").AddComponent<Camera>();
        _cutsceneCamera.gameObject.SetActive(true);

        // Deactivate the player camera to focus on the cutscene
        if (_playerCamera != null)
            _playerCamera.gameObject.SetActive(false);

        // Attempt to retrieve the SplineContainer from the spline parent
        _splineContainer = splineParent.GetComponent<SplineContainer>();

        // If no SplineContainer is found, log an error
        if (_splineContainer == null)
        {
            Debug.LogError("No SplineContainer found on the spline parent.");
        }
    }

    /// <summary>
    /// Enables or disables the player's input by toggling the player's movement controller.
    /// </summary>
    /// <param name="disable">If true, player input is disabled; if false, player input is enabled.</param>
    private static void DisablePlayerInput(bool disable)
    {
        // Locate the player by tag
        var player = GameObject.FindWithTag("Player");

        // If no player is found, exit the method
        if (player == null) return;

        // Access the player's input or movement controller (assumed to be FirstPersonController)
        var playerController = player.GetComponent<FirstPersonController>();

        // If a controller is found, enable/disable it based on the provided flag
        if (playerController != null)
        {
            playerController.enabled = !disable;
        }
    }

    /// <summary>
    /// Updates the cutscene camera's position and rotation along the spline path over time,
    /// and switches back to the player camera once the cutscene duration has elapsed.
    /// </summary>
    private void Update()
    {
        // Increment the elapsed time by the time passed since the last frame
        _elapsedTime += Time.deltaTime;

        // If the cutscene is still in progress (i.e., within its duration)
        if (_elapsedTime < Duration)
        {
            // Normalize the elapsed time into a value between 0 and 1 for progress along the spline
            float normalizedTime = _elapsedTime / Duration;

            // Calculate the progress along the spline based on the normalized time
            float splineProgress = normalizedTime;

            // Get the camera's target position and direction (tangent) on the spline
            Vector3 targetPosition = _splineContainer.EvaluatePosition(splineProgress);
            Vector3 targetDirection = _splineContainer.EvaluateTangent(splineProgress);

            // Update the cutscene camera's position and rotation to follow the spline path
            _cutsceneCamera.transform.position = targetPosition;
            _cutsceneCamera.transform.rotation = Quaternion.LookRotation(targetDirection);

            // Disable player input for the duration of the cutscene
            DisablePlayerInput(true);
        }
        else
        {
            // When the cutscene finishes, reactivate the player camera
            if (_playerCamera != null)
                _playerCamera.gameObject.SetActive(true);

            // Destroy the cutscene camera to clean up resources
            Destroy(_cutsceneCamera.gameObject);

            // Re-enable player input after the cutscene ends
            DisablePlayerInput(false);

            // Disable this script, as the cutscene has ended
            enabled = false;
        }
    }

    public void PauseUnpause()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }
}
