using UnityEngine;
using UnityEngine.UI;

public class RhythmStreakManager : MonoBehaviour
{
    public static RhythmStreakManager Instance { get; private set; }

    [SerializeField] Slider streakBar;
    [SerializeField] float maxStreak = 200f;
    [SerializeField] float streakGain = 5f; // Streak gained per successful hit (whiout modifier)
    [SerializeField] float streakDecayRate = 2f; // Streak lost per second
    [SerializeField] float streak = 0f;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        streak = 0f;

        if (streakBar)
        {
            streakBar.maxValue = maxStreak;
            streakBar.value = 0;
        }
    }

    private void Update()
    {
        streak -= streakDecayRate * Time.deltaTime;
        streak = Mathf.Clamp(streak, 0, maxStreak);
        UpdateUI();
        UpdateMusicLevel();
    }

    public void RegisterHit(float modifier)
    {
        streak += streakGain * modifier;
    }

    private void UpdateUI()
    {
        if (streakBar)
        streakBar.value = streak;
    }

    private void UpdateMusicLevel()
    {
        if (streak >= 200)
        {
            BEAT_Manager.Instance.SetMusicLevel(3);
        }
        else if (streak >= 100)
        {
            BEAT_Manager.Instance.SetMusicLevel(2);
        }
        else
        {
            BEAT_Manager.Instance.SetMusicLevel(1);
        }
    }
}