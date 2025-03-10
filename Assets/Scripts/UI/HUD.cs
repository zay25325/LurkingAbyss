using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class HUD : MonoBehaviour
{

    public Slider intSlider;
    private List<Item> items;
    private Item currentItem;


    // item hud hotbar 
    //[SerializeField] private RectTransform[] itemSlots;
    //[SerializeField] private RectTransform selectionIndicator;
    //private int itemIndex;

    public void Start()
    {
        items = new List<Item>();
        intSlider.value = 17.4f;

        if (intSlider == null)
        {
            intSlider = GetComponentInChildren<Slider>();
        }

        if (intSlider != null)
            Debug.LogError("HUD: intSlider is NULL, is it assigned?");
        else 
        {
            intSlider.value = 17.4f;
            Debug.LogError("HUD: intSlider is not assigned");
        }
    }

    public void Update() { }

    //private void SelectItem(int itemIndex)
    //{
    //    if (itemIndex >=0)
    //    {

    //    }
    //}

    public void AdjustHealthBar(bool isHealing, float health)
    {
        
        switch (isHealing)
        {
            case true:
                if (intSlider.value != 5)
                {
                    intSlider.value += health;
                    // change color here somehow
                }
                break;
            case false:
                if (intSlider.value != 5)
                {
                    intSlider.value -= health;
                    // change color here somehow
                }
                break;
        }
    }

    public void InsertItem(Item item)
    {
        try
        {
            items.Add(item);
        }
        catch (Exception error)
        {
            Debug.Log("Error inserting items into item bar inventory | Unity Error: " + error.Message);
        }
    }

    public void DisplayItems()
    {
        try
        {
            foreach (Item item in items)
            {

            }
        }
        catch (Exception error)
        {
            Debug.Log("Error adding items into display bar | Unity Error: " + error.Message);
        }
    }
}
