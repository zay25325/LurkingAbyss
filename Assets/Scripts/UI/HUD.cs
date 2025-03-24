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

    private Image fillImage;
    //private Color normalHealth = new(47f, 243f, 237f);
    private Color normalHealth = new(47f / 255f, 243f / 255f, 237f / 255f);
    private Color lowHealth = Color.red;
    private const float lowHealthThreshold = 8.1f;


    public void Start()
    {
        items = new List<Item>();
        intSlider.value = 17.4f;

        if (intSlider == null)
        {
            intSlider = GetComponentInChildren<Slider>();
            if (intSlider == null)
            {
                Debug.LogError("HUD: intSlider is null and couldn't be found");
            }
        }
        intSlider.value = 17.4f;

        // fill image component
        fillImage = intSlider.fillRect.GetComponent<Image>();
        if (fillImage != null)
        {
            fillImage.color = normalHealth;
        }
        else
        {
            Debug.LogError("HUD: fill image not found");
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
        if (intSlider.value > 5)
        {
            switch (isHealing)
            {
                case true:
                    intSlider.value += health;
                    UpdateHealthBarColor(intSlider.value); // change color
                    break;
                case false:
                    intSlider.value -= health;
                    UpdateHealthBarColor(intSlider.value);
                    break;
            }
        }
    }

    public void UpdateHealthBarColor(float health)
    {
        if (intSlider.value < 8.1f)
        {
            fillImage.color = lowHealth;
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
