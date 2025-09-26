using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : Singleton<PlayerManager>
{
    [HideInInspector] public PlayerHealth playerHealth;
    [HideInInspector] public PlayerInput playerInputSystem;
    [HideInInspector] public FPSController fpsController;
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public bool playerInput = true;

    protected override void OnAwake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        fpsController = GetComponent<FPSController>();
        playerInputSystem = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
    }
    public void SetPalyerInput(bool enable)
    {
        fpsController.SetInput(enable);
        playerInput = enable;
    }
    public void DisableCharacterController(float time = 0.01f)
    {
        characterController.enabled = false;
        SetPalyerInput(false);
        Invoke("EnableCollider", time);
    }
    private void EnableCollider()
    {
        characterController.enabled = true;
        SetPalyerInput(true);
    }
}
