/*
Class Name: Inventory
Description: This class is responsible for managing the player's inventory. 
             It contains the properties and methods to add, remove, and select items in the inventory.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    private List<Item> items = new List<Item>(); // List to store all items
    private const int maxItems = 3; // Maximum number of items
    private Item activeItem; // Currently active item
    private int currentActiveIndex = 0;

    /*
        FUNCTION : AddItem
        DESCRIPTION : This function is responsible for adding an item to the player's inventory. 
                      It checks if the inventory is full and if the item is tagged as an item. 
                      If the inventory is not full and the item is tagged as an item, the item is added to the inventory and deactivated in the scene.
        PARAMETERS : gameobject item - The item to add to the inventory
        RETURNS : bool - Returns true if the item was added successfully, false otherwise
    */
    public bool AddItem(GameObject itemObject)
    {
        Item item = itemObject.GetComponent<Item>();
        Debug.Log("Item Script: " + itemObject.GetComponent<Item>());

        item.DebuggingChildInfo(itemObject);

        if (item != null)
        {
            if (items.Count < maxItems)
            {
                Debug.Log("Item added to inventory");
                items.Add(item);
                itemObject.SetActive(false); // Deactivate the item so it is removed from the scene

                // Set the first instance of the new item to be the active item
                if (items.Count == 1)
                {
                    ActiveItem(0);
                }

                return true; // Item added successfully
            }
            else
            {
                // Drop the currently held item
                DropActiveItem(new InputAction.CallbackContext());

                // Add the new item to the inventory
                items.Add(item);
                itemObject.SetActive(false); // Deactivate the item so it is removed from the scene

                // Set the new item as the active item
                ActiveItem(items.Count - 1);

                return true; // Item added successfully after dropping the held item
            }
        }
        Debug.Log("Item not added to inventory");
        return false; // Inventory full or item does not have an Item component
    }


    /*
        FUNCTION : RemoveItem
        DESCRIPTION : This function is responsible for removing an item from the player's inventory and placing it in the game world. 
                      It checks if the item is in the inventory and if it is, the item is removed from the inventory and placed in front of the player.
        PARAMETERS : gameobject item - The item to remove from the inventory
        RETURNS : bool - Returns true if the item was removed successfully, false otherwise
    */
    public bool RemoveItem(Item item)
    {
        if (items.Contains(item))
        {
            // Place the item in front of the player
            items.Remove(item);
            if (item.ItemObject != null)
            {
                item.ItemObject.SetActive(true); // Activate the item so it is added back to the scene
            }
            // Update the active item if necessary
            if (activeItem == item)
            {
                activeItem = null;
                if (items.Count > 0)
                {
                    ActiveItem(0);
                }
            }
            return true;
        }
        return false; // Item not found in inventory
    }

    /*
        FUNCTION : DropActiveItem
        DESCRIPTION : This function is responsible for dropping the active item from the player's inventory. 
                      It checks if the inventory is not empty and if it is not, the active item is dropped in front of the player.
        PARAMETERS : InputAction.CallbackContext context - Input context for the drop action.
        RETURNS : NONE
    */
public void DropActiveItem(InputAction.CallbackContext context)
{
    if (activeItem != null)
    {
        // Get the player's facing direction from rotation
        Vector2 dropDirection = (Vector2)(Quaternion.Euler(0, 0, transform.eulerAngles.z) * Vector2.right);

        // Set the drop position in front of the player
        Vector3 dropPosition = transform.position + (Vector3)dropDirection;

        // Move and activate the item
        activeItem.ItemObject.transform.position = dropPosition;
        activeItem.ItemObject.SetActive(true);

        // Remove the item from inventory
        RemoveItem(activeItem);
        activeItem = null;

        // Set next active item if available
        if (items.Count > 0)
        {
            currentActiveIndex = 0;
            ActiveItem(currentActiveIndex);
        }
    }
}



    /*
        FUNCTION : ActiveItem
        DESCRIPTION : This function is responsible for activating the selected item. 
                      It checks if the item is in the inventory and if it is, the item is activated.
        PARAMETERS : int index - Index of the item in the inventory
        RETURNS : NONE
    */
    private void ActiveItem(int index)
    {
        // Check if the index is within the inventory range
        if (index >= 0 && index < items.Count)
        {
            // // Check if the active item is not null
            // if (activeItem != null)
            // {
            //     // Deactivate the current active item
            //     activeItem.SetActive(false);
            // }

            // Activate the new item
            activeItem = items[index];
            currentActiveIndex = index;
            checkActiveItemState();
        }
    }
    
    /*
        FUNCTION : checkActiveItemState
        DESCRIPTION : Just for debugging purposes to check if the active item is active or not
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void checkActiveItemState()
    {
        if (activeItem != null)
        {
            Debug.Log($"Active item is: {activeItem.ItemName}");
        }
    }

    // /*
    //     FUNCTION : GetItems
    //     DESCRIPTION : This function is responsible for returning the player's inventory. 
    //                   It returns the list of items in the player's inventory.
    //     PARAMETERS : NONE
    //     RETURNS : items - List of items in the player's inventory
    // */
    public List<Item> GetItems()
    {
        return items;
    }

    /*
        FUNCTION : Scrolling
        DESCRIPTION : This function is responsible for handling player scrolling through the inventory. 
                      It checks if the inventory is not empty and if it is not, the player can 
                      scroll through the inventory.
        PARAMETERS : InputAction.CallbackContext context - Input context for the scroll action.
        RETURNS :  NONE
    */
    public void Scrolling(InputAction.CallbackContext context)
    {
        // Get the scroll value
        float scrollValue = context.ReadValue<float>();

        // Check if the inventory is not empty
        if (items.Count > 0)
        {
            // Scroll through the inventory. Check if the scroll value is positive or negative

            // If positive, increment the current active index
            if (scrollValue > 0)
            {
                currentActiveIndex = (currentActiveIndex + 1) % items.Count;
            }

            // If negative, decrement the current active index
            else if (scrollValue < 0)
            {
                currentActiveIndex = (currentActiveIndex - 1 + items.Count) % items.Count;
            }

            // Set the active item based on the current active index
            ActiveItem(currentActiveIndex);

            Debug.Log($"Current active index: {currentActiveIndex}");
            Debug.Log($"Current active item: {activeItem.ItemName}");
        }
    }

    /*
        FUNCTION : Slot1Pressed
        DESCRIPTION : This function is responsible for handling player selecting items based on the slot. 
                      It checks if the player has an item in the first slot and if it does, the item is selected.
        PARAMETERS : InputAction.CallbackContext context - Input context for the slot 1 action.
        RETURNS : NONE
    */
    public void Slot1Pressed(InputAction.CallbackContext context)
    {
        // set the active item to the first item in the inventory
        currentActiveIndex = 0; // Slot 1
        ActiveItem(currentActiveIndex);
    }

    /*
        FUNCTION : Slot2Pressed
        DESCRIPTION : This function is responsible for handling player selecting items based on the slot. 
                      It checks if the player has an item in the second slot and if it does, the item is selected.
        PARAMETERS : InputAction.CallbackContext context - Input context for the slot 2 action.
        RETURNS : NONE
    */
    public void Slot2Pressed(InputAction.CallbackContext context)
    {
        // set the active item to the second item in the inventory
        currentActiveIndex = 1; // Slot 2
        ActiveItem(currentActiveIndex);
    }

    /*
        FUNCTION : Slot3Pressed
        DESCRIPTION : This function is responsible for handling player selecting items based on the slot. 
                      It checks if the player has an item in the third slot and if it does, the item is selected.
        PARAMETERS : InputAction.CallbackContext context - Input context for the slot 3 action.
        RETURNS : NONE
    */
    public void Slot3Pressed(InputAction.CallbackContext context)
    {
        // set the active item to the third item in the inventory
        currentActiveIndex = 2; // Slot 3
        ActiveItem(currentActiveIndex);
    }
}
