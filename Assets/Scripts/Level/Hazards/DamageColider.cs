using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class DamageColider : MonoBehaviour
{
    public float damage;
    public float scaleDuration = 0.7f;
    public Vector3 targetScale = new Vector3(1f, 1f, 1f);

    private void OnEnable()
    {
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

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; // Ensure it reaches full size
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision");
        if (other.TryGetComponent(out Damageable target))
        {
            target.Damage(damage);
        }
    }
}