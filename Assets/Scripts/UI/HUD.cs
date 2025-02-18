using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    public Slider intSlider;

    public void SetMaxHealth(int health)
    {
        intSlider.maxValue = health;
        intSlider.value = health;
    }

    public void SetHealth(int health)
    {
        intSlider.value = health;
    }
}
