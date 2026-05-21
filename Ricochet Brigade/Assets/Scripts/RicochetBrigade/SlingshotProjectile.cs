using System.Collections.Generic;
using UnityEngine;

namespace RicochetBrigade
{
    public sealed class SlingshotProjectile : MonoBehaviour
    {
        private V2ArenaLayout layout;
        private Rigidbody2D body;
        private int maxBounces;
        private int bounceCount;
        private float explosionRadius;
        private float explosionDamage;
        private float firedPower;
        private float spawnedAt;
        private bool exploded;
        private SpriteRenderer spriteRenderer;

        public void Setup(V2ArenaLayout arenaLayout, Vector2 velocity, int allowedBounces, float radius, float damage, float power)
        {
            layout = arenaLayout;
            maxBounces = allowedBounces;
            explosionRadius = radius;
            explosionDamage = damage;
            firedPower = power;
            spawnedAt = Time.time;
            EnsureComponents();
            body.linearVelocity = velocity;
        }

        private void Update()
        {
            if (exploded || layout == null)
            {
                return;
            }

            if (!layout.WorldBounds.Contains(transform.position) || Time.time - spawnedAt > 6f)
            {
                Explode("bounds_or_timeout");
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (exploded)
            {
                return;
            }

            V2PlatformBlock platform = collision.collider.GetComponent<V2PlatformBlock>();
            if (platform == null)
            {
                Explode("blocked");
                return;
            }

            bounceCount++;
            ContactPoint2D contact = collision.GetContact(0);
            CreateBounceFlash(contact.point);
            if (bounceCount >= maxBounces)
            {
                Explode("max_bounces");
                return;
            }

            Vector2 normal = contact.normal;
            body.linearVelocity = Vector2.Reflect(body.linearVelocity, normal) * 0.72f;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (exploded)
            {
                return;
            }

            V2EnemyUnit enemy = other.GetComponent<V2EnemyUnit>();
            if (enemy != null)
            {
                Explode("enemy_hit");
            }
        }

        private void Explode(string reason)
        {
            if (exploded)
            {
                return;
            }

            exploded = true;
            List<V2EnemyUnit> enemies = V2EnemyUnit.EnemiesInRadius(transform.position, explosionRadius);
            int hits = 0;
            for (int i = 0; i < enemies.Count; i++)
            {
                float distance = Vector2.Distance(transform.position, enemies[i].transform.position);
                float falloff = Mathf.Lerp(1f, 0.55f, Mathf.Clamp01(distance / explosionRadius));
                enemies[i].TakeDamage(explosionDamage * falloff, "slingshot");
                hits++;
            }

            CreateExplosionVisual(hits > 0);

            if (V2GameController.Instance != null)
            {
                V2GameController.Instance.RecordSlingshotResult(firedPower, bounceCount, hits, explosionRadius, reason);
            }

            Destroy(gameObject);
        }

        private void EnsureComponents()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }

            spriteRenderer.sprite = GreyboxSprites.Circle;
            spriteRenderer.color = new Color(1f, 0.92f, 0.16f);
            spriteRenderer.sortingOrder = 26;
            transform.localScale = Vector3.one * 0.32f;

            CircleCollider2D collider = GetComponent<CircleCollider2D>();
            if (collider == null)
            {
                collider = gameObject.AddComponent<CircleCollider2D>();
            }

            collider.radius = 0.42f;

            body = GetComponent<Rigidbody2D>();
            if (body == null)
            {
                body = gameObject.AddComponent<Rigidbody2D>();
            }

            body.gravityScale = 1f;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;

            TrailRenderer trail = GetComponent<TrailRenderer>();
            if (trail == null)
            {
                trail = gameObject.AddComponent<TrailRenderer>();
            }

            trail.time = 0.35f;
            trail.startWidth = 0.18f;
            trail.endWidth = 0.02f;
            trail.sortingOrder = 25;
            trail.material = new Material(Shader.Find("Sprites/Default"));
            trail.startColor = new Color(1f, 0.86f, 0.18f, 0.85f);
            trail.endColor = new Color(1f, 0.86f, 0.18f, 0f);
        }

        private void CreateExplosionVisual(bool hit)
        {
            GameObject burst = new GameObject(hit ? "Slingshot Explosion Hit" : "Slingshot Explosion Miss");
            burst.transform.position = transform.position;
            V2ExplosionPulse pulse = burst.AddComponent<V2ExplosionPulse>();
            pulse.Setup(explosionRadius, hit ? new Color(1f, 0.62f, 0.08f, 0.48f) : new Color(1f, 1f, 1f, 0.24f), 0.42f);
        }

        private void CreateBounceFlash(Vector2 position)
        {
            GameObject flash = new GameObject("Slingshot Bounce Flash");
            flash.transform.position = position;
            V2ExplosionPulse pulse = flash.AddComponent<V2ExplosionPulse>();
            pulse.Setup(0.32f, new Color(1f, 1f, 1f, 0.5f), 0.18f);
        }
    }
}
