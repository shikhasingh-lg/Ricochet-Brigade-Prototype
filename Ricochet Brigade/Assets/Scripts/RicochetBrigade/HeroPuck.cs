using System.Collections.Generic;
using UnityEngine;

namespace RicochetBrigade
{
    public sealed class HeroPuck : MonoBehaviour
    {
        private static readonly List<HeroPuck> allHeroes = new List<HeroPuck>();

        public BrigadeColor HeroColor { get; private set; }
        public HeroTier Tier { get; private set; }
        public bool IsFlying { get; private set; }
        public bool IsSettled { get; private set; }
        public Vector2Int AssignedCell { get; set; } = new Vector2Int(-1, -1);

        private Rigidbody2D body;
        private CircleCollider2D solidCollider;
        private CircleCollider2D mergeTrigger;
        private SpriteRenderer spriteRenderer;
        private float flightStartedAt;
        private float fireTimer;
        private int mergesThisFlight;
        private bool isSettling;

        public static IReadOnlyList<HeroPuck> All
        {
            get { return allHeroes; }
        }

        public static int SettledCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < allHeroes.Count; i++)
                {
                    if (allHeroes[i] != null && allHeroes[i].IsSettled)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        private void Awake()
        {
            EnsureComponents();
        }

        private void OnEnable()
        {
            if (!allHeroes.Contains(this))
            {
                allHeroes.Add(this);
            }
        }

        private void OnDisable()
        {
            allHeroes.Remove(this);
        }

        private void OnDestroy()
        {
            if (ArenaGrid.Instance != null)
            {
                ArenaGrid.Instance.Release(this);
            }
        }

        private void Update()
        {
            if (IsFlying)
            {
                UpdateFlight();
                return;
            }

            if (IsSettled)
            {
                UpdateAutoFire();
            }
        }

        public void PrepareLoaded(BrigadeColor color)
        {
            EnsureComponents();
            HeroColor = color;
            Tier = HeroTier.Bronze;
            IsFlying = false;
            IsSettled = false;
            mergesThisFlight = 0;
            body.simulated = false;
            solidCollider.enabled = false;
            mergeTrigger.enabled = false;
            UpdateVisuals();
        }

        public void Launch(Vector2 velocity)
        {
            EnsureComponents();
            IsFlying = true;
            IsSettled = false;
            mergesThisFlight = 0;
            isSettling = false;
            flightStartedAt = Time.time;
            body.simulated = true;
            body.bodyType = RigidbodyType2D.Dynamic;
            body.gravityScale = 0f;
            body.linearDamping = 0.18f;
            body.angularDamping = 0f;
            body.linearVelocity = velocity;
            solidCollider.enabled = true;
            mergeTrigger.enabled = true;
            UpdateVisuals();

            if (GameController.Instance != null)
            {
                GameController.Instance.RecordFlickRelease(HeroColor, Tier, velocity);
            }
        }

        public void ConsumeByMerge()
        {
            if (ArenaGrid.Instance != null)
            {
                ArenaGrid.Instance.Release(this);
            }

            Destroy(gameObject);
        }

        private void UpdateFlight()
        {
            if (Time.time - flightStartedAt < BrigadeTuning.MinFlightTime)
            {
                return;
            }

            if (ArenaGrid.Instance != null && ArenaGrid.Instance.IsBaseWorld(transform.position))
            {
                SettleAt(transform.position, true);
                return;
            }

            if (body.linearVelocity.magnitude <= BrigadeTuning.SettleSpeed)
            {
                SettleAt(transform.position, false);
            }
        }

        private void UpdateAutoFire()
        {
            fireTimer -= Time.deltaTime;
            if (fireTimer > 0f)
            {
                return;
            }

            EnemyMover target = EnemyMover.FindTarget(transform.position, BrigadeTuning.HeroRangeCells(HeroColor) * BrigadeTuning.CellSize);
            if (target == null)
            {
                return;
            }

            fireTimer = 0.35f;
            float damage = BrigadeTuning.HeroDamagePerSecond(HeroColor, Tier) * fireTimer;
            target.TakeDamage(damage, this, false);
            Debug.DrawLine(transform.position, target.transform.position, BrigadeTuning.HeroColor(HeroColor), 0.12f);

            if (GameController.Instance != null)
            {
                GameController.Instance.RecordDefenderAttack(this, target, damage);
            }
        }

        private void SettleAt(Vector2 stopWorldPosition, bool returnedToBase)
        {
            if (isSettling)
            {
                return;
            }

            isSettling = true;
            IsFlying = false;
            IsSettled = true;
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
            body.bodyType = RigidbodyType2D.Static;
            solidCollider.enabled = false;
            mergeTrigger.enabled = true;

            Vector2Int cell = new Vector2Int(-1, -1);
            float distanceToPath = 0f;
            bool settled = ArenaGrid.Instance != null && ArenaGrid.Instance.TrySettleHero(this, stopWorldPosition, out cell, out distanceToPath);
            if (!settled)
            {
                Destroy(gameObject);
                return;
            }

            UpdateVisuals();

            if (GameController.Instance != null)
            {
                GameController.Instance.RecordHeroSettle(this, cell, distanceToPath, returnedToBase, mergesThisFlight);
            }
        }

        private void PromoteFlyingHero()
        {
            if (Tier == HeroTier.Gold)
            {
                return;
            }

            HeroTier oldTier = Tier;
            Tier = Tier == HeroTier.Bronze ? HeroTier.Silver : HeroTier.Gold;
            mergesThisFlight++;
            UpdateVisuals();

            if (GameController.Instance != null)
            {
                GameController.Instance.RecordMidFlightMerge(HeroColor, oldTier, Tier);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsFlying || other.attachedRigidbody == body)
            {
                return;
            }

            HeroPuck otherHero = other.GetComponent<HeroPuck>();
            if (otherHero != null && otherHero != this && otherHero.IsSettled)
            {
                if (otherHero.HeroColor == HeroColor && Tier != HeroTier.Gold)
                {
                    otherHero.ConsumeByMerge();
                    PromoteFlyingHero();
                }

                return;
            }

            EnemyMover enemy = other.GetComponent<EnemyMover>();
            if (enemy != null)
            {
                float velocityFactor = Mathf.Clamp01(body.linearVelocity.magnitude / BrigadeTuning.MaxLaunchSpeed);
                float damage = ((int)Tier) * Mathf.Lerp(20f, 90f, velocityFactor);
                enemy.TakeDamage(damage, this, true);

                if (GameController.Instance != null)
                {
                    GameController.Instance.RecordMidFlightDamage(enemy, this, damage);
                }

                SettleAt(transform.position, false);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!IsFlying)
            {
                return;
            }

            RicochetWall wall = collision.collider.GetComponent<RicochetWall>();
            if (wall != null && GameController.Instance != null)
            {
                GameController.Instance.RecordRicochet(wall.surfaceName, body.linearVelocity.magnitude);
            }
        }

        private void EnsureComponents()
        {
            if (body == null)
            {
                body = GetComponent<Rigidbody2D>();
                if (body == null)
                {
                    body = gameObject.AddComponent<Rigidbody2D>();
                }

                body.gravityScale = 0f;
                body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                body.interpolation = RigidbodyInterpolation2D.Interpolate;
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                }

                spriteRenderer.sprite = GreyboxSprites.Circle;
                spriteRenderer.sortingOrder = 10;
            }

            CircleCollider2D[] colliders = GetComponents<CircleCollider2D>();
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].isTrigger)
                {
                    mergeTrigger = colliders[i];
                }
                else
                {
                    solidCollider = colliders[i];
                }
            }

            if (solidCollider == null)
            {
                solidCollider = gameObject.AddComponent<CircleCollider2D>();
                solidCollider.isTrigger = false;
            }

            if (mergeTrigger == null)
            {
                mergeTrigger = gameObject.AddComponent<CircleCollider2D>();
                mergeTrigger.isTrigger = true;
            }

            solidCollider.radius = 0.34f;
            mergeTrigger.radius = 0.48f;
        }

        private void UpdateVisuals()
        {
            EnsureComponents();
            Color color = BrigadeTuning.HeroColor(HeroColor);
            float scale = 0.7f + ((int)Tier - 1) * 0.18f;
            transform.localScale = Vector3.one * scale;
            spriteRenderer.color = IsFlying ? Color.Lerp(color, Color.white, 0.15f) : color;
            spriteRenderer.sortingOrder = IsFlying ? 20 : 12 + (int)Tier;
        }
    }
}
