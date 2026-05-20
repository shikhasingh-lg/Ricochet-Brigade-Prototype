using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RicochetBrigade
{
    public sealed class GameController : MonoBehaviour
    {
        private struct SpawnEntry
        {
            public float time;
            public BrigadeColor color;
            public bool boss;

            public SpawnEntry(float time, BrigadeColor color, bool boss)
            {
                this.time = time;
                this.color = color;
                this.boss = boss;
            }
        }

        public static GameController Instance { get; private set; }

        [Header("Scene Links")]
        public ArenaGrid grid;
        public SlingshotLauncher launcher;

        [Header("Run State")]
        public int baseHitPoints = 100;
        public int currentStage = 1;

        private readonly List<SpawnEntry> spawnSchedule = new List<SpawnEntry>();
        private Text topHud;
        private Text bottomHud;
        private Text centerHud;
        private int spawnIndex;
        private float stageStartedAt;
        private bool waitingForNextStage;
        private bool runEnded;
        private float nextStageAt;
        private int totalFlicks;
        private int totalMerges;
        private int totalKills;
        private int totalLeaks;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (grid == null)
            {
                grid = ArenaGrid.Instance != null ? ArenaGrid.Instance : FindFirstObjectByType<ArenaGrid>();
            }

            if (launcher == null)
            {
                launcher = FindFirstObjectByType<SlingshotLauncher>();
            }

            CreateHud();
            StartStage(1);
        }

        private void Update()
        {
            if (runEnded)
            {
                UpdateHud();
                return;
            }

            if (waitingForNextStage)
            {
                if (Time.time >= nextStageAt)
                {
                    waitingForNextStage = false;
                    StartStage(currentStage + 1);
                }

                UpdateHud();
                return;
            }

            SpawnDueEnemies();
            CheckStageClear();
            UpdateHud();
        }

        public void NotifyEnemyKilled(EnemyMover enemy, HeroPuck killer, bool midFlight)
        {
            totalKills++;
            Debug.Log("enemy_death color=" + enemy.EnemyColor + " killed_by=" + (midFlight ? "mid_flight" : "defender"));
        }

        public void NotifyEnemyReachedBase(EnemyMover enemy, int damage)
        {
            baseHitPoints = Mathf.Max(0, baseHitPoints - damage);
            totalLeaks++;
            Debug.Log("enemy_reached_base color=" + enemy.EnemyColor + " damage=" + damage + " base_hp=" + baseHitPoints);

            if (baseHitPoints <= 0)
            {
                runEnded = true;
                centerHud.text = "STAGE FAIL\nBase destroyed\nKills: " + totalKills + "\nLeaks: " + totalLeaks;
                Debug.Log("stage_fail stage=" + currentStage + " reason=base_hp");
            }
        }

        public void RecordFlickRelease(BrigadeColor color, HeroTier tier, Vector2 velocity)
        {
            totalFlicks++;
            Debug.Log("flick_release color=" + color + " tier=" + tier + " speed=" + velocity.magnitude.ToString("0.0"));
        }

        public void RecordRicochet(string surface, float velocityAfter)
        {
            Debug.Log("flick_ricochet surface=" + surface + " velocity_after=" + velocityAfter.ToString("0.0"));
        }

        public void RecordMidFlightMerge(BrigadeColor color, HeroTier oldTier, HeroTier newTier)
        {
            totalMerges++;
            Debug.Log("mid_flight_merge color=" + color + " tier_before=" + oldTier + " tier_after=" + newTier);
        }

        public void RecordMidFlightDamage(EnemyMover enemy, HeroPuck hero, float damage)
        {
            Debug.Log("mid_flight_damage target_color=" + enemy.EnemyColor + " hero_color=" + hero.HeroColor + " damage=" + damage.ToString("0.0"));
        }

        public void RecordHeroSettle(HeroPuck hero, Vector2Int cell, float distanceToPath, bool returnedToBase, int mergesThisFlight)
        {
            Debug.Log("hero_settle color=" + hero.HeroColor + " tier=" + hero.Tier + " cell=" + cell + " dist_to_path=" + distanceToPath.ToString("0.0") + " merges=" + mergesThisFlight + " returned_to_base=" + returnedToBase);
        }

        public void RecordDefenderAttack(HeroPuck hero, EnemyMover target, float damage)
        {
            if (Random.value < 0.1f)
            {
                Debug.Log("td_attack hero_color=" + hero.HeroColor + " target_color=" + target.EnemyColor + " damage=" + damage.ToString("0.0"));
            }
        }

        public string LauncherStatus()
        {
            return launcher == null ? "Launcher missing" : launcher.StatusText();
        }

        private void StartStage(int stageNumber)
        {
            currentStage = stageNumber;
            stageStartedAt = Time.time;
            spawnIndex = 0;
            spawnSchedule.Clear();
            BuildStageSchedule(stageNumber);
            baseHitPoints = Mathf.Min(100, baseHitPoints + (stageNumber == 1 ? 0 : 25));
            centerHud.text = "Stage " + currentStage + "\nFlick heroes from the base.\nAim through same-color allies to merge.";
            Debug.Log("stage_start stage=" + currentStage + " spawns=" + spawnSchedule.Count);
        }

        private void BuildStageSchedule(int stageNumber)
        {
            if (stageNumber == 1)
            {
                AddRepeatingSpawns(10, 5f, BrigadeColor.Red, BrigadeColor.Red, BrigadeColor.Red);
                return;
            }

            if (stageNumber == 2)
            {
                AddRepeatingSpawns(15, 4f, BrigadeColor.Red, BrigadeColor.Blue, BrigadeColor.Red);
                return;
            }

            if (stageNumber == 3)
            {
                AddRepeatingSpawns(22, 3.5f, BrigadeColor.Red, BrigadeColor.Blue, BrigadeColor.Yellow);
                return;
            }

            if (stageNumber == 4)
            {
                AddRepeatingSpawns(26, 3f, BrigadeColor.Red, BrigadeColor.Blue, BrigadeColor.Yellow);
                return;
            }

            for (int index = 0; index < 30; index++)
            {
                BrigadeColor color = index % 3 == 0 ? BrigadeColor.Red : index % 3 == 1 ? BrigadeColor.Blue : BrigadeColor.Yellow;
                spawnSchedule.Add(new SpawnEntry(index * 3f, color, false));
            }

            spawnSchedule.Add(new SpawnEntry(60f, BrigadeColor.Purple, true));
            spawnSchedule.Sort((first, second) => first.time.CompareTo(second.time));
        }

        private void AddRepeatingSpawns(int count, float interval, BrigadeColor firstColor, BrigadeColor secondColor, BrigadeColor thirdColor)
        {
            for (int index = 0; index < count; index++)
            {
                BrigadeColor color = index % 3 == 0 ? firstColor : index % 3 == 1 ? secondColor : thirdColor;
                spawnSchedule.Add(new SpawnEntry(index * interval, color, false));
            }
        }

        private void SpawnDueEnemies()
        {
            if (grid == null)
            {
                return;
            }

            float elapsed = Time.time - stageStartedAt;
            while (spawnIndex < spawnSchedule.Count && spawnSchedule[spawnIndex].time <= elapsed)
            {
                int unitCount = EnemyMover.ActiveEnemies.Count + HeroPuck.SettledCount;
                if (unitCount >= BrigadeTuning.UnitCap)
                {
                    return;
                }

                SpawnEntry entry = spawnSchedule[spawnIndex];
                SpawnEnemy(entry.color, entry.boss);
                spawnIndex++;
            }
        }

        private void SpawnEnemy(BrigadeColor color, bool boss)
        {
            GameObject enemyObject = new GameObject(boss ? "Boss Enemy" : color + " Enemy");
            EnemyMover enemy = enemyObject.AddComponent<EnemyMover>();
            enemy.Setup(grid, color, boss);
            Debug.Log("enemy_spawn color=" + color + " stage=" + currentStage + " boss=" + boss);
        }

        private void CheckStageClear()
        {
            if (spawnIndex < spawnSchedule.Count || EnemyMover.ActiveEnemies.Count > 0)
            {
                return;
            }

            Debug.Log("stage_clear stage=" + currentStage + " hp=" + baseHitPoints + " flicks=" + totalFlicks + " merges=" + totalMerges);

            if (currentStage >= 5)
            {
                runEnded = true;
                centerHud.text = "RUN COMPLETE\nKills: " + totalKills + "\nFlicks: " + totalFlicks + "\nMerges: " + totalMerges;
                return;
            }

            waitingForNextStage = true;
            nextStageAt = Time.time + 3f;
            centerHud.text = "STAGE " + currentStage + " CLEAR\nNext stage in 3s\nHeroes carry forward.";
        }

        private void CreateHud()
        {
            Canvas canvas = new GameObject("Greybox HUD").AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            canvas.gameObject.AddComponent<GraphicRaycaster>();

            topHud = CreateText(canvas.transform, "Top HUD", TextAnchor.UpperLeft, new Vector2(24f, -24f), new Vector2(0f, 1f), new Vector2(0f, 1f), 42);
            bottomHud = CreateText(canvas.transform, "Bottom HUD", TextAnchor.LowerLeft, new Vector2(24f, 24f), new Vector2(0f, 0f), new Vector2(0f, 0f), 36);
            centerHud = CreateText(canvas.transform, "Center HUD", TextAnchor.MiddleCenter, Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), 44);
            centerHud.color = new Color(1f, 1f, 1f, 0.92f);
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

            RectTransform rect = text.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(960f, 240f);
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

        private void UpdateHud()
        {
            if (topHud == null || bottomHud == null)
            {
                return;
            }

            topHud.text = "Stage " + currentStage + "/5   Enemies " + spawnIndex + "/" + spawnSchedule.Count + "   Alive " + EnemyMover.ActiveEnemies.Count;
            bottomHud.text = "Base HP " + baseHitPoints + "   Heroes " + HeroPuck.SettledCount + "   " + LauncherStatus();

            if (!waitingForNextStage && !runEnded && Time.time - stageStartedAt > 3f)
            {
                centerHud.text = "";
            }
        }
    }
}
