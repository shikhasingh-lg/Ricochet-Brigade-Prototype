using System.Collections.Generic;
using UnityEngine;

namespace RicochetBrigade
{
    public sealed class V2HeroUnit : MonoBehaviour
    {
        public BrigadeColor HeroColor { get; private set; }
        public HeroTier Tier { get; private set; }
        public HeroSlot Slot { get; private set; }
        public float UltCharge { get; private set; }

        private SpriteRenderer bodyRenderer;
        private SpriteRenderer ultRingRenderer;
        private TextMesh label;
        private float attackTimer;

        public bool IsUltReady
        {
            get { return UltCharge >= 100f; }
        }

        private void Update()
        {
            UpdateCombat();
            UpdateVisualState();
        }

        private void OnDestroy()
        {
            if (Slot != null)
            {
                Slot.ClearOccupant(this);
            }
        }

        public void Setup(BrigadeColor color, HeroTier tier, HeroSlot slot)
        {
            HeroColor = color;
            Tier = tier;
            Slot = slot;
            transform.position = slot.WorldPosition;
            EnsureComponents();
            UpdateVisuals();
        }

        public bool CanMergeWith(V2HeroUnit other)
        {
            if (other == null || other == this || Tier == HeroTier.Gold)
            {
                return false;
            }

            return HeroColor == other.HeroColor && Tier == other.Tier && Slot != null && Slot.IsAdjacentTo(other.Slot);
        }

        public bool MergeInto(V2HeroUnit target)
        {
            if (!CanMergeWith(target))
            {
                return false;
            }

            HeroTier oldTier = Tier;
            target.Promote();
            Destroy(gameObject);

            if (V2GameController.Instance != null)
            {
                V2GameController.Instance.RecordHeroMerge(HeroColor, oldTier, target.Tier);
            }

            return true;
        }

        public int FireUlt()
        {
            if (!IsUltReady)
            {
                return 0;
            }

            int hits = 0;
            if (HeroColor == BrigadeColor.Red)
            {
                float radius = 1.45f + (int)Tier * 0.25f;
                List<V2EnemyUnit> enemies = V2EnemyUnit.EnemiesInRadius(transform.position, radius);
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].Stun(2f);
                    enemies[i].TakeDamage(22f * (int)Tier * DamageUpgradeMultiplier(), "red_ult");
                    hits++;
                }
            }
            else if (HeroColor == BrigadeColor.Blue)
            {
                int arrows = 4 + (int)Tier * 2;
                for (int i = 0; i < arrows; i++)
                {
                    V2EnemyUnit target = V2EnemyUnit.FindTarget(transform.position, 7.5f);
                    if (target == null)
                    {
                        break;
                    }

                    target.TakeDamage(18f * (int)Tier, "blue_ult");
                    hits++;
                }
            }
            else
            {
                V2EnemyUnit clusterCenter = V2EnemyUnit.FindDensestClusterCenter(1.5f);
                if (clusterCenter != null)
                {
                    float radius = (1.25f + (int)Tier * 0.25f) * YellowSplashUpgradeMultiplier();
                    List<V2EnemyUnit> enemies = V2EnemyUnit.EnemiesInRadius(clusterCenter.transform.position, radius);
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        enemies[i].TakeDamage(32f * (int)Tier, "yellow_ult");
                        hits++;
                    }

                    CreateBurst(clusterCenter.transform.position, radius, BrigadeTuning.HeroColor(HeroColor));
                }
            }

            UltCharge = 0f;
            return hits;
        }

        private void Promote()
        {
            if (Tier == HeroTier.Bronze)
            {
                Tier = HeroTier.Silver;
            }
            else if (Tier == HeroTier.Silver)
            {
                Tier = HeroTier.Gold;
            }

            UltCharge = Mathf.Min(UltCharge, 80f);
            UpdateVisuals();
        }

        private void UpdateCombat()
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer > 0f)
            {
                return;
            }

            V2EnemyUnit target = V2EnemyUnit.FindTarget(transform.position, Range());
            if (target == null)
            {
                return;
            }

            attackTimer = AttackInterval();
            float damage = Damage();

            if (HeroColor == BrigadeColor.Yellow)
            {
                List<V2EnemyUnit> enemies = V2EnemyUnit.EnemiesInRadius(target.transform.position, (0.75f + (int)Tier * 0.08f) * YellowSplashUpgradeMultiplier());
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].TakeDamage(damage * 0.75f, "hero_attack");
                }
            }
            else
            {
                target.TakeDamage(damage, "hero_attack");
            }

            UltCharge = Mathf.Min(100f, UltCharge + AttackInterval() * 10f * UltChargeUpgradeMultiplier());
            Debug.DrawLine(transform.position, target.transform.position, BrigadeTuning.HeroColor(HeroColor), 0.12f);
        }

        private float Range()
        {
            if (HeroColor == BrigadeColor.Red)
            {
                return 2.5f;
            }

            if (HeroColor == BrigadeColor.Blue)
            {
                return 5.8f;
            }

            return 3.8f;
        }

        private float AttackInterval()
        {
            float interval;
            if (HeroColor == BrigadeColor.Red)
            {
                interval = 0.55f;
            }
            else if (HeroColor == BrigadeColor.Blue)
            {
                interval = 0.82f;
            }
            else
            {
                interval = 1.12f;
            }

            if (V2GameController.Instance != null)
            {
                interval /= V2GameController.Instance.HeroAttackSpeedMultiplier(HeroColor);
            }

            return interval;
        }

        private float Damage()
        {
            float baseDamage;
            if (HeroColor == BrigadeColor.Red)
            {
                baseDamage = 14f;
            }
            else if (HeroColor == BrigadeColor.Blue)
            {
                baseDamage = 10f;
            }
            else
            {
                baseDamage = 12f;
            }

            return baseDamage * Mathf.Pow(2f, (int)Tier - 1) * DamageUpgradeMultiplier();
        }

        private float DamageUpgradeMultiplier()
        {
            return V2GameController.Instance != null ? V2GameController.Instance.HeroDamageMultiplier(HeroColor) : 1f;
        }

        private float YellowSplashUpgradeMultiplier()
        {
            return V2GameController.Instance != null ? V2GameController.Instance.YellowSplashRadiusMultiplier() : 1f;
        }

        private float UltChargeUpgradeMultiplier()
        {
            return V2GameController.Instance != null ? V2GameController.Instance.UltChargeMultiplier() : 1f;
        }

        private void EnsureComponents()
        {
            bodyRenderer = GetComponent<SpriteRenderer>();
            if (bodyRenderer == null)
            {
                bodyRenderer = gameObject.AddComponent<SpriteRenderer>();
            }

            CircleCollider2D collider = GetComponent<CircleCollider2D>();
            if (collider == null)
            {
                collider = gameObject.AddComponent<CircleCollider2D>();
            }

            collider.isTrigger = true;
            collider.radius = 0.42f;

            if (label == null)
            {
                GameObject labelObject = new GameObject("Hero Label");
                labelObject.transform.SetParent(transform, false);
                labelObject.transform.localPosition = Vector3.zero;
                label = labelObject.AddComponent<TextMesh>();
                label.fontSize = 48;
                label.characterSize = 0.055f;
                label.anchor = TextAnchor.MiddleCenter;
                label.alignment = TextAlignment.Center;
                label.color = Color.black;
            }

            if (ultRingRenderer == null)
            {
                GameObject ring = new GameObject("Ult Ready Ring");
                ring.transform.SetParent(transform, false);
                ring.transform.localScale = Vector3.one * 1.35f;
                ultRingRenderer = ring.AddComponent<SpriteRenderer>();
                ultRingRenderer.sprite = GreyboxSprites.Circle;
                ultRingRenderer.color = Color.clear;
                ultRingRenderer.sortingOrder = 15;
            }
        }

        private void UpdateVisuals()
        {
            EnsureComponents();
            bodyRenderer.sprite = HeroColor == BrigadeColor.Blue ? GreyboxSprites.Triangle : HeroColor == BrigadeColor.Yellow ? GreyboxSprites.Circle : GreyboxSprites.Square;
            bodyRenderer.color = BrigadeTuning.HeroColor(HeroColor);
            bodyRenderer.sortingOrder = 16;
            transform.localScale = Vector3.one * (0.62f + ((int)Tier - 1) * 0.12f);
            label.text = LabelText();
        }

        private void UpdateVisualState()
        {
            if (ultRingRenderer == null)
            {
                return;
            }

            ultRingRenderer.color = IsUltReady ? new Color(1f, 1f, 1f, 0.65f + Mathf.Sin(Time.time * 12f) * 0.18f) : Color.clear;
        }

        private string LabelText()
        {
            string letter = HeroColor == BrigadeColor.Red ? "R" : HeroColor == BrigadeColor.Blue ? "B" : "Y";
            if (Tier == HeroTier.Bronze)
            {
                return letter + "\n●○○";
            }

            return Tier == HeroTier.Silver ? letter + "\n●●○" : letter + "\n●●●";
        }

        private void CreateBurst(Vector2 position, float radius, Color color)
        {
            GameObject burst = new GameObject("Hero Ult Burst");
            burst.transform.position = position;
            burst.transform.localScale = Vector3.one * radius * 2f;
            SpriteRenderer renderer = burst.AddComponent<SpriteRenderer>();
            renderer.sprite = GreyboxSprites.Circle;
            renderer.color = new Color(color.r, color.g, color.b, 0.22f);
            renderer.sortingOrder = 30;
            Destroy(burst, 0.22f);
        }
    }
}
