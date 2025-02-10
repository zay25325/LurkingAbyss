using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashBooster : Item
{
    public Sprite dashBoosterIcon = null;  // Icon for the dash booster
    private GameObject dashBoosterPrefab = null;    // Prefab for the dash booster

    private PlayerStats playerMovement = null;

    private void Awake()
    {
        // Set the prefab reference here
        dashBoosterPrefab = this.gameObject;
        // Initialize the item properties manually

        // Set the properties of the dash booster item
        ItemName = "Dash Booster";
        ItemDescription = "Allows the player a quick burst of movement";
        ItemIcon = null;
        ItemID = 0;
        maxItemCharge = 4;
        ItemCharge = 4;
        ItemRarity = Rarity.Common;
        ItemValue = 0;
        CanItemDestroy = false;
        ItemSubtype = Subtype.Movement;
        ItemObject = dashBoosterPrefab;
    }

    public override void Use()
    {
        if (CanUseItem())
        {
            BoostDash();
            ReduceItemCharge();
            DestroyItem(ItemObject);
        }
        else
        {
            Debug.Log("Not enough charges to use " + ItemName);
        }
    }

    private void BoostDash()
    {
        // Implement dash boosting functionality
        Debug.Log("Boosting dash with: " + ItemName);
        float playerBoost = 2.0f;

        // Get the player's movement script
        playerMovement = GameObject.FindObjectOfType<PlayerStats>();

        playerMovement.OriginalSpeed = playerMovement.PlayerSpeed;
        if (playerMovement != null)
        {
            //originalSpeed = playerMovement.GetSpeed();
            StartCoroutine(TemporaryBoost(playerMovement, playerBoost));
        }
        else
        {
            Debug.Log("Player movement script not found");
        }
    }

    private IEnumerator TemporaryBoost(PlayerStats playerMovement, float boost)
    {
        playerMovement.PlayerSpeed = playerMovement.PlayerSpeed * boost;

        yield return new WaitForSeconds(1.0f);
        

        playerMovement.PlayerSpeed = playerMovement.OriginalSpeed;

    }
}
