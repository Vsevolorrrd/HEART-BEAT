using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : Singleton<CheckpointManager>
{
    [SerializeField] List<CheckPoint> checkpoints;
    private int currentCheckpointIndex = -1;

    public void SetCheckpoint(CheckPoint point)
    {
        int index = checkpoints.IndexOf(point);
        if (index >= 0)
        {
            currentCheckpointIndex = index;
        }
    }
    public int GetLastCheckpointIndex()
    {
        if (currentCheckpointIndex >= 0 && currentCheckpointIndex < checkpoints.Count)
        {
            return currentCheckpointIndex;
        }

        return -1;
    }

    public void GetPlayerToCheckpoint(int index)
    {
        foreach (var point in checkpoints)
        point.loadLevelParts();

        if (index >= 0 && index < checkpoints.Count)
        {
            currentCheckpointIndex = index;
            Debug.Log($"checkpoint set: " + checkpoints[currentCheckpointIndex].name);
        }
        else
        {
            Debug.Log("no checkpoint found");
            return;
        }

        PlayerManager.Instance.DisableCharacterController();
        Transform player = PlayerManager.Instance.gameObject.transform;
        player.position = checkpoints[currentCheckpointIndex].gameObject.transform.position;
        player.rotation = checkpoints[currentCheckpointIndex].gameObject.transform.rotation;
        RhythmStreakManager.Instance.RegisterHit(checkpoints[currentCheckpointIndex].StartingRhythmStreak);
    }
}
