using UnityEngine;

public class StartTutorial : MonoBehaviour // yea this cript is messy
{
    [SerializeField] GameObject Snaphint;
    [SerializeField] GameObject Movehint;
    private bool nextStep = false;
    private bool used = false;
    void Start()
    {
        PlayerManager.Instance.fpsController.SetInput(false);
        BEAT_Manager.MusicLevelIncreased += SetTutorialOff;
        Invoke("Delay", 0.8f);
    }
    private void Delay()
    {
        PlayerManager.Instance.fpsController.SetInput(true);
        MakeACheck();
    }
    private void MakeACheck()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 6f);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Started tutorial");
                PlayerManager.Instance.fpsController.SetInput(false);
                PauseMenu.Instance.BlockPauseMenu(true);
                Snaphint.SetActive(true);
                Movehint.SetActive(false);
                used = true;
                break;
            }
        }
    }
    private void OnDestroy()
    {
        BEAT_Manager.MusicLevelIncreased -= SetTutorialOff;
    }
    private void SetTutorialOff(int level)
    {
        if (!used) return;

        switch (level)
        {
            case 3:
                MoveHint();
                break;
            case 2:
                MoveHint();
                break;
            case 1:
                break;
        }
    }
    private void MoveHint()
    {
        PlayerManager.Instance.fpsController.SetInput(true);
        PauseMenu.Instance.BlockPauseMenu(false);
        Snaphint.SetActive(false);
        Movehint.SetActive(true);
        nextStep = true;
    }
    private void Update()
    {
        if (!used) return;

        if (nextStep)
        {
            bool isMoving = false;

            Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            isMoving = inputDirection.x != 0 || inputDirection.z != 0;

            if (isMoving)
            {
                Movehint.SetActive(false);
                Destroy(this);
            }
        }
    }
}
