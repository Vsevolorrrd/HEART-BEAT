using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Burst.CompilerServices;

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
    [SerializeField] Color beatDotColor = Color.red;


    [SerializeField] float baseSpeed = 200f;
    [SerializeField] float startOffset = 400f;
    [SerializeField] float stopDistance = 100f;
    private float dotSpeed;

    private List<(RectTransform dot, CanvasGroup cg)> activeDots = 
    new List<(RectTransform, CanvasGroup)>();
    private Coroutine scaleCoroutine;
    private Coroutine fadeCoroutine;
    private bool beatBar = true;
    private bool isActive = false;

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

    public void StartBeatUI()
    {
        if (isActive) return;

        // Adjust dotSpeed based on BPM
        float BPM = BEAT_Manager.Instance.GetSongBPM();
        float bpmScale = BPM / 120f;
        dotSpeed = baseSpeed * bpmScale;

        isActive = true;
        hitFeedbackText.text = "";
        hitFeedbackText.gameObject.SetActive(false);
        originalScale = hitFeedbackText.transform.localScale;

        foreach (var (dot, _) in activeDots)
        {
            Destroy(dot.gameObject);
        }
        activeDots.Clear();

        BEAT_Manager.BEAT += OnBeat;
        SetBarsActive(beatBar);
    }
    void Update()
    {
        if (!isActive) return;

        for (int i = activeDots.Count - 1; i >= 0; i--)
        {
            var (dot, cg) = activeDots[i];

            // Move left dots to the right, right dots to the left
            if (dot.anchoredPosition.x < 0)
            dot.anchoredPosition += Vector2.right * (dotSpeed * Time.deltaTime);
            else
            dot.anchoredPosition += Vector2.left * (dotSpeed * Time.deltaTime);

            if (beatBar)
            {
                // Calculate distance from stop point
                float distanceToStop = Mathf.Abs(dot.anchoredPosition.x) - stopDistance;
                float fadeProgress = 0.7f - Mathf.Clamp01(distanceToStop / startOffset);

                bool isHint = dot.GetComponent<HintDot>() != null;

                if (Mathf.Abs(dot.anchoredPosition.x) <= stopDistance)
                {
                    StartCoroutine(FadeOutAndRemoveDot(dot, cg));
                    activeDots.RemoveAt(i);
                }
                else if (!isHint)
                {
                    if (Mathf.Abs(dot.anchoredPosition.x) <= stopDistance * 1.18f) // Slight buffer
                    {
                        dot.GetComponent<Image>().color = Color.white;
                        cg.alpha = 1f;
                    }
                    else
                    {
                        cg.alpha = fadeProgress;
                    }
                }

            }

        }
    }

    private void OnBeat()
    {
        if (activeDots.Count < 16)
        {
            SpawnBeatDot(-startOffset); // Left dot
            SpawnBeatDot(startOffset); // Right dot
        }
    }
    private void SpawnBeatDot(float startX)
    {
        RectTransform newDot = Instantiate(beatDotPrefab, beatBarContainer);
        newDot.anchoredPosition = new Vector2(startX, 0);

        // Add a CanvasGroup for alpha control
        CanvasGroup cg = newDot.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f; // Start fully transparent
        newDot.GetComponent<Image>().color = beatDotColor;

        activeDots.Add((newDot, cg));
    }
    private IEnumerator FadeOutAndRemoveDot(RectTransform dot, CanvasGroup cg)
    {
        float moveDistance = dotSpeed * 0.2f;
        float fadeTime = moveDistance / dotSpeed;

        float elapsedTime = 0f;
        Vector2 startPos = dot.anchoredPosition;
        Vector2 endPos = startPos + (dot.anchoredPosition.x < 0 ? Vector2.right : Vector2.left) * moveDistance;

        while (elapsedTime < fadeTime)
        {
            float t = elapsedTime / fadeTime;

            if (elapsedTime >= fadeTime/5f)
            dot.GetComponent<Image>().color = beatDotColor;

            dot.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            cg.alpha = Mathf.Lerp(1f, 0f, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cg.alpha = 0f;

        Destroy(dot.gameObject);
    }
    public void AddHintDots(Color hintColor)
    {
        if (activeDots.Count < 16)
        {
            SpawnHintDot(-startOffset, hintColor); // Left dot
            SpawnHintDot(startOffset, hintColor); // Right dot
        }
    }
    private void SpawnHintDot(float startX, Color hintColor)
    {
        RectTransform newDot = Instantiate(beatDotPrefab, beatBarContainer);
        newDot.anchoredPosition = new Vector2(startX, 0);

        CanvasGroup cg = newDot.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 1f;

        Image img = newDot.GetComponent<Image>();
        img.color = hintColor;

        // Mark this dot as a hint
        newDot.gameObject.AddComponent<HintDot>();
        activeDots.Add((newDot, cg));
    }
    public void ShowHitFeedback(string result)
    {
        // Stop any ongoing animation
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

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
        scaleCoroutine = StartCoroutine(ScaleBackAnimation());
        fadeCoroutine = StartCoroutine(FadeOutFeedback());
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

        foreach (var (dot, _) in activeDots)
        {
            Destroy(dot.gameObject);
        }
        activeDots.Clear();
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