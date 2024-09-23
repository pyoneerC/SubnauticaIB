using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WaterLogic : MonoBehaviour
{
    [FormerlySerializedAs("max_oxygen")] public int maxOxygen = 45;
    [FormerlySerializedAs("current_oxygen")] public int currentOxygen;

    public Rigidbody playerRigidbody;
    public float waterGravityScale = 0.5f;
    public float swimSpeed = 5f;
    public float verticalSwimSpeed = 2f;
    public AudioClip underwaterAmbience;
    public AudioClip underwaterBreathing;
    public AudioClip oxygenWarning1;
    public AudioClip oxygenWarning2;
    public AudioClip oxygenWarning3;
    public Image blackFadeImage;

    private AudioSource _audioSource;
    private Coroutine _oxygenDecrementCoroutine;
    private Coroutine _restoreOxygenCoroutine;
    private float _previousWaitTime;
    private bool _isUnderwater;
    private bool _isFadingIn;

    private bool _hasPlayedWarning1;
    private bool _hasPlayedWarning2;
    private bool _hasPlayedWarning3;

    private void Start()
    {
        currentOxygen = maxOxygen;
        _previousWaitTime = GetWaitTime();
        _audioSource = GetComponent<AudioSource>();
        blackFadeImage.color = new Color(0f, 0f, 0f, 0f);

        _audioSource.volume = 1.0f;
    }

    private void Update()
    {
        if (_isUnderwater)
        {
            HandleUnderwaterMovement();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Sea")) return;

        EnterWater();

        if (_restoreOxygenCoroutine != null)
        {
            StopCoroutine(_restoreOxygenCoroutine);
            _restoreOxygenCoroutine = null;
        }

        currentOxygen = maxOxygen;

        if (_oxygenDecrementCoroutine != null)
        {
            StopCoroutine(_oxygenDecrementCoroutine);
        }

        _oxygenDecrementCoroutine = StartCoroutine(OxygenDecrement());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Sea")) return;

        ExitWater();

        if (_oxygenDecrementCoroutine != null)
        {
            StopCoroutine(_oxygenDecrementCoroutine);
            _oxygenDecrementCoroutine = null;
        }

        _restoreOxygenCoroutine ??= StartCoroutine(RestoreOxygen());
    }

    private IEnumerator OxygenDecrement()
    {
        while (currentOxygen > 0)
        {
            var currentWaitTime = GetWaitTime();

            if (!Mathf.Approximately(currentWaitTime, _previousWaitTime))
            {
                _previousWaitTime = currentWaitTime;
            }

            switch (currentOxygen)
            {
                case <= 6 when !_hasPlayedWarning3:
                    Debug.Log("GET OXYGEN NOW, YOU'RE DYING!");
                    _audioSource.PlayOneShot(oxygenWarning3);
                    _hasPlayedWarning3 = true;
                    StartCoroutine(FadeInBlackCanvas());
                    break;
                case <= 15 when !_hasPlayedWarning2:
                    Debug.Log("Oxygen very low! Get oxygen immediately!");
                    _audioSource.PlayOneShot(oxygenWarning2);
                    _hasPlayedWarning2 = true;
                    break;
                case <= 30 when !_hasPlayedWarning1:
                    Debug.Log("Oxygen critical! Get some oxygen!");
                    _audioSource.PlayOneShot(oxygenWarning1);
                    _hasPlayedWarning1 = true;
                    break;
            }

            yield return new WaitForSeconds(currentWaitTime);
            currentOxygen -= 3;

            if (currentOxygen <= 0)
            {
                _audioSource.Stop();
            }
        }
    }

    private IEnumerator RestoreOxygen()
    {
        const float lerpDuration = 5f;
        var timeElapsed = 0f;
        var startOxygen = currentOxygen;

        while (timeElapsed < lerpDuration)
        {
            timeElapsed += Time.deltaTime;
            currentOxygen = (int)Mathf.Lerp(startOxygen, maxOxygen, timeElapsed / lerpDuration);
            yield return null;
        }

        currentOxygen = maxOxygen;
        _restoreOxygenCoroutine = null;

        _hasPlayedWarning1 = false;
        _hasPlayedWarning2 = false;
        _hasPlayedWarning3 = false;
    }

    private IEnumerator FadeInBlackCanvas()
    {
        _isFadingIn = true;
        const float duration = 6f;
        var timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            var alpha = Mathf.Clamp01(timeElapsed / duration);
            blackFadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        blackFadeImage.color = new Color(0, 0, 0, 1);
        _isFadingIn = false;
    }

    private void EnterWater()
    {
        _isUnderwater = true;
        Physics.gravity *= waterGravityScale;
        playerRigidbody.drag = 3f;
        _audioSource.PlayOneShot(underwaterAmbience);

        _audioSource.clip = underwaterBreathing;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    private void ExitWater()
    {
        _isUnderwater = false;
        Physics.gravity /= waterGravityScale;
        playerRigidbody.drag = 0f;
        _hasPlayedWarning1 = false;
        _hasPlayedWarning2 = false;
        _hasPlayedWarning3 = false;

        _audioSource.loop = false;
        _audioSource.Stop();
    }

    private void HandleUnderwaterMovement()
    {
        var verticalInput = Input.GetAxis("Vertical");
        var ascendInput = Input.GetKey(KeyCode.E) ? 1f : (Input.GetKey(KeyCode.Q) ? -1f : 0f);

        var swimDirection = transform.forward * verticalInput + transform.up * (ascendInput * verticalSwimSpeed);
        playerRigidbody.AddForce(swimDirection * swimSpeed, ForceMode.Acceleration);
    }

    private float GetWaitTime()
    {
        return currentOxygen switch
        {
            > 30 => 1.5f,
            > 15 => 2.5f,
            > 6 => 3f,
            _ => 3f
        };
    }
}
