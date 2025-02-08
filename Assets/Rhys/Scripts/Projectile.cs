using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;   //Configurable speed
    public float maxDistance = 20f; //Configurable max travel distance

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;

        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject); //Destroy the projectile once it reaches max distance
        }
    }
}
