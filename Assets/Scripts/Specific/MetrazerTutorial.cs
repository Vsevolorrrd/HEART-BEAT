using UnityEngine;

public class MetrazerTutorial : MonoBehaviour
{
    [SerializeField] GameObject metrazerTutorial;
    private bool callOnlyOnce;

    private void Start()
    {
        metrazerTutorial.SetActive(false);
        callOnlyOnce = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!callOnlyOnce) return;

        if (other.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PauseMenu.Instance.Stop(true);
            metrazerTutorial.SetActive(true);
        }
    }
    public void Finish()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        callOnlyOnce = false;
        metrazerTutorial.SetActive(false);
        PauseMenu.Instance.Stop(false);
        Destroy(gameObject);
    }
}