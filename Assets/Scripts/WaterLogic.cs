using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class WaterLogic : MonoBehaviour
{
    [FormerlySerializedAs("max_oxygen")] public int maxOxygen = 45;
    [FormerlySerializedAs("current_oxygen")] public int currentOxygen;

    public Rigidbody playerRigidbody;
    public float waterGravityScale = 0.5f;
    public float swimSpeed = 5f;

    private Coroutine _oxygenDecrementCoroutine;
    private Coroutine _restoreOxygenCoroutine;
    private float _previousWaitTime;
    private bool _isUnderwater;

    private void Start()
    {
        currentOxygen = maxOxygen;
        _previousWaitTime = GetWaitTime();
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

        Debug.Log("Player has entered the water volume!");

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

        Debug.Log("Player has exited the water volume!");

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
                case <= 6:
                    Debug.Log("GET OXYGEN NOW, YOU'RE DYING!");
                    break;
                case <= 15:
                    Debug.Log("Oxygen very low! Get oxygen immediately!");
                    break;
                case <= 21:
                    Debug.Log("Oxygen critical! Get some oxygen!");
                    break;
            }

            yield return new WaitForSeconds(currentWaitTime);
            currentOxygen -= 3;
            Debug.Log("Current oxygen: " + currentOxygen);

            if (currentOxygen <= 0)
            {
                Debug.Log("Oxygen has depleted!");
            }
        }
    }

    private IEnumerator RestoreOxygen()
    {
        const float lerpDuration = 5f;
        var timeElapsed = 0f;
        var startOxygen = currentOxygen;

        Debug.Log("Starting oxygen restoration...");

        while (timeElapsed < lerpDuration)
        {
            timeElapsed += Time.deltaTime;
            currentOxygen = (int)Mathf.Lerp(startOxygen, maxOxygen, timeElapsed / lerpDuration);
            Debug.Log("Restoring oxygen: " + currentOxygen);
            yield return null;
        }

        currentOxygen = maxOxygen;
        Debug.Log("Oxygen fully restored: " + currentOxygen);
        _restoreOxygenCoroutine = null;
    }

    private void EnterWater()
    {
        _isUnderwater = true;
        Physics.gravity *= waterGravityScale;
        playerRigidbody.drag = 3f;
        Debug.Log("Underwater effects enabled.");
    }

    private void ExitWater()
    {
        _isUnderwater = false;
        Physics.gravity /= waterGravityScale;
        playerRigidbody.drag = 0f;
        Debug.Log("Underwater effects disabled.");
    }

    private void HandleUnderwaterMovement()
    {
        var verticalInput = Input.GetAxis("Vertical");
        var swimDirection = transform.up * verticalInput;
        playerRigidbody.AddForce(swimDirection * (swimSpeed * 0.2f), ForceMode.Acceleration);
    }

    private float GetWaitTime()
    {
        return currentOxygen switch
        {
            > 21 => 1.5f,
            > 15 => 2.5f,
            > 6 => 3f,
            _ => 3f
        };
    }
}
