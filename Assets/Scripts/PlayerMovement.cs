/*
* Author: Tan Ye Kai
* Date: 15/6/2025
* Description: PlayerMovement Script
*/

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Movement speed of the player while walking or standing.
    /// </summary>
    public float moveSpeed;

    
    /// <summary>
    /// The original movement speed, used to reset when not crouching.
    /// </summary>
    public float originalSpeed;

    /// <summary>
    /// Drag applied when the player is grounded.
    /// </summary>
    public float groundDrag;

    /// <summary>
    /// Upward force applied when the player jumps.
    /// </summary>
    public float jumpForce;

    /// <summary>
    /// Time to wait before the player can jump again.
    /// </summary>
    public float jumpCooldown;

    /// <summary>
    /// Multiplier for movement control while in air.
    /// </summary>
    public float airMultiplier;

    /// <summary>
    /// Determines if the player is ready to jump again.
    /// </summary>
    bool readyToJump;

    /// <summary>
    /// Movement speed while crouching.
    /// </summary>
    public float crouchSpeed;

    /// <summary>
    /// Y scale of the player while crouching.
    /// </summary>
    public float crouchYScale;

    /// <summary>
    /// Original Y scale of the player before crouching.
    /// </summary>
    private float startYScale;

    /// <summary>
    /// Key used to trigger a jump.
    /// </summary>
    public KeyCode jumpKey = KeyCode.Space;

    /// <summary>
    /// Key used to trigger crouch.
    /// </summary>
    public KeyCode crouchKey = KeyCode.LeftControl;
    
    /// <summary>
    /// Height of the player used to determine ground collision.
    /// </summary>
    public float playerHeight;

    /// <summary>
    /// LayerMask used to detect what is considered ground.
    /// </summary>
    public LayerMask whatIsGround;

    /// <summary>
    /// Whether the player is currently grounded.
    /// </summary>
    bool grounded;

    /// <summary>
    /// Maximum angle the player can walk on before sliding.
    /// </summary>
    public float maxSlopeAngle;

    /// <summary>
    /// RaycastHit to store slope surface info.
    /// </summary>
    private RaycastHit slopeHit;

    /// <summary>
    /// Reference to player orientation for movement direction.
    /// </summary>
    public Transform orientation;

    /// <summary>
    /// Horizontal input axis value.
    /// </summary>
    float horizontalInput;

    /// <summary>
    /// Vertical input axis value.
    /// </summary>
    float verticalInput;

    /// <summary>
    /// Calculated movement direction.
    /// </summary>
    Vector3 moveDirection;

    /// <summary>
    /// Rigidbody component reference.
    /// </summary>
    Rigidbody rb;

    /// <summary>
    /// Current movement state of the player (e.g., crouching).
    /// </summary>
    public MovementState state;

    /// <summary>
    /// Enumeration of possible movement states.
    /// </summary>
    public enum MovementState
    {
        crouching
    }

    /// <summary>
    /// Initializes player values and caches Rigidbody and scale.
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;

        originalSpeed = moveSpeed;
    }

    /// <summary>
    /// Handles user input for movement, jumping, and crouching.
    /// </summary>
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //When to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //Start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        //Stop Crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    /// <summary>
    /// Applies force to move the player based on input and ground/slope conditions.
    /// </summary>
    private void MovePlayer()
    {
        //Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //On ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        //In Air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        //On Slope
        if (OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            //Add downward force to keep the player constantly on the slope when going up
            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        //Turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    /// <summary>
    /// Resets the player's jump ability and starts a grounding buffer coroutine.
    /// </summary>
    public void ResetJumpState()
    {
        readyToJump = true;

        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);

        // Force a grounding buffer 
        StartCoroutine(TemporaryGroundedFix());

    }

    /// <summary>
    /// Coroutine to simulate grounding after jump reset.
    /// </summary>
    private IEnumerator TemporaryGroundedFix()
    {
        // Wait for 2 physics frames
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        // After physics has updated, allow the ground check to happen normally
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.ForceGroundedRaycast();

        }
    }

    /// <summary>
    /// Forces a ground raycast to manually check if the player is grounded.
    /// </summary>
    public void ForceGroundedRaycast()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        Debug.Log("Forced grounded raycast. Grounded = " + grounded);
    }

    /// <summary>
    /// Coroutine that waits one frame before forcefully checking if the player is grounded.
    /// </summary>
    private IEnumerator ForceGroundedBuffer()
    {
        yield return null;

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
    }

    /// <summary>
    /// Coroutine to temporarily set the player as grounded after a teleport or sudden movement.
    /// </summary>
    private IEnumerator RecheckGround()
    {
        // Wait for physics to settle after teleport
        yield return new WaitForEndOfFrame(); 
        
        // Temporarily force grounded
        grounded = true; 
    }

    /// <summary>
    /// Limits player's velocity based on movement state and slope.
    /// </summary>
    private void SpeedControl()
    {


        //Limiting speed of slope
        if (OnSlope())
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            //limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }


    }

    /// <summary>
    /// Applies an impulse force upward to simulate a jump.
    /// </summary>
    private void Jump()
    {

        //reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

    }

    /// <summary>
    /// Resets the jump flag so the player can jump again.
    /// </summary>
    private void ResetJump()
    {
        readyToJump = true;
    }

    /// <summary>
    /// Checks if the player is standing on a slope within a climbable angle.
    /// </summary>
    private bool OnSlope()
    {   //out slopeHit source the information of the object the player hits
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            //Caluculate of steep the slope the player is standing on
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    /// <summary>
    /// Projects movement direction along the slope surface.
    /// </summary>
    private Vector3 GetSlopeMoveDirection()
    {   //Projected the force to be parallel to the slope instead of into the slope
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    /// <summary>
    /// Sets the current player movement state and adjusts speed accordingly.
    /// </summary>
    private void StateHandler()
    {
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else
        {
            moveSpeed = originalSpeed;

        }

    }

    /// <summary>
    /// Called once per frame to check input, speed, state, and grounded status.
    /// </summary>
    void Update()
    {
        //groun check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        if (grounded)
            rb.linearDamping = groundDrag;

        else
            rb.linearDamping = 0;

        MyInput();
        SpeedControl();
        StateHandler();

        Debug.Log("Grounded: " + grounded + " | ReadyToJump: " + readyToJump);
    }

   /// <summary>
    /// Called at fixed time intervals to apply movement physics.
    /// </summary>
    void FixedUpdate()
    {
        MovePlayer();
    }
}
