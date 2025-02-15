using UnityEngine;

[CreateAssetMenu(fileName = "Music", menuName = "Scriptable Objects/Music")]
public class Music : ScriptableObject
{
    public float songBPM;
    public AudioClip Leadingtrack;
    public AudioClip track_2;
    public AudioClip track_3;
    public AudioClip footSteps;
    public AudioClip transition;
}