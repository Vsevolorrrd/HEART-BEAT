using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Assign your pause menu UI in the Inspector
    private bool isPaused = false;
    public bool canOpenPauseMenu = false;
    public bool lockCursor = false;

    public static Action<bool> OnPause;

    void Update()
    {
        if (canOpenPauseMenu && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void StartArena()
    {
        SetPauseState(false);
        SceneLoader.Instance.LoadScene("InfiniteArena");
    }
    public void StartGame()
    {
        SetPauseState(false);
        SceneLoader.Instance.LoadScene("Tutorial");
    }

    public void OpenMainMenu()
    {
        Time.timeScale =  1f;
        isPaused = false;
        SceneLoader.Instance.LoadScene("MainMenu");
    }

    public void Restart()
    {
        SceneLoader.Instance.LoadScene(SceneManager.GetActiveScene().name);
        SetPauseState(false);
    }

    public void TogglePause()
    {
        SetPauseState(!isPaused);
    }

    private void SetPauseState(bool state)
    {
        isPaused = state;
        if(pauseMenuUI)
        pauseMenuUI.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
        Cursor.lockState = isPaused ? CursorLockMode.None : (lockCursor ? CursorLockMode.Locked : CursorLockMode.None);
        Cursor.visible = isPaused || !lockCursor;

        OnPause?.Invoke(isPaused);
    }

    public void QuitGame()
    {
        SetPauseState(false);
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}