using UnityEngine;

namespace Movement_v2
{
    public class PlayerMovement : MonoBehaviour
    {
        [HideInInspector] public Rigidbody rb;
        public Transform orientation;
        private Vector3 moveDirection;

        public float moveSpeedGround;
        public float moveSpeedGroundSprint;
        public float moveSpeedAir;

        public float jumpForce;


        [HideInInspector] public bool grounded;
        [HideInInspector] public bool flyMode;


        // Jump/ Take-Off Mechanics
        [HideInInspector] public int spacePressCount = 0;

        [HideInInspector] public float lastSpacePressTime = 0f;

        // prevent multiple jumps when space is held more than one frame
        [HideInInspector] private bool spacePressedThisFrame = false;


        private float horizontalInput;
        private float verticalInput;

        private int _animIDGroundedSpeed;
        private int _animIDGroundedMotionSpeed;
        private int _animIDFlyingSpeed;
        private int _animIDJump;
        private int _animIDFall;
        private int _animIDFly;
        private int _animIDFlySpeed;
        private int _animIDGrounded;

        private Animator _animator;


        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;

            AssignAnimationIDs();
            _animator = GetComponent<Animator>();
        }

        private void AssignAnimationIDs()
        {
            _animIDGroundedSpeed = Animator.StringToHash("groundedSpeed");
            _animIDGroundedMotionSpeed = Animator.StringToHash("groundedMotionSpeed");
            _animIDFlyingSpeed = Animator.StringToHash("flyingSpeed");
            _animIDJump = Animator.StringToHash("isJumping");
            _animIDFly = Animator.StringToHash("isFlying");
            _animIDGrounded = Animator.StringToHash("isGrounded");
        }

        private void Update()
        {
            MyInput();
            SpeedControl();
            flyMode = GetFlyMode();

            // TODO: go in fly mode when ground is more than 10m down with RayCast
            // ground check
            grounded = Physics.Raycast(transform.position, Vector3.down, 0.75f);
            if (grounded)
            {
                _animator.SetBool(_animIDFly, false);
                _animator.SetBool(_animIDGrounded, true);
            }
            else
            {
                _animator.SetBool(_animIDGrounded, false);
            }
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void MyInput()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");


            // 3x jumps in 2 sec -> player enters flyMode
            if (!flyMode && Input.GetKey(KeyCode.Space) && !spacePressedThisFrame)
            {
                // Set the flag to true to indicate that the space key was pressed in this frame
                spacePressedThisFrame = true;
                Jump();

                // --- Check for take-off (3x Space in last 2 secs) ---
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
            else
            {
                _animator.SetBool(_animIDJump, false);
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
                // Remove fly
                _animator.SetBool(_animIDFly, false);
                _animator.SetFloat(_animIDGroundedSpeed, moveSpeedGround);
                rb.AddForce(moveDirection.normalized * moveSpeedGround * 10f, ForceMode.Force);
            }

            // in fly mode
            else if (flyMode)
            {
                // Remove jump
                _animator.SetBool(_animIDJump, false);
                // Add fly
                _animator.SetBool(_animIDFly, true);
                // Add the speed
                _animator.SetFloat(_animIDFlyingSpeed, moveSpeedAir);

                // move ↓ : SHIFT for descending movement
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    // Set the y-velocity for descent
                    moveDirection = Vector3.down * moveSpeedAir * 3f;

                // move ↑ : SPACE for upward movement
                if (Input.GetKey(KeyCode.Space))
                    // Set the y-velocity for descent
                    moveDirection = Vector3.up * moveSpeedAir * 3f;

                rb.AddForce(moveDirection, ForceMode.Force);


                // S for braking
                if (Input.GetKey(KeyCode.S))
                {
                    rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 0.03f);
                }
                else
                {
                    // Forward movement only
                    moveDirection = orientation.forward * verticalInput;
                    rb.AddForce(moveDirection.normalized * moveSpeedAir * 4f, ForceMode.Force);
                }
            }

            // when falling 
            else if (!grounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeedAir * 4f, ForceMode.Force);
            }
        }

        private void SpeedControl()
        {
            // var velocity = rb.velocity;
            // var flatVel = new Vector3(velocity.x, 0f, velocity.z);
            //
            // // limit velocity if needed
            // if (flatVel.magnitude > moveSpeed)
            // {
            //     var limitedVel = flatVel.normalized * moveSpeed;
            //     rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            // }
            //
            // speed = flatVel.magnitude;
        }


        private void Jump()
        {
            if (spacePressCount != 3)
            {
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                _animator.SetBool(_animIDJump, true);
            }
        }


        private bool GetFlyMode()
        {
            // Checks if Player is near the ground
            var groundIsNear = Physics.Raycast(transform.position, Vector3.down, 8f);
            if (!groundIsNear) return true;

            if (grounded) return false;
            return flyMode;
        }
    }
}