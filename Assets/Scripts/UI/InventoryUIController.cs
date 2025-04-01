using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] TMP_Text itemText;
    [SerializeField] List<ItemSlotController> itemSlots;

    // Start is called before the first frame update
    void Start()
    {
        inventory.UpdateInventoryUI.AddListener(UpdateItems);
    }

    void UpdateItems(int activeIndex, List<Item> items)
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            // active index
            itemSlots[i].SelectedIndicator.gameObject.SetActive(i == activeIndex);

            // item sprites
            if (i < items.Count)
            {
                if (items[i] != null)
                {
                    itemText.text = items[i].gameObject.name;

                    float maxSize = 75;

                    itemSlots[i].ItemImage.sprite = items[i].GetComponent<SpriteRenderer>().sprite;
                    itemSlots[i].ItemImage.SetNativeSize();


                    Vector2 size = itemSlots[i].ItemImage.rectTransform.sizeDelta;
                    float iconScale = maxSize / Mathf.Max(size.x, size.y);

                    itemSlots[i].ItemImage.rectTransform.localScale = new Vector3(iconScale, iconScale);
                    itemSlots[i].ItemImage.gameObject.SetActive(true);

                    // Change color based on charge
                    if (items[i].ItemCharge == 0)
                    {
                        itemSlots[i].ItemImage.color = Color.blue; // Set to blue if charge is 0
                    }
                    else
                    {
                        itemSlots[i].ItemImage.color = items[i].GetComponent<SpriteRenderer>().color; // Default color
                    }
                }
                else
                {
                    itemSlots[i].ItemImage.gameObject.SetActive(false);
                }
            }
            else
            {
                itemSlots[i].ItemImage.gameObject.SetActive(false);
            }
        }

        // active item text
        if (activeIndex >= 0 && activeIndex < items.Count)
        {
            if (items[activeIndex] != null)
            {
                itemText.text = items[activeIndex].ItemName;
            }
            else
            {
                itemText.text = "";
            }
        }
        else
        {
            itemText.text = "";
        }
    }
}
