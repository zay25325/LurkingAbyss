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

    public GameObject rockprojectilePrefab;

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
            StartCoroutine(Throw());
            //ReduceItemCharge();
            DestroyItem(ItemObject);
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
    private IEnumerator Throw()
    {
        if (Camera.main == null) yield break;

        // Get mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Ensure the rock stays in 2D space

        // Get the player position
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("Player not found!");
            yield break;
        }

        // Spawn the rock projectile at the player's position
        GameObject rock = Instantiate(rockprojectilePrefab, player.transform.position, Quaternion.identity);

        // Get the direction from the player's position to the mouse position
        Vector3 direction = (mousePosition - player.transform.position).normalized;

        // Set the target for the ProjectileController to handle the movement
        ProjectileController projectileController = rock.GetComponent<ProjectileController>();
        if (projectileController != null)
        {
            projectileController.Target = new Vector2(mousePosition.x, mousePosition.y); // Set target to the mouse position
        }
        else
        {
            Debug.LogError("Rock is missing a ProjectileController!");
        }

        // Set projectile movement with velocity
        Rigidbody2D rb = rock.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float rockSpeed = projectileController.Speed; // Adjust as needed
            rb.velocity = direction * rockSpeed;  // Apply velocity in the desired direction
        }
        else
        {
            Debug.LogError("Rock projectile is missing a Rigidbody2D!");
            yield break;
        }

        // Ignore collision with the player
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        Collider2D projectileCollider = rock.GetComponent<Collider2D>();
        if (playerCollider != null && projectileCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider, projectileCollider);
            Debug.Log("Ignoring collision between player and projectile.");
        }

        // Set rock projectile layer and collision rules
        int projectileLayer = LayerMask.NameToLayer("Projectiles");
        rock.layer = projectileLayer;

        int playerLayer = LayerMask.NameToLayer("Player");
        int entitiesLayer = LayerMask.NameToLayer("Entities");
        int visionBlockersLayer = LayerMask.NameToLayer("VisionBlockers");
        int itemLayer = LayerMask.NameToLayer("Item");

        Physics2D.IgnoreLayerCollision(projectileLayer, playerLayer, false);
        Physics2D.IgnoreLayerCollision(projectileLayer, entitiesLayer, false);
        Physics2D.IgnoreLayerCollision(projectileLayer, visionBlockersLayer, false);
        Physics2D.IgnoreLayerCollision(projectileLayer, itemLayer, true);

        // Store the initial position of the rock in 2D space
        Vector2 initialPosition = rock.transform.position;
        // Wait until the rock is destroyed
        while (rock != null && rock.activeInHierarchy)
        {
            initialPosition = rock.transform.position;
            yield return null; // Wait for the next frame before checking again
        }

        Debug.Log("Rock Position: " + initialPosition);
        // Call NoiseDetectionManager before the rock gets destroyed
        NoiseDetectionManager.Instance.NoiseEvent.Invoke(
            initialPosition, noiseLevel, GetComponent<EntityInfo>().Tags
        );
    }
}