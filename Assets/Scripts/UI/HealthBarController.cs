using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class HealthBarController : MonoBehaviour
{

    public Slider intSlider;
    [SerializeField] public Image ECGImage;
    private List<Item> items;
    private Item currentItem;

    private Image fillImage;
    //private Color normalHealth = new(47f, 243f, 237f);
    private Color normalHealth = new(47f / 255f, 243f / 255f, 237f / 255f);
    private Color lowHealth = Color.red;

    
    private const float lowHealthThreshold = 0.26f;


    public void Start()
    {
        items = new List<Item>();
        intSlider.value = intSlider.maxValue;

        if (intSlider == null)
        {
            intSlider = GetComponentInChildren<Slider>();
            if (intSlider == null)
            {
                Debug.LogError("HUD: intSlider is null and couldn't be found");
            }
        }
        intSlider.value = intSlider.maxValue;

        // fill image component
        fillImage = intSlider.fillRect.GetComponent<Image>();
        if (fillImage != null)
        {
            UpdateHealthBarColor(intSlider.value);
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

    public void AdjustHealthBar(float amount)
    {
        if (intSlider.value > 5)
        {
            intSlider.value += amount; //specific amount to show player health sectors properly
            UpdateHealthBarColor(intSlider.value);
        }
    }

    public void SetHealthBar(float shieldamount)
    {
        {
            intSlider.value = (shieldamount); // (+1 is the red square I guess)
            UpdateHealthBarColor(intSlider.value);
        }
    }

    public void UpdateHealthBarColor(float health)
    {
        if (intSlider.value < lowHealthThreshold)
        {
            fillImage.color = lowHealth;
        } else {
            fillImage.color = normalHealth;
        }

        if(intSlider.value <= 0) {
            ECGImage.color = lowHealth;
        } else {
            ECGImage.color = normalHealth;
        }
    }

    //public void InsertItem(Item item)
    //{
    //    try
    //    {
    //        items.Add(item);
    //    }
    //    catch (Exception error)
    //    {
    //        Debug.Log("Error inserting items into item bar inventory | Unity Error: " + error.Message);
    //    }
    //}

    //public void DisplayItems()
    //{
    //    try
    //    {
    //        foreach (Item item in items)
    //        {

    //        }
    //    }
    //    catch (Exception error)
    //    {
    //        Debug.Log("Error adding items into display bar | Unity Error: " + error.Message);
    //    }
    //}
}
