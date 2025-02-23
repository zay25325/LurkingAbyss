using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicAttackState : MonsterState
{
    new private MimicController controller { get => base.controller as MimicController; }

    // private void Update()
    // {
    //     if (controller.Target != null)
    //     {
    //         if (Vector2.Distance(transform.position, controller.Target.position) < controller.AttackDistance)
    //         {
    //             controller.SwitchState<MimicIdleState>();
    //         }
    //         else
    //         {
    //             controller.Agent.SetDestination(controller.Target.position);
    //         }
    //     }
    // }

    // private void OnEnable()
    // {
    //     controller.Agent.isStopped = false;
    //     controller.Agent.speed = controller.AttackSpeed;
    //     controller.Agent.SetDestination(controller.Target.position);
    // }

    // private void OnDisable()
    // {
    //     controller.Agent.isStopped = true;
    // }
}
