using UnityEngine;

/// <summary>
/// Represents a Chelicerate that swims in a circular path and interacts with the player.
/// </summary>
public class Chelicerate : MonoBehaviour
{
    /// <summary>
    /// The speed at which the Chelicerate swims.
    /// </summary>
    [Tooltip("The speed at which the Chelicerate swims.")]
    public float swimSpeed = 2f;

    /// <summary>
    /// The maximum distance from the center within which the Chelicerate swims.
    /// </summary>
    [Tooltip("The maximum swimming radius.")]
    public float swimRadius = 5f;

    /// <summary>
    /// The damage dealt to the player on overlap.
    /// </summary>
    [Tooltip("The damage dealt to the player.")]
    public float damage = 10f;

    /// <summary>
    /// The interval at which the Chelicerate changes its swimming direction.
    /// </summary>
    [Tooltip("The time interval for changing direction.")]
    public float changeDirectionInterval = 2f;

    /// <summary>
    /// The speed at which the Chelicerate rotates to face its movement direction.
    /// </summary>
    [Tooltip("The speed of rotation towards the movement direction.")]
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
        Move();
        CheckDirectionChange();
        KeepWithinBounds();
        RotateTowardsDirection();
    }

    /// <summary>
    /// Moves the Chelicerate in its current direction.
    /// </summary>
    private void Move()
    {
        transform.Translate(_direction * (swimSpeed * Time.deltaTime));
    }

    /// <summary>
    /// Checks if it's time to change direction and calls ChangeDirection if needed.
    /// </summary>
    private void CheckDirectionChange()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            ChangeDirection();
            _timer = changeDirectionInterval;
        }
    }

    /// <summary>
    /// Keeps the Chelicerate within the defined swim radius.
    /// </summary>
    private void KeepWithinBounds()
    {
        if (Vector2.Distance(transform.position, Vector2.zero) > swimRadius)
        {
            _direction = -_direction;
        }
    }

    /// <summary>
    /// Rotates the Chelicerate towards its movement direction.
    /// </summary>
    private void RotateTowardsDirection()
    {
        if (_direction == Vector2.zero) return;

        var angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        var targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Changes the direction of the Chelicerate to a random angle.
    /// </summary>
    private void ChangeDirection()
    {
        // Set a random horizontal direction with a random angle
        var angle = Random.Range(0f, 360f);
        _direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized; // Include y-component for 2D movement
    }

    /// <summary>
    /// Handles collision with the player.
    /// </summary>
    /// <param name="other">The collider of the object this Chelicerate collided with.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Overlap with Player");
            // Here you can implement damage to the player if necessary
            // PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            // if (playerHealth != null)
            // {
            //     playerHealth.TakeDamage(damage);
            // }
        }
    }
}
