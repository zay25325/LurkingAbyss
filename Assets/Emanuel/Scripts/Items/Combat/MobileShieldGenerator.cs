using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileShieldGenerator : Item
{
    private Sprite shieldGeneratorIcon = null;  // Icon for the shield generator
    private GameObject shieldGeneratorPrefab = null;    // Prefab for the shield generator

    private PlayerStats playerStats = null;    // Reference to the player stats

    private void Awake()
    {
        // Set the prefab reference here
        shieldGeneratorPrefab = this.gameObject;
        // Initialize the item properties manually

        // Set the properties of the shield generator item
        ItemName = "Mobile Shield Generator";
        ItemDescription = "Can be placed to generate a shield that absorbs damage. ";
        ItemIcon = shieldGeneratorIcon; 
        ItemID = 0;
        maxItemCharge = 3;
        ItemCharge = 3;
        ItemRarity = Rarity.Common;
        ItemValue = 0;
        CanItemDestroy = false;
        ItemSubtype = Subtype.Combat;
        ItemObject = shieldGeneratorPrefab;
    }

    public override void Use()
    {
        if(CanUseItem())
        {
            increaseShieldCharge();
            Debug.Log("shield generated. 2");
        }

        else
        {
            Debug.Log("Not enough charges to use " + ItemName);
        }
    }

    private void increaseShieldCharge()
    {
        // Increase the charge of player shield
        playerStats = GameObject.FindObjectOfType<PlayerStats>();
        if (playerStats != null && playerStats.Shields < playerStats.MaxShields)
        {
            playerStats.Shields += 1;
            ReduceItemCharge();
            DestroyItem(ItemObject);
            Debug.Log("Shield generated.");
        }
        else
        {
            Debug.Log("Cannot use item.");
        }
    }
}
