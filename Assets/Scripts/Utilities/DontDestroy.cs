using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private static DontDestroy instance; // to avoid duplicates

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}