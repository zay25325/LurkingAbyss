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

    public HUD()
    {
        items = new List<Item>();
        intSlider.value = 17.4f;
    }

    public void Start()
    {
        intSlider.value = 17.4f;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetHealth(3.1f);
        }
    }

    public void SetHealth(float health)
    {
        if (intSlider.value != 5)
        {
            intSlider.value -= health;
            // change color here somehow
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
