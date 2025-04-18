using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RhythmStreakManager : Singleton<RhythmStreakManager>
{
    private PlayerHealth playerHealth;

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
    private int currentMusicLevel = 1; // Start at level 1
    private float streak = 0f;


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
        playerHealth = PlayerManager.Instance.playerHealth;
    }

    private void Update()
    {
        streak -= RhythmDifficulty.streakDecayRate * Time.deltaTime;
        streak = Mathf.Clamp(streak, 0, maxStreak);
        UpdateUI();
        UpdateMusicLevel();
    }

    public void RegisterHit(float modifier)
    {
        streak += streakGain * modifier;
        playerHealth.Heal(modifier * 1.1f);
    }

    private void UpdateUI()
    {
        if (streakBar)
        streakBar.value = streak;
    }

    private void UpdateMusicLevel()
    {
        int newMusicLevel = 1;

        if (streak >= 200)
        {
            newMusicLevel = 3;
        }
        else if (streak >= 100)
        {
            newMusicLevel = 2;
        }
        if (newMusicLevel > currentMusicLevel)
        {
            streak += 10f;  // to prevent instant loss of the level
        }

        if (newMusicLevel != currentMusicLevel) // to only update when level changes
        {
            currentMusicLevel = newMusicLevel;
            BEAT_Manager.Instance.SetMusicLevel(newMusicLevel);

            if (streakBar)
            {
                if (newMusicLevel == 3)
                sliderFillImage.color = level3_Color;
                else if (newMusicLevel == 2)
                sliderFillImage.color = level2_Color;
                else
                sliderFillImage.color = level1_Color;
            }

            if (musicLevelText)
            {
                musicLevelText.color = sliderFillImage.color;
                musicLevelText.text = newMusicLevel.ToString();
            }
        }
    }
}