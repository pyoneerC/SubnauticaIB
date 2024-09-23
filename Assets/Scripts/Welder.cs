using UnityEngine;

public class Welder : MonoBehaviour
{
    public GameObject welderPrefab;
    public float rayDistance = 3f;
    public Camera playerCamera;

    private bool _welderInHand;

    private void Start()
    {
        if (welderPrefab != null)
        {
            welderPrefab.SetActive(false);
        }
        else
        {
            Debug.LogError("Welder prefab not assigned!");
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickUpWelder();
        }
    }

    private void TryPickUpWelder()
    {
        var ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (!Physics.Raycast(ray, out var hit, rayDistance)) return;

        if (!hit.collider.CompareTag("welder")) return;

        hit.collider.gameObject.SetActive(false);

        if (_welderInHand) return;
        welderPrefab.SetActive(true);
        _welderInHand = true;
    }
}