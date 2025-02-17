using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrapController : MonoBehaviour
{
    [SerializeField] float stunDuration;
    [SerializeField] float damage;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnHitEvents onHit = collision.collider.GetComponent<OnHitEvents>();
        if (onHit != null)
        {
            onHit.ApplyHit(stunDuration, damage);
        }
        GameObject.Destroy(gameObject);
    }
}
