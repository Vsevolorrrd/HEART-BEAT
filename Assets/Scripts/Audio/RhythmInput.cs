using UnityEngine;

public class RhythmInput : MonoBehaviour
{
    [SerializeField] private BeatUI beatUI;
    [SerializeField] private KeyCode actionKey = KeyCode.Space;
    [SerializeField] private float perfectThreshold = 0.1f;  // 100 ms for Perfect
    [SerializeField] private float goodThreshold = 0.25f;    // 250 ms for Good

    private void Update()
    {
        if (Input.GetKeyDown(actionKey))
        {
            EvaluateTiming();
        }
    }

    private void EvaluateTiming()
    {
        float songPosition = BEAT_Manager.Instance.GetSongPositionInBeats();
        float nearestBeat = Mathf.Round(songPosition); // Nearest beat in beats
        float timeDifference = Mathf.Abs(songPosition - nearestBeat) * BEAT_Manager.Instance.GetSecPerBeat();

        if (timeDifference <= perfectThreshold)
        {
            Debug.Log("Perfect Hit!");
            beatUI.ShowHitFeedback("Perfect");
        }
        else if (timeDifference <= goodThreshold)
        {
            Debug.Log("Good Hit!");
            beatUI.ShowHitFeedback("Good");
        }
        else
        {
            Debug.Log("Miss!");
            beatUI.ShowHitFeedback("Miss");
        }
    }
}
