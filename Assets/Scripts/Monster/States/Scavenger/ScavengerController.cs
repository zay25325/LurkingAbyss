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
    private List<Item> items = new List<Item>();
    private Item activeItem;

    public float AttackDistance { get => attackDistance; }
    public float AttackCooldown { get => attackCooldown; }
    public float AttackDuration { get => attackDuration; }
    public float AttackDamage { get => attackDamage; }

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
            return;
        }
        items.Add(item);
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
        base.Update();
    }
}
