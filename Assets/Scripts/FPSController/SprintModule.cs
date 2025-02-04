using UnityEngine;
using UnityEngine.UI;

public class SprintModule : MonoBehaviour
{
    private FPSController fpsController;

    public bool unlimitedSprint = false;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float sprintSpeed = 1.7f;
    public float sprintDuration = 5f;
    public float sprintCooldown = .5f;
    private float sprintCooldownReset;
    private float sprintRemaining;
    private bool isSprintCooldown = false;
    private bool isSprinting = false;
    public bool CheckSprinting() { return isSprinting; }

    public SprintBar sprintBar;

    [System.Serializable]
    public class SprintBar
    {
        public bool useSprintBar = false;
        public bool hideBarWhenFull = true;
        public CanvasGroup sprintBarCG;
        public Image sprintBarBG;
        public Image sprintBarObj;
        public float sprintBarWidthPercent = .3f;
        public float sprintBarHeightPercent = .015f;

        [HideInInspector] public float sprintBarWidth;
        [HideInInspector] public float sprintBarHeight;
    }


    public void SetController(FPSController _fpsController)
    {
        fpsController = _fpsController;
    }
    void Start()
    {
        if (!unlimitedSprint)
        {
            sprintRemaining = sprintDuration;
            sprintCooldownReset = sprintCooldown;
        }

        #region Sprint Bar

        if (sprintBar.useSprintBar)
        {
            sprintBar.sprintBarBG.gameObject.SetActive(true);
            sprintBar.sprintBarObj.gameObject.SetActive(true);

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            sprintBar.sprintBarWidth = screenWidth * sprintBar.sprintBarWidthPercent;
            sprintBar.sprintBarHeight = screenHeight * sprintBar.sprintBarHeightPercent;

            sprintBar.sprintBarBG.rectTransform.sizeDelta = new Vector3(sprintBar.sprintBarWidth, sprintBar.sprintBarHeight, 0f);
            sprintBar.sprintBarObj.rectTransform.sizeDelta = new Vector3(sprintBar.sprintBarWidth - 2, sprintBar.sprintBarHeight - 2, 0f);

            if (sprintBar.hideBarWhenFull)
            {
                sprintBar.sprintBarCG.alpha = 0;
            }
        }
        else
        {
            if (sprintBar.sprintBarBG)
            {
                sprintBar.sprintBarBG.gameObject.SetActive(false);
                sprintBar.sprintBarObj.gameObject.SetActive(false);
            }
        }

        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        // All movement calculations while sprint is active
        if (Input.GetKey(sprintKey) && sprintRemaining > 0f && !isSprintCooldown)
        {
            if (fpsController.isMoving)
            {
                fpsController.speedModifier = sprintSpeed;

                isSprinting = true;

                if (sprintBar.useSprintBar)
                {
                    if (sprintBar.hideBarWhenFull && !unlimitedSprint)
                        sprintBar.sprintBarCG.alpha += 5 * Time.deltaTime;

                }
            }

        }
        else
        {
            fpsController.speedModifier = 1f;
            isSprinting = false;

            if (sprintBar.useSprintBar)
            {
                if (sprintBar.hideBarWhenFull && sprintRemaining == sprintDuration)
                    sprintBar.sprintBarCG.alpha -= 3 * Time.deltaTime;
            }
        }

        if (isSprinting)
        {

            // Drain sprint remaining while sprinting
            if (!unlimitedSprint)
            {
                sprintRemaining -= 1 * Time.deltaTime;
                if (sprintRemaining <= 0)
                {
                    isSprinting = false;
                    isSprintCooldown = true;
                }
            }
        }
        else
        {
            // Regain sprint while not sprinting
            sprintRemaining = Mathf.Clamp(sprintRemaining += 1 * Time.deltaTime, 0, sprintDuration);
        }

        // Handles sprint cooldown 
        // When sprint remaining == 0 stops sprint ability until hitting cooldown
        if (isSprintCooldown)
        {
            sprintCooldown -= 1 * Time.deltaTime;
            if (sprintCooldown <= 0)
            {
                isSprintCooldown = false;
            }
        }
        else
        {
            sprintCooldown = sprintCooldownReset;
        }

        // Handles sprintBar 
        if (sprintBar.useSprintBar && !unlimitedSprint)
        {
            float sprintRemainingPercent = sprintRemaining / sprintDuration;
            sprintBar.sprintBarObj.transform.localScale = new Vector3(sprintRemainingPercent, 1f, 1f);
        }
    }
}