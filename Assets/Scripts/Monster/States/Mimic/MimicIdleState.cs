using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicIdleState : MonsterState
{
    new private MimicController controller { get => base.controller as MimicController; }


    private void OnEnable()
    {
        // Enable the Circle Collider 2D
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider != null)
        {
            circleCollider.isTrigger = true;
            Debug.Log("Circle Collider 2D enabled.");
        }
        else
        {
            Debug.LogWarning("CircleCollider2D component not found on the controller.");
        }
        
        StartCoroutine(SetRandomItemSpriteAfterDelay(0.1f)); // Adjust the delay as needed
    }

    private IEnumerator SetRandomItemSpriteAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);
        
        Debug.Log($"Items count in controller: {controller.items?.Count ?? 0}");

        // Find all items in the scene
        List<Item> items = controller.items;

        List<Sprite> itemSprites = new List<Sprite>();
        foreach (var item in items)
        {
            // Access the Sprite variable within each item
            Sprite itemSprite = item.GetComponent<SpriteRenderer>()?.sprite;
            if (itemSprite != null)
            {
                Debug.Log($"Adding item sprite: {itemSprite.name}");
                itemSprites.Add(itemSprite);
            }
            Debug.Log($"Item name: {item.name}");
        }

        // Randomly select one of the items
        if (itemSprites.Count > 0)
        {
            int randomIndex = Random.Range(0, itemSprites.Count);
            Sprite randomItemSprite = itemSprites[randomIndex];
            controller.SetItemSprite(randomItemSprite);

             // Debug logs to check the random selection process
            Debug.Log($"Randomly selected item index: {randomIndex}");
            Debug.Log($"Total items available: {itemSprites.Count}");
            Debug.Log($"Selected item sprite: {randomItemSprite.name}");
        }
    }
}
