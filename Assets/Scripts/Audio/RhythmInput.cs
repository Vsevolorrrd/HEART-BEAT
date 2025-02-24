using UnityEngine;
using System.Collections;

public abstract class RhythmInput : MonoBehaviour
{
    [Header("RhythmInput")]
    public float perfectThreshold = 0.1f;  // 100 ms for Perfect
    public float streakGainPerfect = 2f;
    public float goodThreshold = 0.15f;    // 150 ms for Good
    public float streakGainGood = 1f;
    public KeyCode actionKey = KeyCode.Space;
    [HideInInspector] public bool playerInput = true;

    // Anti Spam
    protected int maxPresses = 3;
    protected float pressWindow = 0.4f; // Time window for counting presses
    protected float inputCooldown = 1.5f; // Cooldown after max presses
    protected int pressCount = 0;
    protected float firstPressTime = -999f;
    protected bool isBlocked = false;

    public virtual void Update()
    {
        if (!playerInput || isBlocked)
        return;

        if (Input.GetKeyDown(actionKey))
        {
            HandleKeyPress();
        }
    }
    public virtual void HandleKeyPress()
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

    public virtual void EvaluateTiming()
    {
        float songPosition = BEAT_Manager.Instance.GetSongPositionInBeats();
        float nearestBeat = Mathf.Round(songPosition); // Nearest beat
        float timeDifference = Mathf.Abs(songPosition - nearestBeat) * BEAT_Manager.Instance.GetSecPerBeat();

        if (timeDifference <= perfectThreshold)
        {
            HitEffect.Instance.playHitEffect("Perfect");
            BeatUI.Instance.ShowHitFeedback("Perfect");
            RhythmStreakManager.Instance.RegisterHit(streakGainPerfect);
            OnPerfectHit();
        }
        else if (timeDifference <= goodThreshold)
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
    public virtual void OnPerfectHit()
    {
        Debug.Log("Perfect Hit!");
    }
    public virtual void OnGoodHit()
    {
        Debug.Log("Good Hit!");
    }
    public virtual void OnMiss()
    {
        Debug.Log("Miss!");
    }

    #region events

    public virtual void Start()
    {
        MainMenu.OnPause += HandlePause;
    }

    public virtual void OnDestroy()
    {
        MainMenu.OnPause -= HandlePause;
    }

    protected void HandlePause(bool isPaused)
    {
        playerInput = !isPaused;
    }
    #endregion
}