using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ScavengerThreatenedState : ScavengerBaseState
{
    public float safeDistance = 20f; // Distance to feel safe
    private ScavengerController scavengerController;
    private NavMeshAgent navMeshAgent;
    private float movementItemCooldown = 5f; // Cooldown duration in seconds
    private float lastMovementItemTime;

    private float environmentItemCooldown = 5f; // Cooldown duration in seconds
    private float lastEnvironmentItemTime;

    new protected void OnEnable()
    {
        base.OnEnable();

        scavengerController = controller as ScavengerController;
        navMeshAgent = controller.GetComponent<NavMeshAgent>();

        // Ensure the NavMeshAgent is enabled
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = true;
        }

        lastMovementItemTime = -movementItemCooldown; // Initialize to allow immediate use of movement item
        lastEnvironmentItemTime = -environmentItemCooldown; // Initialize to allow immediate use of environment item
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
                // Ensure the GameObject is active before using it
                if (activeItem.ItemObject != null)
                {
                    // Hide the sprite or visual representation
                    SpriteRenderer spriteRenderer = activeItem.ItemObject.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.enabled = false; // Disable the sprite
                    }

                    activeItem.ItemObject.SetActive(true); // Activate the GameObject
                }

                activeItem.Use(entityInfo);

                // After using the item, deactivate it again
                if (activeItem.ItemObject != null)
                {
                    if (activeItem is InvisibleBelt)
                    {
                        if (!activeItem.IsInUse)
                        {
                            activeItem.ItemObject.SetActive(false); // Deactivate the GameObject
                        }
                    }
                    else
                    {
                        activeItem.ItemObject.SetActive(false); // Deactivate the GameObject
                    }
                }

                // If the item cannot be used anymore, handle it
                if (!activeItem.CanUseItem())
                {
                    Item battery = items.Find(item => item is Battery);
                    if (battery != null)
                    {
                        scavengerController.SetItem(battery);
                        Item activeBattery = scavengerController.GetActiveItem();
                        if (activeBattery != null && entityInfo != null)
                        {
                            activeBattery.Use(entityInfo);
                        }
                    }
                    else
                    {
                        // Drop the item if no battery is available
                        activeItem.ItemObject.transform.position = transform.position; // Place at scavenger's position
                        activeItem.ItemObject.SetActive(true); // Activate the item in the game world
                        scavengerController.RemoveItem(activeItem);
                    }
                }
            }
        }
    }

    private void UseEnvironmentItem()
    {
        List<Item> items = scavengerController.GetItems();
        EntityInfo entityInfo = scavengerController.GetComponent<EntityInfo>();
        List<Item> environmentItems = new List<Item>();

        // Check for environment items in the inventory
        foreach (Item item in items)
        {
            if (item.ItemSubtype == Subtype.Environment)
            {
                environmentItems.Add(item);
            }
        }

        // Determine which item to use
        if (environmentItems.Count > 0)
        {
            Item selectedItem;
            if (environmentItems.Count == 1)
            {
                selectedItem = environmentItems[0];
            }
            else
            {
                selectedItem = environmentItems[Random.Range(0, environmentItems.Count)];
            }

            scavengerController.SetItem(selectedItem);

            // Use the active item
            Item activeItem = scavengerController.GetActiveItem();
            if (activeItem != null && entityInfo != null)
            {
                // Ensure the GameObject is active before using it
                if (activeItem.ItemObject != null)
                {
                    // Hide the sprite or visual representation
                    SpriteRenderer spriteRenderer = activeItem.ItemObject.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.enabled = false; // Disable the sprite
                    }

                    activeItem.ItemObject.SetActive(true); // Activate the GameObject
                }

                activeItem.Use(entityInfo);

                // After using the item, deactivate it again
                if (activeItem is Rock)
                {
                    activeItem.ItemObject.SetActive(false); // Deactivate the GameObject
                }

                // If the item cannot be used anymore, handle it
                if (!activeItem.CanUseItem())
                {
                    Item battery = items.Find(item => item is Battery);
                    if (battery != null)
                    {
                        scavengerController.SetItem(battery);
                        Item activeBattery = scavengerController.GetActiveItem();
                        if (activeBattery != null && entityInfo != null)
                        {
                            activeBattery.Use(entityInfo);
                        }
                    }
                    else
                    {
                        // Drop the item if no battery is available
                        activeItem.ItemObject.transform.position = transform.position; // Place at scavenger's position
                        activeItem.ItemObject.SetActive(true); // Activate the item in the game world
                        scavengerController.RemoveItem(activeItem);
                    }
                }
            }
        }
    }    

private void RunAway(Vector3 targetLocation)
{
    if (navMeshAgent == null || !navMeshAgent.enabled)
    {
        Debug.LogWarning("NavMeshAgent is not enabled or not found.");
        return;
    }

    // Dynamically find a random position away from the player or threat
    Vector3 safeLocation = FindSafeLocation(targetLocation);
    
    // Attempt to find a valid position on the NavMesh near the calculated safe location
    NavMeshHit hit;
    if (NavMesh.SamplePosition(safeLocation, out hit, safeDistance, NavMesh.AllAreas))
    {
        navMeshAgent.SetDestination(hit.position);
    }
    else
    {
        Debug.LogWarning("Failed to find a valid position on the NavMesh.");
    }
}

    private Vector3 FindSafeLocation(Vector3 currentPosition)
    {
        // You can customize this logic based on your level layout and threats
        // Randomize a location around the current position but within a safe range
        float randomDistance = Random.Range(10f, safeDistance);  // You can adjust the range based on the scenario
        Vector2 randomDirection = Random.insideUnitCircle.normalized * randomDistance;

        // Calculate a new destination that avoids the player or threat
        Vector3 safeLocation = new Vector3(currentPosition.x + randomDirection.x, currentPosition.y, currentPosition.z + randomDirection.y);

        return safeLocation;
    }

    private void Update()
    {
        if(Time.time - lastEnvironmentItemTime >= environmentItemCooldown)
        {
            UseEnvironmentItem();
            lastEnvironmentItemTime = Time.time;
        }

        if (Time.time - lastMovementItemTime >= movementItemCooldown)
        {
            UseMovementItem();
            lastMovementItemTime = Time.time;

            // Update the target location dynamically by calling RunAway with the safe location
            Vector3 targetLocation = FindSafeLocation(controller.transform.position); // Calculate new target location
            RunAway(targetLocation); // Move to the safe location
        }

        // Check if the scavenger has reached the target location (within safe distance)
        if (Vector3.Distance(controller.transform.position, navMeshAgent.destination) < safeDistance)
        {
            controller.SwitchState<ScavengerScavengeState>();
        }
    }
}