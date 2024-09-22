using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public float swimSpeed = 2f;
    public float changeDirectionInterval = 2f;
    public float swimHeight = 0.5f;
    public float swimFrequency = 1f;
    public float randomWanderAmount = 0.5f;

    private Vector2 _direction;
    private float _timer;
    private float _initialY;

    private void Start()
    {
        ChangeDirection();
        _timer = changeDirectionInterval;
        _initialY = transform.position.y;
    }

    private void Update()
    {
        transform.Translate(_direction * (swimSpeed * Time.deltaTime));
        float newY = _initialY + Mathf.Sin(Time.time * swimFrequency) * swimHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            ChangeDirection();
            _timer = changeDirectionInterval;
        }

        _direction += new Vector2(Random.Range(-randomWanderAmount, randomWanderAmount), Random.Range(-randomWanderAmount, randomWanderAmount));
        _direction.Normalize();

        Vector2 position = transform.position;
        if (position.x < -5 || position.x > 5 || position.y < -5 || position.y > 5)
        {
            _direction = -_direction;
        }
    }

    private void ChangeDirection()
    {
        float angle = Random.Range(0f, 360f);
        _direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Vector2 fleeDirection = (transform.position - other.transform.position).normalized;
            _direction = fleeDirection;
        }
    }
}