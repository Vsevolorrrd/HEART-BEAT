using UnityEngine;
using System.Collections;

public class EchoJump : RhythmInput
{
    private FPSController controller;
    private CharacterController charController;

    [SerializeField] Camera playerCam;
    [SerializeField] float maxTeleportDistance = 30f;
    [SerializeField] float teleportDuration = 0.2f;
    [SerializeField] AnimationCurve teleportCurve;
    [SerializeField] AudioClip echoJumpClip;

    [Header("FOV Settings")]
    [SerializeField] private float fovIncrease = 15f;
    [SerializeField] private float fovTransitionTime = 0.2f;

    private EchoPoint echoPoint = null;
    private bool isTeleporting = false;

    public override void Start()
    {
        base.Start();

        controller = GetComponent<FPSController>();
        charController = GetComponent<CharacterController>();
    }

    public override void Update()
    {
        if (!playerInput || isTeleporting) return;

        if (Input.GetKeyDown(actionKey))
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
                Debug.Log("No echo point or echo point is out of range");
            }
        }
    }

    public override void OnPerfectHit()
    {
        base.OnPerfectHit();
        AudioManager.Instance.PlaySound(echoJumpClip, 0.7f);
        StartCoroutine(SmoothTeleport(echoPoint.transform.position));
        echoPoint = null;
    }

    public override void OnGoodHit()
    {
        base.OnGoodHit();
        AudioManager.Instance.PlaySound(echoJumpClip, 0.7f);
        StartCoroutine(SmoothTeleport(echoPoint.transform.position));
        echoPoint = null;
    }

    private IEnumerator SmoothTeleport(Vector3 targetPosition)
    {
        isTeleporting = true;
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        // FOV effect
        controller.AdjustFOV(fovIncrease, fovTransitionTime, fovTransitionTime);

        while (elapsedTime < teleportDuration)
        {
            float t = elapsedTime / teleportDuration;
            float easedT = teleportCurve.Evaluate(t);
            Vector3 newPos = Vector3.Lerp(startPosition, targetPosition, easedT);
            
            charController.enabled = false;
            transform.position = newPos;
            charController.enabled = true;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset gravity after teleporting
        controller.ResetVilocity(0);
        transform.position = targetPosition;
        isTeleporting = false;
        Debug.Log("Echo Jump Complete!");
    }
}
