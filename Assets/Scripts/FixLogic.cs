using System.Collections;
using UnityEngine;

public class FixLogic : MonoBehaviour
{
    public int fixedCount;
    public AudioSource audioSource;
    public AudioClip weldingSound;
    public GameObject welderParticles;
    public Welder welder;

    private bool _isOverlapping;
    private GameObject _currentLeak;
    private bool _isFixing;

    private void Start()
    {
        welderParticles.SetActive(false);
    }

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
        audioSource.Stop();
        Debug.Log("Exited leak area.");
        welderParticles.SetActive(false);
    }

    private void Update()
    {
        if (welder != null && !welder.welderInHand)
        {
            Debug.Log("Welder not in hand. Cannot fix leaks.");
            return;
        }

        if (!_isOverlapping || !Input.GetKey(KeyCode.F) || _isFixing) return;

        StartCoroutine(FixLeak());
        audioSource.PlayOneShot(weldingSound);
        welderParticles.SetActive(true);
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

        if (!_isOverlapping || _currentLeak == null)
        {
            Debug.Log("No longer overlapping with leak. Fix aborted.");
            welderParticles.SetActive(false);
            _isFixing = false;
            yield break;
        }

        fixedCount++;
        Debug.Log("Leak fixed! Total fixed leaks: " + fixedCount);

        if (_currentLeak != null)
        {
            GameObject parent = _currentLeak.transform.parent.gameObject;

            foreach (Transform child in parent.transform)
            {
                if (child != _currentLeak.transform)
                {
                    Destroy(child.gameObject);
                }
            }

            Destroy(_currentLeak);
        }

        if (fixedCount >= 7)
        {
            GameOver();
        }

        _isOverlapping = false;
        _isFixing = false;
    }

    private static void GameOver()
    {
        Debug.Log("Game Over! You fixed 7 leaks.");
    }
}
