using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Name: EarlyStopProjectileController
Purpose: A projectile which stops early once it reaches the target or once it hits something.
  Will be used as the base for all grenade and the rock
*/
public class EarlyStopProjectileController : SingleHitProjectileController
{
    new protected void Update()
    {
        base.Update();
        if (Vector2.Distance(StartPos, (Vector2)transform.position) > Vector2.Distance(StartPos, Target))
        {
            OnFlightEnd();
        }
    }
}
