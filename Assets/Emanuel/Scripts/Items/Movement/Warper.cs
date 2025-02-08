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
    public override void Use()
    {
        if(CanUseItem())
        {
            Teleport();
            ReduceItemCharge();
            DestroyItem(ItemObject);
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

        // Set the player's position to the clamped target position
        playerTransform.position = clampedTargetPosition;

        Debug.Log("Player teleported to: " + clampedTargetPosition);
    }
}
