using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(JumpModule))]
public class FPSController : MonoBehaviour
{
    private CharacterController controller;
    private JumpModule jumpModule;

    [Header("Camera Settings")]
    [SerializeField] CinemachineCamera playerCam;
    [SerializeField] bool invertCamera = false;
    [SerializeField] bool cameraCanMove = true;
    [SerializeField] float maxLookAngle = 85f;
    public float MouseSensitivity = 2f;

    [Header("Cursor")]
    [SerializeField] bool lockCursor = true;
    private float yaw = 0f;
    private float pitch = 0f;

    [Header("Movement")]
    [SerializeField] bool playerCanMove = true;
    [SerializeField] float speed = 5f;
    [SerializeField] float acceleration = 10f;

    private float startSpeed;
    private float startAcceleration;
    private Vector3 moveDirection = Vector3.zero;
    [HideInInspector] public bool isMoving = false;

    [Header("Jumping")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] float jumpPower = 5f;
    [SerializeField] int maxJumps = 3;

    private float startJumpPower;
    private int jumpCount = 0;
    private bool isGrounded = false;
    private bool isJumping = false;
    private bool firstJumpUsed = false;

    [Header("Gravity & Coyote Time")]
    [SerializeField] float gravity = 20f;
    [SerializeField] float coyoteTime = 0.2f;
    [SerializeField] float coyoteTimer = 0f;
    [SerializeField] float verticalVelocity = 0f;

    [SerializeField] float defaultFOV;

    [Header("Sound Effects")]
    [SerializeField] AudioClip jumpSound;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        jumpModule = GetComponent<JumpModule>();
        defaultFOV = playerCam.Lens.FieldOfView;
    }

    void Start()
    {
        startSpeed = speed;
        startJumpPower = jumpPower;
        startAcceleration = acceleration;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        BEAT_Manager.MusicLevelIncreased += changePlayerStats;

    }
    private void Update()
    {
        if (!playerCanMove) // Stop movement & camera when paused
        return;

        CameraMovement();
        Jumping();

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

        yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * MouseSensitivity;
        pitch += (invertCamera ? 1 : -1) * MouseSensitivity * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        transform.localEulerAngles = new Vector3(0, yaw, 0);
        playerCam.transform.localEulerAngles = new Vector3(pitch, 0, 0);
    }
    public void AdjustFOV(float fovIncrease, float duration, float returnToDefaultDuration)
    {
        StopAllCoroutines();
        StartCoroutine(AdjustFOVCoroutine(fovIncrease, duration, returnToDefaultDuration));
    }

    private IEnumerator AdjustFOVCoroutine(float fovIncrease, float duration, float returnToDefaultDuration)
    {
        float targetFOV = defaultFOV + fovIncrease;
        float elapsedTime = 0f;

        // Increase FOV
        while (elapsedTime < duration)
        {
            playerCam.Lens.FieldOfView = Mathf.Lerp(defaultFOV, targetFOV, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        playerCam.Lens.FieldOfView = targetFOV;

        // Reset time
        elapsedTime = 0f;

        // Decrease FOV back
        while (elapsedTime < returnToDefaultDuration)
        {
            playerCam.Lens.FieldOfView = Mathf.Lerp(targetFOV, defaultFOV, elapsedTime / returnToDefaultDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        playerCam.Lens.FieldOfView = defaultFOV;
    }
    private void Jumping()
    {
        if (!isGrounded) coyoteTimer -= Time.deltaTime;

        // First jump can be done anytime
        if (!firstJumpUsed && Input.GetKeyDown(jumpKey) && (jumpCount > 0 || coyoteTimer > 0f))
        {
            Jump();
            firstJumpUsed = true;
            return;
        }

        // All other jumps must be on beat
        if (firstJumpUsed && Input.GetKeyDown(jumpKey) && jumpCount > 0)
        {
            jumpModule.CheckJump();
        }
    }
    private void Movement()
    {
        if (!playerCanMove) return;

        // Input-based movement direction
        Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        isMoving = inputDirection.x != 0 || inputDirection.z != 0;

        // Convert local input
        Vector3 move = transform.TransformDirection(inputDirection) * speed;
        // Apply movement acceleration
        moveDirection = Vector3.Lerp(moveDirection, move, acceleration * Time.deltaTime);

        // Apply gravity
        if (isGrounded)
        {
            verticalVelocity = -gravity * Time.deltaTime;
        }
        else
        {
            verticalVelocity -= gravity/1.2f * Time.deltaTime;
        }

        moveDirection.y = verticalVelocity;

        // Move the character
        controller.Move(moveDirection * Time.deltaTime);
    }

    private void CheckGround()
    {
        if (isJumping) return;

        isGrounded = controller.isGrounded;

        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
            jumpCount = maxJumps;
            firstJumpUsed = false;
        }
    }
    public void Jump()
    {
        isJumping = true;
        Invoke("ResetJump", 0.2f);

        AudioManager.Instance.PlayPooledSound(jumpSound, 0.8f);
        verticalVelocity = jumpPower; // Apply jump force

        if (isGrounded || coyoteTimer > 0f)
        jumpCount = maxJumps - 1;
        else
        jumpCount--;

        isGrounded = false;
        coyoteTimer = 0f;
    }
    private void ResetJump() { isJumping = false; } // to prevent reseting the jump count

    public void ResetVilocity(float velocity)
    {
        verticalVelocity = velocity;
    }
    public void ChangeSpeed(float modifier)
    {
        speed += modifier;
    }

    #region events

    private void changePlayerStats(int level)
    {
        switch (level)
        {
            case 3:
                jumpPower = startJumpPower * 1.2f;
                speed = startSpeed * 1.3f;
                acceleration = startAcceleration * 1.5f;
                break;
            case 2:
                jumpPower = startJumpPower * 1.1f;
                speed = startSpeed * 1.1f;
                acceleration = startAcceleration * 1.2f;
                break;
            case 1:
                jumpPower = startJumpPower;
                speed = startSpeed;
                acceleration = startAcceleration;
                break;
        }
    }

    public void SetInput(bool enable)
    {
        playerCanMove = enable;
    }
    private void OnDestroy()
    {
        BEAT_Manager.MusicLevelIncreased += changePlayerStats;
    }

    #endregion
}