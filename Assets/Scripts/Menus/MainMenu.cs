using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Collections.LowLevel.Unsafe;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuUI; // Assign pause menu UI in the Inspector
    [SerializeField] GameObject mainBlock;
    [SerializeField] GameObject settings;
    [SerializeField] Slider sensitivitySlider;
    private bool isPaused = false;
    public bool canOpenPauseMenu = false;
    public bool lockCursor = false;

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

    public void StartArena()
    {
        SetPauseState(false);
        SceneLoader.Instance.LoadScene("InfiniteArena");
    }
    public void StartGame()
    {
        SetPauseState(false);
        SceneLoader.Instance.LoadScene("Tutorial_Old");
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
    }

    public void QuitGame()
    {
        SetPauseState(false);
        Debug.Log("Quitting Game...");
        Application.Quit();
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