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
        items.Remove(item);
    }

    public void AddItem(Item item)
    {
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
}
