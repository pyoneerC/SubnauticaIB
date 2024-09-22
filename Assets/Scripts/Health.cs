using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 100f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Chelicerate"))
        {
            TakeDamage(35f);
        }
    }

    private void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"Health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        // Add death logic here (e.g., restart the game, show a game over screen)
    }
}