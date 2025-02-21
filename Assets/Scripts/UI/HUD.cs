using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    public Slider intSlider;

    /*public HUD() // was causing errors. Also SHOULD NOT have constructors for MonoBehaviours
    {
        intSlider.value = 17.4f;
    }*/

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
            intSlider.value -= health;
    }


}
