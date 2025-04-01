using UnityEngine;

public class SpriteChanger : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] sprites;
    [SerializeField] float changeInterval = 0.1f;

    private int currentSpriteIndex = 0;

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (sprites.Length > 0)
            StartCoroutine(ChangeSpriteLoop());
    }

    private System.Collections.IEnumerator ChangeSpriteLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(changeInterval);

            // Cycle through sprites
            currentSpriteIndex = (currentSpriteIndex + 1) % sprites.Length;
            spriteRenderer.sprite = sprites[currentSpriteIndex];
        }
    }
}
