using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHitLimitModifier : ProjectileModifier
{
    [SerializeField] int hitLimit = 1;
    int hitCount = 0;

    new protected void OnEnable()
    {
        base.OnEnable();
        hitCount = 0;
    }

    public override void OnHit(Collider2D collision)
    {
        hitCount++;
        if (hitCount >= hitLimit)
        {
            controller.EndFlight();
        }
    }
}
