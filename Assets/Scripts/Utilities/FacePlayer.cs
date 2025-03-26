using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    private Transform camTransform;
    [SerializeField] private bool inAllAxes = false;

    void Start()
    {
        camTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (inAllAxes)
        {
            transform.LookAt(camTransform); // Face the camera in all axes
        }
        else
        {
            transform.LookAt(camTransform);
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f); // Keep upright
        }
    }
}