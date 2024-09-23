using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Raycast : MonoBehaviour
{
    public Camera mainCamera;
    public float rayDistance = 3f;
    public int itemCount;
    public int goalCount = 8;

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

    public TextMeshProUGUI equipmentCount;

    private bool _isCountingDown;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !_isCountingDown)
        {
            StartCoroutine(TryPickUpItem());
        }
    }

    private IEnumerator TryPickUpItem()
    {
        var ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, rayDistance) || !hit.collider.CompareTag("elementos")) yield break;
        var halo = hit.collider.GetComponent("Halo");

        if (!halo)
        {
            _isCountingDown = true;
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green, 4f);
            Debug.Log("Countdown started! Picking up item in 4 seconds...");

            yield return new WaitForSeconds(4f);
        }

        PickUpItem(hit.collider);

        _isCountingDown = false;
    }

    private void PickUpItem(Collider itemCollider)
    {
        itemCount++;
        equipmentCount.text = itemCount.ToString();
        UpdateEquipmentCountColor();  // Update color based on item count

        Destroy(itemCollider.gameObject);

        foreach (Transform child in itemCollider.transform)
        {
            switch (child.tag)
            {
                case "oxygenTank":
                    oxygenTank.color = Color.green;
                    break;
                case "body":
                    body.color = Color.green;
                    legL.color = Color.green;
                    legR.color = Color.green;
                    break;
                case "goggles":
                    gogglesLeft.color = Color.green;
                    gogglesRight.color = Color.green;
                    break;
                case "feet":
                    feetL.color = Color.green;
                    feetR.color = Color.green;
                    break;
                case "weight":
                    weightL.color = Color.green;
                    weightR.color = Color.green;
                    break;
                case "oxygenTankCharge":
                    oxygenTankCharge.color = Color.green;
                    break;
                case "breather":
                    breather.color = Color.green;
                    break;
                case "knife":
                    knifeUp.color = Color.green;
                    break;
                default:
                    Debug.Log("No matching tag found.");
                    break;
            }
        }

        if (itemCount == goalCount)
        {
            SceneManager.LoadScene("Reparation");
        }
    }

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
}
