using TMPro;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RhythmInput))]
public class BeatUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI hitFeedbackText;
    [SerializeField] Color perfectColor = Color.red;
    [SerializeField] Color goodColor = Color.yellow;
    [SerializeField] Color missColor = Color.white;

    [SerializeField] private float displayDuration = 0.3f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float punchScale = 1.3f;
    [SerializeField] private float punchDuration = 0.15f;

    private Vector3 originalScale;
    private static BeatUI _instance;

    #region Singleton
    public static BeatUI Instance
    {
        get
        {
            // Check if the instance is already created
            if (_instance == null)
            {
                // Try to find an existing BeatUI in the scene
                _instance = FindAnyObjectByType<BeatUI>();

                // If no BeatUI exists, create a new one
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("BeatUI");
                    _instance = singletonObject.AddComponent<BeatUI>();
                }

                // Make the BeatUI persist across scenes
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

    private void Start()
    {
        hitFeedbackText.text = ""; // Start with no text
        hitFeedbackText.gameObject.SetActive(false);
        originalScale = hitFeedbackText.transform.localScale;
    }

    public void ShowHitFeedback(string result)
    {
        StopAllCoroutines(); // Stop any ongoing animation
        switch (result)
        {
            case "Perfect":
                hitFeedbackText.text = "PERFECT!";
                hitFeedbackText.color = perfectColor;
                break;
            case "Good":
                hitFeedbackText.text = "GOOD!";
                hitFeedbackText.color = goodColor;
                break;
            case "Miss":
                hitFeedbackText.text = "MISS";
                hitFeedbackText.color = missColor;
                break;
        }

        hitFeedbackText.gameObject.SetActive(true);
        hitFeedbackText.alpha = 1f;
        hitFeedbackText.transform.localScale = originalScale * punchScale;

        // Start scale-back and fade-out animations
        StartCoroutine(ScaleBackAnimation());
        StartCoroutine(FadeOutFeedback());
    }

    private IEnumerator ScaleBackAnimation()
    {
        float elapsedTime = 0f;
        Vector3 startScale = hitFeedbackText.transform.localScale;

        while (elapsedTime < punchDuration)
        {
            hitFeedbackText.transform.localScale = Vector3.Lerp(startScale, originalScale, elapsedTime / punchDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        hitFeedbackText.transform.localScale = originalScale; // Ensure final scale is precise
    }
    private IEnumerator FadeOutFeedback()
    {
        yield return new WaitForSeconds(displayDuration); // Wait before fading

        float elapsedTime = 0f;
        Color startColor = hitFeedbackText.color;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            hitFeedbackText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        hitFeedbackText.gameObject.SetActive(false);
        hitFeedbackText.text = ""; // Clear text after fading
    }
}
