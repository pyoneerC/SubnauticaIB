using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the health of the player, including taking damage and healing.
/// </summary>
public class Health : MonoBehaviour
{
    /// <summary>
    /// The player's current health value.
    /// </summary>
    [Tooltip("The player's health.")]
    [Range(0,100)]
    public float health = 100f;

    /// <summary>
    /// A list of possible damage values to take.
    /// </summary>
    [Tooltip("List of damage values that can be inflicted.")]
    public List<int> damageValues = new();

    /// <summary>
    /// The index of the current damage value being used.
    /// </summary>
    private int _currentDamageIndex;

    /// <summary>
    /// The audio source for playing sound effects.
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// The sound played when healing.
    /// </summary>
    public AudioClip healSound;

    /// <summary>
    /// The UI text element displaying the player's health.
    /// </summary>
    public Text healthText;

    /// <summary>
    /// Coroutine for handling damage over time.
    /// </summary>
    private Coroutine _damageOverTimeCoroutine;

    private void Start()
    {
        // Initialize damage values
        damageValues.Add(50);
        damageValues.Add(30);
        damageValues.Add(19);
        damageValues.Add(1);

        // Update the health UI at the start
        UpdateHealthUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has collided with a Chelicerate
        if (other.CompareTag("Chelicerate"))
        {
            TakeDamage();
            _damageOverTimeCoroutine ??= StartCoroutine(DamageOverTime(3f, 0.5f));
        }

        // Check if the player has collided with a healing kit
        if (!other.CompareTag("Kit") || !(health < 100f)) return;
        StartCoroutine(Heal(5f));
        audioSource?.PlayOneShot(healSound);
        Destroy(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        // Stop damage over time if exiting a Chelicerate trigger
        if (!other.CompareTag("Chelicerate") || _damageOverTimeCoroutine == null) return;
        StopCoroutine(_damageOverTimeCoroutine);
        _damageOverTimeCoroutine = null;
    }

    /// <summary>
    /// Handles taking damage from enemies or other sources.
    /// </summary>
    private void TakeDamage()
    {
        if (damageValues.Count > 0)
        {
            float damage = damageValues[_currentDamageIndex];
            health -= damage;
            _currentDamageIndex = (_currentDamageIndex + 1) % damageValues.Count; // Loop through damage values

            if (health <= 0)
            {
                Die();
            }

            UpdateHealthUI();
        }
        else
        {
            Debug.LogWarning("No more damage values available.");
        }
    }

    /// <summary>
    /// Applies damage over time at a specified interval.
    /// </summary>
    /// <param name="damageAmount">Amount of damage to apply.</param>
    /// <param name="interval">Time in seconds between each damage application.</param>
    /// <returns>An IEnumerator for the coroutine.</returns>
    private IEnumerator DamageOverTime(float damageAmount, float interval)
    {
        while (true)
        {
            health -= damageAmount;
            health = Mathf.Max(health, 0f); // Ensure health does not drop below 0

            if (health <= 0)
            {
                Die();
                break;
            }

            UpdateHealthUI();
            yield return new WaitForSeconds(interval);
        }
    }

    /// <summary>
    /// Heals the player over a specified duration.
    /// </summary>
    /// <param name="duration">Duration over which to heal.</param>
    /// <returns>An IEnumerator for the coroutine.</returns>
    private IEnumerator Heal(float duration)
    {
        var targetHealth = Mathf.Min(health + 50f, 100f);
        var healingAmount = targetHealth - health;
        var healedAmountPerSecond = healingAmount / duration;
        var timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            health += healedAmountPerSecond * Time.deltaTime;
            health = Mathf.Max(health, 10f); // Ensure health does not drop below 10
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        health = Mathf.Clamp(health, 10f, 100f);
        UpdateHealthUI();
    }

    /// <summary>
    /// Updates the health display UI.
    /// </summary>
    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"{(int)health}%";
        }
    }

    /// <summary>
    /// Handles the player's death.
    /// </summary>
    private void Die()
    {
        // Handle death logic here
        Debug.Log("Player died!");
    }
}
