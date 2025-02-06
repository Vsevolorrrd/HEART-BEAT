using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class HitEffect : MonoBehaviour
{
    [SerializeField] Volume hitEffect;
    [SerializeField] float effectDecaySpeed = 5f;

    private static HitEffect _instance;

    #region Singleton
    public static HitEffect Instance
    {
        get
        {
            // Check if the instance is already created
            if (_instance == null)
            {
                // Try to find an existing HitEffect in the scene
                _instance = FindAnyObjectByType<HitEffect>();

                // If no HitEffect exists, create a new one
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("HitEffect");
                    _instance = singletonObject.AddComponent<HitEffect>();
                }

                // Make the HitEffect persist across scenes
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    void Awake()
    {
        // If the instance is already set, destroy this duplicate object
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;  // Assign this object as the instance
        }
    }
    #endregion

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
