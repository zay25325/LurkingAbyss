using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public Sprite frame0, frame2, frame4, frame5, frame7, // Idle
                 frame8, frame10, frame9, frame11,        // Right (also used for Left)
                 frame1, frame3,                          // Down
                 frame6;                                  // Up

    private Vector2 moveDirection;
    private Vector2 lastMoveDirection = Vector2.down; // Initialize to face down
    
    private float animationTimer = 0f; 
    private float frameInterval = 0.15f; // Change frame every 0.15 seconds
    private int stepIndex = 0; // Alternates between 0 and 1 for stepping animation

    void Update() {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY).normalized;

        // **Idle Animation**
        if (moveDirection == Vector2.zero) {
            if (lastMoveDirection.x > 0) {
                spriteRenderer.sprite = frame8;  
                spriteRenderer.flipX = false;
            } 
            else if (lastMoveDirection.x < 0) {
                spriteRenderer.sprite = frame8;  // Reuse right sprite
                spriteRenderer.flipX = true;
            } 
            else if (lastMoveDirection.y < 0) spriteRenderer.sprite = frame0;  // Last down
            else spriteRenderer.sprite = frame6;  // Last up
            return;
        }

        // **Update Timer**
        animationTimer += Time.deltaTime;
        if (animationTimer >= frameInterval) {
            animationTimer = 0f; // Reset timer
            stepIndex = (stepIndex == 0) ? 1 : 0; // Toggle between 0 and 1
        }

        // **Moving Right**
        if (moveX > 0) {
            spriteRenderer.sprite = (stepIndex == 0) ? frame8 : frame9;
            spriteRenderer.flipX = false;  
            lastMoveDirection = Vector2.right;
        }
        // **Moving Left**
        else if (moveX < 0) {
            spriteRenderer.sprite = (stepIndex == 0) ? frame8 : frame9; // Reuse right animation
            spriteRenderer.flipX = true;
            lastMoveDirection = Vector2.left;
        }
        // **Moving Down**
        else if (moveY < 0) {
            spriteRenderer.sprite = (stepIndex == 0) ? frame1 : frame3;
            lastMoveDirection = Vector2.down;
        }
        // **Moving Up (WITH FLIP)**
        else if (moveY > 0) {
            spriteRenderer.sprite = frame6;
            spriteRenderer.flipX = (stepIndex == 0); // Flip the sprite every step
            lastMoveDirection = Vector2.up;
        }
    }
}

