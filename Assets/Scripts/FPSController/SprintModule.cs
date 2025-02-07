using UnityEngine;
using UnityEngine.UI;

public class SprintModule : MonoBehaviour
{
    private FPSController fpsController;

    [Header("Sprint Settings")]
    public bool unlimitedSprint = false;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float sprintSpeed = 1.7f;
    public float sprintDuration = 5f;
    public float sprintCooldown = 0.5f;

    private float sprintRemaining;
    private float sprintCooldownTimer;
    private bool isSprintCooldown;

    public bool IsSprinting { get; private set; }

    [Header("Sprint Bar")]
    public SprintBar sprintBar;

    [System.Serializable]
    public class SprintBar
    {
        public bool useSprintBar = false;
        public bool hideBarWhenFull = true;
        public CanvasGroup sprintBarCG;
        public Image sprintBarBG;
        public Image sprintBarObj;
        public float sprintBarWidthPercent = 0.3f;
        public float sprintBarHeightPercent = 0.015f;

        [HideInInspector] public float sprintBarWidth;
        [HideInInspector] public float sprintBarHeight;
    }

    private void Start()
    {
        fpsController = GetComponent<FPSController>();

        if (!unlimitedSprint)
        {
            sprintRemaining = sprintDuration;
            sprintCooldownTimer = sprintCooldown;
        }

        InitializeSprintBar();
    }

    private void Update()
    {
        HandleSprinting();
        HandleSprintCooldown();
        UpdateSprintBar();
    }

    private void HandleSprinting()
    {
        bool isTryingToSprint = Input.GetKey(sprintKey) && sprintRemaining > 0f && !isSprintCooldown;

        if (isTryingToSprint && fpsController.isMoving)
        {
            fpsController.speedModifier = sprintSpeed;
            IsSprinting = true;

            if (sprintBar.useSprintBar && sprintBar.hideBarWhenFull && !unlimitedSprint)
                sprintBar.sprintBarCG.alpha = Mathf.Lerp(sprintBar.sprintBarCG.alpha, 1, 5 * Time.deltaTime);

            if (!unlimitedSprint)
            {
                sprintRemaining -= Time.deltaTime;
                if (sprintRemaining <= 0)
                {
                    sprintRemaining = 0;
                    isSprintCooldown = true;
                }
            }
        }
        else
        {
            fpsController.speedModifier = 1f;
            IsSprinting = false;

            if (!unlimitedSprint)
                sprintRemaining = Mathf.Clamp(sprintRemaining + Time.deltaTime, 0, sprintDuration);

            if (sprintBar.useSprintBar && sprintBar.hideBarWhenFull && sprintRemaining == sprintDuration)
                sprintBar.sprintBarCG.alpha = Mathf.Lerp(sprintBar.sprintBarCG.alpha, 0, 3 * Time.deltaTime);
        }
    }

    private void HandleSprintCooldown()
    {
        if (isSprintCooldown)
        {
            sprintCooldownTimer -= Time.deltaTime;
            if (sprintCooldownTimer <= 0)
            {
                isSprintCooldown = false;
                sprintCooldownTimer = sprintCooldown;
            }
        }
    }

    private void InitializeSprintBar()
    {
        if (!sprintBar.useSprintBar)
        {
            if (sprintBar.sprintBarBG)
            {
                sprintBar.sprintBarBG.gameObject.SetActive(false);
                sprintBar.sprintBarObj.gameObject.SetActive(false);
            }
            return;
        }

        sprintBar.sprintBarBG.gameObject.SetActive(true);
        sprintBar.sprintBarObj.gameObject.SetActive(true);

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        sprintBar.sprintBarWidth = screenWidth * sprintBar.sprintBarWidthPercent;
        sprintBar.sprintBarHeight = screenHeight * sprintBar.sprintBarHeightPercent;

        sprintBar.sprintBarBG.rectTransform.sizeDelta = new Vector2(sprintBar.sprintBarWidth, sprintBar.sprintBarHeight);
        sprintBar.sprintBarObj.rectTransform.sizeDelta = new Vector2(sprintBar.sprintBarWidth - 2, sprintBar.sprintBarHeight - 2);

        if (sprintBar.hideBarWhenFull)
            sprintBar.sprintBarCG.alpha = 0;
    }

    private void UpdateSprintBar()
    {
        if (sprintBar.useSprintBar && !unlimitedSprint)
        {
            float sprintRemainingPercent = sprintRemaining / sprintDuration;
            sprintBar.sprintBarObj.transform.localScale = new Vector3(sprintRemainingPercent, 1f, 1f);
        }
    }
}