using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class EchoJump : RhythmInput
{
    private Rigidbody rb;

    [SerializeField] Camera playerCam;
    [SerializeField] float maxTeleportDistance = 30f;
    [SerializeField] float teleportDuration = 0.2f;
    [SerializeField] AnimationCurve teleportCurve;
    [SerializeField] AudioClip echoJumpClip;


    [Header("FOV Settings")]
    [SerializeField] private float fovIncrease = 15f;
    [SerializeField] private float fovTransitionTime = 0.2f;

    private float defaultFOV;

    private EchoPoint echoPoint = null;
    private bool isTeleporting = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        defaultFOV = playerCam.fieldOfView;
    }
    public override  void Update()
    {
        if (Input.GetKeyDown(actionKey) && !isTeleporting)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, maxTeleportDistance))
            {
                echoPoint = hit.collider.GetComponent<EchoPoint>();
                if (echoPoint != null)
                {
                    EvaluateTiming();
                }
            }
            else
            {
                Debug.Log("No echo point or echo point is outide of range");
            }
        }
    }

    public override void OnPerfectHit()
    {
        base.OnPerfectHit();
        AudioManager.Instance.PlaySound(echoJumpClip, 0.7f);
        StartCoroutine(SmoothTeleport(echoPoint.transform.position));
        echoPoint = null;
        Debug.Log("Echo Jumped!");
    }
    public override void OnGoodHit()
    {
        base.OnGoodHit();
        AudioManager.Instance.PlaySound(echoJumpClip, 0.7f);
        StartCoroutine(SmoothTeleport(echoPoint.transform.position));
        echoPoint = null;
        Debug.Log("Echo Jumped!");
    }
    private IEnumerator SmoothTeleport(Vector3 targetPosition)
    {
        isTeleporting = true;
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        // Increases FOV
        StartCoroutine(ChangeFOV(defaultFOV + fovIncrease, fovTransitionTime));

        // Play sound effect

        while (elapsedTime < teleportDuration)
        {
            float t = elapsedTime / teleportDuration; // Normalize time (0 to 1)
            float easedT = teleportCurve.Evaluate(t); // Apply animation curve
            transform.position = Vector3.Lerp(startPosition, targetPosition, easedT);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // Ensure exact final position
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); // Reset vertical velocity

        // resets FOV back to normal
        StartCoroutine(ChangeFOV(defaultFOV, fovTransitionTime));

        isTeleporting = false;
        Debug.Log("Echo Jump Complete!");
    }

    private IEnumerator ChangeFOV(float targetFOV, float transitionTime)
    {
        float startFOV = playerCam.fieldOfView;
        float elapsed = 0f;

        while (elapsed < transitionTime)
        {
            playerCam.fieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsed / transitionTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCam.fieldOfView = targetFOV; // Ensure exact FOV
    }
}