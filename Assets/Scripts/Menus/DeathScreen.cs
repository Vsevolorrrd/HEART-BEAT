using UnityEngine;

public class DeathScreen : Singleton<DeathScreen>
{
    [SerializeField] GameObject deathScreen;
    public void Death()
    {
        PauseMenu.Instance.Stop(true);
        deathScreen.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void MainMenu()
    {
        PauseMenu.Instance.OpenMainMenu();
    }
    public void Retry()
    {

    }
}