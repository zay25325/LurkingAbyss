using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleBelt : Item
{
    public Sprite invisibleBeltIcon = null;  // Icon for the invisible belt
    private GameObject invisibleBeltPrefab = null;    // Prefab for the invisible belt

    private float invisibilityDuration = 5f; // Wait for the invisibility duration

    private void Awake()
    {
        // Set the prefab reference here
        invisibleBeltPrefab = this.gameObject;
        // Initialize the item properties manually

        // Set the properties of the invisible belt item
        ItemName = "Invisible Belt";
        ItemDescription = "Can be worn to make the player invisible. ";
        ItemIcon = invisibleBeltIcon; 
        ItemID = 0;
        maxItemCharge = 3;
        ItemCharge = 3;
        ItemRarity = Rarity.Anomalies;
        ItemValue = 0;
        CanItemDestroy = false;
        ItemSubtype = Subtype.Movement;
        ItemObject = invisibleBeltPrefab;
    }

    public override void Use()
    {
        if(CanUseItem())
        {
            StartCoroutine(MakePlayerInvisible());
            ReduceItemCharge();
            DestroyItem(ItemObject);
        }
        else
        {
            Debug.Log("Not enough charges to use " + ItemName);
        }
    }

    private IEnumerator MakePlayerInvisible()
    {
        // Get the player object
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found!");
            yield break;
        }

        // Make the player invisible
        SetPlayerVisibility(player, false);
        Debug.Log("Player is now invisible.");

        yield return new WaitForSeconds(invisibilityDuration);

        // Make the player visible again
        SetPlayerVisibility(player, true);
        Debug.Log("Player is now visible again.");
    }

    private void SetPlayerVisibility(GameObject player, bool isVisible)
    {
    // Assuming the player has a SpriteRenderer component
    SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
    if (spriteRenderer != null)
    {
        if (isVisible)
        {
            // Restore the original color
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        }
        else
        {
            // Set the color to semi-transparent
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.3f);
        }
    }

    // Assuming the player has a Collider2D component
    Collider2D collider = player.GetComponent<Collider2D>();
    if (collider != null)
    {
        collider.enabled = isVisible;
    }

        // Add any additional logic to handle enemy detection here
        // For example, you can disable enemy AI targeting the player
    }
}

