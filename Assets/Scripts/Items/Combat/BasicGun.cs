using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGun : Item
{
    public Sprite gunIcon = null;  // Icon for the gun
    private GameObject gunPrefab = null;    // Prefab for the

    private BulletSpawner bulletSpawner;
    private void Awake()
    {
        // Set the prefab reference here
        gunPrefab = this.gameObject;
        // Initialize the item properties manually

        // Set the properties of the gun item
        ItemName = "Default Gun";
        ItemDescription = "Harms monsters at a distance, makes a lot of noise";
        ItemIcon = gunIcon;
        ItemID = 0;
        maxItemCharge = 3;
        ItemCharge = 3;
        ItemRarity = Rarity.Common;
        ItemValue = 0;
        CanItemDestroy = false;
        ItemSubtype = Subtype.Combat;
        ItemObject = gunPrefab;

        // Find the spawner in the scene (Make sure there is one)
        bulletSpawner = FindObjectOfType<BulletSpawner>(); 
        if (bulletSpawner == null)
        {
            Debug.LogError("BulletSpawner not found in the scene! Ensure it's attached to a GameObject.");
        }

    }

    public override void Use()
    {
        if (CanUseItem())
        {
            Shoot();
            ReduceItemCharge();
            DestroyItem(ItemObject);
        }
        else
        {
            Debug.Log("Not enough charges to use " + ItemName);
        }
    }

    private void Shoot()
    {
        if (bulletSpawner == null)
        {
            Debug.LogError("BulletSpawner is missing. Cannot shoot!");
            return;
        }

        // Get target position (mouse position converted to world space)
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0; // Ensure it's in 2D space

        // Get the player's current position
        Vector3 playerPosition = FindObjectOfType<PlayerController>().transform.position;

        // Fire a bullet from the player's position towards the target
        bulletSpawner.FireBullet(playerPosition, targetPosition);

        Debug.Log("Shooting with: " + ItemName);
    }


}
