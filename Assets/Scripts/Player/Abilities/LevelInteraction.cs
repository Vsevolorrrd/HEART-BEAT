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

    [Header("Interaction Settings")]
    [SerializeField] Camera playerCam;
    [SerializeField] float maxRayDistance = 30f;
    [SerializeField] LayerMask interactionLayer;

    [Header("Snap")]
    [SerializeField] Animator snapAnim;
    [SerializeField] AudioClip snapClip;

    [Header("Echo Jump")]
    [SerializeField] float maxTeleportDistance = 30f;
    [SerializeField] float minTeleportDistance = 3f;
    [SerializeField] float teleportDuration = 0.2f;
    [SerializeField] AnimationCurve teleportCurve;
    [SerializeField] AudioClip echoJumpClip;

    [Header("FOV Settings")]
    [SerializeField] private float fovIncrease = 15f;
    [SerializeField] private float fovTransitionTime = 0.2f;

    private EchoPoint echoPoint = null;
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
            HandleKeyPress();
        }
    }
    protected override void OnPerfectHit()
    {
        PerformInteraction();
    }

    protected override void OnGoodHit()
    {
        PerformInteraction();
    }
    protected override void OnMiss()
    {
        Snap();
    }
    private void PerformInteraction()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, maxRayDistance, interactionLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                interactable.Interact();  // Calls the appropriate interaction
            }
            else
            {
                Snap(); // Snap if no interactable object is hit
            }
        }
        else
        {
            Snap(); // Snap if no interactable object is hit
        }
    }
    private void Snap()
    {
        if (snapAnim)
        snapAnim.SetTrigger("Snap");

        AudioManager.Instance.PlayPooledSound(snapClip, 0.8f);
    }

    public void TeleportTo(Vector3 targetPosition)
    {
        AudioManager.Instance.PlaySound(echoJumpClip, 0.8f);
        StartCoroutine(SmoothTeleport(targetPosition));
        //echoPoint = null;
    }
    private void FindEchoPoint()
    {
        RaycastHit[] hits = Physics.RaycastAll(playerCam.transform.position, playerCam.transform.forward, maxTeleportDistance);
        float closestDistance = float.MaxValue;
        EchoPoint bestEchoPoint = null;

        // Loop through all raycast hits
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
            echoPoint = bestEchoPoint;
            Debug.Log($"Selected EchoPoint at {closestDistance} meters.");
            EvaluateTiming();
        }
        else
        {
            Debug.Log("No valid EchoPoint found.");
            echoPoint = null;
        }
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
