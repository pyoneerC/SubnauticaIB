using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Raycast : MonoBehaviour
{
    public Camera mainCamera;
    public float rayDistance = 3f;
    public int itemCount;
    public int goalCount = 8;

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

        if (Physics.Raycast(ray, out hit, rayDistance) && hit.collider.CompareTag("elementos"))
        {
            var halo = hit.collider.GetComponent("Halo");

            if (halo != null)
            {
                Debug.Log("Halo detected! Picking up item automatically.");
                PickUpItem(hit.collider);
            }
            else
            {
                _isCountingDown = true;
                Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green, 4f);
                Debug.Log("Countdown started! Picking up item in 4 seconds...");

                yield return new WaitForSeconds(4f);
                PickUpItem(hit.collider);
            }

            _isCountingDown = false;
        }
    }

    private void PickUpItem(Collider itemCollider)
    {
        itemCount++;
        Destroy(itemCollider.gameObject);
        Debug.Log("Contador de ítems: " + itemCount);

        if (itemCount != goalCount) return;
        SceneManager.LoadScene("Reparation");
    }
}