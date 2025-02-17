using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public static BulletSpawner Instance;
    public GameObject bulletPrefab;
    public int poolSize = 20;
    private Queue<GameObject> bulletPool;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        bulletPool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }

    // void Update()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //         targetPosition.z = 0;
    //         FireBullet(targetPosition);
    //     }
    // }

    public void FireBullet(Vector3 startPosition, Vector3 targetPosition)
    {
        if (bulletPool.Count > 0)
        {
            GameObject bullet = bulletPool.Dequeue();
            bullet.transform.position = startPosition;
            bullet.SetActive(true);

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.SetTarget(targetPosition);
        }
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.transform.position = transform.position;
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }
}
