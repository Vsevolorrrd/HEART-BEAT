using UnityEngine;

public class NextLevel : MonoBehaviour
{
    [SerializeField] string nextLevel;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneLoader.Instance.LoadScene(nextLevel);
        }
    }
}
