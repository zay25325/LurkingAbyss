using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public bool useGravity = false; 

    private Vector3 targetPosition;
    private Rigidbody2D rb;
    private bool hasTarget = false;

    void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = useGravity ? 1 : 0;
    }

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
        hasTarget = true;
    }

    void Update()
    {
        if (hasTarget)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (Vector3.SqrMagnitude(transform.position - targetPosition) <= Mathf.Epsilon)  // More precise than Vector3.Distance
            {
                Debug.Log("Projectile reached target and is being returned.");
                hasTarget = false;
                ResetProjectile();
            }
        }
    }

    private void ResetProjectile()
    {
        gameObject.SetActive(false);
        ProjectileSpawner.Instance.ReturnProjectile(gameObject);
    }
}
