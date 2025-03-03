
// FileName:     ProjectileMonsterFleeState.cs
// Assignment:   Capstone Project
// Author:       Rhys McCash
// Student #:    8825169
// Date:         03/01/2025
// Description:  Handles fleeing behavior when the monster is too close to the target.

using UnityEngine;
using UnityEngine.AI;

public class ProjectileMonsterFleeState : MonsterState
{
    new protected ProjectileMonsterController controller => base.controller as ProjectileMonsterController;

    private void OnEnable()
    {
        Flee();
    }

    private void Flee()
    {
        if (controller.Target == null) return;

        Vector3 fleeDirection = (controller.transform.position - controller.Target.transform.position).normalized;
        Vector3 fleePosition = controller.transform.position + fleeDirection * controller.FleeDistance;

        if (NavMesh.SamplePosition(fleePosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            controller.Agent.SetDestination(hit.position);
        }
    }

    private void Update()
    {
        if (Vector3.Distance(controller.transform.position, controller.Target.transform.position) >= controller.FleeDistance)
        {
            controller.SwitchState<ProjectileMonsterStalkingState>();
        }
    }
}
