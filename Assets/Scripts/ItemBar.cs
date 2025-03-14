using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ItemBar : MonoBehaviour
{

    private HorizontalLayoutGroup itemBarHUD; // for displayting the images of the childs
    private Inventory playerInventory; // temp stash for items so we can view them
    

    private void Start()
    {
        itemBarHUD = GetComponent<HorizontalLayoutGroup>();
        if (itemBarHUD == null)
            Debug.LogError("ItemBarHUD not found!");


        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInventory = player.GetComponent<Inventory>();
            if (playerInventory != null)
            {
                playerInventory.OnInventoryChanged += UpdateInventoryUI;
                UpdateInventoryUI();
            }
            else
                Debug.Log("Player inventory component wasnt found");
        }
        else
            Debug.Log("Player inventory component wasnt found");
    }

    private void UpdateInventoryUI()
    {
        //Debug.Log("UpdateInventoryUI called!"); // debugging

        if (itemBarHUD == null)
        {
            Debug.LogError("ItemBarHUD is not assigned!");
            return;
        }

        // delete prev items
        foreach (Transform child in itemBarHUD.transform)
            Destroy(child.gameObject);

        List<Item> items = playerInventory.GetItems();

        foreach(Item item in items) // debug
        {
            Debug.Log($"Item in inventory: {item}");
        }

        // somehow get the sprite image

        //throw new NotImplementedException();
    }
}
