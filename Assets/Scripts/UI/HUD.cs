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
        //itemIndex = 0;
    }

    public void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    SetHealth(3.1f);
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        //{

        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        //{

        //}
        //if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        //{

        //}
    }

    //private void SelectItem(int itemIndex)
    //{
    //    if (itemIndex >=0)
    //    {

    //    }
    //}

    public void SetHealth(float health)
    {
        if (intSlider.value != 5)
        {
            intSlider.value -= health;
            // change color here somehow
        }
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    SetHealth(3.1f);
        //}
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
