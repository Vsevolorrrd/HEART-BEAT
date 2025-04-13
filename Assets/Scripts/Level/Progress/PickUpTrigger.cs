using UnityEngine;

public class PickUpTrigger : RespondToBeat
{
    [SerializeField] ArenaSpawn ArenaSpawn;
    [SerializeField] GameObject shotgun;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Interact();
        }
    }
    public void Interact()
    {
        shotgun.SetActive(true);
        ArenaSpawn.StartArena();
        Destroy(gameObject);
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
}