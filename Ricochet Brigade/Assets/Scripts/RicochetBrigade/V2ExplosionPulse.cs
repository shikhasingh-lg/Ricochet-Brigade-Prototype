using UnityEngine;

namespace RicochetBrigade
{
    public sealed class V2ExplosionPulse : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private float lifetime = 0.35f;
        private float elapsed;
        private float targetScale;
        private Color startColor;

        public void Setup(float radius, Color color, float duration)
        {
            targetScale = radius * 2f;
            lifetime = duration;
            startColor = color;
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }

            spriteRenderer.sprite = GreyboxSprites.Circle;
            spriteRenderer.color = startColor;
            spriteRenderer.sortingOrder = 31;
            transform.localScale = Vector3.zero;
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            float t = lifetime > 0f ? Mathf.Clamp01(elapsed / lifetime) : 1f;
            float eased = 1f - Mathf.Pow(1f - t, 2f);
            transform.localScale = Vector3.one * Mathf.Lerp(0.1f, targetScale, eased);

            if (spriteRenderer != null)
            {
                Color color = startColor;
                color.a *= 1f - t;
                spriteRenderer.color = color;
            }

            if (t >= 1f)
            {
                Destroy(gameObject);
            }
        }
    }
}
