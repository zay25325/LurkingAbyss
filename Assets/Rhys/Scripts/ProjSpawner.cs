using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    public static ProjectileSpawner Instance;
    public GameObject projectilePrefab;
    public int poolSize = 10;
    private Queue<GameObject> projectilePool;

    [HideInInspector]
    public Projectile projScript;

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

    // void Update()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //         targetPosition.z = 0;
    //         FireProjectile(targetPosition);
    //     }
    // }

    public void FireProjectile(Vector3 startPosition, Vector3 targetPosition)
    {
        if (projectilePool.Count > 0)
        {
            GameObject proj = projectilePool.Dequeue();
            proj.transform.position = startPosition;
            proj.SetActive(true);

            projScript = proj.GetComponent<Projectile>();
            projScript.SetTarget(targetPosition);
        }
    }

    public void ReturnProjectile(GameObject proj)
    {
        if (proj != null && proj.activeInHierarchy)
        {
            proj.transform.position = transform.position;
            proj.SetActive(false);
            projectilePool.Enqueue(proj);
        }
        else
        {
            Debug.Log("Projectile is null or not active in hierarchy.");
        }
    }
}
