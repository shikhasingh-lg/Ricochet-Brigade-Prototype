using System.Collections.Generic;
using UnityEngine;

namespace RicochetBrigade
{
    public sealed class EnemyMover : MonoBehaviour
    {
        private static readonly List<EnemyMover> activeEnemies = new List<EnemyMover>();

        public BrigadeColor EnemyColor { get; private set; }
        public bool IsBoss { get; private set; }
        public float PathProgress { get; private set; }

        private ArenaGrid grid;
        private Vector2[] pathPoints;
        private int nextPathIndex;
        private float speedCellsPerSecond;
        private float health;
        private float maxHealth;
        private int baseDamage;
        private SpriteRenderer spriteRenderer;
        private Transform healthFill;

        public static IReadOnlyList<EnemyMover> ActiveEnemies
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
            MoveAlongPath();
            UpdateHealthBar();
        }

        public void Setup(ArenaGrid arenaGrid, BrigadeColor enemyColor, bool boss)
        {
            grid = arenaGrid;
            EnemyColor = enemyColor;
            IsBoss = boss;
            pathPoints = grid.PathWorldPoints();
            nextPathIndex = pathPoints.Length > 1 ? 1 : 0;
            speedCellsPerSecond = boss ? 0.3f : 0.5f;
            maxHealth = BrigadeTuning.EnemyHealth(enemyColor, boss);
            health = maxHealth;
            baseDamage = BrigadeTuning.EnemyBaseDamage(enemyColor, boss);
            transform.position = pathPoints.Length > 0 ? pathPoints[0] : Vector2.zero;

            EnsureComponents();
            UpdateVisuals();
        }

        public void TakeDamage(float amount, HeroPuck source, bool midFlight)
        {
            float finalDamage = amount;
            if (source != null && source.HeroColor == EnemyColor && EnemyColor != BrigadeColor.Purple)
            {
                finalDamage *= 2f;
            }

            health -= finalDamage;
            if (health > 0f)
            {
                return;
            }

            if (GameController.Instance != null)
            {
                GameController.Instance.NotifyEnemyKilled(this, source, midFlight);
            }

            Destroy(gameObject);
        }

        public static EnemyMover FindTarget(Vector2 origin, float range)
        {
            EnemyMover bestTarget = null;
            float bestDistance = float.MaxValue;
            float bestProgress = -1f;

            for (int i = 0; i < activeEnemies.Count; i++)
            {
                EnemyMover enemy = activeEnemies[i];
                if (enemy == null)
                {
                    continue;
                }

                float distance = Vector2.Distance(origin, enemy.transform.position);
                if (distance > range)
                {
                    continue;
                }

                bool closer = distance < bestDistance - 0.01f;
                bool tieAhead = Mathf.Abs(distance - bestDistance) <= 0.01f && enemy.PathProgress > bestProgress;
                if (closer || tieAhead)
                {
                    bestTarget = enemy;
                    bestDistance = distance;
                    bestProgress = enemy.PathProgress;
                }
            }

            return bestTarget;
        }

        private void MoveAlongPath()
        {
            if (pathPoints == null || pathPoints.Length == 0)
            {
                return;
            }

            if (nextPathIndex >= pathPoints.Length)
            {
                ReachBase();
                return;
            }

            Vector2 current = transform.position;
            Vector2 target = pathPoints[nextPathIndex];
            float step = speedCellsPerSecond * grid.cellSize * Time.deltaTime;
            Vector2 next = Vector2.MoveTowards(current, target, step);
            transform.position = next;

            Vector2 previous = pathPoints[Mathf.Max(0, nextPathIndex - 1)];
            float segmentLength = Vector2.Distance(previous, target);
            float segmentProgress = segmentLength > 0f ? 1f - Vector2.Distance(next, target) / segmentLength : 1f;
            PathProgress = Mathf.Clamp(nextPathIndex - 1 + segmentProgress, 0f, pathPoints.Length - 1);

            if (Vector2.Distance(next, target) <= 0.001f)
            {
                nextPathIndex++;
                if (nextPathIndex >= pathPoints.Length)
                {
                    ReachBase();
                }
            }
        }

        private void ReachBase()
        {
            if (GameController.Instance != null)
            {
                GameController.Instance.NotifyEnemyReachedBase(this, baseDamage);
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
            spriteRenderer.sortingOrder = 15;

            CircleCollider2D collider = GetComponent<CircleCollider2D>();
            if (collider == null)
            {
                collider = gameObject.AddComponent<CircleCollider2D>();
            }

            collider.isTrigger = true;
            collider.radius = IsBoss ? 0.52f : 0.38f;

            if (healthFill == null)
            {
                GameObject barRoot = new GameObject("Health Bar");
                barRoot.transform.SetParent(transform, false);
                barRoot.transform.localPosition = new Vector3(0f, 0.58f, 0f);

                GameObject back = new GameObject("Back");
                back.transform.SetParent(barRoot.transform, false);
                SpriteRenderer backRenderer = back.AddComponent<SpriteRenderer>();
                backRenderer.sprite = GreyboxSprites.Square;
                backRenderer.color = new Color(0.05f, 0.05f, 0.05f);
                backRenderer.sortingOrder = 24;
                back.transform.localScale = new Vector3(1.2f, 0.12f, 1f);

                GameObject fill = new GameObject("Fill");
                fill.transform.SetParent(barRoot.transform, false);
                SpriteRenderer fillRenderer = fill.AddComponent<SpriteRenderer>();
                fillRenderer.sprite = GreyboxSprites.Square;
                fillRenderer.color = new Color(0.3f, 1f, 0.28f);
                fillRenderer.sortingOrder = 25;
                healthFill = fill.transform;
            }
        }

        private void UpdateVisuals()
        {
            Color color = BrigadeTuning.HeroColor(EnemyColor);
            spriteRenderer.color = IsBoss ? Color.Lerp(color, Color.black, 0.1f) : Color.Lerp(color, Color.black, 0.32f);
            transform.localScale = Vector3.one * (IsBoss ? 0.95f : 0.58f);
        }

        private void UpdateHealthBar()
        {
            if (healthFill == null || maxHealth <= 0f)
            {
                return;
            }

            float ratio = Mathf.Clamp01(health / maxHealth);
            healthFill.localScale = new Vector3(1.2f * ratio, 0.12f, 1f);
            healthFill.localPosition = new Vector3(-0.6f * (1f - ratio), 0f, 0f);
        }
    }
}
