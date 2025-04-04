using UnityEngine;

public class ArenaSpawn : MonoBehaviour
{
    [SerializeField] SpawnWaves spawnWaves;
    [SerializeField] ParticleSystem spawnEffect;
    [SerializeField] Transform[] spawPoints;
    [SerializeField] bool infinite = false;
    private int enemyCount;
    private int currentWave;

    private void Start()
    {
        currentWave = 0;
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
                ai.Stun(1f);// Stuns the enemy to give player time to react

                if (spawnEffect)
                Instantiate(spawnEffect, enemy.transform.position, Quaternion.identity);
            }
        }

        enemyCount += GetTotalEnemiesInWave();
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
    public void Died()
    {
        enemyCount--;
        if (enemyCount <= spawnWaves.minimumEnemies || enemyCount <= 0)
        {
            if (currentWave < spawnWaves.waves.Length || infinite)
            Spawn();
        }
    }
}