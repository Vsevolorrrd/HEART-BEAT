using UnityEngine;

[CreateAssetMenu(fileName = "EmotionContainer", menuName = "Scriptable Objects/EmotionContainer")]
public class EmotionContainer : ScriptableObject
{
    public Sprite idle;
    public Sprite speaking;
    public Sprite hurt;
    public Sprite angry;
    public Sprite superAngry;
    public Sprite happy;
    public Sprite surprised;
    public Sprite sad;
    public Sprite deadInside;

    public Sprite GetSprite(EmotionType emotion)
    {
        return emotion switch
        {
            EmotionType.Hurt => hurt,
            EmotionType.Speaking => speaking,
            EmotionType.Angry => angry,
            EmotionType.SuperAngry => superAngry,
            EmotionType.Happy => happy,
            EmotionType.Surprised => surprised,
            EmotionType.Sad => sad,
            EmotionType.DeadInside => deadInside,
            _ => idle, // Default to idle
        };
    }
}