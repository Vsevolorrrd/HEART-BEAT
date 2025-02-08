using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RhythmStreakManager : MonoBehaviour
{
    public static RhythmStreakManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] Slider streakBar;
    [SerializeField] Image sliderFillImage;
    [SerializeField] TextMeshProUGUI musicLevelText;
    [SerializeField] Color level3_Color = Color.red;
    [SerializeField] Color level2_Color = Color.yellow;
    [SerializeField] Color level1_Color = Color.white;

    [Header("Variables")]
    [SerializeField] float maxStreak = 300f;
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
        if (musicLevelText)
        {
            musicLevelText.text = "1";
        }
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

            if (streakBar)
            sliderFillImage.color = level3_Color;

            if (musicLevelText)
            {
                musicLevelText.color = level3_Color;
                MusicLevelUI(3);
            }
        }
        else if (streak >= 100)
        {
            BEAT_Manager.Instance.SetMusicLevel(2);

            if (streakBar)
            sliderFillImage.color = level2_Color;

            if (musicLevelText)
            {
                musicLevelText.color = level2_Color;
                MusicLevelUI(2);
            }
        }
        else
        {
            BEAT_Manager.Instance.SetMusicLevel(1);

            if (streakBar)
            sliderFillImage.color = level1_Color;

            if (musicLevelText)
            {
                musicLevelText.color = level1_Color;
                MusicLevelUI(1);
            }
        }
    }
    private void MusicLevelUI(int level)
    {
        musicLevelText.text = level.ToString();
    }
}