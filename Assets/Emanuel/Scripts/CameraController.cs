using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float smoothSpeed = 0.125f; // Smoothing speed for camera movement
    public Vector3 offset; // Offset from the player

    private Vector3 velocity = Vector3.zero; // Velocity used by SmoothDamp

    private void Start()
    {
        // Set the near clipping plane to -2
        Camera.main.nearClipPlane = -2f;
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            // Smoothly move the camera to the desired position
            Vector3 desiredPosition = player.position + offset;
            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
            transform.position = smoothedPosition;

            // Ensure the camera does not rotate
            transform.rotation = Quaternion.identity;
        }
    }
}
