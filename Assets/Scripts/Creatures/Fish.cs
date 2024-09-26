using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// Controls the movement of a fish along a specified spline path.
/// </summary>
public class Fish : MonoBehaviour
{
    /// <summary>
    /// Reference to the GameObject that holds the SplineContainer.
    /// </summary>
    [Tooltip("Reference to the GameObject that holds the SplineContainer.")]
    public GameObject splineObject;

    /// <summary>
    /// Speed of the fish along the spline.
    /// </summary>
    [Tooltip("Speed along the spline.")]
    public float swimSpeed = 1f;

    /// <summary>
    /// Speed of rotation to face the spline direction.
    /// </summary>
    [Tooltip("Speed of rotation to face the spline direction.")]
    public float rotationSpeed = 2f;

    /// <summary>
    /// Should the fish loop back to the start of the spline?
    /// </summary>
    [Tooltip("Should the fish loop back to the start of the spline?")]
    public bool loop = true;

    /// <summary>
    /// Offset to correct fish orientation.
    /// </summary>
    [Tooltip("Offset to correct fish orientation.")]
    public Vector3 rotationOffset = new Vector3(0f, -90f, 0f);

    private float _progress; // Progress along the spline (0 to 1)
    private SplineContainer _splineContainer;
    private Spline _spline;

    private void Start()
    {
        // Validate splineObject and retrieve SplineContainer component
        if (splineObject == null)
        {
            Debug.LogError("No GameObject with a SplineContainer assigned to Fish.");
            return;
        }

        _splineContainer = splineObject.GetComponent<SplineContainer>();
        if (_splineContainer == null)
        {
            Debug.LogError("No SplineContainer found on the referenced GameObject.");
            return;
        }

        // Get the spline from the container
        _spline = _splineContainer.Spline;
    }

    private void Update()
    {
        if (_spline == null) return;

        // Move fish along the spline
        _progress += (swimSpeed * Time.deltaTime) / _spline.GetLength(); // Normalize speed across spline length

        // Check if we need to loop or clamp progress
        if (_progress > 1f)
        {
            _progress = loop ? 0f : 1f;
        }

        // Get the global position and tangent from the spline
        Vector3 currentPosition = _spline.EvaluatePosition(_progress);
        Vector3 tangent = _spline.EvaluateTangent(_progress);

        // Move the fish to the global spline position
        transform.position = currentPosition;

        // Rotate the fish to face the direction of movement (using tangent)
        Quaternion targetRotation = Quaternion.LookRotation(tangent);

        // Apply additional rotation offset to adjust the fish's orientation
        targetRotation *= Quaternion.Euler(rotationOffset);

        // Smoothly rotate the fish to the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        // Draw the spline path in the editor
        if (_splineContainer == null || _splineContainer.Spline == null) return;

        Gizmos.color = Color.cyan;
        Spline spline = _splineContainer.Spline;

        // Draw spheres along the spline for visualization
        for (float t = 0; t <= 1; t += 0.01f)
        {
            Vector3 point = spline.EvaluatePosition(t);
            Gizmos.DrawSphere(point, 0.05f);
        }
    }
}
