using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages item pickup and UI updates in the game using raycasting.
/// </summary>
public class Raycast : MonoBehaviour
{
    /// <summary>
    /// The main camera used for raycasting.
    /// </summary>
    public Camera mainCamera;

    /// <summary>
    /// The maximum distance for item pickup raycasting.
    /// </summary>
    public float rayDistance = 3f;

    /// <summary>
    /// The current count of items collected.
    /// </summary>
    public int itemCount;

    /// <summary>
    /// The goal count of items to collect.
    /// </summary>
    public int goalCount = 8;

    /// <summary>
    /// Audio source for playing sound effects.
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// Sound effect played when an item is picked up.
    /// </summary>
    public AudioClip itemPickupSound;

    /// <summary>
    /// Sound effect played when the suit is equipped.
    /// </summary>
    public AudioClip suitUpSound;

    /// <summary>
    /// UI elements representing equipment.
    /// </summary>
    public Image head;
    public Image body;
    public Image oxygenTank;
    public Image oxygenTankCharge;
    public Image legL;
    public Image legR;
    public Image feetR;
    public Image feetL;
    public Image breather;
    public Image knifeUp;
    public Image weightL;
    public Image weightR;
    public Image gogglesRight;
    public Image gogglesLeft;

    /// <summary>
    /// Text element showing the current count of collected items.
    /// </summary>
    public TextMeshProUGUI equipmentCount;

    private bool _isCountingDown;

    private void Update()
    {
        // Check for interaction input to pick up items
        if (Input.GetKeyDown(KeyCode.E) && !_isCountingDown)
        {
            StartCoroutine(TryPickUpItem());
        }
    }

    /// <summary>
    /// Attempts to pick up an item if it is in range.
    /// </summary>
    private IEnumerator TryPickUpItem()
    {
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        // Check if the ray hits an object
        if (!Physics.Raycast(ray, out var hit, rayDistance) || !hit.collider.CompareTag("elementos"))
            yield break;

        // Optional halo check can be implemented as needed
        var halo = hit.collider.GetComponent("Halo");
        if (halo == null)
        {
            _isCountingDown = true;
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green, 4f);
            Debug.Log("Countdown started! Picking up item in 4 seconds...");

            yield return new WaitForSeconds(4f);
        }

        PickUpItem(hit.collider);
        _isCountingDown = false;
    }

    /// <summary>
    /// Handles the logic for picking up an item and updating the equipment UI.
    /// </summary>
    /// <param name="itemCollider">The collider of the item to pick up.</param>
    private void PickUpItem(Collider itemCollider)
    {
        itemCount++;
        equipmentCount.text = itemCount.ToString();
        UpdateEquipmentCountColor();  // Update color based on item count

        Destroy(itemCollider.gameObject);

        // Update the equipment UI based on the item picked up
        foreach (Transform child in itemCollider.transform)
        {
            switch (child.tag)
            {
                case "oxygenTank":
                    oxygenTank.color = Color.green;
                    PlaySound(itemPickupSound);
                    break;
                case "body":
                    body.color = Color.green;
                    legL.color = Color.green;
                    legR.color = Color.green;
                    PlaySound(suitUpSound);
                    break;
                case "goggles":
                    gogglesLeft.color = Color.green;
                    gogglesRight.color = Color.green;
                    PlaySound(itemPickupSound);
                    break;
                case "feet":
                    feetL.color = Color.green;
                    feetR.color = Color.green;
                    PlaySound(itemPickupSound);
                    break;
                case "weight":
                    weightL.color = Color.green;
                    weightR.color = Color.green;
                    PlaySound(itemPickupSound);
                    break;
                case "oxygenTankCharge":
                    oxygenTankCharge.color = Color.green;
                    PlaySound(itemPickupSound);
                    break;
                case "breather":
                    breather.color = Color.green;
                    PlaySound(itemPickupSound);
                    break;
                case "knife":
                    knifeUp.color = Color.green;
                    PlaySound(itemPickupSound);
                    break;
                default:
                    Debug.Log("No matching tag found.");
                    break;
            }
        }

        // Check if the goal has been reached to load the next scene
        if (itemCount >= goalCount)
        {
            SceneManager.LoadScene("Reparation");
        }
    }

    /// <summary>
    /// Updates the color of the equipment count text based on the current item count.
    /// </summary>
    private void UpdateEquipmentCountColor()
    {
        float progress = (float)itemCount / goalCount;

        equipmentCount.color = progress switch
        {
            <= 0.375f => Color.red,
            > 0.375f and <= 0.75f => new Color(1f, 0.65f, 0f),
            > 0.75f => Color.green,
            _ => equipmentCount.color
        };
    }

    /// <summary>
    /// Plays the specified audio clip if the audio source is assigned.
    /// </summary>
    /// <param name="clip">The audio clip to play.</param>
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Audio source or clip is not assigned.");
        }
    }
}
