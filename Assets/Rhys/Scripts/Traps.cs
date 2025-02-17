using System.Collections;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float fireRate = 1f;
    public float projectileSpeed = 5f;
    public float range = 7f;

    void Start()
    {
        StartCoroutine(FireProjectiles());
    }

    IEnumerator FireProjectiles()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRate);
            FireProjectile();
        }
    }

    void FireProjectile()
    {
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectileScript = proj.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            projectileScript.SetProjectileData(projectileSpeed, range);
        }

        StartCoroutine(DeactivateProjectile(proj, range / projectileSpeed));
    }

    IEnumerator DeactivateProjectile(GameObject proj, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        if (proj.activeSelf)
        {
            proj.SetActive(false);
            ProjectileSpawner.Instance.ReturnProjectile(proj);
        }
    }
}
