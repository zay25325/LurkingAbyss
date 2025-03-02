using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGun : Item
{
    public Sprite gunIcon = null;  // Icon for the gun
    private GameObject gunPrefab = null;    // Prefab for the gun
    public GameObject damagingProjectilePrefab; // Prefab for the damaging projectile

    private void Awake()
    {
        // Set the prefab reference here
        gunPrefab = this.gameObject;
        // Initialize the item properties manually

        // Set the properties of the gun item
        ItemName = "Default Gun";
        ItemDescription = "Harms monsters at a distance, makes a lot of noise";
        ItemIcon = gunIcon;
        ItemID = 0;
        maxItemCharge = 3;
        ItemCharge = 3;
        ItemRarity = Rarity.Common;
        ItemValue = 0;
        CanItemDestroy = false;
        ItemSubtype = Subtype.Combat;
        ItemObject = gunPrefab;

        if (damagingProjectilePrefab == null)
        {
            Debug.LogError("DamagingProjectilePrefab is not assigned! Please assign it in the inspector.");
        }
    }

    public override void Use()
    {
        if (CanUseItem())
        {
            Shoot();
            ReduceItemCharge();
            DestroyItem(ItemObject);
        }
        else
        {
            Debug.Log("Not enough charges to use " + ItemName);
        }
    }

    private void Shoot()
{
    // Check if we have a valid camera
    if (Camera.main == null) return;

    // Get mouse position in world space
    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mousePosition.z = 0f; // Ensure the bullet stays in 2D space

    // Get the player position
    PlayerController player = FindObjectOfType<PlayerController>();
    if (player == null)
    {
        Debug.LogError("Player not found!");
        return;
    }

    // Spawn the projectile at the player's position
    GameObject projectile = Instantiate(damagingProjectilePrefab, player.transform.position, Quaternion.identity);

    // Get the direction from the projectile's spawn position to the mouse position
    Vector3 direction = (mousePosition - player.transform.position).normalized;

    // Set the target for the ProjectileController to handle the movement
    ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
    if (projectileController != null)
    {
        projectileController.Target = new Vector2(mousePosition.x, mousePosition.y); // Set target to the mouse position
    }
    else
    {
        Debug.LogError("Projectile is missing a ProjectileController!");
    }

    // Set the projectile's Rigidbody2D and apply velocity in the direction of the mouse
    Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
    if (rb != null)
    {
        // Ensure speed is applied to the projectile in the correct direction
        float projectileSpeed = projectileController.Speed;
        rb.velocity = direction * projectileSpeed;
    }
    else
    {
        Debug.LogError("Projectile is missing a Rigidbody2D!");
    }

    // Ignore collision with the player
    Collider2D playerCollider = player.GetComponent<Collider2D>();
    Collider2D projectileCollider = projectile.GetComponent<Collider2D>();
    if (playerCollider != null && projectileCollider != null)
    {
        Physics2D.IgnoreCollision(playerCollider, projectileCollider);
        Debug.Log("Ignoring collision between player and projectile.");
    }

    // Set the projectile to the "Projectiles" layer
    int projectileLayer = LayerMask.NameToLayer("Projectiles");
    projectile.layer = projectileLayer;

    // Set collision rules for the projectile layer
    int playerLayer = LayerMask.NameToLayer("Player");
    int entitiesLayer = LayerMask.NameToLayer("Entities");
    int visionBlockersLayer = LayerMask.NameToLayer("VisionBlockers");
    int itemLayer = LayerMask.NameToLayer("Item");

    Physics2D.IgnoreLayerCollision(projectileLayer, playerLayer, false);
    Physics2D.IgnoreLayerCollision(projectileLayer, entitiesLayer, false);
    Physics2D.IgnoreLayerCollision(projectileLayer, visionBlockersLayer, false);
    Physics2D.IgnoreLayerCollision(projectileLayer, itemLayer, true);
}
}
