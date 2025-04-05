using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISound : MonoBehaviour, IPointerEnterHandler
{
    private void Awake()
    {
        if (TryGetComponent(out Button btn))
        btn.onClick.AddListener(() => Play(AudioManager.Instance.buttonSound));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Play(AudioManager.Instance.hoverSound, 0.3f);
    }

    private void Play(AudioClip clip, float volume = 1f)
    {
        AudioManager.Instance.PlaySound(clip, volume);
    }
}