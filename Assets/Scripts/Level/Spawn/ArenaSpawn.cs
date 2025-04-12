using UnityEngine;

public class ArenaSpawn : MonoBehaviour
{
    [SerializeField] SpawnWaves spawnWaves;
    [SerializeField] ParticleSystem spawnEffect;
    [SerializeField] Transform[] spawPoints;
    [SerializeField] bool infinite = false;
    [SerializeField] int enemyCount;
    [SerializeField] int heavyEnemyCount;
    private int currentWave;

    private void Start()
    {
        currentWave = 0;
        heavyEnemyCount = 0;
        Spawn();
    }
    private void Spawn()
    {
        for (int i = 0; i < spawnWaves.waves[currentWave].Enemies.Length; i++)
        {
            for (int y = 0; y < spawnWaves.waves[currentWave].Enemies[i].howManyToSpawn; y++)
            {
                int R = Random.Range(0, spawPoints.Length);
                var enemy = Instantiate(spawnWaves.waves[currentWave].Enemies[i].enemy, spawPoints[R].position, Quaternion.identity);  // Spawn the enemy

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
                Spawn();
            }

            return;
        }

        if (enemyCount <= spawnWaves.minimumEnemies || enemyCount <= 0)
        {
            if (currentWave < spawnWaves.waves.Length || infinite)
            Spawn();
        }
    }
}