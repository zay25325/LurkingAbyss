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
    private Vector3 fleeDestination;

    private void OnEnable()
    {
        PickNewFleeLocation();
    }

    private void PickNewFleeLocation()
    {
        if (controller.Target == null) return;

        Vector3 fleeDirection = (controller.transform.position - controller.Target.transform.position).normalized;
        Vector3 potentialFleePosition = controller.transform.position + fleeDirection * controller.FleeDistance;

        if (NavMesh.SamplePosition(potentialFleePosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            fleeDestination = hit.position;
            controller.Agent.SetDestination(fleeDestination);
        }
        else
        {
            Debug.LogWarning("Failed to find a valid flee position!");
        }
    }

    private void Update()
    {
        float distanceToTarget = Vector3.Distance(controller.transform.position, controller.Target.transform.position);

        if (!controller.Agent.pathPending && controller.Agent.remainingDistance <= 0.5f)
        {
            if (distanceToTarget < controller.FleeDistance)
            {
                PickNewFleeLocation();
                return;
            }
            else
            {
                controller.SwitchState<ProjectileMonsterStalkingState>();
            }
        }
    }
}
