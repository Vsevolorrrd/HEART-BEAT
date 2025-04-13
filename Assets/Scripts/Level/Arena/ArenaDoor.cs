using UnityEngine;

public class ArenaDoor : MonoBehaviour
{
    private Animator anim;
    [SerializeField] bool startOpen = false;

    private void Start()
    {
        anim = GetComponent<Animator>();

        if(startOpen)
        OpenDoor();
    }
    public void CloseDoor()
    {
        anim.SetBool("Close", true);
    }
    public void OpenDoor()
    {
        anim.SetBool("Close", false);
    }
}
