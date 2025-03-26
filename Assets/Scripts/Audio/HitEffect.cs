using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class HitEffect : Singleton<HitEffect>
{
    [SerializeField] Volume hitEffect;
    [SerializeField] float effectDecaySpeed = 5f;

    void Start()
    {
        if (hitEffect)
        hitEffect.weight = 0;
    }

    public void playHitEffect(string result)
    {
        switch (result)
        {
            case "Perfect":
                StartCoroutine(PerfectHitEffect());
                break;
            case "Good":
                StartCoroutine(GoodHitEffect());
                break;
        }
    }
    private IEnumerator PerfectHitEffect()
    {
        if (hitEffect == null)
            yield break;

        hitEffect.weight = 1f;

        while (hitEffect.weight > 0)
        {
            hitEffect.weight -= effectDecaySpeed * Time.deltaTime;
            hitEffect.weight = Mathf.Max(hitEffect.weight, 0);
            yield return null;
        }
    }
    private IEnumerator GoodHitEffect()
    {
        if (hitEffect == null)
            yield break;

        hitEffect.weight = 0.5f;

        while (hitEffect.weight > 0)
        {
            hitEffect.weight -= effectDecaySpeed * Time.deltaTime;
            hitEffect.weight = Mathf.Max(hitEffect.weight, 0);
            yield return null;
        }
    }
}
