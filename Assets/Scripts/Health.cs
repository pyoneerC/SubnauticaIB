using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float health = 100f;
    public List<int> damageValues = new();
    private int _currentDamageIndex;
    public AudioSource audioSource;
    public AudioClip healSound;

    public Text healthText;

    private Coroutine _damageOverTimeCoroutine;

    private void Start()
    {
        damageValues.Add(50);
        damageValues.Add(30);
        damageValues.Add(19);
        damageValues.Add(1);
        UpdateHealthUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Chelicerate"))
        {
            TakeDamage();

            _damageOverTimeCoroutine ??= StartCoroutine(DamageOverTime(3f, 0.5f));
        }

        if (!other.CompareTag("Kit") || !(health < 100f)) return;
        StartCoroutine(Heal(5f));
        audioSource.PlayOneShot(healSound);
        Destroy(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Chelicerate") || _damageOverTimeCoroutine == null) return;
        StopCoroutine(_damageOverTimeCoroutine);
        _damageOverTimeCoroutine = null;
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

            UpdateHealthUI();
        }
        else
        {
            Debug.Log("No more damage values available.");
        }
    }

    private IEnumerator DamageOverTime(float damageAmount, float interval)
    {
        while (true)
        {
            health -= damageAmount;
            health = Mathf.Max(health, 0f);

            if (health <= 0)
            {
                Die();
                break;
            }

            UpdateHealthUI();
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator Heal(float duration)
    {
        var targetHealth = Mathf.Min(health + 50f, 100f);
        var healingAmount = targetHealth - health;
        var healedAmountPerSecond = healingAmount / duration;
        var timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            health += healedAmountPerSecond * Time.deltaTime;
            health = Mathf.Max(health, 10f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        health = Mathf.Clamp(health, 10f, 100f);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthText == null) return;
        healthText.text = (int)health + "%";
    }

    private void Die()
    {
        // Handle death logic here
        Debug.Log("Player died!");
    }
}
