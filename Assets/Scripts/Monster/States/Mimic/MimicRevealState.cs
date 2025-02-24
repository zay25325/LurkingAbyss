using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MimicRevealState : MonsterState
{
    private MimicController controller => base.controller as MimicController;

    NavMeshPlus.Components.NavMeshSurface navSurface;
    Vector3? currentNavPoint = null;

    private float safeDistance = 5f; // Minimum distance away from the player
    private float maxDistance = 10f; // Maximum distance for nav point search
    private float checkRadius = 1.5f; // Distance to check if player is nearby
    private bool reachedDestination = false;

    private void Awake()
    {
        navSurface = GameObject.FindFirstObjectByType<NavMeshPlus.Components.NavMeshSurface>();
    }

    private void OnEnable()
    {
        if (controller != null)
        {
            Debug.Log("Mimic is revealing itself.");
            controller.spriteRenderer.sprite = controller.OriginalSprite;
            controller.transform.localScale = new Vector3(3, 3, 1);
            reachedDestination = false;
            FindSafeSpot();

            if (currentNavPoint.HasValue)
            {
                controller.Agent.SetDestination(currentNavPoint.Value);
                StartCoroutine(CheckIfSafe());
            }
        }
        else
        {
            Debug.LogWarning("MimicController is null.");
        }
    }

    private void FindSafeSpot()
    {
        Transform playerTransform = GameObject.FindWithTag("Player").transform;
        Vector3 playerPosition = playerTransform.position;

        Vector3 randomDirection = Random.insideUnitSphere.normalized * Random.Range(safeDistance, maxDistance);
        randomDirection += playerPosition; // Offset from the player’s position

        // Ensure the point is on the NavMesh
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            currentNavPoint = hit.position;
        }
        else
        {
            Debug.LogWarning("Failed to find valid NavMesh point, using fallback.");
            currentNavPoint = playerPosition + (Vector3.right * safeDistance); // Fallback to a straight-line safe spot
        }
    }

    private IEnumerator CheckIfSafe()
    {
        while (!reachedDestination)
        {
            if (!controller.Agent.pathPending && controller.Agent.remainingDistance < 0.5f)
            {
                reachedDestination = true;
                StartCoroutine(CheckPlayerDistance());
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator CheckPlayerDistance()
    {
        while (true)
        {
            Transform playerTransform = GameObject.FindWithTag("Player").transform;
            float distanceToPlayer = Vector3.Distance(playerTransform.position, controller.transform.position);

            if (distanceToPlayer > checkRadius * safeDistance)
            {
                SwitchToIdle();
                yield break;
            }
            
            yield return new WaitForSeconds(1f);
        }
    }

    private void SwitchToIdle()
    {
        Debug.Log("Mimic is safe and player is far. Switching to idle.");
        controller.SwitchState<MimicIdleState>();
    }
}
