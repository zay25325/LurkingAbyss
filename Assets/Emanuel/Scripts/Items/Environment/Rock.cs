/*
Class Name: Rock
Description: This class is a child of the Item class and represents a rock item in the game. 
             It contains the properties and methods specific to the rock item.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EntityInfo;

public class Rock : Item
{
    public float throwForce = 10f;  // Force to throw the rock
    public Sprite rockIcon = null;  // Icon for the rock
    private GameObject rockPrefab = null;    // Prefab for the rock

    private ProjectileSpawner rockSpawner;

    private int noiseLevel = 10;

    /*
        FUNCTION : Awake()
        DESCRIPTION : This function is called when the script instance is being loaded. 
                      It initializes the rock item properties.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void Awake()
    {
        // Set the prefab reference here
        rockPrefab = this.gameObject;
        // Initialize the item properties manually

        // Set the properties of the rock item
        ItemName = "Default Rock";
        ItemDescription = "Can be thrown, making noise on-hit or end of flight. Deals minor damage. ";
        ItemIcon = rockIcon; 
        ItemID = 0;
        maxItemCharge = 1;
        ItemCharge = 1;
        ItemRarity = Rarity.Common;
        ItemValue = 0;
        CanItemDestroy = true;
        ItemSubtype = Subtype.Environment;
        ItemObject = rockPrefab;

            // Find the spawner in the scene (Make sure there is one)
        rockSpawner = FindObjectOfType<ProjectileSpawner>(); 
        if (rockSpawner == null)
        {
            Debug.LogError("rockSpawner not found in the scene! Ensure it's attached to a GameObject.");
        }
    }

    /*
        FUNCTION : Use()
        DESCRIPTION : This function is called when the rock item is used. 
                      It implements the functionality to throw the rock.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    public override void Use()
    {
        if (CanUseItem())
        {
            Throw();
        }
        else
        {
            Debug.Log("Not enough charges to use " + ItemName);
        }
    }

    /*
        FUNCTION : Throw()
        DESCRIPTION : This function is responsible for throwing the rock. 
                      It calculates the force required to throw the rock and throws it.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void Throw()
    {
        StartCoroutine(WaitForProjectileToLand());
    }

    private IEnumerator WaitForProjectileToLand()
    {
        // Get target position (mouse position converted to world space)
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0; // Ensure it's in 2D space

        // Get the player's current position
        Vector3 playerPosition = FindObjectOfType<PlayerController>().transform.position;

        // Fire a rock from the player's position towards the target
        rockSpawner.FireProjectile(playerPosition, targetPosition);

        Projectile rockProj = rockSpawner.projScript;

        // Wait until the projectile has landed
        while (!rockProj.HasLanded())
        {
            yield return null; // Wait for the next frame
        }

        // Continue with the rest of the logic after the projectile has landed
        GenerateNoise(targetPosition);

        Debug.Log("Shooting with: " + ItemName);
    }

    private void GenerateNoise(Vector3 position)
    {
        NoiseDetectionManager.Instance.NoiseEvent.Invoke(position, noiseLevel, new List<EntityTags>());

        ReduceItemCharge();
        DestroyItem(ItemObject);

        // Generate noise at the given position
        Debug.Log("Noise generated at position: " + position);
    }
}