using UnityEngine;
using System.Collections;

public class PlayerFallHandler : MonoBehaviour // a safety measure, in case of player falling of map
{
    [SerializeField] float fallThresholdY = -100f;
    [SerializeField] Vector3 respawnPosition = new Vector3(0, 2, 0);
    [SerializeField] float gravity = -9.81f; // Apply downward force after respawn
    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (transform.position.y < fallThresholdY)
        {
            HandleFall();
        }
    }

    private void HandleFall()
    {
        controller.enabled = false; // Disable CharacterController to allow position reset
        transform.position = respawnPosition;
        controller.enabled = true; // Re-enable controller

        StartCoroutine(ApplyGravityFix());
    }

    private IEnumerator ApplyGravityFix()
    {
        float fallDuration = 0.2f; // Give the player a slight push downward
        float elapsedTime = 0f;
        Vector3 downwardForce = new Vector3(0, gravity, 0);

        while (elapsedTime < fallDuration)
        {
            controller.Move(downwardForce * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}