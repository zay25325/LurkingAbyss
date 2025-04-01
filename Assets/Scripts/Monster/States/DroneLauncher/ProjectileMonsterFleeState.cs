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
        Vector3 bestFleePosition = Vector3.zero;
        bool foundValidFleePosition = false;
        float maxCheckDistance = controller.FleeDistance;
        int attempts = 5; // Try multiple directions

        for (int i = 0; i < attempts; i++)
        {
            Vector3 potentialFleePosition = controller.transform.position + fleeDirection * maxCheckDistance;

            if (NavMesh.SamplePosition(potentialFleePosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                bestFleePosition = hit.position;
                foundValidFleePosition = true;
                break; // Found a valid flee position, exit loop
            }
            
            // Rotate flee direction slightly if the first attempt fails
            fleeDirection = Quaternion.Euler(0, 0, Random.Range(-45f, 45f)) * fleeDirection;
            maxCheckDistance *= 0.8f; // Reduce distance to avoid getting stuck in walls
        }

        if (foundValidFleePosition)
        {
            fleeDestination = bestFleePosition;
            controller.Agent.SetDestination(fleeDestination);
        }
        else
        {
            Debug.LogWarning("ProjectileMonsterFleeState: Failed to find a valid flee position! Using random movement.");

            // Move in a random direction if stuck
            Vector3 randomDirection = Random.insideUnitCircle.normalized * controller.FleeDistance;
            if (NavMesh.SamplePosition(controller.transform.position + randomDirection, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                fleeDestination = hit.position;
                controller.Agent.SetDestination(fleeDestination);
            }
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
