using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] GameObject[] levelPartsToDeload;
    public float StartingRhythmStreak = 30f;
    private bool used = false;

    private void DeloadLevelParts()
    {
        foreach (GameObject obj in levelPartsToDeload)
        {
            obj.SetActive(false);
        }
    }
    public void loadLevelParts()
    {
        used = false;
        foreach (GameObject obj in levelPartsToDeload)
        {
            obj.SetActive(true);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (used) return;

        if (other.CompareTag("Player"))
        {
            CheckpointManager.Instance.SetCheckpoint(this);
            DeloadLevelParts();
            used = true;
        }
    }
}