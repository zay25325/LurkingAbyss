using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    public static ProjectileSpawner Instance;
    public GameObject projectilePrefab;
    public int poolSize = 10;
    private Queue<GameObject> projectilePool;

    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

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
        if (Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FireProjectile()
    {
        if (projectilePool.Count > 0)
        {
            GameObject proj = projectilePool.Dequeue();
            proj.transform.position = transform.position;
            proj.transform.rotation = transform.rotation;
            proj.SetActive(true);
        }
    }

    public void ReturnProjectile(GameObject proj)
    {
        proj.SetActive(false);
        projectilePool.Enqueue(proj);
    }
}
