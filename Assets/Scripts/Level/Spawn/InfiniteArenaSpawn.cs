using UnityEngine;

public class InfiniteArenaSpawn : MonoBehaviour
{
    [SerializeField] GameObject[] enemies;
    [SerializeField] Transform[] spawPoints;
    [SerializeField] int spawnPerWave;
    [SerializeField] protected int enemyCount;

    private void Start()
    {
        SetUpArena();
    }
    private void SetUpArena()
    {
        for (int i = 0; i < spawnPerWave; i++)
        {
            int R = Random.Range(0, spawPoints.Length);
            int R_E = Random.Range(0, enemies.Length);
            var enemy = Instantiate(enemies[R_E], spawPoints[R].position, Quaternion.identity);
            enemy.GetComponent<Actor>().faction = Actor.Faction.BadGuys;
            AIOnDeath damageScript = enemy.AddComponent<AIOnDeath>();
            damageScript.SetSpawner(this);
        }
        enemyCount = spawnPerWave;
    }
    public void Died()
    {
        enemyCount--;
        if (enemyCount <= 0)
        {
            SetUpArena();
        }
    }
}
