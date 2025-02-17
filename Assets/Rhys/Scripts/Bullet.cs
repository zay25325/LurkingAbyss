using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float maxDistance = 10f;

    private Vector3 startPosition;
    private Vector3 direction;
    private bool isFired = false;

    void OnEnable()
    {
        startPosition = transform.position;
        isFired = true;
    }

    public void SetTarget(Vector3 target)
    {
        direction = (target - transform.position).normalized;
    }

    void Update()
    {
        if (isFired)
        {
            transform.position += direction * speed * Time.deltaTime;

            if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
            {
                Debug.Log("Bullet reached max distance and is being returned.");
                isFired = false;
                ResetBullet();
            }
        }
    }

    private void ResetBullet()
    {
        gameObject.SetActive(false);
        BulletSpawner.Instance.ReturnBullet(gameObject);
    }
}
