using UnityEngine;

public class Raycast : MonoBehaviour
{
    public Camera mainCamera;
    public float rayDistance = 50f;
    public int itemCount = 0;

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, rayDistance)) return;

        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green, 1f);

        if (!hit.collider.CompareTag("elementos")) return;
        itemCount++;
        Destroy(hit.collider.gameObject);
        Debug.Log("¡Contador de ítems: " + itemCount);

        if (itemCount == 8)
        {
            Debug.Log("READY!");
        }
    }
}