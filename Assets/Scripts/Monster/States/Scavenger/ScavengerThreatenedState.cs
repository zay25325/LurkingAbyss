using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ScavengerThreatenedState : ScavengerBaseState
{
    public float safeDistance = 20f; // Distance to feel safe
    private Transform playerTarget;
    private ScavengerController scavengerController;
    private NavMeshAgent navMeshAgent;
    private float movementItemCooldown = 5f; // Cooldown duration in seconds
    private float lastMovementItemTime;

    new protected void OnEnable()
    {
        base.OnEnable();

        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        scavengerController = controller as ScavengerController;
        navMeshAgent = controller.GetComponent<NavMeshAgent>();

        // Ensure the NavMeshAgent is enabled
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = true;
        }

        lastMovementItemTime = -movementItemCooldown; // Initialize to allow immediate use of movement item
    }

    private void UseMovementItem()
    {
        List<Item> items = scavengerController.GetItems();
        List<Item> movementItems = new List<Item>();
        EntityInfo entityInfo = scavengerController.GetComponent<EntityInfo>();

        // Check for movement items in the inventory
        foreach (Item item in items)
        {
            if (item.ItemSubtype == Subtype.Movement)
            {
                movementItems.Add(item);
            }
        }

        // Determine which item to use
        if (movementItems.Count > 0)
        {
            Item selectedItem;
            if (movementItems.Count == 1)
            {
                selectedItem = movementItems[0];
            }
            else
            {
                selectedItem = movementItems[Random.Range(0, movementItems.Count)];
            }

            scavengerController.SetItem(selectedItem);

            // Use the active item
            Item activeItem = scavengerController.GetActiveItem();
            if (activeItem != null && entityInfo != null)
            {
                activeItem.Use(entityInfo);
                if (!activeItem.CanUseItem())
                {
                    scavengerController.RemoveItem(activeItem);
                }
            }
        }
    }

    private void RunAway()
    {
        if (navMeshAgent == null || !navMeshAgent.enabled)
        {
            Debug.LogWarning("NavMeshAgent is not enabled or not found.");
            return;
        }

        // Generate a direction away from the player
        Vector2 direction = (transform.position - playerTarget.position).normalized;
        Vector3 destination = controller.transform.position + new Vector3(direction.x, direction.y, 0) * safeDistance;

        // Attempt to find a valid position on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(destination, out hit, safeDistance, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(hit.position);
        }
        else
        {
            Debug.LogWarning("Failed to find a valid position on the NavMesh.");
        }
    }

    private void Update()
    {
        if (Time.time - lastMovementItemTime >= movementItemCooldown)
        {
            UseMovementItem();
            lastMovementItemTime = Time.time;
            RunAway();
        }

        // Check if the scavenger is at a safe distance from the player
        if (Vector2.Distance(transform.position, playerTarget.position) >= safeDistance)
        {
            controller.SwitchState<ScavengerScavengeState>();
        }
    }
}