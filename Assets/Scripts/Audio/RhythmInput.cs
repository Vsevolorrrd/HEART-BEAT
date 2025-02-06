using UnityEngine;

public abstract class RhythmInput : MonoBehaviour
{
    public BeatUI beatUI;
    public float perfectThreshold = 0.1f;  // 100 ms for Perfect
    public float goodThreshold = 0.15f;    // 150 ms for Good
    public KeyCode actionKey = KeyCode.Space;

    public virtual void Update()
    {
        if (Input.GetKeyDown(actionKey))
        {
            EvaluateTiming();
        }
    }

    public virtual void EvaluateTiming()
    {
        float songPosition = BEAT_Manager.Instance.GetSongPositionInBeats();
        float nearestBeat = Mathf.Round(songPosition); // Nearest beat in beats
        float timeDifference = Mathf.Abs(songPosition - nearestBeat) * BEAT_Manager.Instance.GetSecPerBeat();

        if (timeDifference <= perfectThreshold)
        {
            HitEffect.Instance.playHitEffect("Perfect");
            beatUI.ShowHitFeedback("Perfect");
            OnPerfectHit();
        }
        else if (timeDifference <= goodThreshold)
        {
            HitEffect.Instance.playHitEffect("Good");
            beatUI.ShowHitFeedback("Good");
            OnGoodHit();
        }
        else
        {
            beatUI.ShowHitFeedback("Miss");
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
}
