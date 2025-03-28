/*
Class Name: Warper
Description: This class is a child of the Item class and represents a warper item in the game. 
             It contains the properties and methods specific to the warper item. 
             Such as teleporting the player up to a max distance based on mouse position(where the player is facing).
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Warper : Item
{
    public Sprite warperIcon = null;  // Icon for the warper
    private GameObject warperPrefab = null;    // Prefab for the

    private float maxTeleportDistance = 5f;   // Maximum distance the player can teleport

    private bool didTeleport = false; // Flag to check if the player has teleported


    /*
        FUNCTION : Awake()
        DESCRIPTION : This function is called when the script instance is being loaded. 
                      It initializes the warper item properties.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void Awake()
    {
        // Set the prefab reference here
        warperPrefab = this.gameObject;
        // Initialize the item properties manually

        // Set the properties of the warper item
        ItemName = "Warper";
        ItemDescription = "Can be placed to teleport the player to a random location. ";
        ItemIcon = warperIcon; 
        ItemID = 0;
        maxItemCharge = 3;
        ItemCharge = 3;
        ItemRarity = Rarity.Anomalies;
        ItemValue = 0;
        CanItemDestroy = false;
        ItemSubtype = Subtype.Movement;
        ItemObject = warperPrefab;
    }

    /*
        FUNCTION : Use()
        DESCRIPTION : This function is called when the warper item is used. 
                      It implements the functionality to teleport the player.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    public override void Use(EntityInfo entityInfo)
    {
        if(CanUseItem())
        {
            if(entityInfo.Tags.Contains(EntityInfo.EntityTags.Player))
            {
                Teleport();
                if(didTeleport)
                {
                    ReduceItemCharge();
                    didTeleport = false;
                    DestroyItem(ItemObject);
                }
            }
            else if (entityInfo.Tags.Contains(EntityInfo.EntityTags.Scavenger))
            {
                ScavengerTeleport();
                if(didTeleport)
                {
                    ReduceItemCharge();
                    didTeleport = false;
                    DestroyItem(ItemObject);
                }
            }
            // Teleport();
            // if(didTeleport)
            // {
            //     ReduceItemCharge();
            //     didTeleport = false;
            //     DestroyItem(ItemObject);
            // }
        }
        else
        {
            Debug.Log("Not enough charges to use " + ItemName);
        }
    }

    /*
        FUNCTION : Teleport()
        DESCRIPTION : This function teleports the player to the target position based on the mouse position. Has a max teleport distance.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void Teleport()
    {
        // Get the main camera
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            return;
        }

        // Get the mouse position in screen space
        Vector3 mousePosition = Mouse.current.position.ReadValue();

        // Convert the mouse position to world space
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0; // Ensure Z is 0 for 2D

        // Get the player's current position
        Transform playerTransform = GameObject.FindWithTag("Player").transform;
        Vector3 playerPosition = playerTransform.position;

        // Calculate the direction and distance to the target position
        Vector3 direction = (worldPosition - playerPosition).normalized;
        float distance = Vector3.Distance(playerPosition, worldPosition);

        // Clamp the distance to the maximum teleport distance
        float clampedDistance = Mathf.Min(distance, maxTeleportDistance);

        // Calculate the clamped target position
        Vector3 clampedTargetPosition = playerPosition + direction * clampedDistance;

        // Use NavMesh to find the closest valid position
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(clampedTargetPosition, out hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
        {
            // Perform an overlap circle check to ensure no walls or colliders at the new position
            Collider2D[] colliders = Physics2D.OverlapCircleAll(hit.position, 0.5f); // Adjust the radius as needed

            bool isCollision = false;
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Walls")) // Check if the collider is a wall
                {
                    isCollision = true;
                    break;
                }
            }

            if (isCollision)
            {
                Debug.Log("Collision detected at target position. Finding nearest valid position.");
                // Find the nearest valid position within the NavMesh
                if (UnityEngine.AI.NavMesh.SamplePosition(hit.position, out UnityEngine.AI.NavMeshHit nearestHit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    playerTransform.position = nearestHit.position;
                    Debug.Log("Player teleported to nearest valid position: " + nearestHit.position);
                    didTeleport = true;
                }
                else
                {
                    Debug.LogError("No valid position found near the target position!");
                    didTeleport = false;
                }
            }
            else
            {
                // Set the player's position to the closest valid position on the NavMesh
                playerTransform.position = hit.position;
                Debug.Log("Player teleported to: " + hit.position);
                didTeleport = true;
            }
        }
        else
        {
            Debug.LogWarning("No valid position found on the NavMesh!");
            didTeleport = false;
        }
    }

    private void ScavengerTeleport()
    {
        // Get the scavenger's current position
        Transform scavengerTransform = GameObject.FindWithTag("Scavenger").transform;
        Vector3 scavengerPosition = scavengerTransform.position;

        // Generate a random angle behind the scavenger (180 to 360 degrees)
        float randomAngle = Random.Range(180f, 360f);
        Vector3 direction = new Vector3(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad), 0).normalized;

        // Calculate a random distance within the max teleport distance
        float randomDistance = Random.Range(1f, maxTeleportDistance);

        // Calculate the target position
        Vector3 targetPosition = scavengerPosition + direction * randomDistance;

        // Use NavMesh to find the closest valid position
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(targetPosition, out hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
        {
            // Perform an overlap circle check to ensure no walls or colliders at the new position
            Collider2D[] colliders = Physics2D.OverlapCircleAll(hit.position, 0.5f); // Adjust the radius as needed

            bool isCollision = false;
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Walls")) // Check if the collider is a wall
                {
                    isCollision = true;
                    break;
                }
            }

            if (isCollision)
            {
                Debug.Log("Collision detected at target position. Finding nearest valid position.");
                // Find the nearest valid position within the NavMesh
                if (UnityEngine.AI.NavMesh.SamplePosition(hit.position, out UnityEngine.AI.NavMeshHit nearestHit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    scavengerTransform.position = nearestHit.position;
                    Debug.Log("Scavenger teleported to nearest valid position: " + nearestHit.position);
                    didTeleport = true;
                }
                else
                {
                    Debug.LogError("No valid position found near the target position!");
                    didTeleport = false;
                }
            }
            else
            {
                // Set the scavenger's position to the closest valid position on the NavMesh
                scavengerTransform.position = hit.position;
                Debug.Log("Scavenger teleported to: " + hit.position);
                didTeleport = true;
            }
        }
        else
        {
            Debug.LogWarning("No valid position found on the NavMesh!");
            didTeleport = false;
        }
    }
}
