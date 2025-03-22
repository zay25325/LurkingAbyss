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
        // Only attempt to wander after the cooldown period
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

        // Check if the scavenger is stuck (not moving for too long)
        if (Vector3.Distance(controller.transform.position, lastPosition) < 0.1f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckThreshold)
            {
                Debug.LogWarning("Scavenger is stuck! Finding a new path.");
                ForceNewPath();
                stuckTimer = 0f; // Reset stuck timer after attempting to fix
            }
        }
        else
        {
            stuckTimer = 0f; // Reset timer if it's moving
        }

        lastPosition = controller.transform.position; // Update last known position

        // Check if the scavenger has reached its destination or is truly stuck
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude < 0.1f) // If not moving
            {
                ForceNewPath();
            }
        }
    }

    private void ForceNewPath()
    {
        int attempts = 0;
        bool foundValidPosition = false;
        Vector3 destination = Vector3.zero;

        while (attempts < 5) // Limit to avoid infinite loops
        {
            // Generate a new random direction away from the last stuck position
            Vector2 randomDirection = Random.insideUnitCircle.normalized * wanderRadius;
            destination = controller.transform.position + new Vector3(randomDirection.x, 0, randomDirection.y);

            // Check if the position is valid on the NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(destination, out hit, wanderRadius, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                navMeshAgent.CalculatePath(hit.position, path);

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    foundValidPosition = true;
                    navMeshAgent.SetDestination(hit.position);
                    Debug.Log($"New valid path found at {hit.position}");
                    break;
                }
            }

            attempts++;
        }

        if (!foundValidPosition)
        {
            Debug.LogError("Scavenger couldn't find a valid position. Attempting emergency unstuck.");
            controller.transform.position += new Vector3(1f, 0, 1f); // Move slightly to force unstuck
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
            else if (info.Tags.Contains(EntityInfo.EntityTags.Wanderer) || info.Tags.Contains(EntityInfo.EntityTags.Territorial) || info.Tags.Contains(EntityInfo.EntityTags.Hunter))
            {
                Debug.Log("Scavenger sees a monster");
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