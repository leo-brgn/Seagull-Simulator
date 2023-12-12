using UnityEngine;

namespace Movement_v2
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")] public float moveSpeed;

        public float groundDrag;
        public float airborneDrag;

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
                rb.drag = airborneDrag;
        }

        private void FixedUpdate()
        {
            // ground check
            grounded = Physics.Raycast(transform.position, Vector3.down, 0.75f);
            // check flyMode
            flyMode = GetFlyMode();

            MovePlayer();
        }

        private void MyInput()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");


            // 3x jumps in 2 sec -> player enters flyMode
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
            // Disable Gravity in FlyMode
            rb.useGravity = !flyMode;

            // calculate movement direction
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

            // on ground
            if (grounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            }

            // in fly mode
            else if (flyMode)
            {
                moveDirection = orientation.forward * verticalInput; // + orientation.right * horizontalInput;

                // rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

                // move ↓ : Check if the Shift key is pressed for descending
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    // Set the y-velocity for descent
                    rb.AddForce(Vector3.down * moveSpeed * 3f, ForceMode.Force);

                // move ↑ : Check if the Space key is pressed for upward movement
                if (Input.GetKey(KeyCode.Space))
                    // Set the y-velocity for descent
                    rb.AddForce(Vector3.up * moveSpeed * 3f, ForceMode.Force);


                // Decelerate when pressing the down key
                if (Input.GetKey(KeyCode.S))
                    // Interpolate the current velocity towards zero over time
                    rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 0.03f);
                else
                    // Forward movement only
                    rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            }
            // when falling (I guess)
            else if (!grounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            }
        }

        private void SpeedControl()
        {
            var velocity = rb.velocity;
            var flatVel = new Vector3(velocity.x, 0f, velocity.z);

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
            if (spacePressCount != 3) rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
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
}