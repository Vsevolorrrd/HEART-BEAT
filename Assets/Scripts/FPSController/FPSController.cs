using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FPSController : MonoBehaviour
{
    private Rigidbody rb;

    // Modules
    private HeadBobModule headBob;
    private SprintModule sprintModule;

    [Header("Camera Settings")]
    public Camera playerCamera;
    public bool invertCamera = false;
    public bool cameraCanMove = true;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 85f;

    [Header("Cursor")]
    public bool lockCursor = true;
    private float yaw = 0f;
    private float pitch = 0f;

    [Header("Movement")]
    public bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float maxVelocityChange = 10f;
    [HideInInspector] public float speedModifier = 1;
    [HideInInspector] public bool isMoving = false;
    [HideInInspector] public bool isWalking = false;
    private float startWalkSpeed;

    [Header("Jumping")]
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpPower = 5f;
    public int maxJumps = 1; // Total jumps allowed
    private int jumpCount = 0;
    private bool isGrounded = false;
    private bool isjumping = false;

    [Header("Coyote Time")]
    public float coyoteTime = 0.2f; // Extra time for jumping after leaving the ground
    private float coyoteTimer = 0f;

    [Header("Field of View (FOV)")]
    public bool enableFOV = true;
    public float fovMultiplier = 1.5f; // How much the FOV scales with speed
    public float sprintFOVStepTime = 10f;
    private float baseFOV;

    [Header("Footsteps")]
    public bool enableFootSteps = true;
    [SerializeField] AudioSource footStepsSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        baseFOV = playerCamera.fieldOfView;
    }

    void Start()
    {
        startWalkSpeed = walkSpeed;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (enableFootSteps)
        {
            footStepsSource.volume = 0;
            footStepsSource.loop = true;
        }

        MainMenu.OnPause += HandlePause;
        if (BEAT_Manager.Instance != null)
        {
            BEAT_Manager.MusicLevelIncreased += changePlayerStats;
            BEAT_Manager.Instance.OnMusicStart += StartFootsteps;
        }
    }
    private void Update()
    {
        if (!playerCanMove) // Stop movement & camera when paused
        return;

        CameraMovement();
        FOVAdjustment();
        Jumping();
        HeadBob();
        Footsteps();

        CheckGround();
    }

    private void FixedUpdate()
    {
        if (!playerCanMove) // Stop movement & camera when paused
        return;

        Movement();
    }
    private void CameraMovement()
    {
        if (!cameraCanMove) return;

        yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch += (invertCamera ? 1 : -1) * mouseSensitivity * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        transform.localEulerAngles = new Vector3(0, yaw, 0);
        playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
    }
    private void FOVAdjustment()
    {
        if (!enableFOV) return;

        float currentSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
        float targetFOV = baseFOV + (currentSpeed * fovMultiplier);
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, sprintFOVStepTime * Time.deltaTime);
    }
    private void Jumping()
    {
        if (!isGrounded) coyoteTimer -= Time.deltaTime;

        if (Input.GetKeyDown(jumpKey) && (jumpCount > 0 || coyoteTimer > 0f))
        {
            Jump();
        }
    }
    private void HeadBob()
    {
        if (headBob && isWalking)
        {
            float speed = sprintModule != null && sprintModule.IsSprinting ? sprintModule.sprintSpeed : walkSpeed;
            headBob.HeadBob(speed);
        }
    }
    private void Footsteps()
    {
        if (!enableFootSteps) return;
        footStepsSource.volume = isWalking ? 1 : 0;
    }
    private void Movement()
    {
        if (!playerCanMove) return;

        // Calculate how player should be moving
        Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        bool isCurrentlyMoving = targetVelocity.x != 0 || targetVelocity.z != 0; // Checks if player is walking and isGrounded

        isMoving = isCurrentlyMoving;
        isWalking = isGrounded && isCurrentlyMoving;

        targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed * speedModifier;

        // Apply a force
        Vector3 velocity = rb.linearVelocity;
        Vector3 velocityChange = targetVelocity - velocity;
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void CheckGround()
    {
        if (isjumping) return; // to prevent reseting the jump count

        Vector3 origin = transform.position + Vector3.down * (transform.localScale.y * 0.5f);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = 0.75f;

        // Sets isGrounded based on a raycast
        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
            isGrounded = true;
            coyoteTimer = coyoteTime; // Reset coyote timer when grounded
            jumpCount = maxJumps; // Reset jump count when grounded
        }
        else
        {
            isGrounded = false;
        }
    }
    private void Jump()
    {
        isjumping = true;
        Invoke("ResetJump", 0.2f);
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); // Reset vertical velocity
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);

        if (isGrounded || coyoteTimer > 0f) // If using coyote time or normal jump, reset jump count to allow double jump
        jumpCount = maxJumps - 1;
        else // If already in air, just reduce remaining jumps
        jumpCount--;

        isGrounded = false;
        coyoteTimer = 0f; // Prevent multiple jumps during coyote time
    }
    private void ResetJump() { isjumping = false; } // to prevent reseting the jump count

    #region events

    private void changePlayerStats(int level)
    {
        maxJumps = level;
        switch (level)
        {
            case 3:
                walkSpeed = startWalkSpeed * 2f;
                break;
            case 2:
                walkSpeed = startWalkSpeed * 1.50f;
                break;
            case 1:
                walkSpeed = startWalkSpeed;
                break;
        }
    }

    private void StartFootsteps(double startTime)
    {
        if (!enableFootSteps) return;
        footStepsSource.clip = BEAT_Manager.Instance.footStepsClip;
        footStepsSource.PlayScheduled(startTime);
    }

    private void HandlePause(bool isPaused)
    {
        playerCanMove = !isPaused;
    }
    private void OnDestroy()
    {
        MainMenu.OnPause -= HandlePause;
        if (BEAT_Manager.Instance != null)
        {
            BEAT_Manager.MusicLevelIncreased += changePlayerStats;
            BEAT_Manager.Instance.OnMusicStart -= StartFootsteps;
        }
    }

    #endregion
}