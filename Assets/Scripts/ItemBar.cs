using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemBar : MonoBehaviour
{

    private VisualElement root;
    private VisualElement[] itemSlots;

    private void OnEnable()
    {
        // get ui doc
        root = GetComponent<UIDocument>().rootVisualElement;
        itemSlots = new VisualElement[3]; // where items are stored

        // from ui builder
        VisualElement itemBar = new VisualElement();
        itemBar.AddToClassList("item-bar");

        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i] = CreateEmptySlot();
            itemBar.AddToClassList(itemSlots[i].ToString());
        }

        root.Add(itemBar);
    }

    private VisualElement CreateEmptySlot()
    {
        VisualElement itemSlot = new VisualElement();
        itemSlot.AddToClassList("item-slot");
        return itemSlot;
    }


    // called when we want to add an item to the item bar, specifically when the player picks up the item we call this
    public bool AddItemToSlot(int slotIndex, Image itemImage, string itemName, int itemAmount)
    {


        throw new NotImplementedException();
        //return true;
    }
}
