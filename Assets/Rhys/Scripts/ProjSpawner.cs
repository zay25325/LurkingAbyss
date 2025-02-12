using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    public static ProjectileSpawner Instance;
    public GameObject projectilePrefab;
    public int poolSize = 10;
    private Queue<GameObject> projectilePool;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        projectilePool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject proj = Instantiate(projectilePrefab);
            proj.SetActive(false);
            projectilePool.Enqueue(proj);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // Left click
        {
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0; // Ensure it's in 2D space
            FireProjectile(targetPosition);
        }
    }

    void FireProjectile(Vector3 targetPosition)
    {
        if (projectilePool.Count > 0)
        {
            GameObject proj = projectilePool.Dequeue();
            proj.transform.position = transform.position;
            proj.SetActive(true);

            Projectile projScript = proj.GetComponent<Projectile>();
            projScript.SetTarget(targetPosition);
        }
    }

    public void ReturnProjectile(GameObject proj)
    {
        proj.transform.position = transform.position;  // Reset position
        proj.SetActive(false);
        projectilePool.Enqueue(proj);
    }
}
