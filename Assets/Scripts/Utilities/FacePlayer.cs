using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    private Transform camTransform;

    void Start()
    {
        camTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        // Make the object face the camera
        transform.LookAt(camTransform);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f); // Keep it upright
    }
}