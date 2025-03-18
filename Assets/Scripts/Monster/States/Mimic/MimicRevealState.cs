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

    private Transform playerTransform; // Store reference to the player
    private float playerDistanceThreshold = 7f; // Distance threshold for player proximity
    private Vector3 lastDirection = Vector3.zero; // Stores last movement direction

    private void Awake()
    {
        navSurface = GameObject.FindFirstObjectByType<NavMeshPlus.Components.NavMeshSurface>();
    }

    private void OnEnable()
    {
        if (controller != null)
        {
            // Disable the Circle Collider 2D
            CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
            if (circleCollider != null)
            {
                circleCollider.isTrigger = false;
                Debug.Log("Circle Collider 2D enabled.");
            }
            else
            {
                Debug.LogWarning("CircleCollider2D component not found on the controller.");
            }
            Debug.Log("Mimic is revealing itself.");
            controller.spriteRenderer.sprite = controller.OriginalSprite;
            controller.transform.localScale = new Vector3(3, 3, 1);
            reachedDestination = false;

            // Find the player reference
            playerTransform = GameObject.FindWithTag("Player").transform;

            // Trigger ApplyHit on the player
            OnHitEvents playerHitEvents = playerTransform.GetComponent<OnHitEvents>();
            if (playerHitEvents != null)
            {
                playerHitEvents.ApplyHit(stunDuration: 0f, damage: 1f, structuralDamaged: 0f);
                Debug.Log("Applied hit to the player.");
            }
            else
            {
                Debug.LogWarning("Player does not have OnHitEvents component.");
            }

            // Find a safe spot and assign currentNavPoint
            FindSafeSpot();

            if (currentNavPoint.HasValue)
            {
                // Move to the initial valid safe spot
                SetDestination();
                StartCoroutine(CheckIfSafe());
            }
            else
            {
                Debug.LogWarning("No valid nav point found, retrying safe spot search.");
                FindSafeSpot(); // Retry if no valid spot was found
            }
        }
        else
        {
            Debug.LogWarning("MimicController is null.");
        }
    }

    private void SetDestination()
    {
        // Check if the path to the destination is valid before setting it
        NavMeshPath path = new NavMeshPath();
        if (controller.Agent.CalculatePath(currentNavPoint.Value, path) && path.status == NavMeshPathStatus.PathComplete)
        {
            controller.Agent.SetDestination(currentNavPoint.Value);
            Debug.Log($"Moving to: {currentNavPoint.Value}");
        }
        else
        {
            Debug.LogWarning("Calculated path is invalid, retrying safe spot search.");
            FindSafeSpot(); // Retry finding a valid position if the path is invalid
        }
    }

    private void FindSafeSpot()
    {
        Vector3 playerPosition = playerTransform.position;
        Vector3 mimicPosition = controller.transform.position;

        // Calculate a movement direction directly away from the player
        Vector3 escapeDirection = (mimicPosition - playerPosition).normalized;
        
        float bestDistance = 0f;
        Vector3 bestPosition = mimicPosition;

        for (int i = 0; i < 10; i++) // Try up to 10 different potential spots
        {
            Vector3 testDirection = Quaternion.Euler(0, Random.Range(-60, 60), 0) * escapeDirection; // Slight angle variance
            Vector3 potentialPosition = mimicPosition + (testDirection * Random.Range(safeDistance, maxDistance));

            // Ensure the position is valid on the NavMesh
            if (NavMesh.SamplePosition(potentialPosition, out NavMeshHit hit, 30f, NavMesh.AllAreas))
            {
                float distanceFromPlayer = Vector3.Distance(hit.position, playerPosition);
                
                // Choose the farthest valid point from the player
                if (distanceFromPlayer > bestDistance)
                {
                    bestDistance = distanceFromPlayer;
                    bestPosition = hit.position;
                }
            }
        }

        if (bestDistance > 0)
        {
            currentNavPoint = bestPosition;
            Debug.Log($"Found safe spot at {currentNavPoint.Value}");
        }
        else
        {
            Debug.LogWarning("No valid escape spot found. Choosing a fallback location.");
            currentNavPoint = mimicPosition + (escapeDirection * safeDistance); // Last resort: move a fixed distance directly away
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
            float distanceToPlayer = Vector3.Distance(playerTransform.position, controller.transform.position);

            // If the player is far enough away, switch to idle
            if (distanceToPlayer > playerDistanceThreshold)
            {
                // Mimic is far enough from the player, safe to idle
                SwitchToIdle();
                yield break;
            }

            // If the player is close to the destination, recheck the safe spot
            if (Vector3.Distance(playerTransform.position, currentNavPoint.Value) < playerDistanceThreshold)
            {
                // Prevent constant re-checking by adding a buffer zone before retrying
                if (Time.time > nextSafeSpotCheckTime) 
                {
                    Debug.Log("Player is too close to the destination, finding new safe spot.");
                    FindSafeSpot(); // Recalculate safe spot
                    SetDestination(); // Set new destination
                    nextSafeSpotCheckTime = Time.time + 2f; // Delay next check by 2 seconds to avoid ping-ponging
                }
            }

            yield return new WaitForSeconds(1f); // Check every second
        }
    }

private float nextSafeSpotCheckTime = 0f; // Time when the next safe spot check can happen

    private void SwitchToIdle()
    {
        Debug.Log("Mimic is safe and player is far. Switching to idle.");
        controller.SwitchState<MimicIdleState>();
    }
}
