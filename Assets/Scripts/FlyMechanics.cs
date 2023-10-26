using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FlyMechanics : MonoBehaviour
{

    Animator animator;
    Rigidbody rb;
    Transform targetObjectTransform;
    public CinemachineVirtualCamera flyCamera;
    public CinemachineVirtualCamera walkCamera;


    public float walkForwardSpeed = 2.0f;
    public float walkBackwardSpeed = 1.5f;
    public float rotateSpeed = 30.0f;
    public float jumpForce = 1.0f; 
    public float flyingSpeed = 1.0f;
    public float spaceKeyTimeToFly = 1.0f;
    public float spaceKeyPressedStartTime = float.MinValue;
    public float cameraRotationSpeed = 0.5f;
    public float flyingDuration = 50.0f;
    public float maxHeight = 100.0f;
    private float verticalRotation = 0.0f;

    private bool isGrounded = true;
    private bool isFlying = false;

    private readonly string jumpBool = "isJumping";
    private readonly string flyBool = "isFlying";
    private readonly string walkingBool = "isWalking";
    private readonly string walkingAnimationSpeed = "walkingSpeed";
   


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        targetObjectTransform = transform.Find("TargetObject");
        if (targetObjectTransform == null)
        {
            Debug.LogError("Child not found.");
        }
        walkCamera.gameObject.SetActive(true);
        flyCamera.gameObject.SetActive(false);
    }

    void Update()
    {
        
        if (isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                spaceKeyPressedStartTime = Time.time;
            }
            // Jumping
            if (Input.GetKey(KeyCode.Space))
            {
                animator.SetBool(walkingBool, false);
                if (spaceKeyPressedStartTime > 0 && (Time.time - spaceKeyPressedStartTime)  > Time.deltaTime)
                {
                    JumpTakeoff();
                } else
                {
                    Jump();
                }
            } else
            {
                spaceKeyPressedStartTime = float.MinValue;
                // Walking
                // Get inputs
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
            RotateAroundCharacter();

        }
        else
        {
            if (Input.GetKey(KeyCode.Space))
            {
                // TODO: what happens when jumping and not grounded
            } else
            {
                spaceKeyPressedStartTime = float.MinValue;
            }

            float raycastDistance = 0.01f; // Adjust this value as needed

            if (Physics.Raycast(transform.position, Vector3.down, out _, raycastDistance))
            {
                isGrounded = true;
                animator.SetBool(flyBool, false);
            }

            //targetObjectTransform.localEulerAngles = new Vector3(verticalRotation, targetObjectTransform.localEulerAngles.y, targetObjectTransform.localEulerAngles.z);

        }

        if (isFlying)
        {

            if (transform.position.z > maxHeight)
            {
                rb.velocity = Vector3.zero;
            }

            // Flying Mechanics
            float forwardInput = Input.GetKey(KeyCode.W) ? 1.0f : Input.GetKey(KeyCode.S) ? -1.0f : 0.0f;
            float horizontalInput = Input.GetKey(KeyCode.A) ? -1.0f : Input.GetKey(KeyCode.D) ? 1.0f : 0.0f;

            Vector3 flightDirection = transform.forward * forwardInput;

            // Apply flying force
            rb.AddForce(flightDirection * flyingSpeed, ForceMode.Impulse);

            // Limit the duration of flight
            flyingDuration += Time.deltaTime;
            if (flyingDuration >= spaceKeyTimeToFly)
            {
                isFlying = false;
                rb.useGravity = true;
            }

            // Limit the roll angle to 45 degrees
            float maxRollAngle = 45.0f;
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.z = Mathf.Clamp(eulerAngles.z, -maxRollAngle, maxRollAngle);
            transform.eulerAngles = eulerAngles;

            FixCameraTop();

        }

       
    }

    void Jump()
    {
        animator.SetTrigger(jumpBool);
        rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
        isGrounded = false;
    }

    void JumpTakeoff()
    {
        animator.SetBool(flyBool, true);
    }


    void RotateAroundCharacter()
    {
        targetObjectTransform.Rotate(new(0, 0, 0));
        walkCamera.gameObject.SetActive(true);
        flyCamera.gameObject.SetActive(false);
        /*
        float rotationPower = 1.0f; // Vous pouvez ajuster cette valeur selon vos besoins.

        // Obtenir les mouvements de la souris pour la rotation verticale
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Rotation horizontale (gauche/droite)
        targetObjectTransform.Rotate(Vector3.up * mouseX * rotationPower);

        // Rotation verticale (haut/bas) limitée
        verticalRotation -= mouseY * rotationPower;
        verticalRotation = Mathf.Clamp(verticalRotation, -80.0f, 80.0f); // Vous pouvez ajuster les angles limites.

        // Appliquer la rotation verticale au transform du targetObject
        targetObjectTransform.localEulerAngles = new Vector3(verticalRotation, targetObjectTransform.localEulerAngles.y, targetObjectTransform.localEulerAngles.z);
        */
    }


    void FixCameraTop()
    {
        // Rotation horizontale (gauche/droite)
        targetObjectTransform.Rotate(new(45, 0, 0));
        flyCamera.gameObject.SetActive(true);
        walkCamera.gameObject.SetActive(false);
    }


    /**
     * Events
     **/

    void JumpEvent()
    {
        rb.AddForce(3 * jumpForce * Vector3.up, ForceMode.Impulse);
        isGrounded = false;
        isFlying = true;
    }

    void AirForce()
    {
        rb.AddForce(3 * jumpForce * Vector3.up, ForceMode.Impulse);
    }

    


}
