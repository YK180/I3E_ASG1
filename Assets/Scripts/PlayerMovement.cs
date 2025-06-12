
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    //Movement
    public float moveSpeed;
    public float originalSpeed;

    public float groundDrag;

    //Jumping 
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    //Crouching
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    //KeyBinds
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;

    //Ground Check
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    //Slope Handling
    public float maxSlopeAngle;
    private RaycastHit slopeHit;

    public Transform orientation;
    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;

    public MovementState state;

    public enum MovementState
    {
        crouching
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;

        originalSpeed = moveSpeed;
    }

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

    private void SpeedControl()
    {
        

        //Limiting speed of slope
        if (OnSlope())
        {
            if(rb.linearVelocity.magnitude > moveSpeed)
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


    private void Jump()
    {

        //reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

    }

    private void ResetJump()
    {
        readyToJump = true;
    }

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

    private Vector3 GetSlopeMoveDirection()
    {   //Projected the force to be parallel to the slope instead of into the slope
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

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

    // Update is called once per frame
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
    }

    void FixedUpdate()
    {
        MovePlayer();
    }
}
