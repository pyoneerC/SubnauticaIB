using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FixLogic : MonoBehaviour
{
    public int fixedCount;
    public AudioSource audioSource;
    public AudioClip weldingSound;
    public GameObject welderParticles;
    public Welder welder;
    public GameObject welderPrefab;

    // Minimap circles and particles
    public GameObject circle0;
    public GameObject particle0;
    public GameObject fireworks0;
    public GameObject circle1;
    public GameObject particle1;
    public GameObject fireworks1;
    public GameObject circle2;
    public GameObject particle2;
    public GameObject fireworks2;
    public GameObject circle3;
    public GameObject particle3;
    public GameObject fireworks3;
    public GameObject circle4;
    public GameObject particle4;
    public GameObject fireworks4;

    private bool _isOverlapping;
    private GameObject _currentLeak;
    private bool _isFixing;

    private void Start()
    {
        welderParticles.SetActive(false);
        fireworks0.SetActive(false);
        fireworks1.SetActive(false);
        fireworks2.SetActive(false);
        fireworks3.SetActive(false);
        fireworks4.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Leak")) return;
        _isOverlapping = true;
        _currentLeak = other.gameObject;
        StartCoroutine(RotateWelder());
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
        if (welder != null && !welder.WelderInHand)
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

        StartCoroutine(RotateWelder());

        if (!_isOverlapping || _currentLeak == null)
        {
            Debug.Log("No longer overlapping with leak. Fix aborted.");
            welderParticles.SetActive(false);
            _isFixing = false;
            yield break;
        }

        fixedCount++;

        HandleMinimapCirclesAndParticles();

        if (fixedCount >= 7)
        {
            GameOver();
        }

        _isOverlapping = false;
        _isFixing = false;

        StartCoroutine(RotateWelder());
    }

    private void HandleMinimapCirclesAndParticles()
    {
        bool hasHalo = _currentLeak.GetComponent("Halo");
        bool hasWindZone = _currentLeak.GetComponent<WindZone>();
        bool hasProjector = _currentLeak.GetComponent<Projector>();
        bool hasLensFlare = _currentLeak.GetComponent<LensFlare>();
        bool hasMask = _currentLeak.GetComponent<Mask>();

        if (hasHalo)
        {
            if (fixedCount % 2 == 0)
            {
                circle0.SetActive(false);
                particle0.SetActive(false);
                fireworks0.SetActive(true);
            }
        }
        else if (hasWindZone)
        {
            circle1.SetActive(false);
            particle1.SetActive(false);
            fireworks1.SetActive(true);
        }
        else if (hasProjector)
        {
            if (fixedCount % 2 == 0)
            {
                circle2.SetActive(false);
                particle2.SetActive(false);
                fireworks2.SetActive(true);
            }
        }
        else if (hasLensFlare)
        {
            circle3.SetActive(false);
            particle3.SetActive(false);
            fireworks3.SetActive(true);
        }
        else if (hasMask)
        {
            circle4.SetActive(false);
            particle4.SetActive(false);
            fireworks4.SetActive(true);
        }

        var parent = _currentLeak.transform.parent.gameObject;
        foreach (Transform child in parent.transform)
        {
            if (child != _currentLeak.transform)
            {
                Destroy(child.gameObject);
            }
        }

        Destroy(_currentLeak);
    }

    private static void GameOver()
    {
        Debug.Log("Game Over! You fixed 7 leaks.");
    }

    private IEnumerator RotateWelder()
    {
        var originalRotation = welderPrefab.transform.rotation;
        var targetRotation = originalRotation * Quaternion.Euler(10f, 10f, 0f);

        const float rotateDuration = 0.5f;
        for (float t = 0; t < rotateDuration; t += Time.deltaTime)
        {
            welderPrefab.transform.rotation = Quaternion.Lerp(originalRotation, targetRotation, t / rotateDuration);
            yield return null;
        }
        welderPrefab.transform.rotation = targetRotation;

        for (float t = 0; t < rotateDuration; t += Time.deltaTime)
        {
            welderPrefab.transform.rotation = Quaternion.Lerp(targetRotation, originalRotation, t / rotateDuration);
            yield return null;
        }
        welderPrefab.transform.rotation = originalRotation;
    }
}
