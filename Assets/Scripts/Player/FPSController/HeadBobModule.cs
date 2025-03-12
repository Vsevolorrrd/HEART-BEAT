using UnityEngine;

public class HeadBobModule : MonoBehaviour
{
    private FPSController fpsController;

    [SerializeField] Transform joint; // attach an empty parent obj to camera (if there is alredy one, make a new obj as a parent to it)
    public float bobSpeed = 10f;
    public Vector3 bobAmount = new Vector3(.02f, .05f, 0f);

    private float timer = 0;
    private Vector3 jointOriginalPos;

    void Start()
    {
        fpsController = GetComponent<FPSController>();
        jointOriginalPos = joint.localPosition;
    }
    void Update()
    {
        HeadBob(0);
    }
    public void HeadBob(float modifier = 0)
    {
        if (fpsController.isMoving)
        {
            // Calculates HeadBob
            timer += Time.deltaTime * (bobSpeed + modifier);

            // Applies HeadBob movement
            joint.localPosition = new Vector3(jointOriginalPos.x + Mathf.Sin(timer) * bobAmount.x, jointOriginalPos.y + Mathf.Sin(timer) * bobAmount.y, jointOriginalPos.z + Mathf.Sin(timer) * bobAmount.z);
        }
        else
        {
            // Resets when player stops moving
            timer = 0;
            joint.localPosition = new Vector3(Mathf.Lerp(joint.localPosition.x, jointOriginalPos.x, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.y, jointOriginalPos.y, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.z, jointOriginalPos.z, Time.deltaTime * bobSpeed));
        }
    }
}