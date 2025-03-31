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


    // store the images
    UnityEngine.UI.Image[] images;
    public UnityEngine.UI.Image activeItemHighlight; // transparent square

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
        images = new UnityEngine.UI.Image[items.Count];

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

            // make sure no null images are inserted
            if (itemImage != null)
            {
                images[i] = itemImage; // store the image
                Debug.Log("Line 88: Added image to temp list");
            }
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

            // finally highlight the active item in hand
            HighlightActiveItem();
        }
    }

    private void HighlightActiveItem()
    {
        Debug.Log("Got here! line 124"); // testing
        activeItemHighlight.transform.SetAsLastSibling();

        if (activeItemHighlight == null || images == null || images.Length == 0)
        {
            Debug.Log("Error everyrthing is null or something");
            return;
        }

        int activeItemIndex = playerInventory.GetActiveItemIndex();
        Debug.Log($"Active item index: {activeItemIndex}");

        if (activeItemIndex >= 0 && activeItemIndex < images.Length)
        { // put the transparent square on top of the image
            //activeItemHighlight.transform.position = images[activeItemIndex].transform.position;

            activeItemHighlight.transform.SetParent(itemBarHUD.transform, false);

            // get rect transforms
            RectTransform highlightRect = activeItemHighlight.rectTransform;
            RectTransform itemRect = images[activeItemIndex].rectTransform;

            // match the image size and position in the hud bar
            highlightRect.sizeDelta = itemRect.sizeDelta;
            highlightRect.anchoredPosition = itemRect.anchoredPosition;



            // testig after commit above
        }
    }
}