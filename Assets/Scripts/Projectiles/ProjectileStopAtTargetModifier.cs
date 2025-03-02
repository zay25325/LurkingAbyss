using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStopAtTargetModifier : ProjectileModifier
{
    public override void OnMove(Vector2 currentPosition, Vector2 StartPosition, Vector2 TargetPosition)
    {
        if (Vector2.Distance(StartPosition, currentPosition) > Vector2.Distance(StartPosition, TargetPosition))
        {
            controller.EndFlight();
        }
    }
}
