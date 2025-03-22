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
        yield return new WaitForSeconds(0.2f); // Adjust for timing

        // Move the player to the paired tube
        Debug.Log("Teleporting player to tube ID: " + pairedTube.tubeID);
        Vector3 targetPosition = pairedTube.transform.position + Vector3.up * 1.5f;

        // Perform an overlap circle check to ensure no walls or colliders at the new position
        Collider2D[] colliders = Physics2D.OverlapCircleAll(targetPosition, 0.5f); // Adjust the radius as needed

        bool isCollision = false;
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Walls")) // Check if the collider is a wall
            {
                isCollision = true;
                break;
            }
        }

        if (isCollision)
        {
            Debug.Log("Collision detected at target position. Finding nearest valid position.");
            // Find the nearest valid position within the NavMesh
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(targetPosition, out hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
            {
                player.transform.position = hit.position;
                Debug.Log("Player teleported to nearest valid position: " + hit.position);
            }
            else
            {
                Debug.LogError("No valid position found near the target position!");
            }
        }
        else
        {
            // Set the player's position to the target position
            player.transform.position = targetPosition;
            Debug.Log("Player teleported to: " + targetPosition);
        }

        // Small delay after teleporting
        yield return new WaitForSeconds(0.2f); // Adjust for timing

        isTeleporting = false;
    }
}
