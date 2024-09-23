using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 100f;
    public List<int> damageValues = new();
    private int _currentDamageIndex;

    private void Start()
    {
        damageValues.Add(50);
        damageValues.Add(30);
        damageValues.Add(19);
        damageValues.Add(1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Chelicerate"))
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        if (damageValues.Count > 0)
        {
            float damage = damageValues[_currentDamageIndex];
            health -= damage;

            _currentDamageIndex++;
            if (_currentDamageIndex >= damageValues.Count)
            {
                _currentDamageIndex = 0;
            }

            if (health <= 0)
            {
                Die();
            }
        }
        else
        {
            Debug.Log("No more damage values available.");
        }
    }

    private static void Die()
    {

    }
}