using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WaterLogic : MonoBehaviour
{
    [FormerlySerializedAs("max_oxygen")] public int maxOxygen = 45;
    [FormerlySerializedAs("current_oxygen")] public int currentOxygen;

    private void Start()
    {
        currentOxygen = maxOxygen;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Sea")) return;
        Debug.Log("Player has entered the water volume!");
        StartCoroutine(OxygenDecrement());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Sea")) return;
        Debug.Log("Player has exited the water volume!");
        StopCoroutine(OxygenDecrement());
    }

    private IEnumerator OxygenDecrement()
    {
        while (currentOxygen > 0)
        {
            yield return new WaitForSeconds(1.5f);
            currentOxygen -= 3;
            Debug.Log("Current oxygen: " + currentOxygen);

            if (currentOxygen <= 0)
            {
                //death screen, restart buttons blabla
                Debug.Log("Oxygen has depleted!");
            }
        }
    }
}
