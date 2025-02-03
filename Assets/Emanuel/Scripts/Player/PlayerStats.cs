using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float health = 1f;
    [SerializeField] private float shields = 4f;

    public float Health 
    { 
        get => health; 
        set => health = value; 
    }
    public float Shields 
    { 
        get => shields; 
        set => shields = value; 
    }


}
