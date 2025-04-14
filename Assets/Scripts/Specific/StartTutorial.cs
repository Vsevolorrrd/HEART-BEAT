using UnityEngine;

public class StartTutorial : MonoBehaviour
{
    [SerializeField] GameObject hint;
    void Start()
    {
        BEAT_Manager.MusicLevelIncreased += SetTutorialOff;
        PlayerManager.Instance.controller.SetInput(false);
        hint.SetActive(true);
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
                Finish();
                break;
            case 2:
                Finish();
                break;
            case 1:
                break;
        }
    }
    private void Finish()
    {
        PlayerManager.Instance.controller.SetInput(true);
        hint.SetActive(false);
        Destroy(this);
    }
}
