/*
File: ProjectileController.cs
Project: Capstone Project
Programmer: Isaiah Bartlett
First Version: 3/21/2025
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Class Name: ClearPlayer
Description: This class will destroy the player and all their items.
  This is due to the fact the the player is part of "DontDestroyOnLoad", 
  which would cause the player character to be able to walk around the ending cutscene or the main menu.
*/
public class ClearPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Clear();
    }

    static public void Clear()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.DestroyInventory();
            DestroyImmediate(PlayerController.Instance.gameObject);
        }
        if (LevelTransitionManager.Instance != null)
        {
            DestroyImmediate(LevelTransitionManager.Instance.gameObject);
        }
    }
}
