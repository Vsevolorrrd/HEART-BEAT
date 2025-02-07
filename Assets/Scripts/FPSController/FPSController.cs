using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FPSController : MonoBehaviour
{
    private Rigidbody rb;

    //Modules
    private HeadBobModule headBob;
    private SprintModule sprintModule;

    #region Camera Movement Variables

    [Header("Camera")]
    public Camera playerCamera;

    public bool invertCamera = false;
    public bool cameraCanMove = true;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;

    // Crosshair
    public bool lockCursor = true;
    private float yaw = 0f;
    private float pitch = 0f;

    #endregion

    public bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float maxVelocityChange = 10f;

    [HideInInspector] public float speedModifier = 1;
    [HideInInspector] public bool isMoving = false;
    [HideInInspector] public bool isWalking = false;


    [Header("Jump")]
    public bool enableJump = true;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpPower = 5f;

    // Internal Variables
    private bool isGrounded = false;

    [Header("Coyote Time")]
    public float coyoteTime = 0.2f; // Time window where jump is still allowed
    private float coyoteTimer = 0f;

    [Header("FOV")]
    public bool FOV = true;
    public float fovMultiplier = 1.5f; // How much the FOV scales with speed
    public float sprintFOVStepTime = 10f;
    private float baseFOV;

    [Header("FootSteps")]
    public bool footSteps = true;
    [SerializeField] AudioSource footStepsSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        baseFOV = playerCamera.fieldOfView;
    }

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        speedModifier = 1f;

        if (footSteps)
        {
            footStepsSource.volume = 0;
            footStepsSource.loop = true;
        }
    }

    private void Update()
    {
        #region Camera

        // Control camera movement
        if (cameraCanMove)
        {
            yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;

            if (!invertCamera)
            {
                pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
            }
            else
            {
                // Inverted Y
                pitch += mouseSensitivity * Input.GetAxis("Mouse Y");
            }

            // Clamp pitch between lookAngle
            pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

            transform.localEulerAngles = new Vector3(0, yaw, 0);
            playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
        }
        #endregion

        #region FOV

        if (FOV)
        {
            // (ignore vertical to prevent jumps affecting FOV
            float currentSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;

            // Calculate the target FOV based on speed
            float targetFOV = baseFOV + (currentSpeed * fovMultiplier);

            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, sprintFOVStepTime * Time.deltaTime);
        }

        #endregion

        #region Jump

        // Countdown coyote time when not grounded
        if (!isGrounded)
        {
            coyoteTimer -= Time.deltaTime;
        }

        // Allow jumping if within coyote time
        if (enableJump && Input.GetKeyDown(jumpKey) && coyoteTimer > 0f)
        {
            Jump();
        }

        #endregion

        #region HeadBob

        if (headBob)
        {
            if (isWalking)
            {
                if (sprintModule != null)
                {
                    if (sprintModule.CheckSprinting())
                    headBob.HeadBob(sprintModule.sprintSpeed);
                    else
                    headBob.HeadBob();
                }
            }
        }

        #endregion

        #region FootSteps

        if (footSteps)
        {
            if (isWalking)
            {
                footStepsSource.volume = 1;
            }
            else
            {
                footStepsSource.volume = 0;
            }
        }

        #endregion

        CheckGround();
    }

    void FixedUpdate()
    {
        if (playerCanMove)
        {
            // Calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            // Checks if player is walking and isGrounded
            if (targetVelocity.x != 0 || targetVelocity.z != 0)
            {
                isMoving = true;

                if (isGrounded) // for head bob
                isWalking = true;
                else
                isWalking = false;

            }
            else
            {
                isWalking = false;
                isMoving = false;
            }
            // All movement calculations
            targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed * speedModifier;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rb.linearVelocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
    }

    // Sets isGrounded based on a raycast sent straigth down from the player object
    private void CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = 0.75f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
            isGrounded = true;
            coyoteTimer = coyoteTime; // Reset coyote timer when grounded
        }
        else
        {
            isGrounded = false;
        }
    }

    private void Jump()
    {
        // Adds force to the player rigidbody to jump
        if (coyoteTimer > 0f)
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isGrounded = false;
            coyoteTimer = 0f; // Prevent multiple jumps during coyote time
        }

    }

    #region Foot Steps sync

    private void OnEnable()
    {
        if (BEAT_Manager.Instance != null)
        BEAT_Manager.Instance.OnMusicStart += StartFootsteps;
    }

    private void OnDisable()
    {
        if (BEAT_Manager.Instance != null)
        BEAT_Manager.Instance.OnMusicStart -= StartFootsteps;
    }

    private void StartFootsteps(double startTime)
    {
        if (footSteps)
        footStepsSource.PlayScheduled(startTime); // Sync with other tracks
    }

    #endregion
}