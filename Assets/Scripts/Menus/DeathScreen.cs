using UnityEngine;

public class DeathScreen : Singleton<DeathScreen>
{
    [SerializeField] GameObject deathScreen;
    [SerializeField] GameObject MainBlock;
    public void Death()
    {
        PauseMenu.Instance.Stop(true);
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
        MainBlock.SetActive(false);
        PauseMenu.Instance.Stop(false);
        SceneLoader.Instance.RestartScene();
    }
}