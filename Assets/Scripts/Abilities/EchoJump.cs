using UnityEngine;
using System.Collections;

public class EchoJump : RhythmInput
{
    [SerializeField] Camera playerCamera;
    [SerializeField] float maxTeleportDistance = 30f;
    [SerializeField] float teleportDuration = 0.5f;
    [SerializeField] AnimationCurve teleportCurve;
    private EchoPoint echoPoint = null;
    private bool isTeleporting = false;

    public override  void Update()
    {
        if (Input.GetKeyDown(actionKey) && !isTeleporting)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, maxTeleportDistance))
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
        StartCoroutine(SmoothTeleport(echoPoint.transform.position));
        echoPoint = null;
        Debug.Log("Echo Jumped!");
    }
    public override void OnGoodHit()
    {
        base.OnGoodHit();
        StartCoroutine(SmoothTeleport(echoPoint.transform.position));
        echoPoint = null;
        Debug.Log("Echo Jumped!");
    }
    private IEnumerator SmoothTeleport(Vector3 targetPosition)
    {
        isTeleporting = true;
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        // Play teleport-out effect

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

        // Play teleport-in effect

        isTeleporting = false;
        Debug.Log("Echo Jump Complete!");
    }
}