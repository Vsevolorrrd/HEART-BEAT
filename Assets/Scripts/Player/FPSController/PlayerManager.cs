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
        OnAwake();
    }
    #endregion

    [HideInInspector] public PlayerHealth playerHealth;
    [HideInInspector] public FPSController controller;
    [HideInInspector] public bool playerInput = true;

    private void OnAwake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        controller = GetComponent<FPSController>();
    }
    public void SetPalyerInput(bool enable)
    {
        controller.SetInput(enable);
        playerInput = enable;
    }
}
