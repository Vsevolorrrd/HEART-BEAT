using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;

    #region Singleton
    public static PlayerManager Instance => _instance;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("PlayerManager already exists, destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    #endregion
}
