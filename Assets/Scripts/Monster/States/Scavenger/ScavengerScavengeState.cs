using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ScavengerScavengeState : ScavengerBaseState
{
    private float searchRadius = 5f; // Radius to search for items
    private float pickupRange = 1f;  // Range at which items can be picked up
    private LayerMask itemLayer;
    private LayerMask monsterLayer;
    private LayerMask playerLayer;
    private NavMeshAgent navMeshAgent;
    private bool isAngry = false; // Flag to track if the scavenger is angry

    private float wanderCooldown = 2f;  // Time between each wander attempt
    private float wanderTime = 0f;      // Timer to track wander cooldown

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
    }

    void Update()
    {
        // Check if the scavenger is angry
        if (isAngry && IsPlayerNearby())
        {
            controller.SwitchState<ScavengerAngeredState>();
            return;
        }

        // Check for nearby monsters, switch to ThreatenedState if necessary
        if (IsThreatened())
        {
            controller.SwitchState<ScavengerThreatenedState>();
            return;
        }

        // Search for items
        SearchForItems();
    }

    private void SearchForItems()
    {
        Debug.Log("Searching for items");
        Collider2D[] foundItems = Physics2D.OverlapCircleAll(controller.transform.position, searchRadius, itemLayer);

        if (controller.GetItems().Count >= 3)
        {
            Wander();
            return;
        }
        else if (foundItems.Length > 0)
        {
            Item bestItem = FindBestItem(foundItems);
            if (bestItem != null)
            {
                MoveToItem(bestItem);
            }
        }
        else
        {
            Wander();
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

        // Generate a random direction
        Vector2 randomDirection = Random.insideUnitCircle * searchRadius;
        Vector3 destination = controller.transform.position + new Vector3(randomDirection.x, randomDirection.y, 0);

        // Attempt to find a valid position on the NavMesh
        NavMeshHit hit;
        int attempts = 0;

        while (attempts < 5) // Limit number of attempts to avoid infinite loops
        {
            if (NavMesh.SamplePosition(destination, out hit, searchRadius, NavMesh.AllAreas))
            {
                // Set the destination if a valid position is found
                navMeshAgent.SetDestination(hit.position);
                wanderTime = wanderCooldown;  // Reset the cooldown
                return;
            }

            // Otherwise, try another random direction
            randomDirection = Random.insideUnitCircle * searchRadius;
            destination = controller.transform.position + new Vector3(randomDirection.x, randomDirection.y, 0);
            attempts++;
        }

        // If no valid position is found after several attempts, wait before trying again
        wanderTime = wanderCooldown; 
    }

    private Item FindBestItem(Collider2D[] foundItems)
    {
        Item bestItem = null;

        // Check inventory for missing item types
        bool hasCombatItem = false, hasMovementItem = false, hasEnvironmentItem = false;
        foreach (Item item in controller.GetItems())
        {
            if (item.ItemSubtype == Subtype.Combat) hasCombatItem = true;
            if (item.ItemSubtype == Subtype.Movement) hasMovementItem = true;
            if (item.ItemSubtype == Subtype.Environment) hasEnvironmentItem = true;
        }

        // Prioritize missing item types
        foreach (Collider2D col in foundItems)
        {
            Item item = col.GetComponent<Item>();
            if (item == null) continue;

            if (!hasCombatItem && item.ItemSubtype == Subtype.Combat) return item;
            if (!hasMovementItem && item.ItemSubtype == Subtype.Movement) return item;
            if (!hasEnvironmentItem && item.ItemSubtype == Subtype.Environment) return item;

            bestItem = item; // Fallback if no priority match is found
        }
        
        return bestItem;
    }

    private void MoveToItem(Item item)
    {
        if (navMeshAgent == null || !navMeshAgent.enabled)
        {
            Debug.LogWarning("NavMeshAgent is not enabled or not found.");
            return;
        }

        if (navMeshAgent.pathPending) // If the agent is calculating a path
            return;

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) 
        {
            // If the agent is close enough to the item
            if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
            {
                // Pick up the item if close enough
                controller.AddItem(item);
                item.ItemObject.SetActive(false);
            }
        }

        // Set a destination to move towards the item
        navMeshAgent.SetDestination(item.transform.position);
    }

    private bool IsThreatened()
    {
        Collider2D[] nearbyMonsters = Physics2D.OverlapCircleAll(controller.transform.position, searchRadius, monsterLayer);
        foreach (Collider2D col in nearbyMonsters)
        {
            // Ignore the scavenger's own collider
            if (col.gameObject == controller.gameObject)
            {
                continue;
            }

            // Check if the nearby entity is a threat
            if (col.gameObject.layer == LayerMask.NameToLayer("Entities"))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsPlayerNearby()
    {
        Collider2D[] nearbyPlayers = Physics2D.OverlapCircleAll(controller.transform.position, searchRadius, playerLayer);
        return nearbyPlayers.Length > 0;
    }

    // Method to handle being hit by the player
    public void OnHitByPlayer()
    {
        isAngry = true;
    }
}