using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 3f;  // Max range for interaction
    public KeyCode interactKey = KeyCode.E; // Interaction key
    public LayerMask interactableLayer; // Set this to "Interactable"

    private Interactable currentInteractable;

    private void Update()
    {
        CheckForInteractable();

        if (currentInteractable != null && Input.GetKeyDown(interactKey))
        {
            currentInteractable.Interact();
        }
    }

    private void CheckForInteractable()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 1.6f, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                if (interactable != currentInteractable)
                {
                    // Show prompt if new interactable
                    //UIManager.Instance.ShowInteractionPrompt(true);
                    currentInteractable = interactable;
                }
                return;
            }
        }

        // No valid interactable detected, hide prompt
        if (currentInteractable != null)
        {
            //UIManager.Instance.ShowInteractionPrompt(false);
            currentInteractable = null;
        }
    }
}