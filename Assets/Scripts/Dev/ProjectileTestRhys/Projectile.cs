using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public bool useGravity = false; 

    private Vector3 targetPosition;
    private Vector3 startPosition;
    private Rigidbody2D rb;
    private bool hasTarget = false;
    private bool useMaxDistance = false;
    private float maxDistance;

    private bool hasLanded = false;

    void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = useGravity ? 1 : 0;
        startPosition = transform.position;
        hasLanded = false;
    }

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
        hasTarget = true;
        useMaxDistance = false;
        hasLanded = false;
    }

    public void SetProjectileData(float speed, float maxDistance)
    {
        this.speed = speed;
        this.maxDistance = maxDistance;
        startPosition = transform.position;
        hasTarget = false;
        useMaxDistance = true;
        hasLanded = false;
    }

    void Update()
    {
        if (hasTarget)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (Vector3.SqrMagnitude(transform.position - targetPosition) <= Mathf.Epsilon)  
            {
                Debug.Log("Projectile reached target and is being returned.");
                hasLanded = true;
                ResetProjectile();
            }
        }
        else if (useMaxDistance)
        {
            transform.position += transform.right * speed * Time.deltaTime;

            if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
            {
                Debug.Log("Projectile reached max distance and is being destroyed.");
                Destroy(gameObject);
            }
        }
    }

    private void ResetProjectile()
    {
        gameObject.SetActive(false);
        ProjectileSpawner.Instance.ReturnProjectile(gameObject);
    }

    public bool HasLanded()
    {
        return hasLanded;
    }
}
