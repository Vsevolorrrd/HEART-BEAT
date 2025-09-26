using UnityEngine;

public class DeathScreen : Singleton<DeathScreen>
{
    [SerializeField] GameObject deathScreen;
    [SerializeField] GameObject mainBlock;
    [SerializeField] GameObject screenToBlack;
    public void Death()
    {
        PauseMenu.Instance.Stop(true);
        screenToBlack.SetActive(false);
        deathScreen.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        BEAT_Manager.Instance.StopMusic();
    }
    public void MainMenu()
    {
        PauseMenu.Instance.OpenMainMenu();
    }
    public void Retry()
    {
        mainBlock.SetActive(false);
        PauseMenu.Instance.Stop(false);
        SceneLoader.Instance.RestartScene();
    }
    public void FallDeath()
    {
        PlayerManager.Instance.SetPalyerInput(false);
        screenToBlack.SetActive(true);
        Invoke("Death", 1.1f);
    }
}