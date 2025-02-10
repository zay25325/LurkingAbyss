using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmlingRespondState : MonsterState
{
    new private SwarmlingController controller { get => base.controller as SwarmlingController; }

    private void OnEnable()
    {
        controller.Agent.speed = controller.BaseSpeed / 1.5f;
    }

    private void Update()
    {
        if (controller.Agent.remainingDistance < 0.5f)
        {
            controller.SwitchState<SwarmlingReturnState>();
        }
    }
}
