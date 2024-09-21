using UnityEngine;
using UnityEngine.SceneManagement;

public class Raycast : MonoBehaviour
{
    public Camera mainCamera;
    public float rayDistance = 3f;
    public int itemCount;
    public int goalCount = 8;

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        var ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, rayDistance)) return;

        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green, 1f);

        if (!hit.collider.CompareTag("elementos")) return;
        itemCount++;
        Destroy(hit.collider.gameObject);
        Debug.Log("¡Contador de ítems: " + itemCount);

        if (itemCount != goalCount) return;
        Debug.Log("READY!");
        SceneManager.LoadScene("Reparation");
    }
}