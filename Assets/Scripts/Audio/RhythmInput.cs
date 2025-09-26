using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public abstract class RhythmInput : MonoBehaviour
{
    [Header("RhythmInput")]
    public float thresholdModifier = 0.1f;  // 100 ms for Perfect
    public float streakGainPerfect = 2f;
    public float streakGainGood = 1f;

    [Header("Input")]
    public Key actionKey = Key.Space;
    public GamepadButton actionButton = GamepadButton.South;
    public bool UseMouseButton = false;
    public int MouseButton = 0; // 0 = left click, 1 = right click

    [Header("Anti Spam")]
    public int maxPresses = 3;
    protected float pressWindow = 0.4f; // Time window for counting presses
    protected float inputCooldown = 1.5f; // Cooldown after max presses
    protected int pressCount = 0;
    protected float firstPressTime = -999f;
    protected bool isBlocked = false;

    // Input System reference
    protected PlayerInput playerInput;

    protected virtual void Awake()
    {
        playerInput = PlayerManager.Instance.playerInputSystem;
        if (playerInput == null)
        {
            Debug.LogWarning("PlayerInput not found!");
        }
    }

    protected virtual void Update()
    {
        if (!playerInput || isBlocked) return;

        if (IsActionPressed())
        {
            HandleKeyPress();
        }
    }

    // Checks if the action was pressed this frame (keyboard or gamepad)
    protected virtual bool IsActionPressed()
    {
        bool pressed = false;

        // Keyboard
        if (Keyboard.current != null && !UseMouseButton)
        {
            var keyControl = Keyboard.current[actionKey];
            if (keyControl != null && keyControl.wasPressedThisFrame)
            pressed = true;
        }

        // Gamepad
        if (Gamepad.current != null)
        {
            var buttonControl = Gamepad.current[actionButton];
            if (buttonControl != null && buttonControl.wasPressedThisFrame)
            pressed = true;
        }

        // Mouse
        if (Mouse.current != null && UseMouseButton)
        {
            if (MouseButton == 0 && Mouse.current.leftButton.wasPressedThisFrame)
            pressed = true;
            else if (MouseButton == 1 && Mouse.current.rightButton.wasPressedThisFrame)
            pressed = true;
            else if (MouseButton == 2 && Mouse.current.middleButton.wasPressedThisFrame)
            pressed = true;
        }

        return pressed;
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
        pressCount = 0;
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