using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.VisualScripting;
using UnityEngine.InputSystem;
using TMPro;

// controls player movement
// as well as a bunch of other game state

public class TestActorController:MonoBehaviour {
    // Rigidbody of the player.
    private Rigidbody2D rb;


    // Movement along X and Y axes.
    private float movementX;
    private float movementY;


    // Speed at which the player moves.
    public float speed = 0;
    public Transform t;


    // Start is called before the first frame update.
    void Start() {
        // Get and store the Rigidbody component attached to the player.
        rb = GetComponent<Rigidbody2D>();
        t = GetComponent<Transform>();
    }

    void OnMove(InputValue movementDirection) {
        Vector2 movementVector = movementDirection.Get<Vector2>();

        // Store the X and Y components of the movement.
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    // FixedUpdate is called once per fixed frame-rate frame.
    private void FixedUpdate() {
        


        // Create a 3D movement vector using the X and Y inputs.
        Vector2 movement = new Vector2(movementX,movementY);

        // Apply force to the Rigidbody to move the player.
        rb.AddForce(movement * speed);
    }
}
