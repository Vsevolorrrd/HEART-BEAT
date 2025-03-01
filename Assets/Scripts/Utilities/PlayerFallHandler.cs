using UnityEngine;
using System.Collections;

public class PlayerFallHandler : MonoBehaviour // a safety measure, in case of player falling of map
{
    [SerializeField] float fallThresholdY = -100f;
    [SerializeField] Vector3 respawnPosition = new Vector3(0, 2, 0);

    private void Update()
    {
        if (transform.position.y < fallThresholdY)
        {
            HandleFall();
        }
    }

    private void HandleFall()
    {
        transform.position = respawnPosition;
    }
}