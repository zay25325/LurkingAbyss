/*
Class Name: PlayerStats
Description: This class is a component of the player game object and represents the player's health and shields. 
             It contains the properties and methods specific to the player's health and shields.
*/

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

    private void RevivePlayer()
    {
        Inventory inventory = GetComponent<Inventory>();
        if (inventory != null)
        {
            foreach (Item item in inventory.GetItems())
            {
                if (item.name == "Revivor" && item.CanUseItem())
                {
                    item.Use();
                    health = 1f; // Revive player with 1 health
                    Debug.Log("Player revived with Revivor item.");
                    break;
                }
            }
        }
    }

    private void Update()
    {
        if (health <= 0)
        {
            RevivePlayer();
        }
    }


}
