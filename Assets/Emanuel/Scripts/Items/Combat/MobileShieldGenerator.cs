/*
Class Name: MobileShieldGenerator
Description: This class is a child of the Item class and represents a mobile shield generator item in the game. 
              It contains the properties and methods specific to the mobile shield generator item. Such as increasing the charge 
              of the player's shield.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileShieldGenerator : Item
{
    private Sprite shieldGeneratorIcon = null;  // Icon for the shield generator
    private GameObject shieldGeneratorPrefab = null;    // Prefab for the shield generator

    private PlayerStats playerStats = null;    // Reference to the player stats

    /*
        FUNCTION : Awake()
        DESCRIPTION : This function is called when the script instance is being loaded. 
                      It initializes the shield generator item properties.
        PARAMETERS : NONE
        RETURNS : NONE
    */
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
        ItemRarity = Rarity.Anomalies;
        ItemValue = 0;
        CanItemDestroy = false;
        ItemSubtype = Subtype.Combat;
        ItemObject = shieldGeneratorPrefab;
    }

    /*
        FUNCTION : Use()
        DESCRIPTION : This function is called when the shield generator item is used. 
                      It implements the functionality to increase the charge of the player's shield.
        PARAMETERS : NONE
        RETURNS : NONE
    */
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

    /*
        FUNCTION : increaseShieldCharge()
        DESCRIPTION : This function increases the charge of the player's shield.
        PARAMETERS : NONE
        RETURNS : NONE
    */
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
