using UnityEngine;

[CreateAssetMenu(fileName = "SpawnWaves", menuName = "Scriptable Objects/SpawnWaves")]
public class SpawnWaves : ScriptableObject
{
    public Wave[] waves;
    public int minimumEnemies = 3;
}

[System.Serializable]
public class Wave
{
    public EnemiesToSpawn[] Enemies;
}

[System.Serializable]
public class EnemiesToSpawn
{
    public GameObject enemy;
    public int howManyToSpawn;
}