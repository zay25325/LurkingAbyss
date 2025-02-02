using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGun : Item
{
    public Sprite gunIcon = null;  // Icon for the gun
    private GameObject gunPrefab = null;    // Prefab for the
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
        ItemCharge = 1;
        ItemRarity = Rarity.Common;
        ItemValue = 0;
        CanItemDestroy = false;
        ItemSubtype = Subtype.Combat;
        ItemObject = gunPrefab;

    }

    public override void Use()
    {
        // Implement gun-specific functionality
        Debug.Log("Gun used: " + ItemName);
        //Shoot();
    }

    private void Shoot()
    {
        // Implement shooting functionality
        Debug.Log("Shooting with: " + ItemName);
    }


}
