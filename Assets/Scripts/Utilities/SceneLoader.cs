using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] GameObject obj;
    [SerializeField] Music heartOfTheGod;

    private static SceneLoader _instance;

    #region Singleton
    public static SceneLoader Instance
    {
        get
        {
            // Check if the instance is already created
            if (_instance == null)
            {
                // Try to find an existing SceneLoader in the scene
                _instance = FindAnyObjectByType<SceneLoader>();

                // If no SceneLoader exists, create a new one
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("SceneLoader");
                    _instance = singletonObject.AddComponent<SceneLoader>();
                }
            }

            return _instance;
        }
    }

    void Awake()
    {
        // If the instance is already set, destroy this duplicate object
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;  // Assign this object as the instance
        }
    }
    #endregion

    private void Start()
    {
        if (obj)
        obj.SetActive(true);

        BEAT_Manager.Instance.StartTheMusic();
        if (SceneManager.GetActiveScene().name != "MainMenu")
        BeatUI.Instance.StartBeatUI();
    }
    public void LoadScene(string name)
    {
        StartCoroutine(TransitionToScene(name));
    }
    public void RestartScene()
    {
        StartCoroutine(TransitionToScene(SceneManager.GetActiveScene().name));
    }
    IEnumerator TransitionToScene(string name)
    {
        if (anim)
        anim.SetTrigger("Start");

        yield return new WaitForSeconds(1);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(name);

        // Wait until the new scene is fully loaded
        yield return null;
        yield return new WaitUntil(() => PlayerManager.Instance != null && BeatUI.Instance != null);

        if (anim)
        anim.SetTrigger("End");

        yield return new WaitForSeconds(1);

        PlayerManager.Instance.gameObject.transform.position =
        CheckpointManager.Instance.GetLastCheckpoint();

        if (SceneManager.GetActiveScene().name == "Tutorial")
        BEAT_Manager.Instance.SetNewMusic(heartOfTheGod);

        BEAT_Manager.Instance.StartTheMusic();
        BeatUI.Instance.StartBeatUI();
    }
}