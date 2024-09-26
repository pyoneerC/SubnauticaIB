using UnityEngine;

/// <summary>
/// Manages the cutscene sequence by controlling the cutscene camera's movement
/// along a spline for a specified duration, after which it switches back to the
/// player camera.
/// </summary>
public class CutsceneController : MonoBehaviour
{
    /// <summary>
    /// The camera used for the cutscene.
    /// </summary>
    public Camera cutsceneCamera;

    /// <summary>
    /// The camera used for regular gameplay.
    /// </summary>
    public Camera playerCamera;

    /// <summary>
    /// The parent GameObject that controls the spline path for the cutscene.
    /// </summary>
    public Transform splineParent;

    /// <summary>
    /// The speed at which the cutscene camera follows the spline.
    /// </summary>
    public float speed = 1.0f;

    /// <summary>
    /// Duration of the cutscene in seconds.
    /// </summary>
    private float duration = 6.0f;

    /// <summary>
    /// Time elapsed since the start of the cutscene.
    /// </summary>
    private float elapsedTime = 0f;

    /// <summary>
    /// Starting position of the cutscene camera.
    /// </summary>
    private Vector3 startPosition;

    /// <summary>
    /// Target position for the cutscene camera to move towards.
    /// </summary>
    private Vector3 targetPosition;

    /// <summary>
    /// Initializes the cutscene by setting the appropriate cameras and calculating
    /// the target position along the spline.
    /// </summary>
    void Start()
    {
        // Disable the player camera and enable the cutscene camera to start the cutscene
        playerCamera.gameObject.SetActive(false);
        cutsceneCamera.gameObject.SetActive(true);

        // Store the initial position of the cutscene camera
        startPosition = cutsceneCamera.transform.position;

        // Set the target position to the position of the spline parent
        targetPosition = splineParent.position; // Modify this to follow a specific point on your spline if needed
    }

    /// <summary>
    /// Updates the cutscene camera's position over time, and switches back to the
    /// player camera once the cutscene duration is reached.
    /// </summary>
    void Update()
    {
        // Accumulate the time elapsed since the start of the cutscene
        elapsedTime += Time.deltaTime;

        // Move the cutscene camera along the spline until the duration is reached
        if (elapsedTime < duration)
        {
            // Calculate the interpolation factor (t) based on elapsed time
            float t = elapsedTime / duration;

            // Move the camera towards the target position using linear interpolation
            cutsceneCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
        }
        else
        {
            // Once the cutscene duration has elapsed, destroy the cutscene camera
            // and re-enable the player camera
            Destroy(cutsceneCamera.gameObject);
            playerCamera.gameObject.SetActive(true);

            // Disable this script to prevent further updates
            enabled = false;
        }
    }
}
