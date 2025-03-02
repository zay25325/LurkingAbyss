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

    private float maxHealth = 1f;
    private float maxShields = 4f;

    [SerializeField] 
    private float playerSpeed = 5f;    // Speed of the player

    [SerializeField] 
    private float sneakSpeed = 2f;    // Speed of the player when sneaking

    [SerializeField]
    private float dashSpeed = 3f;    // Speed of the player rotation

    //private variables
    [SerializeField]
    private float originalSpeed = 0f;    // Original speed of the player

    [SerializeField]
    private int ichorSamples = 0;    // Number of ichor samples the player has

    private int playerNoise = 0;    // Noise level of the player

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

    public float MaxHealth 
    { 
        get => maxHealth; 
        set => maxHealth = value; 
    }

    public float MaxShields 
    { 
        get => maxShields; 
        set => maxShields = value; 
    }

    public float PlayerSpeed 
    { 
        get => playerSpeed; 
        set => playerSpeed = value; 
    }

    public float SneakSpeed 
    { 
        get => sneakSpeed; 
        set => sneakSpeed = value; 
    }

    public float DashSpeed 
    { 
        get => dashSpeed; 
        set => dashSpeed = value; 
    }

    public float OriginalSpeed 
    { 
        get => originalSpeed; 
        set => originalSpeed = value; 
    }

    public int PlayerNoise 
    { 
        get => playerNoise; 
        set => playerNoise = value; 
    }

    public int IchorSamples 
    { 
        get => ichorSamples; 
        set => ichorSamples = value; 
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
