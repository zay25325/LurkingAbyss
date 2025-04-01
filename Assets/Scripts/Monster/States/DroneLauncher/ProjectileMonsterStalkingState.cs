// FileName:     ProjectileMonsterStalkingState.cs
// Assignment:   Capstone Project
// Author:       Rhys McCash
// Student #:    8825169
// Date:         03/01/2025
// Description:  Handles stalking behavior, maintaining a set distance from the target.

using UnityEngine;

public class ProjectileMonsterStalkingState : MonsterState
{
    new protected ProjectileMonsterController controller => base.controller as ProjectileMonsterController;

    private void Update()
    {
        if (controller.Target == null || !controller.Target.CompareTag("Player"))
        {
            controller.Target = null;
            controller.SwitchState<ProjectileMonsterMoveState>();
            return;
        }

        float distanceToTarget = Vector3.Distance(controller.transform.position, controller.Target.transform.position);

        if (distanceToTarget > controller.MaxChaseDistance)
        {
            controller.Target = null;
            controller.SwitchState<ProjectileMonsterMoveState>();
            return;
        }

        if (distanceToTarget < controller.FleeDistance)
        {
            controller.SwitchState<ProjectileMonsterFleeState>();
        }
        else if (distanceToTarget > controller.StalkingDistance)
        {
            controller.Agent.SetDestination(controller.Target.transform.position);
        }
        else
        {
            controller.Agent.SetDestination(controller.transform.position);
            controller.SwitchState<ProjectileMonsterAttackingState>();
        }
    }
}
