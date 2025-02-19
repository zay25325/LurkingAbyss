using System.Collections;
using UnityEngine;

public class TrapSpawner : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform[] firePoints;
    public float fireInterval = 2f;
    public float projectileSpeed = 5f;
    public float maxRange = 10f;

    private LineRenderer[] lineRenderers;

    private void Start()
    {
        StartCoroutine(FireTrapProjectiles());

        lineRenderers = new LineRenderer[firePoints.Length];
        for (int i = 0; i < firePoints.Length; i++)
        {
            lineRenderers[i] = firePoints[i].gameObject.AddComponent<LineRenderer>();
            lineRenderers[i].positionCount = 2;
            lineRenderers[i].startWidth = 0.05f;
            lineRenderers[i].endWidth = 0.05f;
            lineRenderers[i].material = new Material(Shader.Find("Sprites/Default"));
            lineRenderers[i].startColor = Color.red;
            lineRenderers[i].endColor = Color.red;
        }

        UpdateLineRenderers();
    }

    IEnumerator FireTrapProjectiles()
    {
        while (true)
        {
            FireProjectiles();
            yield return new WaitForSeconds(fireInterval);
        }
    }

    void FireProjectiles()
    {
        foreach (Transform firePoint in firePoints)
        {
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile projectileScript = proj.GetComponent<Projectile>();

            if (projectileScript != null)
            {
                projectileScript.SetProjectileData(projectileSpeed, maxRange);
            }
        }
    }

    void UpdateLineRenderers()
    {
        if (firePoints.Length > 0)
        {
            for (int i = 0; i < firePoints.Length; i++)
            {
                Vector3 rangeEnd = firePoints[i].position + firePoints[i].right * maxRange;
                lineRenderers[i].SetPosition(0, firePoints[i].position);
                lineRenderers[i].SetPosition(1, rangeEnd);
            }
        }
    }

    void Update()
    {
        UpdateLineRenderers();
    }
}
