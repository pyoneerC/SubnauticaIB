using UnityEngine;

/// <summary>
/// Manages the welder object in the game, allowing the player to pick it up and use it.
/// </summary>
public class Welder : MonoBehaviour
{
    /// <summary>
    /// The prefab of the welder that will be activated when picked up.
    /// </summary>
    [Tooltip("The prefab of the welder.")]
    public GameObject welderPrefab;

    /// <summary>
    /// The maximum distance the player can be from the welder to pick it up.
    /// </summary>
    [Tooltip("Maximum distance to pick up the welder.")]
    public float rayDistance = 3f;

    /// <summary>
    /// The camera that the player is using to view the scene.
    /// </summary>
    [Tooltip("The camera used for raycasting.")]
    public Camera playerCamera;

    /// <summary>
    /// Indicates whether the welder is currently in the player's hand.
    /// </summary>
    public bool WelderInHand { get; private set; }

    private void Start()
    {
        // Ensure the welder prefab is inactive at the start
        if (welderPrefab != null)
        {
            welderPrefab.SetActive(false);
        }
        else
        {
            Debug.LogError("Welder prefab not assigned!");
        }

        // Assign the main camera if not already set
        if (playerCamera != null) return;
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            Debug.LogError("No camera found in the scene.");
        }
    }

    private void Update()
    {
        // Check for the key press to pick up the welder
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickUpWelder();
        }
    }

    /// <summary>
    /// Tries to pick up the welder if it is within range.
    /// </summary>
    private void TryPickUpWelder()
    {
        // Cast a ray from the center of the screen
        var ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        // Check if the ray hits an object
        if (!Physics.Raycast(ray, out RaycastHit hit, rayDistance)) return;
        // Check if the hit object is tagged as "welder"
        if (!hit.collider.CompareTag("welder")) return;
        // Deactivate the welder in the world
        hit.collider.gameObject.SetActive(false);

        // If the welder is not already in hand, activate the prefab
        if (WelderInHand) return;
        welderPrefab.SetActive(true);
        WelderInHand = true;
    }
}
