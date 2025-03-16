using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ItemBar : MonoBehaviour
{

    private Inventory playerInventory; // temp stash for items so we can view them
    public HorizontalLayoutGroup itemBarHUD; // for displayting the images of the childs
    public GameObject itemPrefab;

    private void Start()
    {
        itemBarHUD = GetComponent<HorizontalLayoutGroup>();
        if (itemBarHUD == null)
            Debug.LogError("ItemBarHUD not found!");


        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
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

        if (itemPrefab != null)
        {
            Debug.LogError("ItemPrefab is not assigned in Itembar");
        }
    }

    private void UpdateInventoryUI()
    {
        //Debug.Log("UpdateInventoryUI called!"); // debugging
        if (itemBarHUD == null)
        {
            Debug.LogError("ItemBarHUD is not assigned!");
            return;
        }

        // delete prev items/images
        foreach (Transform child in itemBarHUD.transform)
            Destroy(child.gameObject);

        List<Item> items = playerInventory.GetItems();

        foreach(Item item in items) // debug
            Debug.Log($"Item in inventory: {item}");

        if (items.Count == 3)
            Debug.Log("All slots used");

        for (int i = 0; i < 3; i++)
        {
            GameObject slot = Instantiate(itemPrefab, itemBarHUD.transform);

            //Image itemImage = slot.GetComponent<Image>(); // 'Image' keyword wasnt being recognized
            UnityEngine.UI.Image itemImage = slot.GetComponentInChildren<UnityEngine.UI.Image>();
            Debug.Log("About to add images to HUD");
            if (i < items.Count)
            {
                if (items[i].ItemIcon != null)
                {
                    itemImage.sprite = items[i].ItemIcon;
                    Debug.Log($"Added sprite image for item: {items[i].ItemName}");
                }
                else
                    Debug.Log($"item: {items[i].name} does not have a sprite attached");
            }
            else
                Debug.Log("Could not find items");
        }
    }
}
