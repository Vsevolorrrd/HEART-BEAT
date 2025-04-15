using System.Collections;
using UnityEngine;

public class StartTutorial : MonoBehaviour
{
    [SerializeField] GameObject Snaphint;
    [SerializeField] GameObject Movehint;
    private bool nextStep = false;
    void Start()
    {
        BEAT_Manager.MusicLevelIncreased += SetTutorialOff;
        PlayerManager.Instance.controller.SetInput(false);
        Snaphint.SetActive(true);
        Movehint.SetActive(false);
    }
    private void OnDestroy()
    {
        BEAT_Manager.MusicLevelIncreased -= SetTutorialOff;
    }
    private void SetTutorialOff(int level)
    {
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
        PlayerManager.Instance.controller.SetInput(true);
        Snaphint.SetActive(false);
        Movehint.SetActive(true);
        nextStep = true;
    }
    private void Update()
    {
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
