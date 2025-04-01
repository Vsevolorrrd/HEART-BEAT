using System.Collections;
using UnityEngine;

public interface IInteractable
{
    void Interact();
}

public class LevelInteraction : RhythmInput
{
    private FPSController controller;
    private CharacterController charController;

    [Header("Snap")]
    [SerializeField] Snap snap;

    [Header("Interaction Settings")]
    [SerializeField] Camera playerCam;
    [SerializeField] float maxRayDistance = 30f;
    [SerializeField] LayerMask interactionLayer;

    [Header("Echo Jump")]
    [SerializeField] float maxTeleportDistance = 30f;
    [SerializeField] float minTeleportDistance = 3f;
    [SerializeField] float teleportDuration = 0.2f;
    [SerializeField] AnimationCurve teleportCurve;
    [SerializeField] AudioClip echoJumpClip;

    [Header("FOV Settings")]
    [SerializeField] float fovIncrease = 15f;
    [SerializeField] float fovTransitionTime = 0.2f;

    [Header("Audio")]
    [SerializeField] AudioClip interactionSound;

    private IInteractable interactionPoint = null;
    private bool isTeleporting = false;


    private static LevelInteraction _instance;

    #region Singleton
    public static LevelInteraction Instance => _instance;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("LevelInteraction already exists, destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    #endregion

    protected override void Start()
    {
        base.Start();

        controller = GetComponent<FPSController>();
        charController = GetComponent<CharacterController>();
    }
    protected override void Update()
    {
        if (!playerInput || isBlocked || isTeleporting)
        return;

        if (Input.GetKeyDown(actionKey))
        {
            FindEchoPoint();
        }
    }
    private void FindEchoPoint()
    {
        var hits = Physics.RaycastAll(playerCam.transform.position, playerCam.transform.forward, maxTeleportDistance, interactionLayer);
        float closestDistance = float.MaxValue;
        EchoPoint bestEchoPoint = null;

        for (int i = 0; i < hits.Length; i++)
        {
            EchoPoint ep = hits[i].collider.GetComponent<EchoPoint>();

            if (ep != null) // Check if it has EchoPoint component
            {
                float distance = Vector3.Distance(transform.position, ep.transform.position);

                // Ensure it's not too close and not the previous teleport location
                if (distance >= minTeleportDistance)
                {
                    // Track the closest valid EchoPoint
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        bestEchoPoint = ep;
                    }
                }
            }
        }

        if (bestEchoPoint != null)
        {
            interactionPoint = bestEchoPoint;
            EvaluateTiming();
        }
        else
        {
            PerformInteraction();
        }
    }
    private void PerformInteraction()
    {
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, maxRayDistance, interactionLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactionPoint != null)
            {
                interactionPoint = interactable;
                EvaluateTiming();
            }
            else
            {
                snap.PerfomSnap(); // Snap if no interactable object is hit
            }
        }
        else
        {
            snap.PerfomSnap(); // Snap if no interactable object is hit
        }
    }
    protected override void OnPerfectHit()
    {
        interactionPoint.Interact();
    }
    protected override void OnGoodHit()
    {
        interactionPoint.Interact();
    }
    protected override void OnMiss()
    {
        
    }
    public void TeleportTo(Vector3 targetPosition)
    {
        if (isTeleporting) return;
        AudioManager.Instance.PlaySound(echoJumpClip, 0.8f);
        StartCoroutine(SmoothTeleport(targetPosition));
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
    }
}