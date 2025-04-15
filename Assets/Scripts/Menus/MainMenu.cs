using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainBlock;
    [SerializeField] GameObject settings;
    [SerializeField] Slider sensitivitySlider;

    private void Start()
    {
        if (sensitivitySlider)
        {
            sensitivitySlider.value = PlayerManager.Instance.controller.MouseSensitivity;
        }
    }
    public void StartArena()
    {
        mainBlock.SetActive(false);
        SceneLoader.Instance.LoadScene("InfiniteArena");
    }
    public void StartGame()
    {
        mainBlock.SetActive(false);
        SceneLoader.Instance.LoadScene("Tutorial");
    }
    public void OpenMainMenu()
    {
        SceneLoader.Instance.LoadScene("MainMenu");
    }
    public void QuitGame()
    {
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