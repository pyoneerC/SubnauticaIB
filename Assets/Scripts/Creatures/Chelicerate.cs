using UnityEngine;

public class Chelicerate : MonoBehaviour
{
    public float swimSpeed = 2f;
    public float swimRadius = 5f;
    public float damage = 10f;
    public float changeDirectionInterval = 2f;
    public float rotationSpeed = 5f;

    private Vector2 _direction;
    private float _timer;

    private void Start()
    {
        ChangeDirection();
        _timer = changeDirectionInterval;
    }

    private void Update()
    {
        transform.Translate(_direction * (swimSpeed * Time.deltaTime));

        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            ChangeDirection();
            _timer = changeDirectionInterval;
        }

        if (Vector2.Distance(transform.position, Vector2.zero) > swimRadius)
        {
            _direction = -_direction;
        }

        if (_direction == Vector2.zero) return;
        var angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        var targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void ChangeDirection()
    {
        // Set a random horizontal direction
        var angle = Random.Range(0f, 360f);
        _direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), 0).normalized; // Ensure y-component is zero
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Overlap");
        }
    }
}