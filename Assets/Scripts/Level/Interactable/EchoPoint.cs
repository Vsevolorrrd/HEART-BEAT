using UnityEngine;

public class EchoPoint : RespondToBeat, IInteractable
{
    public void Interact()
    {
        LevelInteraction.Instance.TeleportTo(transform.position);
    }

    protected override void OnBeat()
    {
        // Scale the object up
        transform.localScale = Vector3.one * 1.2f;

        // Reset scale after 0.1s
        Invoke(nameof(ResetScale), 0.1f);
    }

    private void ResetScale()
    {
        transform.localScale = Vector3.one;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}