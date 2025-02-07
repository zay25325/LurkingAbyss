using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    public Slider intSlider;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetHealth(3);
        }
    }

    private void SetHealth(int health)
    {
        intSlider.value += health;
    }


}
