using UnityEngine;

public class ArenaTrigger : MonoBehaviour
{
    [SerializeField] ArenaSpawn arenaSpawn;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            arenaSpawn.StartArena();
            Destroy(gameObject);
        }
    }
}