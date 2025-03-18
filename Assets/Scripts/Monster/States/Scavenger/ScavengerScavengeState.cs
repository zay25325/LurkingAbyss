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

        // Check if the scavenger has reached its destination
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
            {
                // Generate a new random direction and set a new destination
                Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
                Vector3 destination = controller.transform.position + new Vector3(randomDirection.x, randomDirection.y, 0);

                // Attempt to find a valid position on the NavMesh
                NavMeshHit hit;
                int attempts = 0;

                while (attempts < 5) // Limit number of attempts to avoid infinite loops
                {
                    if (NavMesh.SamplePosition(destination, out hit, wanderRadius, NavMesh.AllAreas))
                    {
                        // Check if the position is colliding with any walls
                        Collider2D[] colliders = Physics2D.OverlapCircleAll(hit.position, 0.5f);
                        bool isCollidingWithWall = false;
                        foreach (Collider2D collider in colliders)
                        {
                            if (collider.CompareTag("Walls"))
                            {
                                isCollidingWithWall = true;
                                break;
                            }
                        }

                        if (!isCollidingWithWall)
                        {
                            // Set the destination if a valid position is found and not colliding with walls
                            navMeshAgent.SetDestination(hit.position);
                            wanderTime = wanderCooldown;  // Reset the cooldown
                            return;
                        }
                    }

                    // Otherwise, try another random direction
                    randomDirection = Random.insideUnitCircle * wanderRadius;
                    destination = controller.transform.position + new Vector3(randomDirection.x, randomDirection.y, 0);
                    attempts++;
                }

                // If no valid position is found after several attempts, wait before trying again
                wanderTime = wanderCooldown;
            }
        }
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
        item.ItemObject.SetActive(false);
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