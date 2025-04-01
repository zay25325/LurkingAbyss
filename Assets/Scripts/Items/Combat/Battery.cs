/*
Class Name: Battery
Description: This class is a child of the Item class and represents a battery item in the game. 
             It contains the properties and methods specific to the battery item. Such as increasing the charge 
             of other items in the inventory.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    NOTES:
Currently have the battery recharge items within inventory (other than itself of course). Reason being is when
UI is implemented, the player will be able to see the items and charges of items within the inventory. if the player
has the battery as the active slot, i would then have the player select the item they want to recharge via hovering over the
other item and clicking on it. This would then call the IncreaseItemsCharge() function and recharge the item. Since
There is no functionality yet, this can be used as a tempholder.

*/

public class Battery : Item
{
    public Sprite batteryIcon = null;  // Icon for the battery
    private GameObject batteryPrefab = null;    // Prefab for the battery

    private ScavengerController scavengerItems = null;

    /*
        FUNCTION : Awake()
        DESCRIPTION : This function is called when the script instance is being loaded. 
                      It initializes the battery item properties.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void Awake()
    {
        batteryPrefab = this.gameObject;

        // Set the properties of the Battery item
        ItemName = "Battery";
        ItemDescription = "Deplete a charge to charge items in your inventory";
        ItemIcon = batteryIcon;
        ItemID = 0;
        ItemCharge = 1;
        ItemRarity = Rarity.Common;
        ItemValue = 0;
        CanItemDestroy = false;
        ItemSubtype = Subtype.Combat;
        ItemObject = batteryPrefab;
    }

    /*
        FUNCTION : Use()
        DESCRIPTION : This function is called when the battery item is used. 
                      It implements the functionality to increase the charge of other items in the inventory.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    public override void Use(EntityInfo entityInfo)
    {
        if (CanUseItem())
        {
            if (entityInfo.Tags.Contains(EntityInfo.EntityTags.Player))
            {
                IncreaseItemsCharge();
            }
            else if (entityInfo.Tags.Contains(EntityInfo.EntityTags.Scavenger))
            {
                ScavengerIncreaseItemsCharge();
            }
        }
        else
        {
            Debug.Log("Not enough charges to use " + ItemName);
        }
    }

    /*
        FUNCTION : IncreaseItemsCharge()
        DESCRIPTION : This function increases the charge of other items in the inventory.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    public void IncreaseItemsCharge()
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        if (inventory != null)
        {
            List<Item> items = inventory.GetItems();
            bool allItemsFull = true;

            // Check if all items are full
            foreach (Item item in items)
            {
                if (item != this && item.ItemCharge < item.maxItemCharge)
                {
                    allItemsFull = false;
                    break;
                }
            }

            // If not all items are full, increase the charge
            if (!allItemsFull)
            {
                foreach (Item item in items)
                {
                    // Increase the charge of all items except the battery
                    if (item != this && item.ItemCharge < item.maxItemCharge)
                    {
                        item.ItemCharge = item.maxItemCharge;
                    }
                }
                // Reduce the charge of the battery
                ReduceItemCharge();

                // Check to destroy the battery if the charge is zero and it is destroyable
                if (ItemCharge <= 0)
                {
                    DestroyItem(ItemObject);
                }
            }
            else
            {
                Debug.Log("All items are fully charged");
            }
        }
        else
        {
            Debug.Log("Inventory not found");
        }
    }

    public void ScavengerIncreaseItemsCharge()
    {
        scavengerItems = GameObject.FindObjectOfType<ScavengerController>();
        List<Item> items = scavengerItems.GetItems();
        bool allItemsFull = true;

        // Check if all items are full
        foreach (Item item in items)
        {
            if (item != this && item.ItemCharge < item.maxItemCharge)
            {
                allItemsFull = false;
                break;
            }
        }

        // If not all items are full, increase the charge
        if (!allItemsFull)
        {
            foreach (Item item in items)
            {
                // Increase the charge of all items except the battery
                if (item != this && item.ItemCharge < item.maxItemCharge)
                {
                    item.ItemCharge = item.maxItemCharge;
                }
            }
            // Reduce the charge of the battery
            ReduceItemCharge();

            // Check to destroy the battery if the charge is zero and it is destroyable
            if (ItemCharge <= 0)
            {
                DestroyItem(ItemObject);
            }
        }
        else
        {
            Debug.Log("All items are fully charged");
        }
    }  
}
