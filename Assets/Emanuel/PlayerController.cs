/*
Class Name: PlayerController
Description: This class is responsible for handling player controls and interactions. 
             Currently has implemented movement and rotation of player. It uses the new Input System to get input from the player and move the player accordingly. 
             The player can move in any direction and rotate towards the mouse position.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //constants
    const float DASH_DURATION = 0.2f; // Duration of the dash
    const float DASH_COOLDOWN = 0.5f; // Cooldown of the dash

    //serialized fields variables
    [SerializeField] 
    private float playerSpeed = 5f;    // Speed of the player

    [SerializeField] 
    private float sneakSpeed = 2.5f;    // Speed of the player when sneaking

    [SerializeField]
    private float dashSpeed = 7.5f;    // Speed of the player rotation

    //private variables
    private float originalSpeed = 0f;    // Original speed of the player
    private Vector2 movementInput = Vector2.zero;   // Input from the player
    private Rigidbody2D playerRigidBody;    // Rigidbody2D component of the player
    private PlayerInputControls playerInputControls;    // Input system controls
    private Camera mainCamera; // Main camera for screen-to-world calculations

    private bool canDash = true; // Flag to check if player can dash

    //inventory
    //subject to change, this is to test around player interacting/dropping item functionality
    private List<GameObject> inventory = new List<GameObject>();

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
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
        }

        // Subscribe to input events
        playerInputControls.Player.Move.performed += ctx => Move(ctx.ReadValue<Vector2>()); // Subscribe to move event
        playerInputControls.Player.Move.canceled += ctx => StopMoving();                    // Subscribe to stop moving event
        playerInputControls.Player.Look.performed += ctx => Look(ctx.ReadValue<Vector2>()); // Subscribe to look event
        playerInputControls.Player.Sneak.performed += ctx => OnSneak(ctx);                  // Subscribe to sneak event
        playerInputControls.Player.Sneak.canceled += ctx => OnSneak(ctx);                   // Subscribe to stop sneaking event
        playerInputControls.Player.Dash.performed += ctx => Dash(ctx);                      // Subscribe to dash event
        playerInputControls.Player.Interact.performed += ctx => DoInteraction(ctx);      // Subscribe to interact event
        playerInputControls.Player.Drop.performed += ctx => DropItem(ctx);      // Subscribe to Drop event
    }

    /*
        FUNCTION : OnEnable()
        DESCRIPTION : called when the object becomes enabled and active. Currently handles enabling input controls
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void OnEnable()
    {
        if (playerInputControls != null)
        {
            playerInputControls.Enable();
        }
    }

    /*
        FUNCTION : OnDisable()
        DESCRIPTION : called when the behaviour becomes disabled. Currently handles disabling input controls
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void OnDisable()
    {
        if (playerInputControls != null)
        {
            playerInputControls.Disable();
        }
    }

    /*
        FUNCTION : Start
        DESCRIPTION : called before the first frame update. Currently handles Rigidbody2D component initialization and configuration
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void Start()
    {
        originalSpeed = playerSpeed;    // Original speed of the player
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

    /*
        FUNCTION : OnSneak
        DESCRIPTION : Adjusts player speed for sneaking
        PARAMETERS : InputAction.CallbackContext context - Input context for the sneak action. Easy to check if the action was performed or canceled
        RETURNS : NONE
    */
    private void OnSneak(InputAction.CallbackContext context)
    {
        //While the sneak action is performed, reduce the player speed
        if (context.performed)
        {
            playerSpeed = sneakSpeed; // Reduce speed for sneaking
        }

        //When the sneak action is canceled, reset the player speed
        else if (context.canceled)
        {
            playerSpeed = originalSpeed; // Reset speed when not sneaking
        }
    }

    /*
        FUNCTION : Dash
        DESCRIPTION : This function is called when the player performs the dash action. 
                      It checks if the player can dash and then starts the DashCoroutine. A couroutine was chosen to do 
                      because it allows for a delay between the dash and the cooldown. Additionally, it prevents the player from dashing multiple times.
                      As well, the player speed is increased for a short duration and then reset after a cooldown. 
        PARAMETERS : InputAction.CallbackContext context - Input context for the dash action. Easy to check if the action was performed
        RETURNS : NONE
    */
    private void Dash(InputAction.CallbackContext context)
    {
        // Dash action is performed once upon button press
        if (context.performed && canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    /*
        FUNCTION : DashCoroutine
        DESCRIPTION : This coroutine is responsible for handling the dash action. 
                      It increases the player speed for a short duration and then resets it after a cooldown
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private IEnumerator DashCoroutine()
    {
        canDash = false; // Prevent dashing multiple times
        playerSpeed = dashSpeed; // Increase speed for dashing
        yield return new WaitForSeconds(DASH_DURATION); // Dash duration
        playerSpeed = originalSpeed; // Reset speed after dashing
        yield return new WaitForSeconds(DASH_COOLDOWN); // Cooldown duration
        canDash = true; // Allow dashing again
    }

    //adding items
    //functionality is subject to change when inventory & item system is implemented




    /*
        FUNCTION : AddItem
        DESCRIPTION : This function is responsible for adding an item to the player's inventory. 
                      It checks if the inventory is full and if the item is tagged as an item. 
                      If the inventory is not full and the item is tagged as an item, the item is added to the inventory and deactivated in the scene.
        PARAMETERS : gameobject item - The item to add to the inventory
        RETURNS : bool - Returns true if the item was added successfully, false otherwise
    */
    public bool AddItem(GameObject item)
    {
        if (inventory.Count < 3 && item.CompareTag("item"))
        {
            Debug.Log("Item added to inventory");
            inventory.Add(item);
            item.SetActive(false); // Deactivate the item so it is removed from the scene
            return true; // Item added successfully           
        }
        Debug.Log("Item not added to inventory");
        return false; // Inventory full or item not tagged as "item"
    }

    /*
        FUNCTION : RemoveItem
        DESCRIPTION : This function is responsible for removing an item from the player's inventory and placing it in the game world. 
                      It checks if the item is in the inventory and if it is, the item is removed from the inventory and placed in front of the player.
        PARAMETERS : gameobject item - The item to remove from the inventory
        RETURNS : bool - Returns true if the item was removed successfully, false otherwise
    */
    public bool RemoveItem(GameObject item)
    {
        if (inventory.Remove(item)) // Returns true if item was removed
        {
            item.SetActive(true); // Reactivate the item
            // Place the item in front of the player
            Vector3 dropPosition = transform.position + transform.right; // Adjust this vector as needed
            item.transform.position = dropPosition;
            Debug.Log("Item removed from inventory and placed in the game world");
            return true;
        }
        Debug.Log("Item not found in inventory");
        return false; // Item not found in inventory
    }

    /*
        FUNCTION : GetInventory
        DESCRIPTION : This function is responsible for returning the player's inventory. 
                      It returns the list of items in the player's inventory.
        PARAMETERS : NONE
        RETURNS : inventory - List of items in the player's inventory
    */
    // Method to get the current inventory
    public List<GameObject> GetInventory()
    {
        return inventory;
    }


    //thought process behind DoInteraction
    //can seperate objects of interests through tags OR based on what sccript is attached to the object
    //for example, if the player is interacting with an item, the item will have a tag "item" and the player will have a script that interacts with objects with the tag "item"
    //this is a simple way to handle interactions, but can be expanded upon when more complex interactions are needed
    
        /*
        FUNCTION : DoInteraction
        DESCRIPTION : This function is responsible for handling player interactions with items or objects. 
                      It checks for collisions with items or objects with the "item" tag. 
                      If the player collides with an item, the item is added to the player's inventory.
        PARAMETERS : InputAction.CallbackContext context - Input context for the interaction action.
        RETURNS : NONE
    */
    private void DoInteraction(InputAction.CallbackContext context)
    {
        // Check for collision with an item or object with ItemSystem script
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("item"))
            {
                AddItem(hitCollider.gameObject);
                break;
            }
            //added a potential else if statement to handle interactions with objects that have the ItemSystem script (when it is created)
            //and has its own functions related to adding to inventory


            // else if (hitCollider.GetComponent<ItemSystem>() != null)
            // {
            //     // Do something with the object that has the ItemSystem script
            //     Debug.Log("Interacted with object having ItemSystem script");
            //     // Example: Call a method from the ItemSystem script
            //     hitCollider.GetComponent<ItemSystem>().Interact();
            //     break;
            // }
        }
    }

    /*
        FUNCTION : DropItem
        DESCRIPTION : This function is responsible for handling player dropping items from the inventory. 
                      It checks if the inventory is not empty and if it is not, the last item in the inventory is removed.
        PARAMETERS : InputAction.CallbackContext context - Input context for the drop action.
        RETURNS : NONE
    */
    private void DropItem(InputAction.CallbackContext context)
    {
        if (inventory.Count > 0)
        {
            GameObject item = inventory[inventory.Count - 1];
            RemoveItem(item);
        }
    }
}
