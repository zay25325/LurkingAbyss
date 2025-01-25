/*
Class Name: PlayerController
Description: This class is responsible for handling player controls and interactions. 
             Currently has impleemnented movement and rotation of player.It uses the new Input System to get input from the player and move the player accordingly. 
             The player can move in any direction and rotate towards the mouse position.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 5f;    // Speed of the player
    private Vector2 movementInput = Vector2.zero;   // Input from the player
    private Rigidbody2D playerRigidBody;    // Rigidbody2D component of the player
    private PlayerInputControls playerInputControls;    // Input system controls
    private Camera mainCamera; // Main camera for screen-to-world calculations


    /*
        FUNCTION : Awake()
        DESCRIPTION : called when the script instance is being loaded. Currently handles input control initialization, camera reference and input event subscription
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void Awake()
    {
        // Initialize input controls
        playerInputControls = new PlayerInputControls();
        
        // Get main camera
        mainCamera = Camera.main;

        // Subscribe to input events
        playerInputControls.Player.Move.performed += ctx => Move(ctx.ReadValue<Vector2>());
        playerInputControls.Player.Move.canceled += ctx => StopMoving();
        playerInputControls.Player.Look.performed += ctx => Look(ctx.ReadValue<Vector2>());
    }
    /*
        FUNCTION : OnEnable()
        DESCRIPTION : called when the object becomes enabled and active. Currently handles enabling input controls
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void OnEnable()
    {
        playerInputControls.Enable();
    }
    /*
        FUNCTION : OnDisable()
        DESCRIPTION : called when the behaviour becomes disabled. Currently handles disabling input controls
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void OnDisable()
    {
        playerInputControls.Disable();
    }
    /*
        FUNCTION : Start
        DESCRIPTION : called before the first frame update. Currently handles Rigidbody2D component initialization and configuration
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();

        // Check if Rigidbody2D component is missing
        if (playerRigidBody == null)
        {
            Debug.LogError("Rigidbody2D component is missing!");
        }
        else
        {
            // Ensure drag is zero
            playerRigidBody.drag = 0f;

            // Set collision detection mode to continuous
            playerRigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }
    /*
        FUNCTION : FixedUpdate()
        DESCRIPTION : called once per frame. Currently handles player movement
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void FixedUpdate()
    {
        // Update velocity based on input
        playerRigidBody.velocity = movementInput * playerSpeed;
    }
    /*
        FUNCTION : Move
        DESCRIPTION : Move the player in a direction
        PARAMETERS : Vector2 direction - Direction to move
        RETURNS : NONE
    */
    private void Move(Vector2 direction)
    {
        movementInput = direction;
    }

    /*
        FUNCTION : StopMoving()
        DESCRIPTION : Stop the player from moving
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void StopMoving()
    {
        movementInput = Vector2.zero;
    }

    /*
        FUNCTION : Look
        DESCRIPTION : Player looks towards mouse position
        PARAMETERS : Vector2 pointerInput - Mouse position
        RETURNS : NONE
    */
    private void Look(Vector2 pointerInput)
    {
        // Get mouse position in world space
        Vector3 mousePosition = Mouse.current.position.ReadValue(); // Mouse position in screen space
        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition); // Convert to world space
        mousePosition.z = 0; // Ensure Z is 0 for 2D

        // Calculate direction to face the mouse
        Vector2 direction = (mousePosition - transform.position).normalized;

        // Calculate the angle to rotate
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply rotation
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}