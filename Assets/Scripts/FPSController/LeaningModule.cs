using UnityEngine;

public class LeaningModule : MonoBehaviour
{
    public KeyCode LeanLeft = KeyCode.Q;
    public KeyCode LeanRight = KeyCode.E;
    public float leanAngle = 20f; // Angle for leaning
    public float leanSpeed = 5f; // Speed of leaning
    public float leanOffset = 0.7f; // How far the camera moves when leaning
    private float currentLean = 0f;
    private Vector3 originalCameraPosition;

    [SerializeField] Transform CamHolder; // attach an empty parent obj to camera (if there is alredy one, make a new obj as a parent to it)

    void Update()
    {
        float targetLean = 0f;
        Vector3 targetCameraPosition = originalCameraPosition;

        if (Input.GetKey(LeanLeft))
        {
            targetLean = leanAngle;
            targetCameraPosition += Vector3.left * leanOffset;
        }
        else if (Input.GetKey(LeanRight))
        {
            targetLean = -leanAngle;
            targetCameraPosition += Vector3.right * leanOffset;
        }
        // Smoothly transition to the desired lean angle and camera position
        currentLean = Mathf.Lerp(currentLean, targetLean, Time.deltaTime * leanSpeed);
        CamHolder.localRotation = Quaternion.Euler(CamHolder.localRotation.eulerAngles.x, CamHolder.localRotation.eulerAngles.y, currentLean);
        CamHolder.localPosition = Vector3.Lerp(CamHolder.localPosition, targetCameraPosition, Time.deltaTime * leanSpeed);
    }
}
