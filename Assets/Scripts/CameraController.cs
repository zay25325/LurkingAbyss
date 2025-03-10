using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform FollowTransform; // Reference to the player's transform
    public float SmoothSpeed = 0.125f; // Smoothing speed for camera movement
    public Vector3 Offset; // Offset from the player

    private Vector3 velocity = Vector3.zero; // Velocity used by SmoothDamp

    private void Start()
    {
        // Set the near clipping plane to -2
        Camera.main.nearClipPlane = -2f;
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            Transform playerTransform = playerObj.transform;
            transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);
        }
        // Set the near clipping plane to something greater than -2
        Camera.main.nearClipPlane = -20f;
    }

    private void LateUpdate()
    {
        if (FollowTransform != null)
        {
            // Smoothly move the camera to the desired position
            Vector3 desiredPosition = FollowTransform.position + Offset;
            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, SmoothSpeed);
            transform.position = smoothedPosition;

            // Ensure the camera does not rotate
            transform.rotation = Quaternion.identity;
        }
    }
}
