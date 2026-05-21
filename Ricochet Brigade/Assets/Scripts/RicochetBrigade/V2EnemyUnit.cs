using System.Collections.Generic;
using UnityEngine;

namespace RicochetBrigade
{
    public sealed class V2EnemyUnit : MonoBehaviour
    {
        private static readonly List<V2EnemyUnit> activeEnemies = new List<V2EnemyUnit>();

        public int EnemyTier { get; private set; }
        public bool IsBoss { get; private set; }
        public float PathProgress { get; private set; }

        private V2ArenaLayout layout;
        private Vector2[] path;
        private int nextPointIndex;
        private float moveSpeed;
        private float health;
        private float maxHealth;
        private float stunTimer;
        private SpriteRenderer spriteRenderer;
        private TextMesh label;
        private Transform healthFill;

        public static IReadOnlyList<V2EnemyUnit> ActiveEnemies
        {
            get { return activeEnemies; }
        }

        private void OnEnable()
        {
            if (!activeEnemies.Contains(this))
            {
                activeEnemies.Add(this);
            }
        }

        private void OnDisable()
        {
            activeEnemies.Remove(this);
        }

        private void Update()
        {
            if (stunTimer > 0f)
            {
                stunTimer -= Time.deltaTime;
                UpdateHealthBar();
                return;
            }

            MoveAlongPath();
            UpdateHealthBar();
        }

        public void Setup(V2ArenaLayout arenaLayout, int tier, bool boss, float speed)
        {
            layout = arenaLayout;
            EnemyTier = Mathf.Clamp(tier, 1, 4);
            IsBoss = boss;
            moveSpeed = speed;
            maxHealth = EnemyHealth(EnemyTier, IsBoss);
            health = maxHealth;
            path = layout.PathWorldPoints();
            nextPointIndex = path.Length > 1 ? 1 : 0;
            transform.position = path.Length > 0 ? path[0] : Vector2.zero;
            EnsureComponents();
            UpdateVisuals();
        }

        public void TakeDamage(float amount, string source)
        {
            health -= amount;
            if (health > 0f)
            {
                return;
            }

            if (V2GameController.Instance != null)
            {
                V2GameController.Instance.NotifyEnemyKilled(this, source);
            }

            Destroy(gameObject);
        }

        public void Stun(float seconds)
        {
            stunTimer = Mathf.Max(stunTimer, seconds);
        }

        public static V2EnemyUnit FindTarget(Vector2 origin, float range)
        {
            V2EnemyUnit bestTarget = null;
            float bestProgress = -1f;
            float bestDistance = float.MaxValue;

            for (int i = 0; i < activeEnemies.Count; i++)
            {
                V2EnemyUnit enemy = activeEnemies[i];
                if (enemy == null)
                {
                    continue;
                }

                float distance = Vector2.Distance(origin, enemy.transform.position);
                if (distance > range)
                {
                    continue;
                }

                bool furtherAlong = enemy.PathProgress > bestProgress + 0.01f;
                bool tieCloser = Mathf.Abs(enemy.PathProgress - bestProgress) <= 0.01f && distance < bestDistance;
                if (furtherAlong || tieCloser)
                {
                    bestTarget = enemy;
                    bestProgress = enemy.PathProgress;
                    bestDistance = distance;
                }
            }

            return bestTarget;
        }

        public static List<V2EnemyUnit> EnemiesInRadius(Vector2 center, float radius)
        {
            List<V2EnemyUnit> results = new List<V2EnemyUnit>();
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                V2EnemyUnit enemy = activeEnemies[i];
                if (enemy != null && Vector2.Distance(center, enemy.transform.position) <= radius)
                {
                    results.Add(enemy);
                }
            }

            return results;
        }

        public static V2EnemyUnit FindDensestClusterCenter(float clusterRadius)
        {
            V2EnemyUnit best = null;
            int bestCount = -1;
            float bestProgress = -1f;

            for (int i = 0; i < activeEnemies.Count; i++)
            {
                V2EnemyUnit candidate = activeEnemies[i];
                if (candidate == null)
                {
                    continue;
                }

                int count = 0;
                for (int j = 0; j < activeEnemies.Count; j++)
                {
                    V2EnemyUnit other = activeEnemies[j];
                    if (other != null && Vector2.Distance(candidate.transform.position, other.transform.position) <= clusterRadius)
                    {
                        count++;
                    }
                }

                if (count > bestCount || count == bestCount && candidate.PathProgress > bestProgress)
                {
                    best = candidate;
                    bestCount = count;
                    bestProgress = candidate.PathProgress;
                }
            }

            return best;
        }

        public static float EnemyHealth(int tier, bool boss)
        {
            if (boss)
            {
                return tier >= 4 ? 1400f : 650f;
            }

            switch (tier)
            {
                case 1:
                    return 70f;
                case 2:
                    return 120f;
                case 3:
                    return 190f;
                default:
                    return 290f;
            }
        }

        public static int ExitDamage(int tier, bool boss)
        {
            if (boss)
            {
                return tier >= 4 ? 45 : 25;
            }

            switch (tier)
            {
                case 1:
                    return 5;
                case 2:
                    return 8;
                case 3:
                    return 12;
                default:
                    return 16;
            }
        }

        public static int CoinValue(int tier, bool boss)
        {
            if (boss)
            {
                return tier >= 4 ? 35 : 18;
            }

            return Mathf.Clamp(tier + 1, 2, 6);
        }

        private void MoveAlongPath()
        {
            if (path == null || path.Length == 0)
            {
                return;
            }

            if (nextPointIndex >= path.Length)
            {
                ReachExit();
                return;
            }

            Vector2 current = transform.position;
            Vector2 target = path[nextPointIndex];
            float step = moveSpeed * Time.deltaTime;
            Vector2 next = Vector2.MoveTowards(current, target, step);
            transform.position = next;

            float segmentDistance = Vector2.Distance(path[Mathf.Max(0, nextPointIndex - 1)], target);
            float segmentProgress = segmentDistance > 0f ? 1f - Vector2.Distance(next, target) / segmentDistance : 1f;
            PathProgress = Mathf.Clamp(nextPointIndex - 1 + segmentProgress, 0f, path.Length - 1);

            if (Vector2.Distance(next, target) <= 0.001f)
            {
                nextPointIndex++;
                if (nextPointIndex >= path.Length)
                {
                    ReachExit();
                }
            }
        }

        private void ReachExit()
        {
            if (V2GameController.Instance != null)
            {
                V2GameController.Instance.NotifyEnemyReachedExit(this);
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
            spriteRenderer.sortingOrder = 12;

            CircleCollider2D collider = GetComponent<CircleCollider2D>();
            if (collider == null)
            {
                collider = gameObject.AddComponent<CircleCollider2D>();
            }

            collider.isTrigger = true;
            collider.radius = IsBoss ? 0.52f : 0.36f;

            Rigidbody2D body = GetComponent<Rigidbody2D>();
            if (body == null)
            {
                body = gameObject.AddComponent<Rigidbody2D>();
            }

            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;

            if (label == null)
            {
                GameObject labelObject = new GameObject("Tier Label");
                labelObject.transform.SetParent(transform, false);
                labelObject.transform.localPosition = Vector3.zero;
                label = labelObject.AddComponent<TextMesh>();
                label.fontSize = 42;
                label.characterSize = 0.055f;
                label.anchor = TextAnchor.MiddleCenter;
                label.alignment = TextAlignment.Center;
                label.color = Color.black;
            }

            if (healthFill == null)
            {
                GameObject barRoot = new GameObject("Health Bar");
                barRoot.transform.SetParent(transform, false);
                barRoot.transform.localPosition = new Vector3(0f, 0.6f, 0f);

                GameObject back = new GameObject("Back");
                back.transform.SetParent(barRoot.transform, false);
                SpriteRenderer backRenderer = back.AddComponent<SpriteRenderer>();
                backRenderer.sprite = GreyboxSprites.Square;
                backRenderer.color = new Color(0.03f, 0.03f, 0.035f);
                backRenderer.sortingOrder = 23;
                back.transform.localScale = new Vector3(1.1f, 0.1f, 1f);

                GameObject fill = new GameObject("Fill");
                fill.transform.SetParent(barRoot.transform, false);
                SpriteRenderer fillRenderer = fill.AddComponent<SpriteRenderer>();
                fillRenderer.sprite = GreyboxSprites.Square;
                fillRenderer.color = new Color(0.3f, 1f, 0.28f);
                fillRenderer.sortingOrder = 24;
                healthFill = fill.transform;
            }
        }

        private void UpdateVisuals()
        {
            spriteRenderer.color = IsBoss ? new Color(0.42f, 0.38f, 0.48f) : Color.Lerp(Color.white, Color.black, 0.15f + EnemyTier * 0.1f);
            transform.localScale = Vector3.one * (IsBoss ? 0.95f : 0.5f + EnemyTier * 0.08f);
            label.text = IsBoss ? "B" : "T" + EnemyTier;
        }

        private void UpdateHealthBar()
        {
            if (healthFill == null || maxHealth <= 0f)
            {
                return;
            }

            float ratio = Mathf.Clamp01(health / maxHealth);
            healthFill.localScale = new Vector3(1.1f * ratio, 0.1f, 1f);
            healthFill.localPosition = new Vector3(-0.55f * (1f - ratio), 0f, 0f);
        }
    }
}
