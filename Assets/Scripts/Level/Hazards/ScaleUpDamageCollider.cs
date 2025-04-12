using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ScaleUpDamageCollider : MonoBehaviour
{
    public float damage;
    public float scaleDuration = 0.7f;
    public float timeBeforeDamaging = 0.1f;
    public Vector3 targetScale = new Vector3(1f, 1f, 1f);
    private bool canDamage = false;
    private bool callOnce = true;

    private void OnEnable()
    {
        canDamage = false;
        callOnce = true;
        transform.localScale = Vector3.zero;
        StartCoroutine(ScaleUp());
    }
    private IEnumerator ScaleUp()
    {
        float elapsedTime = 0f;
        Vector3 startScale = Vector3.zero;

        while (elapsedTime < scaleDuration)
        {
            float t = elapsedTime / scaleDuration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            if (elapsedTime > timeBeforeDamaging && callOnce)
            {
                canDamage = true;
                callOnce = false;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; // Ensure it reaches full size
    }
    private void OnTriggerStay(Collider other)
    {
        if (!canDamage) return;

        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out Damageable target))
            {
                target.Damage(damage);
                canDamage = false;
            }
        }
    }
}