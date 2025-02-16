using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Item
{
    public Sprite grenadeIcon = null;  // Icon for the grenade
    private GameObject grenadePrefab = null;    // Prefab for the grenade

    private ProjectileSpawner grenadeSpawner;

    private void Awake()
    {
        // Set the prefab reference here
        grenadePrefab = this.gameObject;
        // Initialize the item properties manually

        // Set the properties of the grenade item
        ItemName = "Grenade";
        ItemDescription = "Explodes on impact, dealing damage to nearby enemies.";
        ItemIcon = grenadeIcon;
        ItemID = 0;
        maxItemCharge = 3;
        ItemCharge = 3;
        ItemRarity = Rarity.Common;
        ItemValue = 0;
        CanItemDestroy = false;
        ItemSubtype = Subtype.Combat;
        ItemObject = grenadePrefab;

        // Find the spawner in the scene (Make sure there is one)
        grenadeSpawner = FindObjectOfType<ProjectileSpawner>();
        if (grenadeSpawner == null)
        {
            Debug.LogError("ProjectileSpawner not found in the scene! Ensure it's attached to a GameObject.");
        }
    }

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
        grenadeSpawner.FireProjectile(playerPosition, targetPosition);

        Projectile rockProj = grenadeSpawner.projScript;

        // Wait until the projectile has landed
        while (!rockProj.HasLanded())
        {
            yield return null; // Wait for the next frame
        }

        // Continue with the rest of the logic after the projectile has landed
        Explosion(targetPosition);

        Debug.Log("Shooting with: " + ItemName);
    }

    private void Explosion(Vector3 targetPosition)
    {
        //COMMENTING OUT THIS FOR NOW UNTIL I UNDERSTAND MROE ABOUT THE TAG SYSTEM WE ARE IMPLMENTING

        // Collider2D[] colliders = Physics2D.OverlapCircleAll(targetPosition, 5f);

        // foreach (Collider2D collider in colliders)
        // {
        //     if (collider.CompareTag("Enemy"))
        //     {
        //         collider.GetComponent<Enemy>().TakeDamage(10);
        //     }
        // }

        Debug.Log("BIG BOOM Explosion at: " + targetPosition);

        ReduceItemCharge();
        DestroyItem(ItemObject);
    }
}
