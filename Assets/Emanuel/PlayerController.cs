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

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Initialize input controls
        playerInputControls = new PlayerInputControls();
        playerInputControls.Player.Move.performed += ctx => Move(ctx.ReadValue<Vector2>());
        playerInputControls.Player.Move.canceled += ctx => StopMoving();
    }

    // This function is called when the object becomes enabled and active
    private void OnEnable()
    {
        playerInputControls.Enable();
    }

    // This function is called when the behaviour becomes disabled
    private void OnDisable()
    {
        playerInputControls.Disable();
    }

    // Start is called before the first frame update
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
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // Update velocity based on input
        playerRigidBody.velocity = movementInput * playerSpeed;
    }

    // Move the player
    private void Move(Vector2 direction)
    {
        movementInput = direction;
    }

    // Stop the player from moving
    private void StopMoving()
    {
        movementInput = Vector2.zero;
    }
}