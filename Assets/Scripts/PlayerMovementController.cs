using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif
using Cinemachine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement Parameters")]
    [Tooltip("Move speed of the character in m/s")]
    public float WalkSpeed = 2.0f;
    [Tooltip("Sprint speed of the character in m/s")]
    public float RunSpeed = 5.335f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 30.0f;
    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;
    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;
    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool isGrounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Header("Camera")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;
    [Tooltip("How far in degrees can you move the camera down")]

    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;
    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false; // For locking the camera position on all axis

    [Header("Flying Parameters")]
    public float flyingSpeed = 0.2f;
    public float spaceKeyTimeToFly = 1.0f;
    public float maxHeight = 100.0f;
    public float rollSpeed = 5f; // Speed of roll
    public float pitchSpeed = 5f; // Speed of pitch
    public float accelerationRate = 5f; // Rate of acceleration and deceleration
    public float minFlyingSpeed = 0.1f; // Minimum flying speed
    public float maxFlyingSpeed = 3f; // Maximum flying speed
    public bool isFirstLevel = false;

    private bool isFlying = false;
    // private bool isLanding = false;
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private int _animIDGroundedSpeed;
    private int _animIDGroundedMotionSpeed;
    private int _animIDFlyingSpeed;
    private int _animIDJump;
    private int _animIDFall;
    private int _animIDFly;
    private int _animIDGrounded;

    private Vector2 worldMin = new(60f, 230f); // Coin inférieur gauche de la carte réelle
    private Vector2 worldMax = new(190f, 330f); // Coin supérieur droit de la carte réelle

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif

    private Animator _animator;
    private CharacterController _controller;
    private GameObject _mainCamera;

    private SeagullInputs _input;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private const float _threshold = 0.01f;

    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
            return false;
#endif
        }
    }

    private void Awake()
    {
        // get a reference to our main camera
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    void Start()
    {
        _input = GetComponent<SeagullInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
        Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
        AssignAnimationIDs();

        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;

        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        if (!isGrounded)
        {
            isFlying = true;
            _animator.SetBool(_animIDFly, true);
        }

        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    private void LateUpdate()
    {
        CameraRotation();
    }


    private void AssignAnimationIDs()
    {
        _animIDGroundedSpeed = Animator.StringToHash("groundedSpeed");
        _animIDGroundedMotionSpeed = Animator.StringToHash("groundedMotionSpeed");
        _animIDFlyingSpeed = Animator.StringToHash("flyingSpeed");
        _animIDJump = Animator.StringToHash("isJumping");
        _animIDFly = Animator.StringToHash("isFlying");
        _animIDGrounded = Animator.StringToHash("isGrounded");
        _animIDFall = Animator.StringToHash("FreeFall");

    }

    void Update()
    {
        JumpAndGravity();
        if (isFirstLevel)
        {
            isGrounded = false;
        }
        else
        {
            DoFly();
            GroundedCheck();
        }
        Move();
    }

    void DoFly()
    {
        if (isFlying)
        {
            if (_input.fly)
            {
                // isLanding = true;
                StartCoroutine(LandingSequence());
            }
        }
        if (isGrounded)
        {
            if (_input.fly)
            {
                StartCoroutine(TakeOffSequence());
            }
        }
    }

    void Move()
    {
        if (!isFlying)
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? RunSpeed : WalkSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);


            _animator.SetFloat(_animIDGroundedSpeed, _animationBlend);
            _animator.SetFloat(_animIDGroundedMotionSpeed, inputMagnitude);
        }
        else
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

        // The world is bounded by a rectangle with corners worldMin and worldMax
        // If the player goes near the edge, wind pushes them back with strength proportional to distance from the edge
        Vector3 playerPos = transform.position;
        float _threshold = 10f;
        if (playerPos.x < worldMin.x + _threshold)
        {
            transform.position += new Vector3(1f, 0f, 0f) * (worldMin.x + _threshold - playerPos.x);
        }
        else if (playerPos.x > worldMax.x - _threshold)
        {
            transform.position += new Vector3(-1f, 0f, 0f) * (playerPos.x - (worldMax.x - _threshold));
        }
    }

    void JumpAndGravity()
    {
        if (isGrounded)
        {
            _fallTimeoutDelta = FallTimeout;

            _animator.SetBool(_animIDJump, false);

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                _animator.SetBool(_animIDJump, true);
            }
            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = JumpTimeout;
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _animator.SetBool(_animIDJump, false);
            }
            // if we are not grounded, do not jump
            _input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }


    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        _animator.SetBool(_animIDGrounded, isGrounded);

    }


    IEnumerator TakeOffSequence()
    {
        // Initialize transition
        float time = 0;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + Vector3.up * maxHeight;
        _input.fly = false;
        isGrounded = false;
        _animator.SetBool(_animIDFly, true);

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
        Vector3 endPosition = new Vector3(startPosition.x, 0.1f, startPosition.z); // Make sure 0 is the ground height
        _input.fly = false;

        while (time < spaceKeyTimeToFly)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, time / spaceKeyTimeToFly);
            time += Time.deltaTime;
            yield return null;
        }

        isGrounded = true;
        isFlying = false;
        _animator.SetBool(_animIDFly, false);
    }

    void TakeoffEvent()
    {
        StartCoroutine(TakeOffSequence());
    }

    void JumpEvent()
    {
        // TODO
    }


    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
        }
        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                        _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}