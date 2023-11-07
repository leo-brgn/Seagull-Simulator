using System.Collections;
using UnityEngine;

public class Move : MonoBehaviour
{
    [Header("Movement Parameters")]
    private Rigidbody rb;
    public float walkForwardSpeed = 2.0f;
    public float walkBackwardSpeed = 1.5f;
    public float rotateSpeed = 30.0f;
    public float jumpForce = 1.0f;

    [Header("Animation Parameters")]
    private Animator animator;

    [Header("Flying Parameters")]
    public float flyingSpeed = 0.2f;
    public float spaceKeyTimeToFly = 1.0f;
    private float spaceKeyPressedStartTime = -1f;
    public float maxHeight = 100.0f;
    public float raycastDistance = 0.01f; // Distance to the floor to detect landing
    public float rollSpeed = 5f; // Speed of roll
    public float pitchSpeed = 5f; // Speed of pitch
    public float accelerationRate = 5f; // Rate of acceleration and deceleration
    public float minFlyingSpeed = 0.1f; // Minimum flying speed
    public float maxFlyingSpeed = 3f; // Maximum flying speed

    public bool isGrounded = true;
    public bool isFirstLevel = false;
    private bool isFlying = false;
    private readonly string jumpBool = "isJumping";
    private readonly string flyBool = "isFlying";
    private readonly string walkingBool = "isWalking";
    private readonly string walkingAnimationSpeed = "walkingSpeed";

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        if (!isGrounded)
        {
            isFlying = true;
            animator.SetBool(flyBool, true);
        }
    }

    void Update()
    {
        if (isFirstLevel)
        {
            isGrounded = false;
        }

        if (isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                spaceKeyPressedStartTime = Time.time;
            } else if (Input.GetKey(KeyCode.Space))
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("walk"))
                {
                    animator.SetBool(walkingBool, false);
                }
                if ((Time.time - spaceKeyPressedStartTime) > spaceKeyTimeToFly)
                {
                    animator.SetBool(walkingBool, false);
                    animator.SetBool(flyBool, true);
                }
                else
                {
                    Jump();
                }
            } else
            {
                spaceKeyPressedStartTime = -1f;
                Walk();
            }

        }

        if (!isFirstLevel && !isGrounded && Physics.Raycast(transform.position, Vector3.down, out _, raycastDistance))
        {
            isGrounded = true;
            animator.SetBool(flyBool, false);
        }

        if(isFlying)
        {
            animator.SetBool(walkingBool, false);
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(LandingSequence());
            } else
            {
                Fly();
            }
        }
    }

    void Walk()
    {
        float forwardInput = Input.GetKey(KeyCode.W) ? 1.0f : 0.0f;
        float backwardInput = Input.GetKey(KeyCode.S) ? -1.0f : 0.0f;
        float horizontalInput = Input.GetKey(KeyCode.A) ? -1.0f : Input.GetKey(KeyCode.D) ? 1.0f : 0.0f;

        float verticalInput = forwardInput + backwardInput;

        if (verticalInput != 0 || horizontalInput != 0)
        {
            transform.Translate(Time.deltaTime * verticalInput * (verticalInput > 0 ? walkForwardSpeed : walkBackwardSpeed) * Vector3.forward);
            transform.Rotate(horizontalInput * Time.deltaTime * rotateSpeed * Vector3.up);
            // Set walking animation active and speed
            animator.SetBool(walkingBool, true);
            animator.SetFloat(walkingAnimationSpeed, 0.5f + (verticalInput > 0 ? walkForwardSpeed : walkBackwardSpeed) * verticalInput);
        }
        else
        {
            // Set walking animation active and speed
            animator.SetBool(walkingBool, false);
            animator.SetFloat(walkingAnimationSpeed, 0);
        }
    }

    void Jump()
    {
        animator.SetTrigger(jumpBool);
        rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
        isGrounded = false;
    }

    void Fly()
    {
        // Inputs for roll and pitch
        float rollInput = Input.GetAxis("Horizontal"); // A and D for roll
        float pitchInput = Input.GetAxis("Vertical"); // W and S for pitch

        // Adjust speed with acceleration and deceleration using A and E
        if (Input.GetKey(KeyCode.A))
        {
            flyingSpeed -= accelerationRate * Time.deltaTime; // Decelerate
        }
        else if (Input.GetKey(KeyCode.E))
        {
            flyingSpeed += accelerationRate * Time.deltaTime; // Accelerate
        }

        flyingSpeed = Mathf.Clamp(flyingSpeed, minFlyingSpeed, maxFlyingSpeed);

        // Transform the airplane based on input
        transform.Rotate(pitchInput * pitchSpeed * Time.deltaTime, 0, -rollInput * rollSpeed * Time.deltaTime, Space.Self);

        // Move the airplane forward
        transform.position += transform.forward * flyingSpeed * Time.deltaTime;
    }


    IEnumerator TakeOffSequence()
    {
        // Initialize transition
        float time = 0;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + Vector3.up * maxHeight;

        rb.useGravity = false;
        isGrounded = false;
        animator.SetBool(flyBool, true);

        while (time < spaceKeyTimeToFly)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, time / spaceKeyTimeToFly);
            time += Time.deltaTime;
            yield return null;
        }

        isFlying = true;
    }

    IEnumerator LandingSequence()
    {
        // Initialize transition
        float time = 0;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(startPosition.x, 0, startPosition.z); // Make sure 0 is the ground height

        while (time < spaceKeyTimeToFly)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, time / spaceKeyTimeToFly);
            time += Time.deltaTime;
            yield return null;
        }

        rb.useGravity = true;
        isGrounded = true;
        isFlying = false;
        animator.SetBool(flyBool, false);
        rb.velocity = Vector3.zero;
    }

    void TakeoffEvent()
    {
        StartCoroutine(TakeOffSequence());
    }

    void JumpEvent()
    {
        rb.AddForce(3 * jumpForce * Vector3.up, ForceMode.Impulse);
        isGrounded = false;
        isFlying = true;
    }
}