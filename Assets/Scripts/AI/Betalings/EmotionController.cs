using UnityEngine;

public enum EmotionType
{
    Idle,
    Speaking,
    Hurt,
    Angry,
    SuperAngry,
    Happy,
    Surprised,
    Sad,
    DeadInside
}
public class EmotionController : MonoBehaviour
{

    [SerializeField] private SpriteRenderer bodyRenderor;
    [SerializeField] private SpriteRenderer faceRenderer;
    [SerializeField] private EmotionContainer emotionContainer; // ScriptableObject containing emotion sprites
    [SerializeField] Color deadColor = Color.white;

    private EmotionType currentEmotion = EmotionType.Idle;

    void Start()
    {
        if (faceRenderer == null)
        faceRenderer = GetComponent<SpriteRenderer>();
        SetEmotion(EmotionType.Idle);
    }

    public void SetEmotion(EmotionType emotion, bool resetEmotion = true, float duration = 0.6f)
    {
        if (emotionContainer == null || faceRenderer == null) return;

        CancelInvoke(nameof(ResetEmotion));
        currentEmotion = emotion;
        faceRenderer.sprite = emotionContainer.GetSprite(emotion);

        if (emotion == EmotionType.DeadInside)
        {
            bodyRenderor.color = deadColor;
            return;
        }
        if (resetEmotion)
        Invoke(nameof(ResetEmotion), duration);
    }
    private void ResetEmotion()
    {
        SetEmotion(EmotionType.Idle, false, 0f);
    }

}