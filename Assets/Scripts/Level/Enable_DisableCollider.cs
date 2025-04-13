using UnityEngine;

public class Enable_DisableCollider : MonoBehaviour
{
    [SerializeField] GameObject[] enableObj;
    [SerializeField] GameObject[] disableObj;
    [SerializeField] ArenaDoor[] doorsToClose;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject obj in enableObj)
            {
                obj.SetActive(true);
            }
            foreach (GameObject obj in disableObj)
            {
                obj.SetActive(false);
            }
            foreach(ArenaDoor obj in doorsToClose)
            {
                obj.CloseDoor();
            }

            Destroy(gameObject);
        }
    }
}
