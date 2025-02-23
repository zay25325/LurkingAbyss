using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicIdleState : MonsterState
{
    new private MimicController controller { get => base.controller as MimicController; }

    // private void Update()
    // {
    //     if (controller.Target != null)
    //     {
    //         if (Vector2.Distance(transform.position, controller.Target.position) < controller.AttackDistance)
    //         {
    //             controller.SwitchState<MimicAttackState>();
    //         }
    //         else
    //         {
    //             controller.Agent.SetDestination(controller.Target.position);
    //         }
    //     }
    // }

    private void OnEnable()
    {
        // controller.Agent.isStopped = false;
        // controller.Agent.speed = controller.WalkSpeed;
        // controller.Agent.SetDestination(controller.Target.position);

        // Start the coroutine to set the sprite after a delay
        StartCoroutine(SetRandomItemSpriteAfterDelay(0.1f)); // Adjust the delay as needed
    }

    // private void OnDisable()
    // {
    //     controller.Agent.isStopped = true;
    // }

    private IEnumerator SetRandomItemSpriteAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Find all items in the scene
        Item[] items = GameObject.FindObjectsOfType<Item>();

        List<Sprite> itemSprites = new List<Sprite>();
        foreach (var item in items)
        {
            if (item.ItemIcon != null)
            {
                itemSprites.Add(item.ItemIcon);
            }
        }

        // Randomly select one of the items
        if (itemSprites.Count > 0)
        {
            int randomIndex = Random.Range(0, itemSprites.Count);
            Sprite randomItemSprite = itemSprites[randomIndex];
            controller.SetItemSprite(randomItemSprite);
        }
    }
}

