using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScavengerController : MonsterController
{
    [Header("Scavenger")]
    [SerializeField] float attackDistance = 2f;
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] float attackDuration = 0.5f;
    [SerializeField] float attackDamage = 1f;
    [SerializeField] private List<Item> items = new List<Item>();
    [SerializeField] private Item activeItem;

    [SerializeField] private float shield = 5f;
    private float maxShield = 5f;   

    public float AttackDistance { get => attackDistance; }
    public float AttackCooldown { get => attackCooldown; }
    public float AttackDuration { get => attackDuration; }
    public float AttackDamage { get => attackDamage; }
    public float Shield { get => shield; set => shield = value; }
    public float MaxShield { get => maxShield; }
    public void SetItem(Item item)
    {
        activeItem = item;
    }

    public void RemoveItem(Item item)
    {
        if (items.Contains(item))
        {
            // Place the item in front of the player
            items.Remove(item);
            if (item.ItemObject != null)
            {
                //AddRemoveGameworld(activeItem, true);
                item.ItemObject.SetActive(true); // Activate the item so it is added back to the scene
            }
        items.Remove(item);
        }   
    }

    public void AddItem(Item item)
    {
        if (items.Count >= 3)
        {
            Debug.LogWarning("Inventory is full. Cannot add more items.");
        }
        else
        {
            items.Add(item);
            item.ItemObject.SetActive(false);
        }
    }

    public Item GetActiveItem()
    {
        return activeItem;
    }

    public List<Item> GetItems()
    {
        return items;
    }

    new protected void Update()
    {
        ScavengerChargingShield();
        base.Update();
    }

    public void ReviveScavenger()
    {
        EntityInfo entityInfo = GetComponent<EntityInfo>();
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].name == "Revivor")
            {
                Debug.Log("Scavenger revived using Revivor!");
                Item revivor = items[i];
                revivor.ItemObject.transform.position = transform.position; // Place at scavenger's position
                revivor.ItemObject.SetActive(true); // Activate the item in the game world
                items.RemoveAt(i); // Remove the item from the list before using it
                revivor.Use(entityInfo); // Use the item
                return; // Exit the method after using the Revivor
            }
        }
    }

    protected override void OnHarmed(float damage)
    {
        // Reduce shield first
        if (shield > 0)
        {
            shield -= damage;
            if (shield < 0)
            {
                // Carry over excess damage to HP
                damage = -shield;
                shield = 0;
            }
            else
            {
                //if have a mobile Shield Generator, bring shield up
                ScavengerChargingShield();
                // Shield absorbed all the damage
                return;
            }
        }

        // Reduce HP after shield is depleted
        hp -= damage;

        if (hp <= 0)
        {
            // Attempt to revive using Revivor
            ReviveScavenger();

            // Check if HP was restored by ReviveScavenger
            if (hp > 0)
            {
                return; // Exit if revived
            }

            // Proceed with death logic if not revived
            if (hasDeathAnimation)
            {
                animator.SetTrigger("death");
            }
            else
            {
                OnDeath();
            }
        }
        else
        {
            // React to harm if not dead
            state.OnHarmed(damage);
        }
    }

    public void ScavengerChargingShield()
    {
        EntityInfo entityInfo = GetComponent<EntityInfo>();
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ItemObject != null && items[i].ItemObject.GetComponent<MobileShieldGenerator>() != null)
            {
                Debug.Log("Scavenger charging shield using mobile shield generator!");
                Item mobileShieldGenerator = items[i];
                mobileShieldGenerator.Use(entityInfo); // Use the item

                if (mobileShieldGenerator.ItemCharge <= 0)
                {
                    // Drop the item in front of the scavenger
                    if (mobileShieldGenerator.ItemObject != null)
                    {
                        mobileShieldGenerator.ItemObject.transform.position = transform.position; // Place in front of scavenger
                        mobileShieldGenerator.ItemObject.SetActive(true); // Activate the item in the game world
                    }
                    items.RemoveAt(i); // Remove the item from the list if it has no more charges
                }
                return; // Exit the method after using the MobileShieldGenerator
            }
        }
    }
}
