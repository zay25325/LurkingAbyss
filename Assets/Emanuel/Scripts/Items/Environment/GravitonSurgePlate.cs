using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitonSurgePlate : Item
{
    public Sprite gravitonSurgePlateIcon = null;  // Icon for the graviton surge plate
    private GameObject gravitonSurgePlatePrefab = null;    // Prefab for the graviton surge plate
    public float pullRadius = 5.0f;  // Radius within which enemies are pulled towards the center
    public float pullForce = 10.0f;  // Force with which enemies are pulled towards the center

    private void Awake()
    {
        // Set the prefab reference here
        gravitonSurgePlatePrefab = this.gameObject;
        // Initialize the item properties manually

        // Set the properties of the graviton surge plate item
        ItemName = "Graviton Surge Plate";
        ItemDescription = "Can be placed to generate a graviton surge that pulls enemies towards it.";
        ItemIcon = gravitonSurgePlateIcon; 
        ItemID = 0;
        maxItemCharge = 1;
        ItemCharge = 1;
        ItemRarity = Rarity.Anomalies;
        ItemValue = 0;
        CanItemDestroy = false;
        ItemSubtype = Subtype.Environment;
        ItemObject = gravitonSurgePlatePrefab;
    }

    public override void Use()
    {
        if (CanUseItem())
        {
            Place();
            ReduceItemCharge();
            DestroyItem(ItemObject);
        }
        else
        {
            Debug.Log("Cannot use item.");
        }
    }

    private void Place()
    {
        // Implement the functionality to place the graviton surge plate
        Debug.Log("Placing graviton surge plate.");
        // Instantiate the trap in the game world
        GameObject trap = Instantiate(gravitonSurgePlatePrefab, transform.position, Quaternion.identity);
        trap.GetComponent<GravitonSurgePlate>().Initialize(pullRadius, pullForce);
    }

    public void Initialize(float radius, float force)
    {
        pullRadius = radius;
        pullForce = force;
        // Ensure the collider is set as a trigger
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PullEnemies();
            Debug.Log("Player stepped on the trap.");
        }
        else if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy stepped on the trap.");
            PullEnemies();
        }
    }

    private void PullEnemies()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, pullRadius);
        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy") || enemy.CompareTag("Player"))
            {
                Vector2 direction = (transform.position - enemy.transform.position).normalized;
                enemy.GetComponent<Rigidbody2D>().AddForce(direction * pullForce, ForceMode2D.Impulse);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }
}
