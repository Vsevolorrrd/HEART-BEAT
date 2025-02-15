using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeatUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI hitFeedbackText;
    [SerializeField] Color perfectColor = Color.red;
    [SerializeField] Color goodColor = Color.yellow;
    [SerializeField] Color missColor = Color.white;

    [SerializeField] float displayDuration = 0.3f;
    [SerializeField] float fadeDuration = 0.2f;
    [SerializeField] float punchScale = 1.3f;
    [SerializeField] float punchDuration = 0.15f;

    [Header("BeatBar")]
    [SerializeField] RectTransform beatDotPrefab;
    [SerializeField] RectTransform beatBarLeft;
    [SerializeField] RectTransform beatBarRight;
    [SerializeField] Transform beatBarContainer; // Parent for beat dots


    [SerializeField] float dotSpeed = 200f;
    [SerializeField] float startOffset = 400f;
    [SerializeField] float stopDistance = 80f;

    private List<(RectTransform dot, CanvasGroup cg)> activeDots = 
    new List<(RectTransform, CanvasGroup)>();
    private bool beatBar = true;

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
        hitFeedbackText.text = "";
        hitFeedbackText.gameObject.SetActive(false);
        originalScale = hitFeedbackText.transform.localScale;

        BEAT_Manager.BEAT += OnBeat;

        // Hide bars if disabled
        SetBarsActive(beatBar);
    }
    void Update()
    {
        for (int i = activeDots.Count - 1; i >= 0; i--)
        {
            var (dot, cg) = activeDots[i];

            // Move left dots to the right, right dots to the left
            if (dot.anchoredPosition.x < 0)
            dot.anchoredPosition += Vector2.right * (dotSpeed * Time.deltaTime); // make the movenment more reliable <---
            else
            dot.anchoredPosition += Vector2.left * (dotSpeed * Time.deltaTime);

            if (beatBar)
            {
                // Calculate fade-in effect
                float distanceToStop = Mathf.Abs(dot.anchoredPosition.x) - stopDistance;
                float fadeProgress = 1f - Mathf.Clamp01(distanceToStop / startOffset);
                cg.alpha = fadeProgress; // Adjust alpha based on distance
            }

            // Remove dots when they reach stop distance
            if (Mathf.Abs(dot.anchoredPosition.x) <= stopDistance)
            {
                Destroy(dot.gameObject);
                activeDots.RemoveAt(i);
            }

        }
    }

    private void OnBeat()
    {

        SpawnBeatDot(-startOffset); // Left dot
        SpawnBeatDot(startOffset);  // Right dot
    }
    private void SpawnBeatDot(float startX)
    {
        RectTransform newDot = Instantiate(beatDotPrefab, beatBarContainer);
        newDot.anchoredPosition = new Vector2(startX, 0);

        // Add a CanvasGroup for alpha control
        CanvasGroup cg = newDot.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f; // Start fully transparent

        activeDots.Add((newDot, cg));
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
    public void ToggleBeatBar()
    {
        beatBar = !beatBar;
        SetBarsActive(beatBar);
    }

    private void SetBarsActive(bool state)
    {
        beatBarLeft.gameObject.SetActive(state);
        beatBarRight.gameObject.SetActive(state);
    }
    private void OnEnable()
    {
        BEAT_Manager.BEAT += OnBeat;
    }

    private void OnDisable()
    {
        BEAT_Manager.BEAT -= OnBeat;
    }
}