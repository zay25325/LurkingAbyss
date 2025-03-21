/*
File: ProjectileController.cs
Project: Capstone Project
Programmer: Isaiah Bartlett
First Version: 3/21/2025
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/*
Class Name: TeleporterItem
Description: This class is a child of the Item class and represents a Escape Teleporter item in the game. 
             Once the player uses the item, it will begin the ending animation, allowing the player to win the game.
*/
public class TeleporterItem : Item
{
    public Sprite teleporterIcon = null;  // Icon for the teleporter
    private GameObject teleporterPrefab = null;    // Prefab for the teleporter

    [SerializeField] GameObject TeleporterLevelAnimation;


    /*
        FUNCTION : Awake()
        DESCRIPTION : This function is called when the script instance is being loaded. 
                      It initializes the teleporter item properties.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void Awake()
    {
        // Set the prefab reference here
        teleporterPrefab = this.gameObject;
        // Initialize the item properties manually

        // Set the properties of the teleporter item
        ItemName = "Escape Teleporter";
        ItemDescription = "Can be used to escape this nightmare, and reach safety.";
        ItemIcon = teleporterIcon;
        ItemID = 0;
        maxItemCharge = 100;
        ItemCharge = 100;
        ItemRarity = Rarity.Anomalies;
        ItemValue = 0;
        CanItemDestroy = false;
        ItemSubtype = Subtype.Story;
        ItemObject = teleporterPrefab;
    }

    /*
        FUNCTION : Use()
        DESCRIPTION : This function is called when the teleporter item is used. 
                      It implements the functionality to teleport the player.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    public override void Use()
    {
        // Ignore charge count
        StartEndingAnimation();
    }

    /*
        FUNCTION : Teleport()
        DESCRIPTION : This function teleports the player to the target position based on the mouse position. Has a max teleport distance.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void StartEndingAnimation()
    {
        // Get the player's current position
        Transform playerTransform = GameObject.FindWithTag("Player").transform;
        Vector3 playerPosition = playerTransform.position;
        PlayerController controller = playerTransform.GetComponent<PlayerController>();
        controller.isParalyzed = true;
        GameObject.Instantiate(TeleporterLevelAnimation, playerPosition, new Quaternion());


        StartCoroutine(EndingAnimation());
    }


    private IEnumerator EndingAnimation()
    {
        yield return new WaitForSeconds(4f);
        ClearPlayer.Clear();
        SceneManager.LoadScene("EndingScene");
    }
}
