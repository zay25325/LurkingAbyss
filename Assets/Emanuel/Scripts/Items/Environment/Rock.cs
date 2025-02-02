/*
Class Name: Rock
Description: This class is a child of the Item class and represents a rock item in the game. 
             It contains the properties and methods specific to the rock item.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : Item
{
    public float throwForce = 10f;  // Force to throw the rock
    public Sprite rockIcon = null;  // Icon for the rock
    private GameObject rockPrefab = null;    // Prefab for the rock

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
            Throw();
            ReduceItemCharge();
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
    private void Throw()
    {
        // Implement the logic to throw the rock
        Debug.Log("Throwing the rock with force: " + throwForce);
    }
}