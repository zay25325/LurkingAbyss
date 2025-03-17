using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScavengerAngeredState : ScavengerBaseState
{
    public float switchDistance = 10f;
    private Transform playerTarget;
    private ScavengerController scavengerController;
    new protected void OnEnable()
    {
        base.OnEnable();

        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        scavengerController = controller as ScavengerController;
    }

    private void SwitchToScavengingState()
    {
        controller.SwitchState<ScavengerScavengeState>();
    }

    private void UseCombatItem()
    {
        List<Item> items = scavengerController.GetItems();
        List<Item> combatItems = new List<Item>();
        EntityInfo entityInfo = scavengerController.GetComponent<EntityInfo>();

        // Check for Grenade and BasicGun in the inventory
        foreach (Item item in items)
        {
            if (item is Grenade || item is BasicGun)
            {
                combatItems.Add(item);
            }
        }

        // Determine which item to use
        if (combatItems.Count > 0)
        {
            Item selectedItem;
            if (combatItems.Count == 1)
            {
                selectedItem = combatItems[0];
            }
            else
            {
                selectedItem = combatItems[Random.Range(0, combatItems.Count)];
            }

            scavengerController.SetItem(selectedItem);

            // Use the active item
            Item activeItem = scavengerController.GetActiveItem();
            if (activeItem != null && entityInfo != null)
            {
                activeItem.Use(entityInfo);
                if (!activeItem.CanUseItem())
                {
                    scavengerController.RemoveItem(activeItem);
                }
            }
            else
            {
                Scream();
            }
        }
        else
        {
            Scream();
        }
    }

    private void Scream()
    {
        return;
    }

    public override void OnSeeingEntityEnter(Collider2D collider)
    {
        EntityInfo info = collider.GetComponent<EntityInfo>();
        if (info != null)
        {
            if (info.Tags.Contains(EntityInfo.EntityTags.Player))
            {
                Debug.Log("Scavenger sees entity");
                UseCombatItem();
            }
        }
    }

    public override void OnSeeingEntityExit(Collider2D collider)
    {
        EntityInfo info = collider.GetComponent<EntityInfo>();
        if (info != null)
        {
            if (info.Tags.Contains(EntityInfo.EntityTags.Player))
            {
                Debug.Log("Scavenger no longer sees entity");
                controller.SwitchState<ScavengerScavengeState>();
            }
        }
    }

}
