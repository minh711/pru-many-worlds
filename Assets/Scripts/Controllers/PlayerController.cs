using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;

    Vector2 movementInput;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Animator animator;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (GameData.Instance.IsSetPosition)
        {
            Vector2 startPosition = new Vector2((float)GameData.Instance.PlayerX, (float)GameData.Instance.PlayerY);
            rb.position = startPosition;
        }
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            //If movement input is not 0, try to move
            if (movementInput != Vector2.zero)
            {
                bool success = TryMove(movementInput);
                if (!success)
                {
                    success = TryMove(new Vector2(movementInput.x, 0));
                }
                if (!success)
                {
                    success = TryMove(new Vector2(0, movementInput.y));
                }
                animator.SetBool("isMoving", success);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
            //Set direction of sprite to movement direction
            if (movementInput.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (movementInput.x > 0)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    private bool TryMove(Vector2 direction)
    {
        //if(direction != Vector2.zero)
        //{
        //    //Check fot potential collisions
        //    int count = rb.Cast(
        //        movementInput, // X and Y values -1 and 1 that represent the direction from the body to look for collisions
        //        movementFilter, // The settings that determine where a collision can occur on such as layers to collide with
        //        castCollisions, // List of collsions to store the found collisions into after the Cast is finished
        //        moveSpeed * Time.fixedDeltaTime + collisionOffset); // The amount to cast equal to the movement plus an offset

        //    //Filter out collisions with objects tagged as "Door"
        //    //for (int i = count - 1; i >= 0; i--)
        //    //{
        //    //    if (castCollisions[i].collider.CompareTag("Door"))
        //    //    {
        //    //        castCollisions.RemoveAt(i);
        //    //    }
        //    //}

        //    if (count == 0)
        //    {
        //        rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime);
        //        return true;
        //    } else
        //    {
        //        return false;
        //    }
        //} else
        //{
        //    //Can't move if there's no direction to move in
        //    return false;
        //}
        rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime);
        return true;
    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnInteract()
    {
        animator.SetTrigger("attack");
    }

    void OnActivate()
    {
        animator.SetTrigger("attack");
    }

    public void LockMovement()
    {
        canMove = false;
    }

    public void unlockMovement()
    {
        canMove = true;
    }
}