using UnityEngine;

public class RhythmDifficulty : Singleton<RhythmDifficulty>
{
    public static float perfectThreshold = 0.1f;  // 100 ms for Perfect
    public static float goodThreshold = 0.15f;    // 150 ms for Good

    [Header("Hard")]
    [SerializeField] float hardPerfectThreshold = 0.05f;
    [SerializeField] float hardGoodThreshold = 0.1f;

    [Header("Normal")]
    [SerializeField] float normalPerfectThreshold = 0.07f;
    [SerializeField] float normalGoodThreshold = 0.12f;

    [Header("Easy")]
    [SerializeField] float easyPerfectThreshold = 0.12f;
    [SerializeField] float easyGoodThreshold = 0.16f;


    public void SetDifficulty(int difficulty)
    {
        switch (difficulty)
        {
            case 4:
                ApplyDifficulty(5f, 5f); // always perfect
                break;
            case 3:
                ApplyDifficulty(hardPerfectThreshold, hardGoodThreshold);
                break;
            case 2:
                ApplyDifficulty(normalPerfectThreshold, normalGoodThreshold);
                break;
            case 1:
                ApplyDifficulty(easyPerfectThreshold, easyGoodThreshold);
                break;
        }
    }

    private void ApplyDifficulty(float newPerfect, float newGood)
    {
        perfectThreshold = newPerfect;
        goodThreshold = newGood;
    }
}