using UnityEngine;

public class CrouchModule : MonoBehaviour
{
    private FPSController fpsController;

    public bool holdToCrouch = true;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public float crouchHeight = .75f;
    public float speedReduction = .5f;
    private bool isCrouched = false;
    private Vector3 originalScale;
    public bool CheckCrouch() { return isCrouched; }

    void Start()
    {
        fpsController = GetComponent<FPSController>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (Input.GetKeyDown(crouchKey) && !holdToCrouch)
        {
            Crouch();
        }

        if (Input.GetKeyDown(crouchKey) && holdToCrouch)
        {
            isCrouched = false;
            Crouch();
        }
        else if (Input.GetKeyUp(crouchKey) && holdToCrouch)
        {
            isCrouched = true;
            Crouch();
        }
    }
    private void Crouch()
    {
        // Stands player up to full height
        if (isCrouched)
        {
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
            fpsController.speed /= speedReduction;        // Brings walkSpeed back up to original speed

            isCrouched = false;
        }
        // Crouches player down to set height
        else
        {
            transform.localScale = new Vector3(originalScale.x, crouchHeight, originalScale.z);
            fpsController.speed *= speedReduction;        // Reduces walkSpeed

            isCrouched = true;
        }
    }
}