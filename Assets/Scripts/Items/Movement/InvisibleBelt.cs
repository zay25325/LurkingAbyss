using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleBelt : Item
{
    public Sprite invisibleBeltIcon = null;  // Icon for the invisible belt
    private GameObject invisibleBeltPrefab = null;    // Prefab for the invisible belt

    private float invisibilityDuration = 5f; // Wait for the invisibility duration

    public bool isInvisible = false; // Flag to check if the player is invisible

    private void Awake()
    {
        // Set the prefab reference here
        invisibleBeltPrefab = this.gameObject;
        // Initialize the item properties manually

        // Set the properties of the invisible belt item
        ItemName = "Invisible Suit";
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

    public override void Use(EntityInfo entityInfo)
    {
        if(CanUseItem())
        {
            if (!IsInUse)
            {
                StartCoroutine(MakePlayerInvisible(entityInfo));
                ReduceItemCharge();
                DestroyItem(ItemObject);
            }
            else
            {
                Debug.Log("The invisibility effect is still active. Cannot use the item again.");
            }
        }
        else
        {
            Debug.Log("Not enough charges to use " + ItemName);
        }
    }

    private IEnumerator MakePlayerInvisible(EntityInfo entityInfo)
    {
        GameObject entity = null;
        if (entityInfo.gameObject.CompareTag("Player"))
        {
            // Get the player object
            entity = GameObject.FindWithTag("Player");
            if (entity == null)
            {
                Debug.LogError("Player not found!");
                yield break;
            }
        }
        else if (entityInfo.gameObject.CompareTag("Scavenger"))
        {
            Debug.Log("Scavenger Says Hellur");
            // Get the scavenger object
            entity = GameObject.FindWithTag("Scavenger");
            if (entity == null)
            {
                Debug.LogError("Scavenger not found!");
                yield break;
            }
        }
        else
        {
            Debug.LogError("Entity not found!");
            yield break;
        }

        isInvisible = true; // Set the invincibility flag to true

        // Set the item as in use
        IsInUse = true;

        // Make the player invisible
        SetPlayerVisibility(entity, false, entityInfo);
        Debug.Log(entity.name + " is now invisible.");

        // Store the tags to restore later
        List<EntityInfo.EntityTags> originalTags = new List<EntityInfo.EntityTags>(entityInfo.Tags);

        if (entityInfo.Tags.Contains(EntityInfo.EntityTags.Player))
        {
            // Remove elements 1-3
            entityInfo.Tags.Remove(EntityInfo.EntityTags.Wanderer);
            entityInfo.Tags.Remove(EntityInfo.EntityTags.Player);
            entityInfo.Tags.Remove(EntityInfo.EntityTags.CanOpenDoors);
        }

        if (entityInfo.Tags.Contains(EntityInfo.EntityTags.Scavenger))
        {
            // Remove elements 1-3
            entityInfo.Tags.Remove(EntityInfo.EntityTags.Wanderer);
            entityInfo.Tags.Remove(EntityInfo.EntityTags.Scavenger);
            entityInfo.Tags.Remove(EntityInfo.EntityTags.CanOpenDoors);
        }

        yield return new WaitForSeconds(invisibilityDuration);

        // Restore the original tags
        entityInfo.Tags.Clear();
        entityInfo.Tags.AddRange(originalTags);

        // Make the player visible again
        SetPlayerVisibility(entity, true, entityInfo);
        Debug.Log("Player is now visible again.");

        // Set the item as not in use
        IsInUse = false;

        isInvisible = false; // Reset the invincibility flag
    }

    private void SetPlayerVisibility(GameObject player, bool isVisible, EntityInfo entityInfo)
    {
        
        GameObject spriteObject = null;
        SpriteRenderer spriteRenderer = null;
        if (entityInfo.gameObject.CompareTag("Player"))
        {
            // Assuming the player has a SpriteRenderer component
            spriteObject = player.transform.Find("Sprite").gameObject;
            spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        }

        if (entityInfo.gameObject.CompareTag("Scavenger"))
        {
            // Assuming the scavenger has a SpriteRenderer component
            spriteObject = player.transform.Find("CharacterSprite").gameObject;
            spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on the Sprite object!");
            return;
        }

        if (spriteRenderer != null)
        {
            if (isVisible)
            {
                // Restore the original color
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
                Debug.Log("Setting player visible: Alpha = " + spriteRenderer.color.a);
            }
            else
            {
                // Set the color to semi-transparent
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.3f);
                Debug.Log("Setting player invisible: Alpha = " + spriteRenderer.color.a);
            }
        }
    }
}

