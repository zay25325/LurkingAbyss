using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ScavengerScavengeState : ScavengerBaseState
{
    private float pickupRange = 1f;  // Range at which items can be picked up
    private float wanderRadius = 10f; // Radius to wander
    private LayerMask itemLayer;
    private LayerMask monsterLayer;
    private LayerMask playerLayer;
    private NavMeshAgent navMeshAgent;
    private bool isAngry = false; // Flag to track if the scavenger is angry

    private float wanderCooldown = 1f;  // Time between each wander attempt
    private float wanderTime = 0f;      // Timer to track wander cooldown

    private Item targetItem = null; // The current item the scavenger is moving towards
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private float stuckThreshold = 2f; // Time before considering it stuck
    private Vector3 targetPosition; // Target destination for wandering

    new protected void OnEnable()
    {
        base.OnEnable();
        itemLayer = LayerMask.GetMask("Item");
        monsterLayer = LayerMask.GetMask("Entities");
        playerLayer = LayerMask.GetMask("Player");
        navMeshAgent = controller.GetComponent<NavMeshAgent>();

        // Ensure the NavMeshAgent is enabled
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = true;
        }

        // Add listener for OnHarmedWithInfo event
        GetComponent<OnHitEvents>().OnHarmedWithInfo.AddListener(OnHarmedWithInfo);
    }

    void Update()
    {
        // Continue wandering if not currently targeting an item
        if (targetItem == null)
        {
            Wander();
        }
        else
        {
            // Check if the scavenger is close enough to the item to pick it up
            if (Vector3.Distance(controller.transform.position, targetItem.transform.position) <= pickupRange)
            {
                PickUpItem(targetItem);
            }
        }
    }

    private void Wander()
    {
        if (wanderTime > 0f)
        {
            wanderTime -= Time.deltaTime;
            return;
        }

        if (navMeshAgent == null || !navMeshAgent.enabled)
        {
            Debug.LogWarning("NavMeshAgent is not enabled or not found.");
            return;
        }

        // Check if the scavenger is really close to the target position
        float targetDistance = Vector3.Distance(controller.transform.position, targetPosition);  // Use targetPosition here

        // If the scavenger is within a small threshold (close to target but stuck)
        if (targetDistance < 1f)  // You can adjust this threshold as needed
        {
            // The scavenger is close to its target but unable to progress (stuck or blocked)
            Debug.Log("Scavenger is close to target but stuck. Finding new path.");
            ForceNewPath();  // Force a new destination if stuck
            return;  // Skip further processing
        }

        // Check if the scavenger is touching a wall or vision blocker
        RaycastHit hit;
        if (Physics.Raycast(controller.transform.position, controller.transform.forward, out hit, 0.5f, LayerMask.GetMask("VisionBlockers")) && hit.collider.CompareTag("Walls"))
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckThreshold)
            {
                Debug.LogWarning("Scavenger is stuck against a wall or vision blocker! Finding new path.");
                ForceNewPath();
                stuckTimer = 0f;  // Reset stuck timer
            }
        }
        else
        {
            stuckTimer = 0f;  // Reset stuck timer if not touching a wall or vision blocker
        }

        lastPosition = controller.transform.position;  // Update last position

        // Check if the scavenger is near its destination and stuck
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude < 0.1f)
            {
                ForceNewPath();  // Force new path if it's stuck at the destination
            }
        }

        // Perform forward raycast to avoid walls blocking the path
        RaycastHit hitCheck;
        if (Physics.Raycast(controller.transform.position, controller.transform.forward, out hitCheck, 0.1f, LayerMask.GetMask("VisionBlockers")) && hit.collider.CompareTag("Walls"))
        {
            ForceNewPath();  // Force new path if there's an obstacle ahead
        }
    }

    private void ForceNewPath()
    {
        int attempts = 0;
        float searchRadius = 50f; // Start with a smaller radius
        bool foundValidPosition = false;
        Vector3 destination = Vector3.zero;
        HashSet<Vector3> attemptedPositions = new HashSet<Vector3>(); // Track attempted positions

        while (!foundValidPosition)
        {
            // Generate a random direction and distance
            float randomAngle = Random.Range(0f, 360f); // Random angle in degrees
            float randomDistance = Random.Range(searchRadius / 2f, searchRadius); // Random distance within the radius
            Vector3 randomDirection = Quaternion.Euler(0, randomAngle, 0) * Vector3.forward * randomDistance;

            // Calculate the destination
            destination = controller.transform.position + randomDirection;

            // Avoid reusing previously attempted positions or positions too close to the current position
            if (attemptedPositions.Contains(destination) || Vector3.Distance(controller.transform.position, destination) < 5f)
            {
                attempts++;
                continue;
            }
            attemptedPositions.Add(destination);

            // Check if the position is valid
            if (NavMesh.SamplePosition(destination, out NavMeshHit hit, searchRadius, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                navMeshAgent.CalculatePath(hit.position, path);

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    foundValidPosition = true;
                    navMeshAgent.SetDestination(hit.position);
                    targetPosition = hit.position; // Set the targetPosition here
                    Debug.Log($"New valid wander path found at {hit.position}");
                    break;
                }
            }

            attempts++;
            if (attempts >= 5)
            {
                // Expand the search radius and reset attempts
                searchRadius += 50f;
                attempts = 0;
                Debug.LogWarning($"Expanding search radius to {searchRadius}.");
            }
        }

        wanderTime = wanderCooldown; // Reset cooldown
    }

    private void MoveToItem(Item item)
    {
        if (navMeshAgent == null || !navMeshAgent.enabled)
        {
            Debug.LogWarning("NavMeshAgent is not enabled or not found.");
            return;
        }

        targetItem = item;

        // Set a destination to move towards the item
        navMeshAgent.SetDestination(item.transform.position);
    }

    private void PickUpItem(Item item)
    {
        // Pick up the item if close enough
        controller.AddItem(item);
        targetItem = null; // Reset the target item
    }

    protected void OnHarmedWithInfo (EntityInfo attackerInfo)
    {
            if (attackerInfo != null && attackerInfo.CompareTag("Player"))
            {
                Debug.Log($"Entity was hit by {attackerInfo.name}");
                isAngry = true;
            }
    }

    public override void OnSeeingEntityEnter(Collider2D collider)
    {
        EntityInfo info = collider.GetComponent<EntityInfo>();
        if (info != null)
        {
            if (info.Tags.Contains(EntityInfo.EntityTags.Player))
            {
                Debug.Log("Scavenger sees Player");

                if (isAngry)
                {
                    controller.SwitchState<ScavengerAngeredState>();
                }
            }
            else if (info.Tags.Contains(EntityInfo.EntityTags.Item))
            {
                Debug.Log("Scavenger sees an item");
                Item item = collider.GetComponent<Item>();
                if (item != null)
                {
                    MoveToItem(item);
                }
            }
            else if (info.Tags.Contains(EntityInfo.EntityTags.Hunter))
            {
                Debug.Log("Scavenger sees a Hunter");
                controller.SwitchState<ScavengerThreatenedState>();
            }
        }
    }

    public override void OnSeeingEntityExit(Collider2D collider)
    {
        EntityInfo info = collider.GetComponent<EntityInfo>();
        if (info != null)
        {
            if (info.Tags.Contains(EntityInfo.EntityTags.Player))
            {
                Debug.Log("Scavenger no longer sees entity");  
            }

            else if (info.Tags.Contains(EntityInfo.EntityTags.Item))
            {
                Debug.Log("Scavenger picked up an item");
            }
        }
    }
}