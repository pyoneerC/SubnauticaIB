using System.Collections;
using UnityEngine;

public class FixLogic : MonoBehaviour
{
    public int fixedCount;
    private bool _isOverlapping;
    private GameObject _currentLeak;
    private bool _isFixing;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Leak")) return;
        _isOverlapping = true;
        _currentLeak = other.gameObject;
        Debug.Log("Overlapping with leak. Hold 'F' to fix.");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Leak")) return;
        _isOverlapping = false;
        _currentLeak = null;
        Debug.Log("Exited leak area.");
    }

    private void Update()
    {
        if (_isOverlapping && Input.GetKey(KeyCode.F) && !_isFixing)
        {
            StartCoroutine(FixLeak());
        }
    }

    private IEnumerator FixLeak()
    {
        _isFixing = true;
        Debug.Log("Fixing leak... Hold 'F' for 5 seconds.");
        const float holdDuration = 5f;
        var timer = 0f;

        while (timer < holdDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        fixedCount++;
        Debug.Log("Leak fixed! Total fixed leaks: " + fixedCount);

        if (_currentLeak != null)
        {
            Destroy(_currentLeak);
        }

        _isOverlapping = false;
        _isFixing = false;
    }
}