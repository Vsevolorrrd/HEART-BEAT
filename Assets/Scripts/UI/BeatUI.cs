using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
    [SerializeField] RectTransform beatDotLeft;
    [SerializeField] RectTransform beatDotRight;
    [SerializeField] RectTransform beatBarLeft;
    [SerializeField] RectTransform beatBarRight;
    private bool beatBar = true;

    private float secPerBeat;
    private float timer;
    private float startLeft_X, endLeft_X, startRight_X, endRight_X;

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

        secPerBeat = BEAT_Manager.Instance.GetSecPerBeat(); // Get beat duration
        BEAT_Manager.BEAT += OnBeat;

        // Get left bar start and end
        startLeft_X = beatBarLeft.rect.xMin;
        endLeft_X = 80f;

        // Get right bar start and end
        startRight_X = beatBarRight.rect.xMax;
        endRight_X = -80f;

        // Hide bars if disabled
        SetBarsActive(beatBar);
    }
    void Update()
    {
        if (!beatBar) return;

        timer += Time.deltaTime;
        float progress = timer / secPerBeat; // Normalize time

        // Move the dots toward the center
        float newLeftX = Mathf.Lerp(startLeft_X, endLeft_X, progress);
        float newRightX = Mathf.Lerp(startRight_X, endRight_X, progress);

        beatDotLeft.anchoredPosition = new Vector2(newLeftX, beatDotLeft.anchoredPosition.y);
        beatDotRight.anchoredPosition = new Vector2(newRightX, beatDotRight.anchoredPosition.y);
    }

    private void OnBeat()
    {
        if (!beatBar) return;
        timer = 0; // Reset dot movement on beat
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
        if (beatBar)
        {
            beatBar = false;
            SetBarsActive(false);
        }
        else
        {
            beatBar = true;
            SetBarsActive(true);
        }
    }

    private void SetBarsActive(bool state)
    {
        beatBarLeft.gameObject.SetActive(state);
        beatBarRight.gameObject.SetActive(state);
        beatDotLeft.gameObject.SetActive(state);
        beatDotRight.gameObject.SetActive(state);
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