using UnityEngine;

public class CheckpointManager : Singleton<CheckpointManager>
{
    private Vector3 lastCheckpointPos;

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPos = position;
    }
    public Vector3 GetLastCheckpoint()
    {
        return lastCheckpointPos;
    }

}
