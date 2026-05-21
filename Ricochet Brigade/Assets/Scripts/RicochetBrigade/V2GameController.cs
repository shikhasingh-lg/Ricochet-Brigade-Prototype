using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace RicochetBrigade
{
    public sealed class V2GameController : MonoBehaviour
    {
        private enum V2ScreenState
        {
            Boot,
            Hub,
            PartyPreview,
            Match,
            Paused,
            StageClear,
            StageFail,
            RunEnd
        }

        private sealed class HeroCard
        {
            public BrigadeColor color;
            public HeroTier tier;
            public int cost;
            public Rect worldRect;
            public GameObject root;
            public SpriteRenderer background;
            public TextMesh label;
        }

        private enum UpgradeCategory
        {
            Hero,
            Slingshot,
            Economy
        }

        private enum UpgradeEffect
        {
            RedDamage,
            BlueAttackSpeed,
            YellowSplashRadius,
            UltChargeSpeed,
            SlingBounce,
            SlingRadius,
            SlingDamage,
            SlingCost,
            PassiveIncome,
            HeroCardCost,
            KillCoins,
            StartingCoins
        }

        private sealed class UpgradeDefinition
        {
            public readonly string id;
            public readonly string title;
            public readonly string description;
            public readonly UpgradeCategory category;
            public readonly UpgradeEffect effect;

            public UpgradeDefinition(string id, string title, string description, UpgradeCategory category, UpgradeEffect effect)
            {
                this.id = id;
                this.title = title;
                this.description = description;
                this.category = category;
                this.effect = effect;
            }
        }

        private sealed class FloatingFeedback
        {
            public Text text;
            public RectTransform rect;
            public Vector2 start;
            public Vector2 velocity;
            public Color color;
            public float startedAt;
            public float duration;
        }

        public static V2GameController Instance { get; private set; }

        [Header("Scene Links")]
        public V2ArenaLayout layout;
        public V2SlingshotSpell slingshot;

        [Header("Economy")]
        public int coins = 12;
        public int heroCost = 10;
        public int slingshotCost = 5;

        [Header("Run")]
        public int stage = 1;
        public int hp = 100;

        private const int MaxStage = 5;
        private const int MaxHp = 100;
        private const int StageStartCoins = 12;

        private static readonly UpgradeDefinition[] UpgradeDeck =
        {
            new UpgradeDefinition("hero_red_damage", "Red Training", "Red Bruiser damage +15%", UpgradeCategory.Hero, UpgradeEffect.RedDamage),
            new UpgradeDefinition("hero_blue_speed", "Blue Tempo", "Blue Archer attack speed +20%", UpgradeCategory.Hero, UpgradeEffect.BlueAttackSpeed),
            new UpgradeDefinition("hero_yellow_splash", "Yellow Focus", "Yellow Mage splash radius +20%", UpgradeCategory.Hero, UpgradeEffect.YellowSplashRadius),
            new UpgradeDefinition("hero_ult_charge", "Battle Rhythm", "All heroes charge ults +20%", UpgradeCategory.Hero, UpgradeEffect.UltChargeSpeed),
            new UpgradeDefinition("sling_bounce", "Rubber Band", "Slingshot gains +1 bounce", UpgradeCategory.Slingshot, UpgradeEffect.SlingBounce),
            new UpgradeDefinition("sling_radius", "Bigger Blast", "Slingshot AoE radius +20%", UpgradeCategory.Slingshot, UpgradeEffect.SlingRadius),
            new UpgradeDefinition("sling_damage", "Heavy Stone", "Slingshot damage +25%", UpgradeCategory.Slingshot, UpgradeEffect.SlingDamage),
            new UpgradeDefinition("sling_cost", "Efficient Pull", "Slingshot cost -1 coin", UpgradeCategory.Slingshot, UpgradeEffect.SlingCost),
            new UpgradeDefinition("econ_passive", "Tax Route", "Passive income +50%", UpgradeCategory.Economy, UpgradeEffect.PassiveIncome),
            new UpgradeDefinition("econ_cards", "Bulk Hire", "Bronze hero cards cost -1", UpgradeCategory.Economy, UpgradeEffect.HeroCardCost),
            new UpgradeDefinition("econ_kills", "Bounty Board", "Enemy kill coins +50%", UpgradeCategory.Economy, UpgradeEffect.KillCoins),
            new UpgradeDefinition("econ_start", "War Chest", "Stage-start coins +3", UpgradeCategory.Economy, UpgradeEffect.StartingCoins)
        };

        private readonly HeroCard[] cards = new HeroCard[3];
        private readonly UpgradeDefinition[] offeredUpgrades = new UpgradeDefinition[3];
        private readonly Button[] upgradeButtons = new Button[3];
        private readonly Text[] upgradeButtonLabels = new Text[3];
        private readonly HashSet<string> chosenUpgradeIds = new HashSet<string>();
        private readonly List<FloatingFeedback> floatingFeedback = new List<FloatingFeedback>();
        private Canvas hudCanvas;
        private GameObject matchHudRoot;
        private Text hudText;
        private Text hpText;
        private Text coinText;
        private Text stageText;
        private Text progressText;
        private Text slingshotText;
        private Text hintText;
        private Text centerText;
        private Image hpFillImage;
        private Image progressFillImage;
        private Image screenFlashImage;
        private Text bootBodyText;
        private Text partyBodyText;
        private Text stageClearBodyText;
        private Text stageFailBodyText;
        private Text runEndBodyText;
        private GameObject bootPanel;
        private GameObject hubPanel;
        private GameObject partyPanel;
        private GameObject pausePanel;
        private GameObject stageClearPanel;
        private GameObject stageFailPanel;
        private GameObject runEndPanel;
        private GameObject pauseButtonObject;
        private V2ScreenState screenState;
        private int selectedCardIndex = -1;
        private int drawCounter;
        private int spawnedThisStage;
        private int enemiesThisStage;
        private int enemiesDefeatedThisRun;
        private int mergesThisRun;
        private int ultsThisRun;
        private int slingshotsThisRun;
        private int minTierThisStage;
        private int maxTierThisStage;
        private bool bossStage;
        private bool miniBossStage;
        private float enemySpeed;
        private float spawnInterval;
        private float nextSpawnAt;
        private float nextPassiveCoinAt;
        private float bootEndsAt;
        private V2HeroUnit mergeSource;
        private bool draggingMerge;
        private bool draggingSlingshot;
        private Camera mainCamera;
        private int baseHeroCost;
        private int baseSlingshotCost;
        private int baseSlingshotBounces;
        private float baseSlingshotRadius;
        private float baseSlingshotDamage;
        private float redDamageMultiplier = 1f;
        private float blueAttackSpeedMultiplier = 1f;
        private float yellowSplashRadiusMultiplier = 1f;
        private float ultChargeMultiplier = 1f;
        private float passiveCoinAmount = 1f;
        private float passiveCoinBank;
        private float killCoinMultiplier = 1f;
        private int stageStartCoinBonus;
        private float toastEndsAt;
        private float coinPulseUntil;
        private float hpPulseUntil;
        private float slingshotPulseUntil;
        private float screenFlashUntil;
        private float shakeUntil;
        private float shakeMagnitude;
        private Vector3 cameraBasePosition;
        private bool hasCameraBasePosition;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Time.timeScale = 1f;
            mainCamera = Camera.main;
            if (layout == null)
            {
                layout = V2ArenaLayout.Instance != null ? V2ArenaLayout.Instance : FindFirstObjectByType<V2ArenaLayout>();
            }

            if (slingshot == null)
            {
                slingshot = FindFirstObjectByType<V2SlingshotSpell>();
            }

            if (slingshot != null)
            {
                slingshot.layout = layout;
            }

            StoreBaseTuning();
            CreateHud();
            CreateCardRow();
            ShowBoot();
        }

        private void Update()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            if (screenState == V2ScreenState.Boot && Time.unscaledTime >= bootEndsAt)
            {
                ShowHub();
            }

            if (screenState == V2ScreenState.Match && PausePressed())
            {
                ShowPause();
            }
            else if (screenState == V2ScreenState.Paused && PausePressed())
            {
                ResumeMatch();
            }

            if (screenState == V2ScreenState.Match)
            {
                HandleInput();
                TickEconomy();
                SpawnEnemies();
                CheckStageClear();
            }

            UpdateCards();
            UpdateHud();
            UpdateFloatingFeedback();
            UpdateCameraFeedback();
        }

        public void NotifyEnemyKilled(V2EnemyUnit enemy, string source)
        {
            enemiesDefeatedThisRun++;
            int coinValue = Mathf.Max(1, Mathf.RoundToInt(V2EnemyUnit.CoinValue(enemy.EnemyTier, enemy.IsBoss) * killCoinMultiplier));
            AddCoins(coinValue, source);
            Debug.Log("enemy_death source=" + source + " tier=" + enemy.EnemyTier + " boss=" + enemy.IsBoss);
        }

        public void NotifyEnemyReachedExit(V2EnemyUnit enemy)
        {
            if (screenState != V2ScreenState.Match)
            {
                return;
            }

            hp = Mathf.Max(0, hp - V2EnemyUnit.ExitDamage(enemy.EnemyTier, enemy.IsBoss));
            hpPulseUntil = Time.unscaledTime + 0.45f;
            screenFlashUntil = Time.unscaledTime + 0.28f;
            shakeUntil = Time.unscaledTime + 0.25f;
            shakeMagnitude = enemy.IsBoss ? 0.22f : 0.13f;
            ShowToast("Enemy reached exit! HP " + hp + "/" + MaxHp, new Color(1f, 0.34f, 0.24f), 1.6f);
            Debug.Log("enemy_reached_exit tier=" + enemy.EnemyTier + " boss=" + enemy.IsBoss + " hp_remaining=" + hp);

            if (hp <= 0)
            {
                Debug.Log("stage_fail stage=" + stage + " runtime=" + Time.time.ToString("0.0"));
                ShowStageFail();
            }
        }

        public void RecordHeroMerge(BrigadeColor color, HeroTier fromTier, HeroTier toTier)
        {
            mergesThisRun++;
            Debug.Log("hero_merge color=" + color + " tier_from=" + fromTier + " tier_to=" + toTier);
        }

        public void RecordSlingshotResult(float power, int bounces, int enemiesHit, float radius, string reason)
        {
            Debug.Log("slingshot_fired power=" + power.ToString("0.00") + " bounces=" + bounces + " enemies_hit=" + enemiesHit + " explosion_radius=" + radius.ToString("0.0") + " reason=" + reason);
            if (enemiesHit > 0)
            {
                ShowToast("Slingshot hit " + enemiesHit + "!", new Color(1f, 0.84f, 0.22f), 1.1f);
                slingshotPulseUntil = Time.unscaledTime + 0.45f;
            }
            else
            {
                ShowToast("Slingshot missed", new Color(1f, 0.72f, 0.45f), 1.1f);
            }

            if (enemiesHit <= 0)
            {
                Debug.Log("slingshot_miss reason=" + reason);
            }
        }

        public float HeroDamageMultiplier(BrigadeColor color)
        {
            return color == BrigadeColor.Red ? redDamageMultiplier : 1f;
        }

        public float HeroAttackSpeedMultiplier(BrigadeColor color)
        {
            return color == BrigadeColor.Blue ? blueAttackSpeedMultiplier : 1f;
        }

        public float YellowSplashRadiusMultiplier()
        {
            return yellowSplashRadiusMultiplier;
        }

        public float UltChargeMultiplier()
        {
            return ultChargeMultiplier;
        }

        private void HandleInput()
        {
            if (Pointer.current == null)
            {
                return;
            }

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Vector2 world = ScreenToWorld(Pointer.current.position.ReadValue());

            if (Pointer.current.press.wasPressedThisFrame)
            {
                BeginPointer(world);
            }

            if (Pointer.current.press.isPressed && draggingSlingshot)
            {
                slingshot.UpdateDrag(world);
            }

            if (Pointer.current.press.wasReleasedThisFrame)
            {
                EndPointer(world);
            }
        }

        private void BeginPointer(Vector2 world)
        {
            int cardIndex = CardIndexAt(world);
            if (cardIndex >= 0)
            {
                HeroCard card = cards[cardIndex];
                if (coins < card.cost)
                {
                    ShowToast("Need " + card.cost + " coins for this card.", new Color(1f, 0.35f, 0.25f), 1.2f);
                    PulseCoins();
                    return;
                }

                selectedCardIndex = cardIndex;
                ShowToast(CardLetter(card.color) + " " + card.tier + " selected. Tap a slot.", Color.white, 1.0f);
                return;
            }

            V2HeroUnit hero = layout.FindHeroAt(world);
            if (hero != null)
            {
                if (hero.IsUltReady)
                {
                    int hits = hero.FireUlt();
                    ultsThisRun++;
                    ShowToast(CardLetter(hero.HeroColor) + " ult hit " + hits + "!", new Color(0.72f, 1f, 0.52f), 1.0f);
                    Debug.Log("ult_fired hero_color=" + hero.HeroColor + " hero_tier=" + hero.Tier + " enemies_hit=" + hits);
                    return;
                }

                mergeSource = hero;
                draggingMerge = true;
                return;
            }

            if (selectedCardIndex >= 0)
            {
                TryPlaceSelectedCard(world);
                return;
            }

            if (slingshot != null && slingshot.CanBeginDrag(world))
            {
                if (coins < slingshotCost)
                {
                    ShowToast("Need " + slingshotCost + " coins for slingshot.", new Color(1f, 0.35f, 0.25f), 1.2f);
                    PulseCoins();
                    return;
                }

                draggingSlingshot = true;
                slingshot.BeginDrag(world);
            }
        }

        private void EndPointer(Vector2 world)
        {
            if (draggingSlingshot)
            {
                draggingSlingshot = false;
                float shotPower = slingshot.Release();
                if (shotPower > 0f)
                {
                    slingshotsThisRun++;
                    SpendCoins(slingshotCost, "slingshot");
                }
                else
                {
                    ShowToast("Pull farther to fire slingshot.", new Color(1f, 0.78f, 0.36f), 1.2f);
                }
            }

            if (draggingMerge)
            {
                draggingMerge = false;
                V2HeroUnit target = layout.FindHeroAt(world);
                if (mergeSource != null && target != null && mergeSource.MergeInto(target))
                {
                    ShowToast("Merged to " + target.Tier + "!", new Color(0.65f, 1f, 0.55f), 1.2f);
                }
                else if (mergeSource != null && target != null)
                {
                    ShowToast("Merge needs adjacent matching heroes.", new Color(1f, 0.72f, 0.45f), 1.2f);
                }

                mergeSource = null;
            }
        }

        private bool TryPlaceSelectedCard(Vector2 world)
        {
            if (selectedCardIndex < 0)
            {
                return false;
            }

            HeroCard card = cards[selectedCardIndex];
            if (coins < card.cost)
            {
                ShowToast("Need " + card.cost + " coins for this card.", new Color(1f, 0.35f, 0.25f), 1.2f);
                PulseCoins();
                return false;
            }

            if (!layout.TryFindNearestEmptySlot(world, out HeroSlot slot))
            {
                ShowToast(HasEmptyHeroSlot() ? "Tap an empty platform slot." : "Board full — merge heroes to free a slot.", new Color(1f, 0.78f, 0.36f), 1.4f);
                return false;
            }

            SpendCoins(card.cost, "hero_spawn");
            layout.SpawnHero(slot, card.color, card.tier);
            ShowToast(CardLetter(card.color) + " hero placed!", new Color(0.72f, 1f, 0.52f), 0.9f);
            Debug.Log("hero_spawn color=" + card.color + " tier=" + card.tier + " slot=" + slot.slotId + " stage=" + stage);
            DrawCard(selectedCardIndex);
            selectedCardIndex = -1;
            return true;
        }

        private void StartRun()
        {
            ClearRunObjects();
            ResetRunTuning();
            stage = 1;
            hp = MaxHp;
            coins = StageStartCoins + stageStartCoinBonus;
            drawCounter = 0;
            enemiesDefeatedThisRun = 0;
            mergesThisRun = 0;
            ultsThisRun = 0;
            slingshotsThisRun = 0;
            selectedCardIndex = -1;

            for (int i = 0; i < cards.Length; i++)
            {
                DrawCard(i);
            }

            StartStage(1);
        }

        private void RetryStage()
        {
            ClearStageObjects();
            hp = MaxHp;
            coins = StageStartCoins + stageStartCoinBonus;
            selectedCardIndex = -1;
            StartStage(stage);
        }

        private void StartStage(int newStage)
        {
            Time.timeScale = 1f;
            stage = Mathf.Clamp(newStage, 1, MaxStage);
            hp = Mathf.Min(MaxHp, hp + (stage == 1 ? 0 : 15));
            coins = StageStartCoins + stageStartCoinBonus;
            spawnedThisStage = 0;
            nextSpawnAt = Time.time + 1f;
            nextPassiveCoinAt = Time.time + 2f;
            passiveCoinBank = 0f;
            selectedCardIndex = -1;
            mergeSource = null;
            draggingMerge = false;
            draggingSlingshot = false;
            ConfigureStage(stage);
            ShowMatch();
            ShowToast("Stage " + stage + ": place heroes, merge, ult, and sling.", Color.white, 3.0f);
            Debug.Log("stage_start stage=" + stage + " enemy_count=" + enemiesThisStage);
        }

        private void ConfigureStage(int stageNumber)
        {
            miniBossStage = false;
            bossStage = false;

            if (stageNumber == 1)
            {
                enemiesThisStage = 20;
                minTierThisStage = 1;
                maxTierThisStage = 1;
                enemySpeed = 0.8f;
                spawnInterval = 2.4f;
            }
            else if (stageNumber == 2)
            {
                enemiesThisStage = 35;
                minTierThisStage = 1;
                maxTierThisStage = 2;
                enemySpeed = 0.85f;
                spawnInterval = 1.9f;
            }
            else if (stageNumber == 3)
            {
                enemiesThisStage = 50;
                minTierThisStage = 1;
                maxTierThisStage = 2;
                enemySpeed = 1.05f;
                spawnInterval = 1.55f;
            }
            else if (stageNumber == 4)
            {
                enemiesThisStage = 70;
                minTierThisStage = 2;
                maxTierThisStage = 3;
                enemySpeed = 1.08f;
                spawnInterval = 1.25f;
                miniBossStage = true;
            }
            else
            {
                enemiesThisStage = 100;
                minTierThisStage = 2;
                maxTierThisStage = 4;
                enemySpeed = 1.24f;
                spawnInterval = 1.05f;
                bossStage = true;
            }
        }

        private void SpawnEnemies()
        {
            if (spawnedThisStage >= enemiesThisStage || Time.time < nextSpawnAt || V2EnemyUnit.ActiveEnemies.Count > 36)
            {
                return;
            }

            bool boss = bossStage && spawnedThisStage == enemiesThisStage - 1 || miniBossStage && spawnedThisStage == enemiesThisStage - 1;
            int tier = boss && bossStage ? 4 : Mathf.Clamp(minTierThisStage + spawnedThisStage % (maxTierThisStage - minTierThisStage + 1), minTierThisStage, maxTierThisStage);
            GameObject enemyObject = new GameObject(boss ? "V2 Boss Enemy" : "V2 Enemy T" + tier);
            V2EnemyUnit enemy = enemyObject.AddComponent<V2EnemyUnit>();
            enemy.Setup(layout, tier, boss, boss ? enemySpeed * 0.75f : enemySpeed);
            spawnedThisStage++;
            nextSpawnAt = Time.time + spawnInterval;
        }

        private void CheckStageClear()
        {
            if (spawnedThisStage < enemiesThisStage || V2EnemyUnit.ActiveEnemies.Count > 0)
            {
                return;
            }

            Debug.Log("stage_clear stage=" + stage + " hp=" + hp + " coins=" + coins);
            if (stage >= MaxStage)
            {
                ShowRunEnd();
                return;
            }

            ShowStageClear();
        }

        private void TickEconomy()
        {
            if (Time.time < nextPassiveCoinAt)
            {
                return;
            }

            passiveCoinBank += passiveCoinAmount;
            int payout = Mathf.FloorToInt(passiveCoinBank);
            if (payout > 0)
            {
                AddCoins(payout, "passive");
                passiveCoinBank -= payout;
            }

            nextPassiveCoinAt = Time.time + 2f;
        }

        private void AddCoins(int amount, string reason)
        {
            coins += amount;
            PulseCoins();
            AddFloatingText("+" + amount + " coins", new Color(1f, 0.86f, 0.22f), new Vector2(205f, -92f));
            Debug.Log("coins_changed amount=+" + amount + " reason=" + reason + " coins=" + coins);
        }

        private void SpendCoins(int amount, string reason)
        {
            coins = Mathf.Max(0, coins - amount);
            PulseCoins();
            AddFloatingText("-" + amount + " coins", new Color(1f, 0.45f, 0.34f), new Vector2(205f, -92f));
            Debug.Log("coins_changed amount=-" + amount + " reason=" + reason + " coins=" + coins);
        }

        private void CreateCardRow()
        {
            Vector2[] centers =
            {
                new Vector2(-0.65f, -6.18f),
                new Vector2(1.15f, -6.18f),
                new Vector2(2.95f, -6.18f)
            };

            for (int i = 0; i < cards.Length; i++)
            {
                HeroCard card = new HeroCard();
                card.worldRect = new Rect(centers[i].x - 0.7f, centers[i].y - 0.55f, 1.4f, 1.1f);
                card.root = new GameObject("Hero Card " + (i + 1));
                card.root.transform.position = centers[i];
                card.root.transform.localScale = new Vector3(1.35f, 1.05f, 1f);
                card.background = card.root.AddComponent<SpriteRenderer>();
                card.background.sprite = GreyboxSprites.Square;
                card.background.sortingOrder = 18;

                GameObject labelObject = new GameObject("Card Label");
                labelObject.transform.SetParent(card.root.transform, false);
                labelObject.transform.localPosition = Vector3.zero;
                card.label = labelObject.AddComponent<TextMesh>();
                card.label.fontSize = 58;
                card.label.characterSize = 0.055f;
                card.label.anchor = TextAnchor.MiddleCenter;
                card.label.alignment = TextAlignment.Center;
                card.label.color = Color.black;

                cards[i] = card;
                DrawCard(i);
            }
        }

        private void DrawCard(int index)
        {
            BrigadeColor color = (BrigadeColor)((drawCounter + index) % 3);
            HeroTier tier = HeroTier.Bronze;
            if (drawCounter > 0 && drawCounter % 17 == 0)
            {
                tier = HeroTier.Gold;
            }
            else if (drawCounter > 0 && drawCounter % 7 == 0)
            {
                tier = HeroTier.Silver;
            }

            cards[index].color = color;
            cards[index].tier = tier;
            cards[index].cost = CardCost(tier);
            drawCounter++;
        }

        private int CardCost(HeroTier tier)
        {
            if (tier == HeroTier.Silver)
            {
                return 25;
            }

            return tier == HeroTier.Gold ? 50 : heroCost;
        }

        private void RefreshCardCosts()
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i] != null)
                {
                    cards[i].cost = CardCost(cards[i].tier);
                }
            }
        }

        private void UpdateCards()
        {
            bool visible = screenState == V2ScreenState.Match;
            for (int i = 0; i < cards.Length; i++)
            {
                HeroCard card = cards[i];
                if (card.root != null && card.root.activeSelf != visible)
                {
                    card.root.SetActive(visible);
                }

                Color color = BrigadeTuning.HeroColor(card.color);
                bool affordable = coins >= card.cost;
                bool selected = selectedCardIndex == i;
                float selectedPulse = selected ? 1f + Mathf.Sin(Time.unscaledTime * 12f) * 0.035f : 1f;
                card.root.transform.localScale = new Vector3(selected ? 1.48f * selectedPulse : 1.35f, selected ? 1.14f * selectedPulse : 1.05f, 1f);
                card.background.color = affordable ? (selected ? Color.Lerp(color, Color.white, 0.35f) : Color.Lerp(color, Color.white, 0.08f)) : new Color(0.28f, 0.28f, 0.3f);
                card.label.color = affordable ? Color.black : new Color(1f, 0.42f, 0.34f);
                card.label.text = CardLetter(card.color) + " " + CardClassName(card.color) + "\n" + card.tier + "\n" + card.cost + "c";
            }
        }

        private string CardClassName(BrigadeColor color)
        {
            if (color == BrigadeColor.Red)
            {
                return "Bruiser";
            }

            return color == BrigadeColor.Blue ? "Archer" : "Mage";
        }

        private string CardLetter(BrigadeColor color)
        {
            if (color == BrigadeColor.Red)
            {
                return "R";
            }

            return color == BrigadeColor.Blue ? "B" : "Y";
        }

        private int CardIndexAt(Vector2 world)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i].worldRect.Contains(world))
                {
                    return i;
                }
            }

            return -1;
        }

        private void CreateHud()
        {
            EnsureEventSystem();

            hudCanvas = new GameObject("V2 Greybox HUD").AddComponent<Canvas>();
            hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = hudCanvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            hudCanvas.gameObject.AddComponent<GraphicRaycaster>();

            matchHudRoot = CreateFullRectObject("Match HUD", hudCanvas.transform);
            screenFlashImage = CreateFullScreenImage(matchHudRoot.transform, "Damage Flash", new Color(1f, 0.08f, 0.04f, 0f));

            CreateImage(matchHudRoot.transform, "Top HUD Backplate", new Color(0.02f, 0.025f, 0.035f, 0.72f), new Vector2(0f, -74f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 148f));

            Image hpBack = CreateImage(matchHudRoot.transform, "HP Bar Back", new Color(0.16f, 0.04f, 0.04f, 0.92f), new Vector2(36f, -32f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(315f, 44f));
            hpFillImage = CreateImage(hpBack.transform, "HP Bar Fill", new Color(0.25f, 0.9f, 0.36f, 0.95f), Vector2.zero, Vector2.zero, Vector2.one, Vector2.zero);
            hpFillImage.type = Image.Type.Filled;
            hpFillImage.fillMethod = Image.FillMethod.Horizontal;
            hpFillImage.fillOrigin = 0;

            hpText = CreateText(matchHudRoot.transform, "HP Text", TextAnchor.MiddleLeft, new Vector2(52f, -31f), new Vector2(0f, 1f), new Vector2(0f, 1f), 32);
            hpText.GetComponent<RectTransform>().sizeDelta = new Vector2(290f, 48f);

            coinText = CreateText(matchHudRoot.transform, "Coin Text", TextAnchor.UpperLeft, new Vector2(385f, -24f), new Vector2(0f, 1f), new Vector2(0f, 1f), 38);
            coinText.color = new Color(1f, 0.86f, 0.22f);
            coinText.GetComponent<RectTransform>().sizeDelta = new Vector2(280f, 70f);

            stageText = CreateText(matchHudRoot.transform, "Stage Text", TextAnchor.UpperLeft, new Vector2(650f, -24f), new Vector2(0f, 1f), new Vector2(0f, 1f), 34);
            stageText.GetComponent<RectTransform>().sizeDelta = new Vector2(300f, 70f);

            Image progressBack = CreateImage(matchHudRoot.transform, "Stage Progress Back", new Color(0.08f, 0.1f, 0.14f, 0.95f), new Vector2(36f, -100f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(520f, 18f));
            progressFillImage = CreateImage(progressBack.transform, "Stage Progress Fill", new Color(0.32f, 0.74f, 1f, 0.95f), Vector2.zero, Vector2.zero, Vector2.one, Vector2.zero);
            progressFillImage.type = Image.Type.Filled;
            progressFillImage.fillMethod = Image.FillMethod.Horizontal;
            progressFillImage.fillOrigin = 0;

            progressText = CreateText(matchHudRoot.transform, "Progress Text", TextAnchor.UpperLeft, new Vector2(570f, -91f), new Vector2(0f, 1f), new Vector2(0f, 1f), 24);
            progressText.color = new Color(0.82f, 0.92f, 1f);
            progressText.GetComponent<RectTransform>().sizeDelta = new Vector2(430f, 48f);

            slingshotText = CreateText(matchHudRoot.transform, "Slingshot Text", TextAnchor.LowerRight, new Vector2(-24f, 160f), new Vector2(1f, 0f), new Vector2(1f, 0f), 30);
            slingshotText.GetComponent<RectTransform>().sizeDelta = new Vector2(520f, 84f);

            hintText = CreateText(matchHudRoot.transform, "Action Hint Text", TextAnchor.LowerLeft, new Vector2(24f, 160f), new Vector2(0f, 0f), new Vector2(0f, 0f), 28);
            hintText.color = new Color(0.88f, 0.94f, 1f);
            hintText.GetComponent<RectTransform>().sizeDelta = new Vector2(560f, 84f);

            hudText = CreateText(matchHudRoot.transform, "Alive HUD", TextAnchor.UpperRight, new Vector2(-24f, -88f), new Vector2(1f, 1f), new Vector2(1f, 1f), 24);
            hudText.color = new Color(0.86f, 0.9f, 1f, 0.84f);
            hudText.GetComponent<RectTransform>().sizeDelta = new Vector2(360f, 52f);

            centerText = CreateText(matchHudRoot.transform, "Center Toast", TextAnchor.MiddleCenter, new Vector2(0f, 95f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), 46);
            centerText.GetComponent<RectTransform>().sizeDelta = new Vector2(880f, 220f);

            pauseButtonObject = CreateButton(matchHudRoot.transform, "Pause Button", "PAUSE", new Vector2(-94f, -54f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(170f, 78f), ShowPause).gameObject;
            CreateScreens();
        }

        private void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }

        private void CreateScreens()
        {
            bootPanel = CreateScreenPanel("Boot Screen", "RICOCHET BRIGADE", "V2 greybox prototype", out bootBodyText);
            hubPanel = CreateScreenPanel("Hub Screen", "RICOCHET BRIGADE", "Single-player prototype\nBuild heroes, bend shots, survive five stages.", out _);
            partyPanel = CreateScreenPanel("Party Preview Screen", "PARTY PREVIEW", "Starting party:\nRED Bruiser  |  BLUE Archer  |  YELLOW Mage", out partyBodyText);
            pausePanel = CreateScreenPanel("Pause Screen", "PAUSED", "Run is frozen.", out _);
            stageClearPanel = CreateScreenPanel("Stage Clear Screen", "STAGE CLEAR", string.Empty, out stageClearBodyText);
            stageFailPanel = CreateScreenPanel("Stage Fail Screen", "STAGE FAIL", string.Empty, out stageFailBodyText);
            runEndPanel = CreateScreenPanel("Run End Screen", "RUN COMPLETE", string.Empty, out runEndBodyText);

            CreateButton(hubPanel.transform, "Start Run Button", "START RUN", new Vector2(0f, -300f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(420f, 96f), ShowPartyPreview);
            CreateButton(partyPanel.transform, "Start Stage Button", "START STAGE 1", new Vector2(0f, -330f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(460f, 96f), StartRun);
            CreateButton(pausePanel.transform, "Resume Button", "RESUME", new Vector2(0f, -140f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(380f, 92f), ResumeMatch);
            CreateButton(pausePanel.transform, "Quit To Hub Button", "QUIT TO HUB", new Vector2(0f, -270f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(420f, 92f), ShowHubFromRun);
            CreateUpgradeButton(0, new Vector2(-310f, -205f));
            CreateUpgradeButton(1, new Vector2(0f, -205f));
            CreateUpgradeButton(2, new Vector2(310f, -205f));
            CreateButton(stageFailPanel.transform, "Retry Stage Button", "RETRY STAGE", new Vector2(0f, -220f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(420f, 92f), RetryStage);
            CreateButton(stageFailPanel.transform, "Fail Back To Hub Button", "BACK TO HUB", new Vector2(0f, -350f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(420f, 92f), ShowHubFromRun);
            CreateButton(runEndPanel.transform, "Run End Back To Hub Button", "BACK TO HUB", new Vector2(0f, -350f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(420f, 92f), ShowHubFromRun);
        }

        private void CreateUpgradeButton(int index, Vector2 position)
        {
            int capturedIndex = index;
            Button button = CreateButton(stageClearPanel.transform, "Upgrade Pick " + (index + 1), string.Empty, position, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(280f, 260f), () => ChooseUpgrade(capturedIndex));
            Text label = button.GetComponentInChildren<Text>();
            label.fontSize = 27;
            label.color = Color.black;
            label.alignment = TextAnchor.MiddleCenter;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            upgradeButtons[index] = button;
            upgradeButtonLabels[index] = label;
        }

        private GameObject CreateFullRectObject(string name, Transform parent)
        {
            GameObject root = new GameObject(name);
            root.transform.SetParent(parent, false);
            RectTransform rect = root.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return root;
        }

        private Image CreateFullScreenImage(Transform parent, string name, Color color)
        {
            GameObject imageObject = CreateFullRectObject(name, parent);
            Image image = imageObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private Image CreateImage(Transform parent, string name, Color color, Vector2 anchoredPosition, Vector2 anchorMin, Vector2 anchorMax, Vector2 size)
        {
            GameObject imageObject = new GameObject(name);
            imageObject.transform.SetParent(parent, false);

            RectTransform rect = imageObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            Image image = imageObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private GameObject CreateScreenPanel(string name, string title, string body, out Text bodyText)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(hudCanvas.transform, false);

            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image background = panel.AddComponent<Image>();
            background.color = new Color(0.02f, 0.025f, 0.035f, 0.88f);

            Text titleText = CreateText(panel.transform, name + " Title", TextAnchor.MiddleCenter, new Vector2(0f, 340f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), 70);
            titleText.text = title;
            titleText.color = new Color(1f, 0.92f, 0.45f);

            bodyText = CreateText(panel.transform, name + " Body", TextAnchor.MiddleCenter, new Vector2(0f, 90f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), 42);
            bodyText.text = body;
            RectTransform bodyRect = bodyText.GetComponent<RectTransform>();
            bodyRect.sizeDelta = new Vector2(920f, 520f);

            panel.SetActive(false);
            return panel;
        }

        private Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPosition, Vector2 anchorMin, Vector2 anchorMax, Vector2 size, UnityAction action)
        {
            GameObject buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);

            RectTransform rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.98f, 0.76f, 0.18f, 0.96f);

            Button button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(action);

            GameObject labelObject = new GameObject("Label");
            labelObject.transform.SetParent(buttonObject.transform, false);

            RectTransform labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            Text labelText = labelObject.AddComponent<Text>();
            labelText.font = DefaultFont();
            labelText.text = label;
            labelText.fontSize = 36;
            labelText.alignment = TextAnchor.MiddleCenter;
            labelText.color = Color.black;
            labelText.raycastTarget = false;
            return button;
        }

        private Text CreateText(Transform parent, string name, TextAnchor anchor, Vector2 anchoredPosition, Vector2 anchorMin, Vector2 anchorMax, int fontSize)
        {
            GameObject textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            Text text = textObject.AddComponent<Text>();
            text.font = DefaultFont();
            text.fontSize = fontSize;
            text.alignment = anchor;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;

            RectTransform rect = text.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(1000f, 280f);
            return text;
        }

        private Font DefaultFont()
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            return font;
        }

        private void StoreBaseTuning()
        {
            baseHeroCost = heroCost;
            baseSlingshotCost = slingshotCost;
            if (slingshot != null)
            {
                baseSlingshotBounces = slingshot.maxBounces;
                baseSlingshotRadius = slingshot.explosionRadius;
                baseSlingshotDamage = slingshot.explosionDamage;
            }
        }

        private void ResetRunTuning()
        {
            chosenUpgradeIds.Clear();
            heroCost = baseHeroCost;
            slingshotCost = baseSlingshotCost;
            redDamageMultiplier = 1f;
            blueAttackSpeedMultiplier = 1f;
            yellowSplashRadiusMultiplier = 1f;
            ultChargeMultiplier = 1f;
            passiveCoinAmount = 1f;
            passiveCoinBank = 0f;
            killCoinMultiplier = 1f;
            stageStartCoinBonus = 0;

            if (slingshot != null)
            {
                slingshot.maxBounces = baseSlingshotBounces;
                slingshot.explosionRadius = baseSlingshotRadius;
                slingshot.explosionDamage = baseSlingshotDamage;
            }

            RefreshCardCosts();
        }

        private void ShowBoot()
        {
            screenState = V2ScreenState.Boot;
            bootEndsAt = Time.unscaledTime + 1.1f;
            if (bootBodyText != null)
            {
                bootBodyText.text = "Single-player tactical ricochet prototype";
            }

            ShowOnlyPanel(bootPanel);
            SetMatchUiVisible(false);
            Time.timeScale = 0f;
        }

        private void ShowHub()
        {
            screenState = V2ScreenState.Hub;
            ClearRunObjects();
            ResetRunTuning();
            stage = 1;
            hp = MaxHp;
            coins = StageStartCoins;
            spawnedThisStage = 0;
            enemiesThisStage = 0;
            selectedCardIndex = -1;
            ShowOnlyPanel(hubPanel);
            SetMatchUiVisible(false);
            Time.timeScale = 0f;
        }

        private void ShowHubFromRun()
        {
            ShowHub();
        }

        private void ShowPartyPreview()
        {
            screenState = V2ScreenState.PartyPreview;
            if (partyBodyText != null)
            {
                partyBodyText.text = "Starting party:\nRED Bruiser controls close lanes.\nBLUE Archer covers long lanes.\nYELLOW Mage splashes enemy clusters.";
            }

            ShowOnlyPanel(partyPanel);
            SetMatchUiVisible(false);
            Time.timeScale = 0f;
        }

        private void ShowMatch()
        {
            screenState = V2ScreenState.Match;
            ShowOnlyPanel(null);
            SetMatchUiVisible(true);
            Time.timeScale = 1f;
        }

        private void ShowPause()
        {
            if (screenState != V2ScreenState.Match)
            {
                return;
            }

            screenState = V2ScreenState.Paused;
            ShowOnlyPanel(pausePanel);
            SetMatchUiVisible(false);
            Time.timeScale = 0f;
        }

        private void ResumeMatch()
        {
            if (screenState != V2ScreenState.Paused)
            {
                return;
            }

            ShowMatch();
        }

        private void ShowStageClear()
        {
            screenState = V2ScreenState.StageClear;
            ClearStageObjects();
            BuildUpgradeOffer();
            if (stageClearBodyText != null)
            {
                stageClearBodyText.text = "Stage " + stage + " cleared.\nHP " + hp + "/" + MaxHp + "   Coins " + coins + "\nPick one upgrade to continue.";
            }

            ShowOnlyPanel(stageClearPanel);
            SetMatchUiVisible(false);
            Time.timeScale = 0f;
        }

        private void BuildUpgradeOffer()
        {
            offeredUpgrades[0] = SelectUpgrade(UpgradeCategory.Hero, stage - 1);
            offeredUpgrades[1] = SelectUpgrade(UpgradeCategory.Slingshot, stage - 1);
            offeredUpgrades[2] = SelectUpgrade(UpgradeCategory.Economy, stage - 1);

            for (int i = 0; i < upgradeButtonLabels.Length; i++)
            {
                if (upgradeButtonLabels[i] == null || offeredUpgrades[i] == null)
                {
                    continue;
                }

                upgradeButtonLabels[i].text = UpgradeCategoryLabel(offeredUpgrades[i].category) + "\n\n" + offeredUpgrades[i].title + "\n" + offeredUpgrades[i].description;
                Image buttonImage = upgradeButtons[i] != null ? upgradeButtons[i].targetGraphic as Image : null;
                if (buttonImage != null)
                {
                    buttonImage.color = UpgradeColor(offeredUpgrades[i].category);
                }
            }
        }

        private UpgradeDefinition SelectUpgrade(UpgradeCategory category, int offset)
        {
            int count = 0;
            for (int i = 0; i < UpgradeDeck.Length; i++)
            {
                if (UpgradeDeck[i].category == category)
                {
                    count++;
                }
            }

            for (int attempt = 0; attempt < count; attempt++)
            {
                int categoryIndex = (offset + attempt) % count;
                UpgradeDefinition candidate = UpgradeAtCategoryIndex(category, categoryIndex);
                if (candidate != null && !chosenUpgradeIds.Contains(candidate.id))
                {
                    return candidate;
                }
            }

            return UpgradeAtCategoryIndex(category, offset % Mathf.Max(1, count));
        }

        private UpgradeDefinition UpgradeAtCategoryIndex(UpgradeCategory category, int categoryIndex)
        {
            int seen = 0;
            for (int i = 0; i < UpgradeDeck.Length; i++)
            {
                if (UpgradeDeck[i].category != category)
                {
                    continue;
                }

                if (seen == categoryIndex)
                {
                    return UpgradeDeck[i];
                }

                seen++;
            }

            return null;
        }

        private string UpgradeCategoryLabel(UpgradeCategory category)
        {
            if (category == UpgradeCategory.Hero)
            {
                return "HERO";
            }

            return category == UpgradeCategory.Slingshot ? "SLING" : "ECONOMY";
        }

        private Color UpgradeColor(UpgradeCategory category)
        {
            if (category == UpgradeCategory.Hero)
            {
                return new Color(0.76f, 0.9f, 1f, 0.96f);
            }

            return category == UpgradeCategory.Slingshot ? new Color(1f, 0.78f, 0.22f, 0.96f) : new Color(0.68f, 1f, 0.58f, 0.96f);
        }

        private void ChooseUpgrade(int index)
        {
            if (screenState != V2ScreenState.StageClear || index < 0 || index >= offeredUpgrades.Length)
            {
                return;
            }

            UpgradeDefinition upgrade = offeredUpgrades[index];
            if (upgrade == null)
            {
                return;
            }

            ApplyUpgrade(upgrade);
            chosenUpgradeIds.Add(upgrade.id);
            ShowToast("Picked: " + upgrade.title, new Color(0.72f, 1f, 0.52f), 1.0f);
            Debug.Log("wave_end_pick offered=" + OfferedUpgradeLog() + " chosen=" + upgrade.id + " stage=" + stage);
            StartStage(stage + 1);
        }

        private string OfferedUpgradeLog()
        {
            string result = string.Empty;
            for (int i = 0; i < offeredUpgrades.Length; i++)
            {
                if (offeredUpgrades[i] == null)
                {
                    continue;
                }

                if (result.Length > 0)
                {
                    result += ",";
                }

                result += offeredUpgrades[i].id;
            }

            return result;
        }

        private void ApplyUpgrade(UpgradeDefinition upgrade)
        {
            switch (upgrade.effect)
            {
                case UpgradeEffect.RedDamage:
                    redDamageMultiplier *= 1.15f;
                    break;
                case UpgradeEffect.BlueAttackSpeed:
                    blueAttackSpeedMultiplier *= 1.2f;
                    break;
                case UpgradeEffect.YellowSplashRadius:
                    yellowSplashRadiusMultiplier *= 1.2f;
                    break;
                case UpgradeEffect.UltChargeSpeed:
                    ultChargeMultiplier *= 1.2f;
                    break;
                case UpgradeEffect.SlingBounce:
                    if (slingshot != null)
                    {
                        slingshot.maxBounces += 1;
                    }

                    break;
                case UpgradeEffect.SlingRadius:
                    if (slingshot != null)
                    {
                        slingshot.explosionRadius *= 1.2f;
                    }

                    break;
                case UpgradeEffect.SlingDamage:
                    if (slingshot != null)
                    {
                        slingshot.explosionDamage *= 1.25f;
                    }

                    break;
                case UpgradeEffect.SlingCost:
                    slingshotCost = Mathf.Max(1, slingshotCost - 1);
                    break;
                case UpgradeEffect.PassiveIncome:
                    passiveCoinAmount *= 1.5f;
                    break;
                case UpgradeEffect.HeroCardCost:
                    heroCost = Mathf.Max(1, heroCost - 1);
                    RefreshCardCosts();
                    break;
                case UpgradeEffect.KillCoins:
                    killCoinMultiplier *= 1.5f;
                    break;
                case UpgradeEffect.StartingCoins:
                    stageStartCoinBonus += 3;
                    break;
            }
        }

        private void ShowStageFail()
        {
            screenState = V2ScreenState.StageFail;
            if (stageFailBodyText != null)
            {
                stageFailBodyText.text = "Enemies reached the red exit.\nRetry Stage " + stage + " or return to the hub.";
            }

            ShowOnlyPanel(stageFailPanel);
            SetMatchUiVisible(false);
            Time.timeScale = 0f;
        }

        private void ShowRunEnd()
        {
            screenState = V2ScreenState.RunEnd;
            ClearStageObjects();
            if (runEndBodyText != null)
            {
                runEndBodyText.text = "All " + MaxStage + " stages cleared.\nEnemies defeated: " + enemiesDefeatedThisRun + "\nMerges: " + mergesThisRun + "   Ults: " + ultsThisRun + "\nSlingshots fired: " + slingshotsThisRun;
            }

            ShowOnlyPanel(runEndPanel);
            SetMatchUiVisible(false);
            Time.timeScale = 0f;
        }

        private void ShowOnlyPanel(GameObject visiblePanel)
        {
            SetPanelActive(bootPanel, visiblePanel);
            SetPanelActive(hubPanel, visiblePanel);
            SetPanelActive(partyPanel, visiblePanel);
            SetPanelActive(pausePanel, visiblePanel);
            SetPanelActive(stageClearPanel, visiblePanel);
            SetPanelActive(stageFailPanel, visiblePanel);
            SetPanelActive(runEndPanel, visiblePanel);
        }

        private void SetPanelActive(GameObject panel, GameObject visiblePanel)
        {
            if (panel != null)
            {
                panel.SetActive(panel == visiblePanel);
            }
        }

        private void SetMatchUiVisible(bool visible)
        {
            if (matchHudRoot != null)
            {
                matchHudRoot.SetActive(visible);
            }

            if (!visible)
            {
                ClearFloatingFeedback();
            }
        }

        private void UpdateHud()
        {
            if (hpText == null)
            {
                return;
            }

            int activeEnemies = V2EnemyUnit.ActiveEnemies.Count;
            int resolvedEnemies = Mathf.Clamp(spawnedThisStage - activeEnemies, 0, enemiesThisStage);
            float hpPercent = Mathf.Clamp01((float)hp / MaxHp);
            float stageProgress = enemiesThisStage > 0 ? Mathf.Clamp01((float)resolvedEnemies / enemiesThisStage) : 0f;

            if (hpFillImage != null)
            {
                hpFillImage.fillAmount = hpPercent;
                Color hpColor = hpPercent > 0.55f ? new Color(0.22f, 0.9f, 0.36f) : hpPercent > 0.25f ? new Color(1f, 0.76f, 0.18f) : new Color(1f, 0.22f, 0.18f);
                if (Time.unscaledTime < hpPulseUntil)
                {
                    hpColor = Color.Lerp(hpColor, Color.white, 0.45f);
                }

                hpFillImage.color = hpColor;
            }

            if (progressFillImage != null)
            {
                progressFillImage.fillAmount = stageProgress;
            }

            hpText.text = "HP " + hp + "/" + MaxHp;
            coinText.text = "COINS " + coins;
            coinText.transform.localScale = Vector3.one * (Time.unscaledTime < coinPulseUntil ? 1.12f : 1f);
            stageText.text = "STAGE " + stage + "/" + MaxStage;
            progressText.text = "Cleared " + resolvedEnemies + "/" + enemiesThisStage + "   Alive " + activeEnemies;

            bool canSling = coins >= slingshotCost;
            slingshotText.text = "SLINGSHOT " + slingshotCost + "c\n" + (canSling ? "Drag from yellow zone" : "Need coins");
            slingshotText.color = canSling ? new Color(1f, 0.86f, 0.22f) : new Color(1f, 0.42f, 0.34f);
            slingshotText.transform.localScale = Vector3.one * (Time.unscaledTime < slingshotPulseUntil ? 1.08f : 1f);

            if (selectedCardIndex >= 0)
            {
                HeroCard selected = cards[selectedCardIndex];
                hintText.text = "Selected: " + CardLetter(selected.color) + " " + selected.tier + " — tap an empty slot.";
            }
            else if (!HasEmptyHeroSlot())
            {
                hintText.text = "Board full — merge matching adjacent heroes.";
            }
            else
            {
                hintText.text = "Tap a card, tap a slot. Tap glowing heroes for ults.";
            }

            hudText.text = "Spawned " + spawnedThisStage + "/" + enemiesThisStage;

            if (screenFlashImage != null)
            {
                float flash = Mathf.Clamp01((screenFlashUntil - Time.unscaledTime) / 0.28f);
                screenFlashImage.color = new Color(1f, 0.08f, 0.04f, flash * 0.24f);
            }

            if (screenState == V2ScreenState.Match && centerText != null && centerText.text.Length > 0 && Time.unscaledTime >= toastEndsAt)
            {
                centerText.text = string.Empty;
            }
        }

        private void ShowToast(string message, Color color, float duration)
        {
            if (centerText == null)
            {
                return;
            }

            centerText.text = message;
            centerText.color = color;
            toastEndsAt = Time.unscaledTime + duration;
        }

        private void PulseCoins()
        {
            coinPulseUntil = Time.unscaledTime + 0.35f;
        }

        private void AddFloatingText(string message, Color color, Vector2 anchoredPosition)
        {
            if (matchHudRoot == null)
            {
                return;
            }

            if (floatingFeedback.Count > 10)
            {
                Destroy(floatingFeedback[0].text.gameObject);
                floatingFeedback.RemoveAt(0);
            }

            Text text = CreateText(matchHudRoot.transform, "Floating Feedback", TextAnchor.MiddleLeft, anchoredPosition, new Vector2(0f, 1f), new Vector2(0f, 1f), 30);
            text.text = message;
            text.color = color;
            text.GetComponent<RectTransform>().sizeDelta = new Vector2(340f, 58f);

            FloatingFeedback feedback = new FloatingFeedback();
            feedback.text = text;
            feedback.rect = text.GetComponent<RectTransform>();
            feedback.start = anchoredPosition;
            feedback.velocity = new Vector2(0f, 72f);
            feedback.color = color;
            feedback.startedAt = Time.unscaledTime;
            feedback.duration = 1.05f;
            floatingFeedback.Add(feedback);
        }

        private void UpdateFloatingFeedback()
        {
            for (int i = floatingFeedback.Count - 1; i >= 0; i--)
            {
                FloatingFeedback feedback = floatingFeedback[i];
                if (feedback.text == null)
                {
                    floatingFeedback.RemoveAt(i);
                    continue;
                }

                float progress = Mathf.Clamp01((Time.unscaledTime - feedback.startedAt) / feedback.duration);
                feedback.rect.anchoredPosition = feedback.start + feedback.velocity * progress;
                Color color = feedback.color;
                color.a = 1f - progress;
                feedback.text.color = color;

                if (progress >= 1f)
                {
                    Destroy(feedback.text.gameObject);
                    floatingFeedback.RemoveAt(i);
                }
            }
        }

        private void ClearFloatingFeedback()
        {
            for (int i = floatingFeedback.Count - 1; i >= 0; i--)
            {
                if (floatingFeedback[i].text != null)
                {
                    Destroy(floatingFeedback[i].text.gameObject);
                }
            }

            floatingFeedback.Clear();
        }

        private void UpdateCameraFeedback()
        {
            if (mainCamera == null)
            {
                return;
            }

            if (!hasCameraBasePosition)
            {
                cameraBasePosition = mainCamera.transform.position;
                hasCameraBasePosition = true;
            }

            if (Time.unscaledTime < shakeUntil)
            {
                float remaining = Mathf.Clamp01((shakeUntil - Time.unscaledTime) / 0.25f);
                Vector2 offset = Random.insideUnitCircle * shakeMagnitude * remaining;
                mainCamera.transform.position = cameraBasePosition + new Vector3(offset.x, offset.y, 0f);
                return;
            }

            mainCamera.transform.position = cameraBasePosition;
        }

        private bool HasEmptyHeroSlot()
        {
            if (layout == null || layout.Slots == null)
            {
                return false;
            }

            for (int i = 0; i < layout.Slots.Count; i++)
            {
                if (layout.Slots[i] != null && layout.Slots[i].IsEmpty)
                {
                    return true;
                }
            }

            return false;
        }

        private bool PausePressed()
        {
            return Keyboard.current != null && (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame);
        }

        private void ClearRunObjects()
        {
            ClearStageObjects();
            V2HeroUnit[] heroes = FindObjectsByType<V2HeroUnit>(FindObjectsSortMode.None);
            for (int i = 0; i < heroes.Length; i++)
            {
                if (heroes[i] == null)
                {
                    continue;
                }

                if (heroes[i].Slot != null)
                {
                    heroes[i].Slot.ClearOccupant(heroes[i]);
                }

                heroes[i].gameObject.SetActive(false);
                Destroy(heroes[i].gameObject);
            }
        }

        private void ClearStageObjects()
        {
            List<V2EnemyUnit> enemies = new List<V2EnemyUnit>(V2EnemyUnit.ActiveEnemies);
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null)
                {
                    continue;
                }

                enemies[i].gameObject.SetActive(false);
                Destroy(enemies[i].gameObject);
            }

            SlingshotProjectile[] projectiles = FindObjectsByType<SlingshotProjectile>(FindObjectsSortMode.None);
            for (int i = 0; i < projectiles.Length; i++)
            {
                if (projectiles[i] != null)
                {
                    projectiles[i].gameObject.SetActive(false);
                    Destroy(projectiles[i].gameObject);
                }
            }

            V2ExplosionPulse[] pulses = FindObjectsByType<V2ExplosionPulse>(FindObjectsSortMode.None);
            for (int i = 0; i < pulses.Length; i++)
            {
                if (pulses[i] != null)
                {
                    pulses[i].gameObject.SetActive(false);
                    Destroy(pulses[i].gameObject);
                }
            }

            if (slingshot != null)
            {
                slingshot.CancelDrag();
            }

            mergeSource = null;
            draggingMerge = false;
            draggingSlingshot = false;
        }

        private Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            if (mainCamera == null)
            {
                return Vector2.zero;
            }

            Vector3 screen = new Vector3(screenPosition.x, screenPosition.y, -mainCamera.transform.position.z);
            return mainCamera.ScreenToWorldPoint(screen);
        }
    }
}
