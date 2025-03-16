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

        if (itemPrefab == null)
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
        if (playerInventory.GetItems().Count > 0)
        {
            foreach (Transform child in itemBarHUD.transform)
            Destroy(child.gameObject);
        }

        List<Item> items = playerInventory.GetItems();

        foreach(Item item in items) // debug
            Debug.Log($"Item in inventory: {item}");

        if (items.Count == 3)
            Debug.Log("All slots used");

        for (int i = 0; i < items.Count; i++)
        {
            //GameObject slot = Instantiate(items[i].gameObject, itemBarHUD.transform);

            GameObject slot = new GameObject(items[i].name + "_Slot", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
            slot.transform.SetParent(itemBarHUD.transform);
            RectTransform rectTransform = slot.GetComponent<RectTransform>();
                    rectTransform.localScale = Vector3.one;
                    rectTransform.sizeDelta = new Vector2(100, 100); // Adjust size as needed

            UnityEngine.UI.Image itemImage = slot.GetComponent<UnityEngine.UI.Image>();
            Debug.Log("Item Image: " + itemImage);
            Debug.Log("About to add images to HUD");
            SpriteRenderer itemSpriteRenderer = null;

            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name == items[i].name)
                {
                    itemSpriteRenderer = obj.GetComponent<SpriteRenderer>();
                    if (itemSpriteRenderer != null)
                    {
                        Debug.Log("Found item in scene: " + items[i].name);
                        break;
                    }
                }
            }
            Debug.Log("Item Sprite Renderer: " + itemSpriteRenderer);
            if (itemSpriteRenderer != null && itemSpriteRenderer.sprite != null)
            {
                itemImage.sprite = itemSpriteRenderer.sprite;
                Debug.Log($"Added sprite image for item: {items[i].ItemName}");
            }
            else
                Debug.Log($"item: {items[i].name} does not have a sprite attached");
        
        }
    }
}
