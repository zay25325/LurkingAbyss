using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DashBooster : Item
{
    public Sprite dashBoosterIcon = null;  // Icon for the dash booster
    private GameObject dashBoosterPrefab = null;    // Prefab for the dash booster

    private PlayerStats playerMovement = null;
    private bool isCooldown = false;

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

    public override void Use(EntityInfo entityInfo)
    {
        if (CanUseItem() && !isCooldown)
        {
            if (entityInfo.Tags.Contains(EntityInfo.EntityTags.Player))
            {
                BoostDash();
            }
            else if (entityInfo.Tags.Contains(EntityInfo.EntityTags.Scavenger))
            {
                ScavengerBoostDash();
            }
            ReduceItemCharge();
            DestroyItem(ItemObject);
        }
        else if (isCooldown)
        {
            Debug.Log("Dash booster is on cooldown");
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
        isCooldown = true;
        playerMovement.PlayerSpeed = playerMovement.PlayerSpeed * boost;

        yield return new WaitForSeconds(1.0f);
        

        playerMovement.PlayerSpeed = playerMovement.OriginalSpeed;
        isCooldown = false;
    }

    private void ScavengerBoostDash()
    {
        Debug.Log("Boosting dash for scavenger with: " + ItemName);
        float scavengerBoost = 2.0f;

        // Temporarily activate the DashBooster GameObject
        bool wasActive = gameObject.activeSelf;
        gameObject.SetActive(true);

        // Disable renderer components to make the DashBooster invisible
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        // Find the scavenger GameObject by name
        GameObject scavenger = GameObject.Find("Scavenger");

        if (scavenger != null)
        {
            // Get the scavenger's NavMeshAgent component
            NavMeshAgent scavengerAgent = scavenger.GetComponent<NavMeshAgent>();

            if (scavengerAgent != null)
            {
                float originalSpeed = scavengerAgent.speed;
                StartCoroutine(TemporaryScavengerBoost(scavengerAgent, scavengerBoost, originalSpeed, wasActive, renderers));
            }
            else
            {
                Debug.Log("Scavenger NavMeshAgent not found");
            }
        }
        else
        {
            Debug.Log("Scavenger GameObject not found");
        }

        // Re-enable renderer components to make the DashBooster visible again
        // foreach (Renderer renderer in renderers)
        // {
        //     renderer.enabled = true;
        // }

        // Restore the DashBooster GameObject's active state
        // gameObject.SetActive(wasActive);
    }

    private IEnumerator TemporaryScavengerBoost(NavMeshAgent scavengerAgent, float boost, float originalSpeed, bool wasActive, Renderer[] renderers)
    {
        isCooldown = true;
        scavengerAgent.speed = scavengerAgent.speed * boost;

        yield return new WaitForSeconds(1.0f);

        scavengerAgent.speed = originalSpeed;
        isCooldown = false;

        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = true;
        }

        gameObject.SetActive(wasActive);
    }
}
