using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Item
{
    public Sprite grenadeIcon = null;  // Icon for the grenade
    private GameObject grenadePrefab = null;    // Prefab for the grenade
    public GameObject grenadeProjectilePrefab; // Prefab for the grenade projectile

    private void Awake()
    {
        // Set the prefab reference here
        grenadePrefab = this.gameObject;
        
        // Initialize the item properties manually
        ItemName = "Grenade";
        ItemDescription = "Explodes on impact, dealing damage to nearby enemies.";
        ItemIcon = grenadeIcon;
        ItemID = 1;
        maxItemCharge = 3;
        ItemCharge = 3;
        ItemRarity = Rarity.Common;
        ItemValue = 0;
        CanItemDestroy = false;
        ItemSubtype = Subtype.Combat;
        ItemObject = grenadePrefab;

        if (grenadeProjectilePrefab == null)
        {
            Debug.LogError("GrenadeProjectilePrefab is not assigned! Please assign it in the inspector.");
        }
    }

    public override void Use(EntityInfo entityInfo)
    {
        if (entityInfo.Tags.Contains(EntityInfo.EntityTags.Player))
        {
            ThrowGrenade();
            ReduceItemCharge();
            DestroyItem(ItemObject);
        }
        else if (entityInfo.Tags.Contains(EntityInfo.EntityTags.Scavenger))
        {
            ScavengerThrowGrenade();
            ReduceItemCharge();
            DestroyItem(ItemObject);
        }
        else
        {
            Debug.Log("Not enough charges to use " + ItemName);
        }
    }

    private void ThrowGrenade()
    {
        if (Camera.main == null) return;

        // Get mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Ensure the grenade stays in 2D space

        // Get the player position
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        // Spawn the grenade projectile at the player's position
        GameObject grenade = Instantiate(grenadeProjectilePrefab, player.transform.position, Quaternion.identity);

        // Get the direction from the player's position to the mouse position
        Vector3 direction = (mousePosition - player.transform.position).normalized;

        // Set the target for the ProjectileController to handle the movement
        ProjectileController projectileController = grenade.GetComponent<ProjectileController>();
        if (projectileController != null)
        {
            projectileController.Target = new Vector2(mousePosition.x, mousePosition.y); // Set target to the mouse position
        }
        else
        {
            Debug.LogError("Grenade is missing a ProjectileController!");
        }

        // Set projectile movement with velocity
        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float grenadeSpeed = projectileController.Speed; // Adjust as needed
            rb.velocity = direction * grenadeSpeed;  // Apply velocity in the desired direction
        }
        else
        {
            Debug.LogError("Grenade projectile is missing a Rigidbody2D!");
        }

            // Ignore collision with the player
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            Collider2D projectileCollider = grenade.GetComponent<Collider2D>();
            if (playerCollider != null && projectileCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, projectileCollider);
                Debug.Log("Ignoring collision between player and projectile.");
            }

        // Set grenade projectile layer and collision rules
        int projectileLayer = LayerMask.NameToLayer("Projectiles");
        grenade.layer = projectileLayer;

        int playerLayer = LayerMask.NameToLayer("Player");
        int entitiesLayer = LayerMask.NameToLayer("Entities");
        int visionBlockersLayer = LayerMask.NameToLayer("VisionBlockers");
        int itemLayer = LayerMask.NameToLayer("Item");

        Physics2D.IgnoreLayerCollision(projectileLayer, playerLayer, false);
        Physics2D.IgnoreLayerCollision(projectileLayer, entitiesLayer, false);
        Physics2D.IgnoreLayerCollision(projectileLayer, visionBlockersLayer, false);
        Physics2D.IgnoreLayerCollision(projectileLayer, itemLayer, true);

        // Set the AttackingEntityInfo for the ExplosionProjectile
        ProjectileSpawnOnEndModifier spawnOnEndModifier = grenade.GetComponent<ProjectileSpawnOnEndModifier>();
        if (spawnOnEndModifier != null)
        {
            spawnOnEndModifier.SetAttackingEntityInfo(player.GetComponent<EntityInfo>());
        }
        else
        {
            Debug.LogError("Grenade is missing a ProjectileSpawnOnEndModifier!");
        }
    }

    private void ScavengerThrowGrenade()
    {
        // Get the player position
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        // Spawn the grenade projectile at the scavenger's position
        GameObject grenade = Instantiate(grenadeProjectilePrefab, transform.position, Quaternion.identity);

        // Get the direction from the scavenger's position to the player's position
        Vector3 direction = (player.transform.position - transform.position).normalized;

        // Set the target for the ProjectileController to handle the movement
        ProjectileController projectileController = grenade.GetComponent<ProjectileController>();
        if (projectileController != null)
        {
            projectileController.Target = new Vector2(player.transform.position.x, player.transform.position.y); // Set target to the player's position
        }
        else
        {
            Debug.LogError("Grenade is missing a ProjectileController!");
        }

        // Set projectile movement with velocity
        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float grenadeSpeed = projectileController.Speed; // Adjust as needed
            rb.velocity = direction * grenadeSpeed;  // Apply velocity in the desired direction
        }
        else
        {
            Debug.LogError("Grenade projectile is missing a Rigidbody2D!");
        }

        // Ignore collision with the scavenger
        Collider2D scavengerCollider = GetComponent<Collider2D>();
        Collider2D projectileCollider = grenade.GetComponent<Collider2D>();
        if (scavengerCollider != null && projectileCollider != null)
        {
            Physics2D.IgnoreCollision(scavengerCollider, projectileCollider);
            Debug.Log("Ignoring collision between scavenger and projectile.");
        }

        // Set grenade projectile layer and collision rules
        int projectileLayer = LayerMask.NameToLayer("Projectiles");
        grenade.layer = projectileLayer;

        int scavengerLayer = LayerMask.NameToLayer("Entities");
        int visionBlockersLayer = LayerMask.NameToLayer("VisionBlockers");
        int itemLayer = LayerMask.NameToLayer("Item");

        Physics2D.IgnoreLayerCollision(projectileLayer, scavengerLayer, false);
        Physics2D.IgnoreLayerCollision(projectileLayer, visionBlockersLayer, false);
        Physics2D.IgnoreLayerCollision(projectileLayer, itemLayer, true);

        // Set the AttackingEntityInfo for the ExplosionProjectile
        ProjectileSpawnOnEndModifier spawnOnEndModifier = grenade.GetComponent<ProjectileSpawnOnEndModifier>();
        if (spawnOnEndModifier != null)
        {
            spawnOnEndModifier.SetAttackingEntityInfo(GetComponent<EntityInfo>());
        }
        else
        {
            Debug.LogError("Grenade is missing a ProjectileSpawnOnEndModifier!");
        }
    }
}
