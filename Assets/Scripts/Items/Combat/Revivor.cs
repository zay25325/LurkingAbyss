/*
Class Name: Revivor
Description: This class is a child of the Item class and represents a revivor item in the game. 
             It contains the properties and methods specific to the revivor item. Such as reviving a downed player.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revivor : Item
{
    public Sprite revivorIcon = null;  // Icon for the revivor
    private GameObject revivorPrefab = null;    // Prefab for the revivor

    private PlayerStats playerHealthStat = null;

    private void Awake()
    {
        // Set the prefab reference here
        revivorPrefab = this.gameObject;
        // Initialize the item properties manually

        // Set the properties of the revivor item
        ItemName = "Revivor";
        ItemDescription = "Revives a downed player";
        ItemIcon = null;
        ItemID = 0;
        maxItemCharge = 1;
        ItemCharge = 1;
        ItemRarity = Rarity.Anomalies;
        ItemValue = 0;
        CanItemDestroy = true;
        ItemSubtype = Subtype.Combat;
        ItemObject = revivorPrefab;
    }

/*
    Function : Use()
    Description : This function is called when the revivor item is used. 
                  It implements the functionality to revive a downed player.
    Parameters : NONE
    Returns : NONE
*/
    public override void Use(EntityInfo entityInfo)
    {
        if (CanUseItem())
        {
            RevivePlayer();
        }
        else
        {
            Debug.Log("Not enough charges to use " + ItemName);
        }
    }

/*
    FUNCTION : RevivePlayer()
    DESCRIPTION : This function is called when the revivor item is used. 
                  It implements the functionality to revive a downed player.
    PARAMETERS : NONE
    RETURNS : NONE
*/
    private void RevivePlayer()
    {
        // Implement reviving functionality
        Debug.Log("Reviving player with: " + ItemName);

        // Get the player's combat script
        playerHealthStat = GameObject.FindObjectOfType<PlayerStats>();

        if (playerHealthStat != null && playerHealthStat.Health == 0)
        {
            playerHealthStat.Health = 1;
            Debug.Log("Player revived with 1 health.");
            ReduceItemCharge();
            DestroyItem(ItemObject);
        }
        else
        {
            Debug.Log("Player is not down or already has health.");
        }
    }
}
