using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScavengerBaseState : MonsterState
{
    new protected ScavengerController controller { get => base.controller as ScavengerController; }

    protected void OnEnable()
    {
        controller.OverrideSightDirection = false;
    }
}
