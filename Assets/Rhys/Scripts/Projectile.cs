using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public float maxDistance = 10f;
    public bool useGravity = false;  // ✅ Toggle gravity in Inspector

    private Vector3 startPosition;
    private Rigidbody2D rb;

    void OnEnable()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();

        if (useGravity)
        {
            rb.gravityScale = 1;  // Enable gravity
            rb.velocity = new Vector2(speed * transform.right.x, 0);
        }
        else
        {
            rb.gravityScale = 0;  // Disable gravity
            rb.velocity = new Vector2(speed * transform.right.x, 0);
        }
    }

    void FixedUpdate()
    {
        if (useGravity)
        {
            // Velocity is updated naturally by gravity
            rb.velocity = new Vector2(speed * transform.right.x, rb.velocity.y);
        }
    }

    void Update()
    {
        // Deactivate projectile when it exceeds max distance
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            gameObject.SetActive(false);
            ProjectileSpawner.Instance.ReturnProjectile(gameObject);
        }
    }
}
