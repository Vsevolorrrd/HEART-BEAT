using UnityEngine;
using System.Collections;

public abstract class RhythmInput : MonoBehaviour
{
    [Header("RhythmInput")]
    public float thresholdModifier = 0.1f;  // 100 ms for Perfect
    public float streakGainPerfect = 2f;
    public float streakGainGood = 1f;
    public KeyCode actionKey = KeyCode.Space;

    [Header("Anti Spam")]
    public int maxPresses = 3;
    protected float pressWindow = 0.4f; // Time window for counting presses
    protected float inputCooldown = 1.5f; // Cooldown after max presses
    protected int pressCount = 0;
    protected float firstPressTime = -999f;
    protected bool isBlocked = false;

    protected virtual void Update()
    {
        if (!PlayerManager.Instance.playerInput || isBlocked)
        return;

        if (Input.GetKeyDown(actionKey))
        {
            HandleKeyPress();
        }
    }
    protected virtual void HandleKeyPress()
    {
        float currentTime = Time.time;

        // Reset count if outside press window
        if (currentTime - firstPressTime > pressWindow)
        {
            firstPressTime = currentTime;
            pressCount = 0;
        }

        pressCount++;

        if (pressCount >= maxPresses)
        {
            StartCoroutine(BlockInput());
            return;
        }

        EvaluateTiming();
    }

    protected IEnumerator BlockInput()
    {
        isBlocked = true;
        Debug.Log($"Input blocked for {inputCooldown} seconds after {maxPresses} presses!");

        yield return new WaitForSeconds(inputCooldown);

        isBlocked = false;
        pressCount = 0; // Reset press count
        Debug.Log("Input unblocked.");
    }

    protected virtual void EvaluateTiming()
    {
        float songPositionInBeats = BEAT_Manager.Instance.GetSongPositionInBeats();
        float nearestBeat = Mathf.Round(songPositionInBeats); // Nearest beat
        float timeDifference = Mathf.Abs(songPositionInBeats - nearestBeat) * BEAT_Manager.Instance.GetSecPerBeat();

        if (timeDifference <= RhythmDifficulty.perfectThreshold + thresholdModifier)
        {
            HitEffect.Instance.playHitEffect("Perfect");
            BeatUI.Instance.ShowHitFeedback("Perfect");
            RhythmStreakManager.Instance.RegisterHit(streakGainPerfect);
            OnPerfectHit();
        }
        else if (timeDifference <= RhythmDifficulty.goodThreshold + thresholdModifier)
        {
            HitEffect.Instance.playHitEffect("Good");
            BeatUI.Instance.ShowHitFeedback("Good");
            RhythmStreakManager.Instance.RegisterHit(streakGainGood);
            OnGoodHit();
        }
        else
        {
            BeatUI.Instance.ShowHitFeedback("Miss");
            OnMiss();
        }
    }
    protected virtual void OnPerfectHit() { Debug.Log("Perfect Hit!"); }
    protected virtual void OnGoodHit() { Debug.Log("Good Hit!"); }
    protected virtual void OnMiss() { Debug.Log("Miss!"); }
    protected virtual void Start() { }

}