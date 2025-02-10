using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    public Slider intSlider;

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

    private void SetHealth(float health)
    {
        intSlider.value -= health;
    }


}
