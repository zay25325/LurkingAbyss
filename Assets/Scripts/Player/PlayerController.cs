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
    public static PlayerController Instance;

    [SerializeField] SightMeshController sightMeshController;
    [SerializeField] GameObject teleporterItemPrefab;

    //constants
    const float DASH_DURATION = 0.2f; // Duration of the dash
    const float DASH_COOLDOWN = 0.5f; // Cooldown of the dash

    private PlayerStats playerStats; // Player stats component

    private Vector2 movementInput = Vector2.zero;   // Input from the player
    private Rigidbody2D playerRigidBody;    // Rigidbody2D component of the player
    private PlayerInputControls playerInputControls;    // Input system controls
    private readonly HUD hud = new();

    private bool canDash = true; // Flag to check if player can dash

    //inventory
    private Inventory inventory;
    private Item activeItem;

    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component

    public bool isInvincible { get; set; } // Flag to check if player is invincible

    public bool isParalyzed = false;

    /*
        FUNCTION : Awake()
        DESCRIPTION : called when the script instance is being loaded. Currently handles input control initialization, camera reference and input event subscription
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize input controls
            playerInputControls = new PlayerInputControls();

            // Subscribe to input events
            playerInputControls.Player.Move.performed += ctx => Move(ctx.ReadValue<Vector2>()); // Subscribe to move event
            playerInputControls.Player.Move.canceled += ctx => StopMoving();                    // Subscribe to stop moving event
            playerInputControls.Player.Look.performed += ctx => Look(ctx.ReadValue<Vector2>()); // Subscribe to look event
            playerInputControls.Player.Sneak.performed += ctx => OnSneak(ctx);                  // Subscribe to sneak event
            playerInputControls.Player.Sneak.canceled += ctx => OnSneak(ctx);                   // Subscribe to stop sneaking event
            playerInputControls.Player.Dash.performed += ctx => Dash(ctx);                      // Subscribe to dash event
            playerInputControls.Player.Interact.performed += ctx => DoInteraction(ctx);         // Subscribe to interact event
            playerInputControls.Player.Drop.performed += ctx => DropItem(ctx);                  // Subscribe to Drop event
            playerInputControls.Player.Scroll.performed += OnScroll;                            // Subscribe to scroll event                         
            playerInputControls.Player.Slot1.performed += OnSlot1Pressed;                       // Subscribe to slot 1 event
            playerInputControls.Player.Slot2.performed += OnSlot2Pressed;                       // Subscribe to slot 2 event
            playerInputControls.Player.Slot3.performed += OnSlot3Pressed;                       // Subscribe to slot 3 event
            playerInputControls.Player.Fire.performed += Fire;                                  // Subscribe to fire event  
        }
        else
        {
            Destroy(gameObject);
        }
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
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component is missing!");
        }
        else
        {
            playerStats.OriginalSpeed = playerStats.PlayerSpeed;   // Original speed of the player
        }
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
            
            // Freeze rotation to prevent spinning
            playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // Get the Inventory component
        inventory = GetComponent<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("Inventory component is missing!");
        }

        // Get the SpriteRenderer component from the child GameObject named "Sprite"
        Transform spriteTransform = transform.Find("Sprite");
        if (spriteTransform != null)
        {
            spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer component is missing on the child GameObject!");
            }
        }
        else
        {
            Debug.LogError("Child GameObject named 'Sprite' is missing!");
        }
    }

    /*
        FUNCTION : FixedUpdate()
        DESCRIPTION : called once per fixed update. Currently handles player movement
        PARAMETERS : NONE
        RETURNS : NONE
    */
    private void FixedUpdate()
    {
        if (isParalyzed)
        {
            StopMoving();
            playerRigidBody.velocity = Vector2.zero;
            // Disable Player Animation Script in the child GameObject "Sprite"
            Transform spriteTransform = transform.Find("Sprite");
            if (spriteTransform != null)
            {
                var playerAnimation = spriteTransform.GetComponent<PlayerAnimation>();
                if (playerAnimation != null)
                {
                    playerAnimation.enabled = false;
                }

                // Start flashing blue to indicate shock
                StartCoroutine(FlashBlue(spriteRenderer));
            }
            return;
        }
        else
        {
            // Enable Player Animation Script in the child GameObject "Sprite"
            Transform spriteTransform = transform.Find("Sprite");
            if (spriteTransform != null)
            {
                var playerAnimation = spriteTransform.GetComponent<PlayerAnimation>();
                if (playerAnimation != null)
                {
                    playerAnimation.enabled = true;
                }

                // Stop flashing and reset color to normal
                StopCoroutine(FlashBlue(spriteRenderer));
                spriteRenderer.color = new Color(1f, 1f, 1f, 1f); // Original color
            }
       
        }
        // Update velocity based on input
        playerRigidBody.velocity = movementInput * playerStats.PlayerSpeed;
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
        MovingNoise();
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
        MovingNoise();
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
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition); // Convert to world space
        mousePosition.z = 0; // Ensure Z is 0 for 2D

        // Calculate direction to face the mouse
        Vector2 direction = (mousePosition - transform.position).normalized;

        // Calculate the distance between the player and the mouse position
        float distance = Vector2.Distance(mousePosition, transform.position);

        // Define a minimum distance threshold to avoid erratic rotations
        float minDistanceThreshold = 0.1f;

        // Only rotate if the distance is greater than the threshold
        // if (distance > minDistanceThreshold)
        // {
        //     // Calculate the angle to rotate
        //     float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //     // Apply rotation with an offset to align the top of the sprite
        //     transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        // }

        sightMeshController.LookDirection = SightMeshController.GetAngleFromVectorFloat(direction) + 90;
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
            playerStats.PlayerSpeed = playerStats.PlayerSpeed / playerStats.SneakSpeed; // Reduce speed for sneaking
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f); // Dim the sprite (reduce alpha to 50%)
            }
        }

        //When the sneak action is canceled, reset the player speed
        else if (context.canceled)
        {
            playerStats.PlayerSpeed = playerStats.OriginalSpeed; // Reset speed when not sneaking
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 1f); // Restore the sprite color (set alpha to 100%)
            }
        }


        // currently having MovingNoise commented since unsure if we want sneaking to cause no noise at all
        // rather than just reducing the noise level
        //MovingNoise();
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
        //hud.SetHealth(3.1f);
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
        isInvincible = true; // Set invincibility flag
        playerStats.PlayerSpeed = playerStats.PlayerSpeed * playerStats.DashSpeed; // Increase speed for dashing
        // Change sprite color to yellow to indicate invincibility
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 1f, 0f, 1f); // Yellow color
        }
        MovingNoise();
        yield return new WaitForSeconds(DASH_DURATION); // Dash duration
        playerStats.PlayerSpeed = playerStats.OriginalSpeed; // Reset speed after dashing
        isInvincible = false; // Reset invincibility flag
        // Reset sprite color to original
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f); // Original color
        }
        MovingNoise();
        yield return new WaitForSeconds(DASH_COOLDOWN); // Cooldown duration
        canDash = true; // Allow dashing again
    }


    //thought process behind DoInteraction
    //can seperate objects of interests based on what script is attached to the object
    //for example, If interacting with a potential item, it needs to have a children class script derived from Items
    
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
        // Get the mouse position in world coordinates
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    
        float detectionRadius = 0.5f; // Default radius for left, right, and down
        Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;

        if (direction.y > 0.5f) // If the direction is mostly upward
        {
            detectionRadius = 1.5f; // Increase radius when interacting above
        }

        // Detect all colliders within a radius of 1 unit around the player
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        foreach (var hitCollider in hitColliders)
        {
            // Ensure the mouse is over the object
            if (hitCollider.OverlapPoint(mousePosition))  // Check if the mouse is over this collider
            {
                // Check if the object has an Item script attached
                Item item = hitCollider.GetComponentInChildren<Item>();
                if (item != null)
                {
                    inventory.AddItem(hitCollider.gameObject);
                    Debug.Log("Picked up item: " + item.name);
                    return;
                }

                // Check if the object is an Ichor
                if (hitCollider.CompareTag("Ichor"))
                {
                    playerStats.IchorSamples++;
                    Debug.Log("Picked up Ichor Sample");
                    return;
                }

                // Check if the object is the stairs
                if (hitCollider.CompareTag("Stairs"))
                {
                    hitCollider.GetComponent<StairController>().TriggerLevelTransision();
                }

                // Check if the object is a teleportation shard
                if (hitCollider.CompareTag("TeleportationShard"))
                {
                    Destroy(hitCollider.gameObject);
                    playerStats.TeleportationShardCount++;
                    if (playerStats.TeleportationShardCount >= 3)
                    {
                        playerStats.TeleportationShardCount -= 3;
                        Instantiate(teleporterItemPrefab, transform.position, new Quaternion());
                    }
                }

                // Check for interactions with a Mimic
                EntityInfo entityInfo = hitCollider.GetComponent<EntityInfo>();
                if (entityInfo != null && entityInfo.Tags.Contains(EntityInfo.EntityTags.Mimic))
                {
                    Debug.Log("Interacted with a Mimic!");
                    MimicController mimicController = hitCollider.GetComponent<MimicController>();
                    //playerStats.TakeDamage(mimicController.AttackDamage);
                    mimicController.SwitchState<MimicRevealState>();
                    Debug.Log("Player Shield: " + playerStats.Shields);
                    return;
                }
            }
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
        inventory.DropActiveItem(context);
    }

    /*
        FUNCTION : OnScroll
        DESCRIPTION : This function calls a function from the inventory class is responsible for handling player scrolling through the inventory. 
                      It checks if the inventory is not empty and if it is not, the player can 
                      scroll through the inventory.
        PARAMETERS : InputAction.CallbackContext context - Input context for the scroll action.
        RETURNS :  NONE
    */
    private void OnScroll(InputAction.CallbackContext context)
    {
        inventory.Scrolling(context);
    }

    /*
        FUNCTION : OnSlot1Pressed
        DESCRIPTION : This function calls a function from the inventory class responsible for handling player selecting items based on the slot. 
                      It checks if the player has an item in the first slot and if it does, the item is selected.
        PARAMETERS : InputAction.CallbackContext context - Input context for the slot 1 action.
        RETURNS : NONE
    */
    // Functions to select items based on the slot
    private void OnSlot1Pressed(InputAction.CallbackContext context)
    {
        inventory.Slot1Pressed(context);
    }

    /*
        FUNCTION : OnSlot2Pressed
        DESCRIPTION : This function calls a function from the inventory class responsible for handling player selecting items based on the slot. 
                      It checks if the player has an item in the second slot and if it does, the item is selected.
        PARAMETERS : InputAction.CallbackContext context - Input context for the slot 2 action.
        RETURNS : NONE
    */
    private void OnSlot2Pressed(InputAction.CallbackContext context)
    {
        inventory.Slot2Pressed(context);
    }

    /*
        FUNCTION : OnSlot3Pressed
        DESCRIPTION : This function calls a function from the inventory class responsible for handling player selecting items based on the slot. 
                      It checks if the player has an item in the third slot and if it does, the item is selected.
        PARAMETERS : InputAction.CallbackContext context - Input context for the slot 3 action.
        RETURNS : NONE
    */
    private void OnSlot3Pressed(InputAction.CallbackContext context)
    {
        inventory.Slot3Pressed(context);
    }

    private void Fire(InputAction.CallbackContext context)
    {
        if(isParalyzed)
        {
            return;
        }
        activeItem = inventory.GetActiveItem();
        EntityInfo entityInfo = GetComponent<EntityInfo>();
        // activeItem.Use();
        if (activeItem != null && !activeItem.IsInUse && entityInfo != null)
        {
            activeItem.Use(entityInfo);
        }
    }

    private void MovingNoise()
    {
        playerStats.PlayerNoise = (int)(movementInput.magnitude * playerStats.PlayerSpeed); // Calculate noise level based on movement and speed
        //NoiseDetectionManager.Instance.NoiseEvent.Invoke(transform.position, playerStats.PlayerNoise);

        Debug.Log("Player noise level: " + playerStats.PlayerNoise);
    }

    private IEnumerator FlashBlue(SpriteRenderer spriteRenderer)
    {
        float elapsedTime = 0f;
        float flashDuration = 0.5f; // Duration of one flash cycle

        while (isParalyzed)
        {
            elapsedTime += Time.deltaTime;
            float lerpValue = Mathf.PingPong(elapsedTime, flashDuration) / flashDuration;
            spriteRenderer.color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(0f, 0f, 1f, 1f), lerpValue);
            yield return null;
        }

        // Reset color to original when no longer paralyzed
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }


    /*
        FUNCTION : DestroyInventory
        DESCRIPTION : The player's inventory needs to be destoryed when going back to the main menu or ending cutscene.
                      instead of making the inventory variable public, the function is stored in the player controller.
        PARAMETERS : NONE
        RETURNS : NONE
    */
    public void DestroyInventory()
    {
        List<Item> items = inventory.GetItems();
        for (int i = items.Count-1; i >= 0; i--)
        {
            if (items[i] != null)
            {
                Destroy(items[i].gameObject);
            }
        }
        items.Clear();
    }
}
