using UnityEngine;

public class Laser : MonoBehaviour
{
    //these values are set by the weapon shooting projectile
    [SerializeField] protected float speed;
    [SerializeField] protected float duration;

    public void Update()
    {
        float moveby = speed * Time.deltaTime;
        transform.position += transform.forward * moveby;
    }
}