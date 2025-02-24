using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    private CharacterController controller;

    [Header("Camera Settings")]
    public CinemachineCamera playerCam;
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
    public float speed = 5f;
    public float acceleration = 10f;
    private float startSpeed;
    private float startAcceleration;
    private Vector3 moveDirection = Vector3.zero;
    [HideInInspector] public bool isMoving = false;

    [Header("Jumping")]
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpPower = 5f;
    public int maxJumps = 1;
    private int jumpCount = 0;
    private bool isGrounded = false;
    private bool isJumping = false;

    [Header("Gravity & Coyote Time")]
    public float gravity = 20f;
    public float coyoteTime = 0.2f;
    private float coyoteTimer = 0f;
    private float verticalVelocity = 0f;

    private float defaultFOV;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        defaultFOV = playerCam.Lens.FieldOfView;
    }

    void Start()
    {
        startSpeed = speed;
        startAcceleration = acceleration;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        MainMenu.OnPause += HandlePause;

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

        yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch += (invertCamera ? 1 : -1) * mouseSensitivity * Input.GetAxis("Mouse Y");
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

        if (Input.GetKeyDown(jumpKey) && (jumpCount > 0 || coyoteTimer > 0f))
        {
            Jump();
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
        }
    }
    private void Jump()
    {
        isJumping = true;
        Invoke("ResetJump", 0.2f);

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
        maxJumps = level;
        switch (level)
        {
            case 3:
                speed = startSpeed * 1.5f;
                acceleration = startAcceleration * 1.5f;
                break;
            case 2:
                speed = startSpeed * 1.2f;
                acceleration = startAcceleration * 1.2f;
                break;
            case 1:
                speed = startSpeed;
                acceleration = startAcceleration;
                break;
        }
    }

    private void HandlePause(bool isPaused)
    {
        playerCanMove = !isPaused;
    }
    private void OnDestroy()
    {
        MainMenu.OnPause -= HandlePause;

        BEAT_Manager.MusicLevelIncreased += changePlayerStats;
    }

    #endregion
}