using UnityEngine;

public class ArenaSpawn : MonoBehaviour
{
    [SerializeField] SpawnWaves spawnWaves;
    [SerializeField] ParticleSystem spawnEffect;
    [SerializeField] Transform[] spawPoints;
    [SerializeField] bool spawnInOrder = false;
    [SerializeField] bool infinite = false;

    [SerializeField] ArenaDoor[] doorsToClose;
    [SerializeField] ArenaDoor[] doorsToOpen;

    [Header("Debug")]
    [SerializeField] int enemyCount;
    [SerializeField] int heavyEnemyCount;

    private int currentWave;
    private int orderedSpawnIndex = 0;

    public void StartArena()
    {
        CloseDoors();
        currentWave = 0;
        heavyEnemyCount = 0;
        orderedSpawnIndex = 0;
        Spawn();
    }
    private void Spawn()
    {
        for (int i = 0; i < spawnWaves.waves[currentWave].Enemies.Length; i++)
        {
            for (int y = 0; y < spawnWaves.waves[currentWave].Enemies[i].howManyToSpawn; y++)
            {
                GameObject enemy = null;

                if (spawnInOrder)
                {
                    Transform spawnPoint = spawPoints[orderedSpawnIndex];
                    enemy = Instantiate(spawnWaves.waves[currentWave].Enemies[i].enemy, spawnPoint.position, Quaternion.identity);

                    orderedSpawnIndex = (orderedSpawnIndex + 1) % spawPoints.Length;
                }
                else
                {
                    int R = Random.Range(0, spawPoints.Length);
                    enemy = Instantiate(spawnWaves.waves[currentWave].Enemies[i].enemy, spawPoints[R].position, Quaternion.identity);
                }

                // Set the enemy's faction and attach AIOnDeath script
                enemy.GetComponent<Actor>().faction = Actor.Faction.BadGuys;
                AI ai = enemy.GetComponent<AI>();
                ai.ArenaSpawn = this;
                ai.Stun(2f);// Stuns the enemy to give player time to react

                if (spawnEffect)
                Instantiate(spawnEffect, enemy.transform.position, Quaternion.identity);
            }
        }

        enemyCount += GetTotalEnemiesInWave();
        heavyEnemyCount += GetTotalHeavyEnemiesInWave();
        currentWave++;
    }
    private int GetTotalEnemiesInWave()
    {
        int totalEnemies = 0;
        for (int i = 0; i < spawnWaves.waves[currentWave].Enemies.Length; i++)
        {
            totalEnemies += spawnWaves.waves[currentWave].Enemies[i].howManyToSpawn;
        }
        return totalEnemies;
    }
    private int GetTotalHeavyEnemiesInWave()
    {
        int totalHeavyEnemies = 0;
        for (int i = 2; i < spawnWaves.waves[currentWave].Enemies.Length; i++)
        {
            totalHeavyEnemies += spawnWaves.waves[currentWave].Enemies[i].howManyToSpawn;
        }
        return totalHeavyEnemies;
    }
    public void Died()
    {
        enemyCount--;

        if (spawnWaves.KillheavyEnemiesToProgress)
        {
            if ((enemyCount <= spawnWaves.minimumEnemies && heavyEnemyCount <= 0) || enemyCount <= 0)
            {
                if (currentWave < spawnWaves.waves.Length || infinite)
                {
                    Spawn();
                }
                else
                {
                    FinishArena();
                }
            }

            return;
        }

        if (enemyCount <= spawnWaves.minimumEnemies || enemyCount <= 0)
        {
            if (currentWave < spawnWaves.waves.Length || infinite)
            {
                Spawn();
            }
            else
            {
                FinishArena();
            }
        }
    }
    private void FinishArena()
    {
        OpenDoors();
        Debug.Log("Open");
    }
    private void CloseDoors()
    {
        for (int i = 0; i < doorsToClose.Length; i++)
        {
            doorsToClose[i].CloseDoor();
        }
    }
    private void OpenDoors()
    {
        for (int i = 0; i < doorsToOpen.Length; i++)
        {
            doorsToOpen[i].OpenDoor();
        }
    }
}