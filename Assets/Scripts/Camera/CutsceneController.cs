using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// Manages the cutscene sequence by controlling the cutscene camera's movement
/// along a spline for a specified duration, after which it switches back to the
/// player camera.
/// </summary>
public class CutsceneController : MonoBehaviour
{
    private Camera playerCamera; // Regular gameplay camera
    public Transform splineParent; // The parent GameObject that controls the spline path
    public float speed = 1.0f; // Speed at which the cutscene camera follows the spline

    private Camera cutsceneCamera; // Camera for the cutscene
    private SplineContainer splineContainer; // Reference to the spline
    private float duration = 8.0f; // Duration of the cutscene
    private float elapsedTime = 0f; // Time elapsed since the start of the cutscene

    private void Start()
    {
        playerCamera = Camera.main;
        cutsceneCamera = new GameObject("Cutscene Camera").AddComponent<Camera>();
        cutsceneCamera.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);

        splineContainer = splineParent.GetComponent<SplineContainer>();
        if (splineContainer != null) return;
        Debug.LogError("No SplineContainer found on the spline parent.");
        return;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime < duration)
        {
            // Normalize the elapsed time to a 0-1 range
            float normalizedTime = elapsedTime / duration;

            // Get normalized progress along the spline (from 0 to 1)
            float splineProgress = normalizedTime;

            // Move the camera along the spline based on progress
            Vector3 targetPosition = splineContainer.EvaluatePosition(splineProgress);
            Vector3 targetDirection = splineContainer.EvaluateTangent(splineProgress);

            // Set camera position and rotation along the spline
            cutsceneCamera.transform.position = targetPosition;
            cutsceneCamera.transform.rotation = Quaternion.LookRotation(targetDirection);
        }
        else
        {
            // After cutscene duration, switch back to the player camera
            playerCamera.gameObject.SetActive(true);
            Destroy(cutsceneCamera.gameObject);
            enabled = false;
        }
    }
}
