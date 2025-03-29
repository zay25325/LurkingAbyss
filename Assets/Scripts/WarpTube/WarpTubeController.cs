using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Add this for NavMesh support

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
            // Use NavMesh sampling to find a valid position
            Vector3 validPosition = GetValidNavMeshPosition(dir);

            // Check if the position is valid and clear of walls and vision blockers
            if (IsValidTeleportPosition(validPosition))
            {
                player.transform.position = validPosition;  // Teleport the player
                foundValidPosition = true;
                Debug.Log("Player teleported to valid position: " + validPosition);
                break;
            }
            else
            {
                // If blocked, move further in the same direction until clear of walls and WarpTubes
                Vector3 newDir = MoveFurtherAway(dir);
                validPosition = GetValidNavMeshPosition(newDir);
                if (IsValidTeleportPosition(validPosition))
                {
                    player.transform.position = validPosition;
                    foundValidPosition = true;
                    Debug.Log("Player teleported to valid position: " + validPosition);
                    break;
                }
            }
        }

        // If no valid position was found, log an error
        if (!foundValidPosition)
        {
            Debug.LogError("No valid teleport position found after checking nearby spaces!");
        }

        // Small delay after teleporting
        yield return new WaitForSeconds(0.2f);

        isTeleporting = false;
    }

    private Vector3 MoveFurtherAway(Vector3 direction)
    {
        // Move further away in the same direction by a certain distance
        float moveDistance = 0.1f; // Move 0.1 units further in the same direction
        return direction + direction.normalized * moveDistance;
    }

    private bool IsValidTeleportPosition(Vector3 position)
    {
        // Make sure the position is not within or touching anything with tag "Walls" or layer "VisionBlockers"
        float checkRadius = 0.5f; // Adjust radius as needed
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, checkRadius);
        foreach (var collider in colliders)
        {
            // Check if it's a wall or vision blocker
            if (collider.CompareTag("Walls") || collider.gameObject.layer == LayerMask.NameToLayer("VisionBlockers"))
            {
                Debug.Log("Invalid position detected for Walls and Vision Blockers: " + position);
                return false; // Invalid position, don't place player here
            }

            // Check if it's a WarpTube
            if (collider.CompareTag("WarpTube"))
            {
                Debug.Log("Invalid position detected for WarpTube: " + position);
                return false; // Invalid position, don't place player here
            }
        }

        // Additional check: Ensure the position is not inside any collider
        if (Physics2D.OverlapPoint(position))
        {
            Debug.Log("Position is inside a collider: " + position);
            return false;
        }

        return true; // Valid position
    }

    private Vector3 GetValidNavMeshPosition(Vector3 targetPosition)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, 1.0f, NavMesh.AllAreas))
        {
            return hit.position; // Return the closest valid NavMesh position
        }
        else
        {
            Debug.LogError("No valid NavMesh position found near: " + targetPosition);
            return targetPosition; // Fallback to the original position
        }
    }
}