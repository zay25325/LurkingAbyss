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

    public float grappleRange = 1.0f;   // Range of the grappling hook
    public float grappleSpeed = 10.0f;  // Speed of the grappling hook
    public string grappleTagWalls = "Walls";  // Tag for objects that can be grappled

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

        // Set the material for the LineRenderer
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
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

        // Get the player's position and the mouse position
        playerMovement = GameObject.FindObjectOfType<PlayerStats>();

        // Check if the player is not null
        if (playerMovement != null)
        {
            // Get the player's position and the mouse position
            Vector2 playerPosition = playerMovement.transform.position;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Perform a raycast from the player position towards the mouse position
            Vector2 direction = (mousePosition - playerPosition).normalized;
            float distanceToMouse = Vector2.Distance(playerPosition, mousePosition);

            // Raycast from the player's position towards the mouse position
            RaycastHit2D mouseHit = Physics2D.Raycast(playerPosition, direction, distanceToMouse, LayerMask.GetMask("VisionBlockers"));

            // Debug the raycast
            Debug.DrawRay(playerPosition, direction * distanceToMouse, Color.red, 1f);

            if (mouseHit.collider != null && mouseHit.collider.CompareTag(grappleTagWalls))
            {
                Debug.Log("Mouse clicked on a wall: " + mouseHit.collider.name);

                // Perform raycast within the grapple range to check for valid targets
                ContactFilter2D filter = new ContactFilter2D();
                filter.useTriggers = false;
                filter.SetLayerMask(Physics2D.DefaultRaycastLayers);
                filter.useLayerMask = true;

                // Array to store the hits
                RaycastHit2D[] hits = new RaycastHit2D[10];
                int hitCount = Physics2D.Raycast(playerPosition, direction, filter, hits, grappleRange); // Now uses grappleRange

                // Check if any object was hit
                for (int i = 0; i < hitCount; i++)
                {
                    // Get the hit object
                    RaycastHit2D hit = hits[i];

                    // Check if the hit object is not the player
                    if (hit.collider.gameObject != playerMovement.gameObject)
                    {
                        Debug.Log("Raycast hit: " + hit.collider.name);
                        Debug.Log("Hit object tag: " + hit.collider.tag);

                        // Check if the hit object is within the grapple range
                        float distanceToTarget = Vector2.Distance(playerPosition, hit.point);

                        // Check if the hit object is within the grapple range
                        if (distanceToTarget <= grappleRange)
                        {
                            // Check if the hit object is a valid grapple target
                            if (hit.collider.CompareTag(grappleTagWalls))
                            {
                                UnityEngine.AI.NavMeshHit navHit;

                                // Use NavMesh to find the closest valid position
                                if (UnityEngine.AI.NavMesh.SamplePosition(hit.point, out navHit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
                                {
                                    lineRenderer.SetPosition(0, playerPosition);
                                    lineRenderer.SetPosition(1, playerPosition);

                                    // Start the grapple coroutine
                                    StartCoroutine(GrappleToTarget(navHit.position));
                                }
                                else
                                {
                                    Debug.LogError("No valid position found on the NavMesh!");
                                }
                            }
                            else
                            {
                                Debug.Log("Hit object is not a valid grapple target.");
                            }
                        }
                        else
                        {
                            Debug.Log("Target is too far to grapple!");
                        }

                        break; // Stop checking after the first valid hit
                    }
                }

                if (hitCount == 0)
                {
                    Debug.Log("No object to grapple to in that direction.");
                }
            }
            else
            {
                Debug.Log("Mouse did not click on a wall.");
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
            Vector2 newPosition = Vector2.Lerp(startPosition, target, fractionOfJourney);

            // Cast a ray to detect collision with walls during movement
            Vector2 direction = (target - (Vector2)playerMovement.transform.position).normalized;
            float distanceToCheck = Vector2.Distance(playerMovement.transform.position, target);
            RaycastHit2D hit = Physics2D.Raycast(newPosition, direction, distanceToCheck, LayerMask.GetMask("VisionBlockers"));

            // Debug the raycast (optional, helps with visualization)
            Debug.DrawRay(newPosition, direction * distanceToCheck, Color.red, 1f);

            if (hit.collider != null)
            {
                // Wall collision detected
                Debug.Log("Collision detected at: " + hit.point);

                // Stop movement and adjust position
                Vector2 collisionPoint = hit.point;
                Vector2 offset = direction * 1f; // Adjust this value as needed
                playerMovement.transform.position = collisionPoint - offset; // Move player to just before the collision point
                lineRenderer.enabled = false; // Disable the rope
                yield break; // Exit the grappling process
            }

            // Move the player to the new position
            playerMovement.transform.position = newPosition;

            // Update LineRenderer to follow player
            lineRenderer.SetPosition(0, playerMovement.transform.position); // Player position updated
            lineRenderer.SetPosition(1, target); // Keep rope attached to the target

            yield return null;
        }

        // Ensure player reaches the target exactly
        //playerMovement.transform.position = target;

        // Disable the rope after reaching the target
        lineRenderer.enabled = false;

        ReduceItemCharge();
        DestroyItem(ItemObject);
    }
}