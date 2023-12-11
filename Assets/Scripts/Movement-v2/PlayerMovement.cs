using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")] public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float airMultiplier;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")] public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")] public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        MyInput();
        SpeedControl();
        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, 0.75f);


        // check flyMode
        flyMode = GetFlyMode();
        rb.useGravity = !flyMode;
        if (flyMode)
        {
            // Apply forces only in the horizontal plane
            var horizontalForce = Vector3.ProjectOnPlane(moveDirection.normalized, Vector3.up);

            // Set the vertical velocity to zero
            var currentVelocity = rb.velocity;
            currentVelocity.y = 0f;
            rb.velocity = currentVelocity;

            // Add forces only in the horizontal plane
            rb.AddForce(horizontalForce * moveSpeed * airMultiplier, ForceMode.Force);
        }

        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (flyMode)
        {
        }


        // when to jump 
        if (Input.GetKey(jumpKey) && !spacePressedThisFrame)
        {
            // Set the flag to true to indicate that the space key was pressed in this frame
            spacePressedThisFrame = true;

            Jump();

            // --- Check for take-off ---

            // Check the time since the last space press
            var currentTime = Time.time;
            // If it's been less than 2 seconds since the last press, increment the count
            if (currentTime - lastSpacePressTime < 2f)
            {
                spacePressCount++;
                if (spacePressCount == 3) flyMode = true;
            }
            else
            {
                // Reset the count if more than 2 seconds have passed since the last press
                spacePressCount = 1;
            }

            // Update the last space press time
            lastSpacePressTime = currentTime;
        }

        // Check if the space key is released to reset the flag
        if (Input.GetKeyUp(KeyCode.Space)) spacePressedThisFrame = false;
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        // in air
        // in fly mode
        else if (flyMode)
        {
            if (verticalInput < 0) verticalInput = 0;
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

            // move ↓ : Check if the Shift key is pressed for descending
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                // Set the y-velocity for descent
                rb.AddForce(Vector3.down * moveSpeed * 50f, ForceMode.Force);

            // move ↑ : Check if the Space key is pressed for upward movement
            if (Input.GetKey(KeyCode.Space))
                // Set the y-velocity for descent
                rb.AddForce(Vector3.up * moveSpeed * 50f, ForceMode.Force);

            // decelerate
            if (Input.GetKey(KeyCode.S))
                rb.velocity = rb.velocity * 0.95f * Time.deltaTime;
        }
        // when falling (I guess)
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        var flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            var limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

        speed = flatVel.magnitude;
    }


    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    // --- Start Fly mechanics ---

    public bool flyMode;
    public float speed = 0f;
    public int spacePressCount = 0;
    public float lastSpacePressTime = 0f;

    // prevent multiple jumps when space is held more than one frame
    private bool spacePressedThisFrame = false;

    private bool GetFlyMode()
    {
        if (grounded) return false;
        return flyMode;
    }
}