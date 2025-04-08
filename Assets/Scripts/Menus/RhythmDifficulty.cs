using UnityEngine;

public class RhythmDifficulty : Singleton<RhythmDifficulty>
{
    public static float perfectThreshold = 0.07f;  // 70 ms for Perfect
    public static float goodThreshold = 0.12f;    // 120 ms for Good
    public static float streakDecayRate = 5f;

    [Header("Hard")]
    [SerializeField] float hardPerfectThreshold = 0.05f;
    [SerializeField] float hardGoodThreshold = 0.1f;
    [SerializeField] float hardstreakDecayRate = 7f;

    [Header("Normal")]
    [SerializeField] float normalPerfectThreshold = 0.07f;
    [SerializeField] float normalGoodThreshold = 0.12f;
    [SerializeField] float normalstreakDecayRate = 5f;

    [Header("Easy")]
    [SerializeField] float easyPerfectThreshold = 0.12f;
    [SerializeField] float easyGoodThreshold = 0.16f;
    [SerializeField] float easystreakDecayRate = 3f;


    private void Start()
    {
        ApplyDifficulty(normalPerfectThreshold, normalGoodThreshold, normalstreakDecayRate);
    }
    public void SetDifficulty(int difficulty)
    {
        switch (difficulty)
        {
            case 4:
                ApplyDifficulty(5f, 5f, 1f); // always perfect
                break;
            case 3:
                ApplyDifficulty(hardPerfectThreshold, hardGoodThreshold, hardstreakDecayRate);
                break;
            case 2:
                ApplyDifficulty(normalPerfectThreshold, normalGoodThreshold, normalstreakDecayRate);
                break;
            case 1:
                ApplyDifficulty(easyPerfectThreshold, easyGoodThreshold, easystreakDecayRate);
                break;
        }
    }

    private void ApplyDifficulty(float newPerfect, float newGood, float newDecaySpeed)
    {
        perfectThreshold = newPerfect;
        goodThreshold = newGood;
        streakDecayRate = newDecaySpeed;
    }
}