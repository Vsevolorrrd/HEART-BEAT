using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PauseMenu : Singleton<PauseMenu>
{
    [SerializeField] GameObject pauseMenuUI; // Assign pause menu UI in the Inspector
    [SerializeField] GameObject mainBlock;
    [SerializeField] GameObject settings;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] bool canOpenPauseMenu = true;
    [SerializeField] bool lockCursor = true;
    private bool isPaused = false;

    public static Action<bool> OnPause;

    private void Start()
    {
        if (sensitivitySlider)
        {
            sensitivitySlider.value = PlayerManager.Instance.controller.MouseSensitivity;
        }
    }
    void Update()
    {
        if (canOpenPauseMenu && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void OpenMainMenu()
    {
        SetPauseState(false);
        SceneLoader.Instance.LoadScene("MainMenu");
    }

    public void Restart()
    {
        SetPauseState(false);
        SceneLoader.Instance.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TogglePause()
    {
        SetPauseState(!isPaused);
    }

    private void SetPauseState(bool state)
    {
        isPaused = state;
        if (pauseMenuUI)
        pauseMenuUI.SetActive(isPaused);

        if (isPaused)
        {
            mainBlock.gameObject.SetActive(true);
            settings.gameObject.SetActive(false);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        OnPause?.Invoke(isPaused);
        PlayerManager.Instance.SetPalyerInput(!isPaused);
    }
    public void QuitGame()
    {
        SetPauseState(false);
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
    public void Stop(bool stop)
    {
        isPaused = stop;
        canOpenPauseMenu = !isPaused;

        if (isPaused)
        Time.timeScale = 0f;
        else
        Time.timeScale = 1f;

        OnPause?.Invoke(isPaused);
        PlayerManager.Instance.SetPalyerInput(!isPaused);
    }
    public void BlockPauseMenu(bool block)
    {
        canOpenPauseMenu = !block;
    }
    public void SetRhythmDifficulty(int difficulty)
    {
        RhythmDifficulty.Instance.SetDifficulty(difficulty);
    }
    public void UpdateSensitivity(float newValue)
    {
        PlayerManager.Instance.controller.MouseSensitivity = newValue;
    }
}