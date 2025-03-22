using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpTubeController : MonoBehaviour
{
    public int tubeID; // Assigned during generation
    public WarpTubeController pairedTube; // Will be assigned dynamically
    public float teleportDelay = 0.5f;

    private bool isTeleporting = false;

    private void Start()
    {
        // Register the tube in the manager
        WarpTubeManager.Instance.RegisterTube(this);
        Debug.Log("WarpTube started with ID: " + tubeID);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player enters the tube and if teleportation is not happening already
        if (other.CompareTag("Player") && !isTeleporting && pairedTube != null)
        {
            Debug.Log("Player entered WarpTube with ID: " + tubeID);
            StartCoroutine(TeleportPlayer(other.gameObject));
        }
        else
        {
            // Debug message if the player doesn't trigger teleportation
            Debug.Log("Player failed to trigger WarpTube: " + tubeID);
        }
    }

    private IEnumerator TeleportPlayer(GameObject player)
    {
        isTeleporting = true;

        // Small delay before teleporting
        yield return new WaitForSeconds(teleportDelay);

        // Determine the base target position (paired tube's position)
        Vector3 basePosition = pairedTube.transform.position;

        // Check the four possible directions around the paired tube (no upward modification)
        Vector3[] directions = {
            basePosition + Vector3.up,
            basePosition + Vector3.down,
            basePosition + Vector3.left,
            basePosition + Vector3.right
        };

        bool foundValidPosition = false;
        foreach (var dir in directions)
        {
            // Check if the position is valid and clear of walls and vision blockers
            if (IsValidTeleportPosition(dir))
            {
                player.transform.position = dir;  // Teleport the player
                foundValidPosition = true;
                Debug.Log("Player teleported to valid position: " + dir);
                break;
            }
        }

        // If no valid position was found, log an error
        if (!foundValidPosition)
        {
            Debug.LogError("No valid teleport position found!");
        }

        // Small delay after teleporting
        yield return new WaitForSeconds(0.2f);

        isTeleporting = false;
    }

    // Check if the target position is valid (not colliding with walls, vision blockers, or warp tubes)
    private bool IsValidTeleportPosition(Vector3 position)
    {
        // Make sure the position is not within or touching anything with tag "Walls" or layer "VisionBlockers"
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.5f); // Adjust radius as needed
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Walls") || collider.gameObject.layer == LayerMask.NameToLayer("VisionBlockers"))
            {
                return false; // Invalid position, don't place player here
            }
        }

        // Now make sure the player is not colliding with WarpTubes after teleportation
        Collider2D[] postTeleportColliders = Physics2D.OverlapCircleAll(position, 0.5f); // Adjust radius as needed
        foreach (var collider in postTeleportColliders)
        {
            if (collider.CompareTag("WarpTube"))
            {
                return false; // Invalid position, player can't land on a WarpTube
            }
        }

        return true; // Valid position
    }
}


