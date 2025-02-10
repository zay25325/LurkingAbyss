/*
Class Name: Grappler
Description: This class is a child of the Item class and represents a grappler item in the game. 
              It contains the properties and methods specific to the grappler item. Such as allowing the player to grapple to objects and pull themselves towards it
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappler : Item
{
    public Sprite grapplerIcon = null;  // Icon for the grappler
    private GameObject grapplerPrefab = null;    // Prefab for the grappler

    public float grappleRange = 5.0f;   // Range of the grappling hook
    public float grappleSpeed = 10.0f;  // Speed of the grappling hook
    public string grappleTagWalls = "Walls";  // Tag for objects that can be grappled
    public string grappleTagItem = "Item";  // Tag for objects that can be grappled

    private PlayerStats playerMovement = null;  // Reference to the player's movement script
    private LineRenderer lineRenderer = null;   // LineRenderer for the rope

    /*
        FUNCTION : Awake()
        DESCRIPTION : This function is called when the script instance is being loaded. 
                      It initializes the grappler item properties.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void Awake()
    {
        // Set the prefab reference here
        grapplerPrefab = this.gameObject;
        // Initialize the item properties manually

        // Set the properties of the grappler item
        ItemName = "Grappler";
        ItemDescription = "Allows the player to grapple to objects";
        ItemIcon = null;
        ItemID = 0;
        maxItemCharge = 4;
        ItemCharge = 4;
        ItemRarity = Rarity.Anomalies;
        ItemValue = 0;
        CanItemDestroy = false;
        ItemSubtype = Subtype.Movement;
        ItemObject = grapplerPrefab;

        // Initialize the LineRenderer for the rope
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    /*
        FUNCTION : Use()
        DESCRIPTION : This function is called when the grappler item is used. 
                      It implements the functionality to grapple to objects and pull the player towards it.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    public override void Use()
    {
        if (CanUseItem())
        {
            Grapple();
        }
        else
        {
            Debug.Log("Not enough charges to use " + ItemName);
        }
    }

    /*
        FUNCTION : Grapple()
        DESCRIPTION : This function implements the grapple functionality. 
                      It casts a ray from the player to the mouse position and checks if the object hit can be grappled.
                      If the object can be grappled, it extends a rope to the object and pulls the player towards it.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void Grapple()
    {
        Debug.Log("Grapple with: " + ItemName);

        // Get the player's movement script
        playerMovement = GameObject.FindObjectOfType<PlayerStats>();

        if (playerMovement != null)
        {
            // Cast a ray from the player to the mouse position
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Check if the object hit can be grappled
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("Raycast hit: " + hit.collider.name);
                Debug.Log("Hit object tag: " + hit.collider.tag);

                // Check if the object hit is a valid grapple target
                if (hit.collider.CompareTag(grappleTagWalls) || hit.collider.CompareTag(grappleTagItem))
                {
                    // **Ensure the line starts from the player and has no extra segment**
                    lineRenderer.SetPosition(0, playerMovement.transform.position);
                    lineRenderer.SetPosition(1, playerMovement.transform.position); // Set both points to prevent trailing line

                    StartCoroutine(GrappleToTarget(hit.point));
                }
                else
                {
                    Debug.Log("Hit object is not a valid grapple target.");
                }
            }
            else
            {
                Debug.Log("No object to grapple to at mouse position.");
            }
        }
    }

    /*
        FUNCTION : GrappleToTarget()
        DESCRIPTION : This function implements the grapple functionality. 
                      It extends a rope to the target position and pulls the player towards it.
        PARAMETERS : Vector2 target - The target position to grapple to
        RETURNS : IEnumerator
    */
    private IEnumerator GrappleToTarget(Vector2 target)
    {
        lineRenderer.enabled = true;
        Vector2 startPosition = playerMovement.transform.position;

        // Ensure the rope starts at the player's position
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, startPosition);

        // Extend the rope first before pulling
        float extendTime = 0.2f;
        float elapsedTime = 0f;

        // Extend the rope to the target
        while (elapsedTime < extendTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / extendTime;
            Vector2 currentEndPoint = Vector2.Lerp(startPosition, target, t);

            lineRenderer.SetPosition(0, startPosition);  // Always stays at player's original position
            lineRenderer.SetPosition(1, currentEndPoint); // Move towards target

            yield return null;
        }

        lineRenderer.SetPosition(1, target);

        yield return new WaitForSeconds(0.1f); // Short delay before pulling

        // Move the player towards the target
        float distance = Vector2.Distance(startPosition, target);
        float startTime = Time.time;

        // Pull the player towards the target
        while (Vector2.Distance(playerMovement.transform.position, target) > 0.1f)
        {
            float coveredDistance = (Time.time - startTime) * grappleSpeed;
            float fractionOfJourney = coveredDistance / distance;
            playerMovement.transform.position = Vector2.Lerp(startPosition, target, fractionOfJourney);

            // **Fix: Update the LineRenderer so it follows the player correctly**
            lineRenderer.SetPosition(0, playerMovement.transform.position); // Player moves -> update start position
            lineRenderer.SetPosition(1, target); // Keep rope attached to the target

            yield return null;
        }

        // Ensure player is exactly at the target
        playerMovement.transform.position = target;

        // Disable the rope after reaching the target
        lineRenderer.enabled = false;

        ReduceItemCharge();
        DestroyItem(ItemObject);
    }
}
