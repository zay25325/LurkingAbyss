using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class HealthBarController : MonoBehaviour
{
    [SerializeField] public PlayerStats playerstats;
    public Slider intSlider;
    [SerializeField] public Image ECGImage;

    private Image fillImage;
    //private Color normalHealth = new(47f, 243f, 237f);
    private Color normalHealth = new(47f / 255f, 243f / 255f, 237f / 255f);
    private Color lowHealth = Color.red;

    
    private const float lowHealthThreshold = 0.26f;


    public void Start()
    {
        playerstats.OnShieldsChanged.AddListener(SetHealthBar);
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

    public void AdjustHealthBar(float amount)
    {
        intSlider.value += amount;
        UpdateHealthBarColor(intSlider.value);
    }

    // attach this to an event when the player's health is changed
    public void SetHealthBar(float shieldamount)
    {
        {
            intSlider.value = (shieldamount/playerstats.MaxShields);
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
}
