/*
Class Name: Item
Description: This abstract class is the base class for all items in the game. 
             It contains the properties and methods that all items should have.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    // Properties of the item
    public string ItemName { get; set; }
    public string ItemDescription { get; set; }
    public Sprite ItemIcon { get; set; }
    public int ItemID { get; set; }
    public int maxItemCharge { get; set; }
    public int ItemCharge { get; set; }
    public Rarity ItemRarity { get; set; }

    public Subtype ItemSubtype { get; set; }
    public int ItemValue { get; set; }
    public bool CanItemDestroy { get; set; }
    public GameObject ItemObject { get; set; }  
    public bool isDiscovered { get; set; }

    public bool IsInUse { get; set; } = false;

    // Constructor for the item
    public Item(string name, string description, Sprite icon, int id, int maxCharge, int charge, Rarity rarity, int value, bool destroyable, Subtype subtype, GameObject itemObject)
    {
        ItemName = name;
        ItemDescription = description;
        ItemIcon = icon;
        ItemID = id;
        maxItemCharge = maxCharge;
        ItemCharge = charge;
        ItemRarity = rarity;
        ItemValue = value;
        CanItemDestroy = destroyable;
        ItemSubtype = subtype;
        ItemObject = itemObject;
    }

    // Default constructor
    public Item()
    {
        ItemName = null;
        ItemDescription = null;
        ItemIcon = null;
        ItemID = 0;
        maxItemCharge = 1;
        ItemCharge = 1;
        ItemRarity = Rarity.Common;
        ItemValue = 0;
        CanItemDestroy = true;
        ItemSubtype = Subtype.Combat;
        ItemObject = null;
    }

    // This method will be overridden by the child classes
    
    /*
        FUNCTION : Use()
        DESCRIPTION : This function is called when the item is used. 
                      It contains the functionality specific to each item. However, 
                        the base class implementation is empty and should be overridden by the child classes.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    public abstract void Use();

    /*
        FUNCTION : CanDestroy()
        DESCRIPTION : This function checks if the item can be destroyed. 
                      It returns true if the item can be destroyed, false otherwise.
        PARAMETERS : NONE
        RETURNS : CanItemDestroy - A boolean value indicating if the item can be destroyed
    */
    public bool CanDestroy()
    {
        return CanItemDestroy;
    }

    /*
        FUNCTION : DestroyItem()
        DESCRIPTION : This function destroys the item object. 
                      It checks if the item can be destroyed and if the item charge is zero. 
                      If both conditions are met, the item object is destroyed.
        PARAMETERS : GameObject itemObject - The item object to be destroyed
        RETURNS : NONE
    */
    public void DestroyItem(GameObject itemObject)
    {
        if (CanItemDestroy && ItemCharge <= 0)
        {
            Object.Destroy(itemObject);
        }
    }

    /*
        FUNCTION : CanUseItem()
        DESCRIPTION : This function checks if the item can be used. 
                      It returns true if the item charge is greater than zero, false otherwise.
        PARAMETERS : NONE
        RETURNS : ItemCharge - A boolean value indicating if the item can be used
    */
    public bool CanUseItem()
    {
        return ItemCharge > 0;
    }

    /*
        FUNCTION : ReduceItemCharge()
        DESCRIPTION : This function reduces the charge of the item by one.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    public void ReduceItemCharge()
    {
        ItemCharge--;
    }

    /*
        FUNCTION : DebuggingChildInfo()
        DESCRIPTION : This function is for debugging purposes for the child classes. 
                      It prints the information of the child class item.
        PARAMETERS : GameObject itemObject - The item object to debug
        RETURNS : NONE
    */
    public void DebuggingChildInfo(GameObject itemObject)
    {
        // Get the subclass of the item
        var subclass = itemObject.GetComponent<Item>();
        
        // get the properties of the subclass
        if (subclass != null)
        {
            ItemName = subclass.ItemName;
            ItemDescription = subclass.ItemDescription;
            ItemIcon = subclass.ItemIcon;
            ItemID = subclass.ItemID;
            ItemCharge = subclass.ItemCharge;
            ItemRarity = subclass.ItemRarity;
            ItemValue = subclass.ItemValue;
            CanItemDestroy = subclass.CanItemDestroy;
            ItemSubtype = subclass.ItemSubtype;
            ItemObject = subclass.ItemObject;
        }

        // Print the information of the child class item
        Debug.Log("Item created: " + (string.IsNullOrEmpty(ItemName) ? "N/A" : ItemName));
        Debug.Log("Item description: " + (string.IsNullOrEmpty(ItemDescription) ? "N/A" : ItemDescription));
        Debug.Log("Item icon: " + ItemIcon);
        Debug.Log("Item ID: " + ItemID);
        Debug.Log("Item charge: " + ItemCharge);
        Debug.Log("Item rarity: " + ItemRarity);
        Debug.Log("Item value: " + ItemValue);
        Debug.Log("Item destroyable: " + CanItemDestroy);
        Debug.Log("Item subtype: " + ItemSubtype);
        Debug.Log("Item object: " + ItemObject);
    }

    public bool ItemDiscovered()
    {
        return isDiscovered;
    }

    public void DiscoverItem()
    {
        isDiscovered = true;
    }
}
