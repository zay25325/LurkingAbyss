/*
Class Name: Noise Maker
Description: This class is a component of the noise maker game object and represents the noise maker item in the game. 
             It contains the properties and methods specific to the noise maker item. Which can be placed and generates
                noise to distract enemies.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static EntityInfo;

public class NoiseMaker : Item
{
    public Sprite noiseMakerIcon = null;  // Icon for the noise maker
    private GameObject noiseMakerPrefab = null;    // Prefab for the noise maker

    private Inventory inventoryManager = null;    // Reference to the inventory manager

    private int noiseLevel = 20;

    /*
        FUNCTION : Awake()
        DESCRIPTION : This function is called when the script instance is being loaded. 
                      It initializes the noise maker item properties.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void Awake()
    {
        // Set the prefab reference here
        noiseMakerPrefab = this.gameObject;
        // Initialize the item properties manually

        // Set the properties of the noise maker item
        ItemName = "Noise Maker";
        ItemDescription = "Can be placed to generate noise and distract enemies. ";
        ItemIcon = noiseMakerIcon; 
        ItemID = 0;
        maxItemCharge = 1;
        ItemCharge = 1;
        ItemRarity = Rarity.Common;
        ItemValue = 0;
        CanItemDestroy = false;
        ItemSubtype = Subtype.Environment;
        ItemObject = noiseMakerPrefab;
    }

    /*
        FUNCTION : Use()
        DESCRIPTION : This function is called when the noise maker item is used. 
                      It implements the functionality to place the noise maker.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    public override void Use(EntityInfo entityInfo)
    {
        if (CanUseItem())
        {
            if(entityInfo.Tags.Contains(EntityTags.Player))
            {
                Place(entityInfo);
                ReduceItemCharge();
                DestroyItem(ItemObject);
            }
            else if(entityInfo.Tags.Contains(EntityTags.Scavenger))
            {
                ScavengerPlace(entityInfo);
                ReduceItemCharge();
                DestroyItem(ItemObject);
            }
            // Place();
            // ReduceItemCharge();
            // DestroyItem(ItemObject);
        }
        else
        {
            Debug.Log("Cannot use item.");
        }
    }

    /*
        FUNCTION : Place()
        DESCRIPTION : This function is called when the noise maker item is placed. 
                      It implements the functionality to generate noise.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void Place(EntityInfo entityInfo)
    {
        inventoryManager = FindObjectOfType<Inventory>();

        if (inventoryManager != null)
        {
            inventoryManager.DropActiveItem(new InputAction.CallbackContext());
            
            StartCoroutine(GenerateNoise(entityInfo));

            Debug.Log("Placing the noise maker.");
        }

    }

    private void ScavengerPlace(EntityInfo entityInfo)
    {
        ScavengerController scavengerController = GameObject.FindObjectOfType<ScavengerController>();
        List<Item> itemList = scavengerController.GetItems();

        if (scavengerController != null && itemList.Contains(this))
        {
            transform.position = scavengerController.transform.position;
            StartCoroutine(GenerateNoise(entityInfo));
            Debug.Log("Scavenger placed the noise maker.");
        }
    }

    /*
        FUNCTION : GenerateNoise()
        DESCRIPTION : This function generates noise over time to distract enemies.
        PARAMETERS : NONE
        RETURNS : IEnumerator
    */
    private IEnumerator GenerateNoise(EntityInfo entityInfo)
    {
        int noiseCounter = noiseLevel;
        while (noiseCounter > 0)
        {
            NoiseDetectionManager.Instance.NoiseEvent.Invoke(this.transform.position, noiseLevel, entityInfo.Tags);
            noiseCounter--;
            Debug.Log($"Noise Level: {noiseLevel}, Noise Counter: {noiseCounter}");
            yield return new WaitForSeconds(1f);
        }
    }
}
