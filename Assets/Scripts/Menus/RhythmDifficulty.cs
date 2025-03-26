using UnityEngine;

public class RhythmDifficulty : MonoBehaviour
{
    public static float perfectThreshold = 0.1f;  // 100 ms for Perfect
    public static float goodThreshold = 0.15f;    // 150 ms for Good

    [Header("Easy")]
    [SerializeField] float easyPerfectThreshold = 0.1f;
    [SerializeField] float easyGoodThreshold = 0.15f;

    [Header("Normal")]
    [SerializeField] float normalPerfectThreshold = 0.7f;
    [SerializeField] float normalGoodThreshold = 0.12f;

    [Header("Hard")]
    [SerializeField] float hardPerfectThreshold = 0.5f;
    [SerializeField] float hardGoodThreshold = 0.1f;

    public void EasyDificulty()
    {

    }
    public void NormalDificulty()
    {

    }
    public void HardDificulty()
    {

    }
}
