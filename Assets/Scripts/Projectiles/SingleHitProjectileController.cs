using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Name: HitStopProjectileController
Purpose: A projectile which stops after hitting it's first target
  Good for simple guns like a pistol or taser.
*/
public class SingleHitProjectileController : ProjectileController
{
    new protected void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        OnFlightEnd();
    }
}
