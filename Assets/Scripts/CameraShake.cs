using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Camera mainCamera;
    public AudioSource audioSource;
    public AudioClip explosionSound;

    private Transform _cameraTransform;
    private Vector3 _originalPosition;

    private void Awake()
    {
        if (mainCamera != null)
        {
            _cameraTransform = mainCamera.transform;
            _originalPosition = _cameraTransform.localPosition;
            StartDelayedShake(11f, 300f, 1f);
        }
        else
        {
            Debug.LogError("Main Camera is not assigned.");
        }
    }

    private void StartDelayedShake(float delay, float duration, float magnitude)
    {
        StartCoroutine(DelayedShakeCoroutine(delay, duration, magnitude));
    }

    private IEnumerator DelayedShakeCoroutine(float delay, float duration, float magnitude)
    {
        yield return new WaitForSeconds(delay);

        var elapsed = 0.0f;

        audioSource.PlayOneShot(explosionSound);

        while (elapsed < duration)
        {
            var x = Random.Range(-1f, 1f) * magnitude;
            var y = Random.Range(-1f, 1f) * magnitude;

            _cameraTransform.localPosition = new Vector3(x, y, _originalPosition.z);

            elapsed += Time.deltaTime;

            magnitude = Mathf.Lerp(magnitude, 0, elapsed / duration);
            yield return null;
        }

        _cameraTransform.localPosition = _originalPosition;
    }
}