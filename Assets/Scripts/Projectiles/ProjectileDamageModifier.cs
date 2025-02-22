using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamageModifier : ProjectileModifier
{
    [SerializeField] private float stunDuration;
    [SerializeField] private float damage;
    [SerializeField] private float structuralDamage;

    public override void OnHit(Collider2D collision)
    {
        OnHitEvents onHit = collision.GetComponent<OnHitEvents>();
        if (onHit != null)
        {
            onHit.ApplyHit(stunDuration, damage, structuralDamage);
        }
    }
}
