using UnityEngine;

public class AIOnDeath : MonoBehaviour
{
    private InfiniteArenaSpawn spawner;

    public void SetSpawner(InfiniteArenaSpawn spaw)
    {
        spawner = spaw;
    }
    public void Dead()
    {
        spawner.Died();
    }
}
