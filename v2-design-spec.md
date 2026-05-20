---
name: Ricochet Brigade — v2 Design Spec
status: locked for v2 greybox
created: 2026-05-20
supersedes: v1-design-spec.md
v1_summary: Slingshot was the primary spawn input. Heroes were flicked onto a path-relative platform; mid-flight merge was the locked variant. Killed 2026-05-20 — flick-as-input was a structural risk (skill-gated for casuals, slingshot screen vs lane felt disconnected) and the differentiation was thin against Coop TD / Lucky Defense.
v2_thesis: Stay in the proven merge-defense TD lane (Coop TD + Lucky Defense template). Layer three slight twists that each independently add a decision surface. Slingshot is reframed from "primary input" to "coin-fueled secondary spell" — earns its place tactically without gating the core loop.
locked_twists:
  - T1 — Pick-1-of-3 hero spawn (vs both leaders' pure RNG summon)
  - T2 — Tap-charged hero ults (vs both leaders' zero mid-wave agency)
  - Slingshot as coin-spell — AoE explosion, ricochets off platforms, Angry Birds aim
---

# Ricochet Brigade — v2 Design Spec

> **Scope:** Prototype-ready spec for v2 greybox. Solo-only (coop deferred to post-launch). Covers core mechanics, economy, content, and test hypotheses. UI flow in `v2-ui-flow.md`.

> **What's different from v1:** slingshot is no longer how heroes spawn. Heroes spawn via a pick-1-of-3 card UI; player taps a hero card, then taps a platform slot to place. Slingshot is now a tactical secondary attack: spend coins, drag Angry Birds-style, release, projectile arcs through the arena, ricochets off elevated platforms, lands as an AoE explosion on enemies on the path.

---

## Section 1 — Pitch

### One-line hook
A **classic-path tower defense** with a tactical slingshot. Pick-1-of-3 heroes spawn on elevated platforms and auto-fire down at enemies snaking around them on a sunken path. Spend coins to fire AoE slingshot blasts that ricochet off your platforms to reach enemies tucked behind cover. Three mid-wave decisions every few seconds — both genre leaders have zero.

### Fantasy
You command a brigade of color-coded heroes defending a base from waves of marching enemies. Heroes stand on raised stone platforms; enemies winds through the sunken corridors between. From the bottom yellow rampart, you draw your slingshot back, aim a curving shot over the high platforms — bounce, bounce, BOOM — clearing a knot of enemies your archers couldn't reach. Meanwhile your wizards charge their ults; tap to unleash. Every few seconds another hero card slides up — pick the one your line is starving for. Stage clear: take a buff, deepen the slingshot, or rev the coin tap. The brigade grows; the path gets meaner.

### Target player
Lucky Defense / Coop TD player. Hybrid-casual, mixed gender, 5–15 min sessions, comfortable with 3 simultaneous decision layers (spawn pick / ult tap / slingshot fire). Comp anchor: **Coop TD's brain + Angry Birds' hands.**

### Why this concept (v2 thesis)
Coop TD ($X/mo) and Lucky Defense ($Y/mo) are both proven, but both ship with **zero mid-wave agency** — after placement, you watch. The "slight twist" lane between them is open on (1) spawn agency, (2) mid-wave skill expression, (3) secondary attack systems. v2 adds all three as additive, low-risk increments on the proven lane. Innovation budget: ~35% total (T1 ~15%, T2 ~10%, slingshot-as-spell ~10%). Each twist is small enough to fail independently without sinking the build.

### What we are testing in v2
| Question | Pass signal | Kill signal |
|---|---|---|
| Do the **three decision surfaces** read as one game, not three? | Testers describe loop in one sentence without separating "the slingshot" from "the heroes" | Testers describe them as separate mini-games or ignore one entirely |
| Is the slingshot **load-bearing** (you need it to clear waves) or **decorative** (waves auto-clear with heroes)? | Testers reference slingshot as needed; success without using slingshot is impossible by stage 3 | Testers ignore slingshot or use it once and forget |
| Do **ricochet shots** feel like puzzle solving (good) or random pinball (bad)? | Testers describe shots in terms of bounces — "off the left platform, into the gap" — and iterate | Testers describe shots as "I just yeet it and hope" |
| Does **pick-1-of-3** create meaningful build choices? | Testers articulate trade-offs ("I need more red because the boss is coming") | Testers always pick the leftmost / first option |
| Can a Coop TD / Lucky Defense player learn it in **90 seconds** without tutorial? | First wave completed without confusion >5s | Multiple confused pauses; cannot articulate loop at 90s |
| Does the 30s loop produce a **highlight moment per minute**? | Tester yelps / smiles ≥1× per minute observed | Flat affect across the run |

### Production frame
- Team: 10 (studio constraint)
- v2 build target: 5 stages, 3 hero colors (Red/Blue/Yellow), 1 enemy type per stage, no meta, no art — greybox only
- v2 timeline: 3 weeks from spec lock to internal playtest
- Engine: **Unity** (continuing from v1 prototype in this repo; 2D physics for slingshot arc + ricochet)
- v2 platform: Android internal build
- Solo only — no coop in v2

### Comp anchor
| Game | What we steal | What we change |
|---|---|---|
| **Coop TD** | Puzzle path layout, 2-tile merge, shared HP bar, color = class | Add pick-3 spawn, add hero ults, add slingshot spell — three mid-wave decisions vs zero |
| **Lucky Defense** | Hero-class color, run-based progression, gacha-style summon flavor | Tap-to-place on constrained platforms (not free grid); spawn choice (not pure RNG); merge is 2-tile not 3-tile random |
| **Angry Birds** | Drag-aim physics, projectile path preview, ricochet/bounce satisfaction | Repurposed as a coin-cost spell inside a TD loop, not a standalone game |

---

## Section 2 — Three-loop diagram

> Habby-tier hybrid casuals run on three nested loops. v2 proves 30s + session loops; meta loop is scoped (not built) for v2.

### Loop 1 — 30-second loop ("decide → act → see impact" beat)

```
                                  ┌──────────────────────────────────┐
                                  │                                  │
                                  ▼                                  │
                          ┌──────────────────┐                       │
                          │  COINS tick up   │                       │
                          │ (kills + passive)│                       │
                          └────────┬─────────┘                       │
                                   │                                 │
                                   ▼                                 │
                ┌──────────────────────────────────────┐             │
                │  PLAYER CHOOSES ONE OF THREE:        │             │
                │                                      │             │
                │  A) Spend coins on a hero card       │             │
                │     (pick-1-of-3 → tap platform slot)│             │
                │                                      │             │
                │  B) Spend coins on slingshot shot    │             │
                │     (drag-aim → release → ricochet   │             │
                │      → AoE explosion on impact)      │             │
                │                                      │             │
                │  C) Tap a glowing hero (free ult)    │             │
                │     (free if ult meter is full)      │             │
                └────────┬─────────────────────────────┘             │
                         │                                           │
            ┌────────────┼────────────┐                              │
            ▼            ▼            ▼                              │
       ┌────────┐  ┌────────┐  ┌────────┐                            │
       │ HERO   │  │ SLING  │  │ ULT    │                            │
       │ placed │  │ explode│  │ fires  │                            │
       │ on slot│  │ AoE    │  │ class  │                            │
       │ → auto-│  │ damage │  │ skill  │                            │
       │ fires  │  │ → coins│  │ → coins│                            │
       └───┬────┘  └───┬────┘  └───┬────┘                            │
           │           │           │                                 │
           └───────────┼───────────┘                                 │
                       │                                             │
                       ▼                                             │
              Enemies still march                                    │
              along fixed path                                       │
              green → red                                            │
                       │                                             │
                       └─────────────────────────────────────────────┘
```

### Loop 2 — Session loop (5–15 min run)

```
   START RUN
        │
        ▼
   ┌──────────────────┐
   │ PARTY (v2: R+B+Y)│  (no choice in v2; meta loop later picks 3 of N)
   └────────┬─────────┘
            │
            ▼
   ┌──────────────────┐
   │ STAGE 1 (60–90s) │ ◄────┐
   │ waves 1–N        │      │
   └────────┬─────────┘      │
   cleared? │  failed?       │
            ▼                ▼
   ┌──────────────────┐  ┌──────────┐
   │ WAVE-END PICK    │  │ RUN OVER │
   │ 1-of-3:          │  └────┬─────┘
   │   • hero buff    │       │
   │   • sling damage │       │
   │   • coin tick++  │       │
   └────────┬─────────┘       │
            │                 │
            └────► next stage─┘
            (5 stages total in v2 greybox)
```

### Loop 3 — Meta loop (spec only, not built in v2)

Reserved for post-greybox: gacha hero pulls, persistent hero collection (R/B/Y + future G/Y/P/etc.), hero leveling, slingshot cosmetics. Standard hybrid-casual playbook. Not load-bearing for greybox test.

---

## Section 3 — Core mechanics spec

### 3.1 Arena layout

**Single screen, portrait orientation.** Five elevated solid platforms; enemies walk on the white space between them (sunken corridors). Slingshot fires from the bottom yellow rampart.

```
┌─────────────────────────────────┐
│ 🔴 (red exit — HP damage)       │
│  ╔═══════════════════════╗      │
│  ║ TOP BLOCK (3 slots)   ║      │
│  ╚═══════════════════════╝      │
│                                 │
│      ╔═══════════════════╗      │
│      ║ T-HORIZONTAL (5)  ║      │
│      ╚═════╦═════════════╝      │
│            ║                    │
│            ║ T-VERTICAL  🟢     │
│            ║ (3 slots)   spawn  │
│            ║                    │
│  ╔═══════╗ ╔═══════╗            │
│  ║ BLOCK ║ ║ BLOCK ║            │
│  ║ A (4) ║ ║ B (4) ║            │
│  ╚═══════╝ ╚═══════╝            │
│                                 │
│ ╔═════════════════════════════╗ │
│ ║ 🟡 SLINGSHOT + CARD UI      ║ │
│ ╚═════════════════════════════╝ │
└─────────────────────────────────┘
```

**Total hero slots: 19** — Block A: 4, Block B: 4, T-horizontal: 5, T-vertical: 3, Top block: 3.

**Enemy path:** Spawn at green dot (right side, mid-screen) → snake around platforms following the white space → exit at red dot (top-left). Path is a single fixed snake, not branching. Continuous march; no round breaks within a stage.

**Yellow zone (player area):** Slingshot anchored at center-bottom. Card pick UI hovers above the rampart — three hero cards visible at all times; tap to select; then tap a platform slot to place.

**Platforms block line-of-sight for the slingshot.** This is the load-bearing constraint: shots fired straight up will hit the underside of platforms and not reach enemies in the corridors behind them. To hit enemies behind platforms, the player must aim a ricochet — bounce off platform side, curve around, land on enemies.

### 3.2 Hero spawn — Pick-1-of-3 (T1)

- Card area shows 3 hero cards at all times. Each card = a hero (color + class + tier).
- Cards refill from a deterministic pool weighted toward the player's chosen 3 colors. (v2: forced R/B/Y; meta loop later picks 3 of N.)
- **Cost:** flat per-card cost in coins (tunable; target: ~1 spawn per 3–5s of play).
- **Action:** tap card → card highlights → tap a platform slot → hero appears, card refills.
- **Tap-place only** (no drag). Snap to nearest valid slot if tap is between slots.
- If all 19 slots are filled, spawn is blocked until merge frees a slot.

### 3.3 Hero merge

- **2-tile merge.** Tap a hero, drag onto an adjacent same-color same-tier hero → both consumed, replaced by tier+1.
- Same-platform-only? **No** — merge works across adjacent slots on the same platform. (v2 keeps it tight; cross-platform merge is a future variant if testers ask for it.)
- 3 tiers: Bronze → Silver → Gold. Each tier ~2× damage of the previous.

### 3.4 Hero classes (color = class)

| Color | Class | Auto-attack | Ult (T2) | Notes |
|---|---|---|---|---|
| 🔴 Red | Bruiser | Short range, high single-target | **Slam:** stuns all enemies in a circle around the hero for 2s | Best on platforms near corridor turns |
| 🔵 Blue | Archer | Long range, line attack | **Volley:** rapid-fires 6 arrows at the longest line of enemies | Best on top block / T-horizontal |
| 🟡 Yellow | Mage | Medium range, slow attack, splash | **Storm:** AoE explosion at densest enemy cluster | Best on platforms with line-of-sight to corridor bends |

### 3.5 Hero ults (T2)

- Each hero has an **ult meter** (0–100) that fills passively from auto-attacks.
- Fill rate: ~10 seconds of active firing → 100%. Tunable.
- When meter is full, hero glows + small icon appears.
- **Tap a glowing hero → ult fires.** Meter resets to 0.
- Ult is **free** (no coin cost — earned through play, not bought).
- Higher-tier heroes do not charge faster, but their ult scales with tier (e.g., Gold Archer's Volley = 12 arrows instead of 6).

### 3.6 Slingshot — secondary spell

- Slingshot is anchored at the center-bottom of the yellow zone, fixed position.
- **Cost:** flat coin cost per shot, **cheaper than a hero card spawn** (target: ~40–60% of hero cost).
- **Aim model:** Angry Birds-style. Touch and drag from the slingshot → trajectory arc preview appears as dashed line including bounces off platforms → release to fire.
- **Projectile physics:** projectile arcs under gravity; **ricochets off the blue platforms** with reduced velocity per bounce (max 3 bounces before fizzling); explodes on impact with any enemy OR after final bounce expires.
- **Damage:** AoE explosion on impact. Radius ~2 enemy widths. Damage = constant base + scales with slingshot upgrades (see §4).
- **What it does NOT do:** does not damage your heroes or platforms; does not bounce off the screen edges (treat edges as walls that absorb).
- **Why ricochet matters:** elevated platforms block direct shots to enemies in corridors behind them. To clear a wave, the player must aim ricochet shots.

### 3.7 Coin economy

- **Coin sources:**
  - **Kills:** primary source. Each enemy killed by ANY source (hero attack, hero ult, slingshot) drops coins. Drop value scales with enemy tier.
  - **Passive tick:** small background income (e.g., +1 coin per 2s) so play never fully starves between kills.
- **Coin spends:**
  - Hero card spawn (most expensive)
  - Slingshot shot (cheaper — see §3.6)
- **No coin cap.** Coins persist across waves within a single stage but reset on stage clear/fail.

### 3.8 Failure

- **Single HP bar** at the top of the HUD (shared HP — solo means one bar but design language matches Coop TD).
- Every enemy reaching the red exit reduces HP by an amount that scales with the enemy's tier.
- HP reaches 0 → stage fails → run ends (no revives in v2 greybox).

### 3.9 Wave-end upgrade pick

Between stages (not within a stage), player picks **1 of 3** upgrades. Pool draws from:

| Upgrade type | Examples |
|---|---|
| **Hero buff** | +10% damage to Red; +20% attack speed to Blue; Mage splash radius +1 |
| **Slingshot buff** | +1 max ricochet bounce; +20% explosion radius; +25% damage |
| **Economy** | +50% passive coin tick; reduce hero card cost by 1 coin; reduce slingshot cost by 1 coin |

Three categories ensure every pick is a real trade-off. Cards are drawn pseudo-randomly with rarity weights.

---

## Section 4 — Economy & progression

### 4.1 Coin pacing target (v2 greybox baseline — tune in playtest)

| Metric | Target |
|---|---|
| Time to first hero spawn | 3–5 seconds after stage start |
| Time between hero spawns (mid-wave) | 3–5 seconds |
| Slingshot shots per minute | 6–10 (so it feels frequent but not free) |
| Coin balance at end of wave | Near-zero (force player to spend, not hoard) |

Starting coins: ~1 hero spawn's worth. Stage start passive tick: 1 coin / 2s. Bumped by wave-end upgrades.

### 4.2 Hero costs (greybox — tune)

| Action | Coin cost |
|---|---|
| Spawn Bronze hero (from card) | 10 |
| Spawn Silver hero (from card) | 25 (rare cards) |
| Spawn Gold hero (from card) | 50 (very rare cards) |
| Merge (no cost) | 0 |
| Slingshot shot | 5 |

### 4.3 Stage difficulty curve (v2 greybox — 5 stages)

| Stage | Enemy count | Enemy tiers | Path speed | Boss? |
|---|---|---|---|---|
| 1 | 20 | T1 only | slow | no |
| 2 | 35 | T1+T2 | slow | no |
| 3 | 50 | T1+T2 | medium | no |
| 4 | 70 | T2+T3 | medium | mini-boss at wave end |
| 5 | 100 | T2+T3+T4 | fast | full boss |

### 4.4 What's NOT in v2 (out of scope)

- Gacha / hero collection
- Persistent meta currency
- Hero leveling outside of in-run merge
- Multiple parties / hero selection screen (v2: forced R/B/Y)
- Art, polished VFX, audio polish
- Coop
- Settings menu, profile, social

---

## Section 5 — Content scope for v2

| Asset | Count | Notes |
|---|---|---|
| Stages | 5 | Same arena layout; varying enemy density/speed |
| Hero colors | 3 (R/B/Y) | One class each |
| Hero tiers | 3 (Bronze/Silver/Gold) | 9 total hero variants |
| Enemy types | 4 tiers (T1–T4) | One archetype per stage's threat level |
| Boss | 1 | Stage 5 only |
| Ults | 3 | One per color |
| Wave-end upgrades | ~12 | 4 per category × 3 categories |

Greybox-only. No art beyond colored shapes + simple sprites.

---

## Section 6 — Test hypothesis + instrumentation

### 6.1 Pass / kill questions (repeated from §1)

| # | Question | Pass | Kill |
|---|---|---|---|
| Q1 | Three layers feel like one game | One-sentence loop description | Separates layers in description |
| Q2 | Slingshot is load-bearing | Needed to clear stage 3+ | Ignored entirely or used once |
| Q3 | Ricochet feels strategic | Iterates aim across bounces | "Yeet and hope" |
| Q4 | Pick-3 creates real choices | Articulates trade-offs | Always picks first option |
| Q5 | Learnable in 90s | First wave clean | Confused pauses |
| Q6 | Highlight moments | ≥1 yelp/smile per minute | Flat affect |

### 6.2 Instrumentation (events to log in greybox build)

- `stage_start`, `stage_clear`, `stage_fail` (with stage #, run time)
- `hero_spawn` (color, tier, slot position, time-since-stage-start)
- `hero_merge` (color, tier-from, tier-to)
- `ult_fired` (hero color, hero tier, enemies hit)
- `slingshot_fired` (drag angle, drag power, # bounces, enemies hit, explosion radius)
- `slingshot_miss` (no enemies hit on final impact)
- `enemy_reached_exit` (enemy tier, HP remaining after)
- `wave_end_pick` (upgrade chosen out of 3 offered)

Funnels to watch:
- Slingshot use rate per stage (target: ≥6/min by stage 3)
- Slingshot hit rate (target: ≥50% — if much lower, ricochet is too hard)
- Merge rate (target: ≥1 merge per 30s)
- Ult tap latency (time from glow to tap — target: <3s, indicates engaged play)

### 6.3 Tester pool

- 6 testers (3 Lucky Defense players, 3 Coop TD players, all Android)
- 30-min session each: 5 min onboarding-by-observation (no script), 20 min play, 5 min open Q&A
- Disjoint from Pop Brigade tester pool (per `game_concept_variants.md` cross-contamination rule)

---

## Section 7 — Open questions & risks

### 7.1 Risks (ranked)

| Rank | Risk | Mitigation |
|---|---|---|
| 1 | **Slingshot becomes vestigial** — if hero ults + auto-attacks clear waves, slingshot is decorative | Tune stage 3+ to force slingshot use (enemies behind platforms only the slingshot can reach) |
| 2 | **Ricochet aim too hard** — testers can't predict bounces, frustration spikes | Mandatory aim-preview line shows full bounce trajectory; tune ricochet velocity for predictability |
| 3 | **Cognitive overload** — three decision surfaces is too much per second | Cards refill slowly; ult charge is forgiving; if testers freeze, dial spawn frequency down |
| 4 | **19 slots is too many** — board feels cluttered, players struggle to track which slot does what | Tune platform highlighting (target slot glow), consider reducing slot count if testers can't read board |
| 5 | **Pick-3 always picks first** — choice is theatre | Tune card pool so colors are genuinely scarce; force trade-offs |
| 6 | **Wave-end upgrade is overwhelming** — third decision layer between stages | Auto-skip option after 5s; offer 1-of-2 if testers struggle |

### 7.2 Open questions (to answer in playtest)

1. Is ricochet bounce count of 3 the right ceiling, or should it be 2 (predictability) or 5 (skill ceiling)?
2. Should slingshot ricochet off heroes too (buff them on pass-through), or only off platforms?
3. Should the slingshot have a charge time, or be instant-fire on release?
4. Should slot density be reduced to 12–15 if 19 feels chaotic?
5. Does the player ever feel the urge to upgrade the slingshot before the heroes? (Tests whether slingshot pacing is right.)
6. Does the wave-end upgrade pick feel like a meaningful run-shaping decision, or noise?
7. Is the "three colors = three classes" enough variety for a 30-min session, or do we need 5 colors for greybox?

### 7.3 What v2 explicitly does NOT prove

- Long-term retention (D7+) — meta loop not built
- Monetization fit — no gacha, no IAP scaffolding
- Coop dynamics — solo-only
- Whether the lane (merge-defense TD) is the right one — that was decided pre-v1
- Art / theme polish — greybox

### 7.4 If v2 fails

Decision tree if greybox playtest fails the must-pass questions:
- **Q1 fails (separates as multiple games):** kill slingshot-as-spell entirely; collapse to pure pick-3 + ults TD (still differentiated from leaders, just less). Rename concept.
- **Q2 fails (slingshot vestigial):** tune stage gating + retest before killing. If still fails after one re-tune, kill slingshot.
- **Q3 fails (ricochet random):** simplify physics — single bounce only, larger AoE, slower projectile. Retest.
- **Q4 fails (pick-3 theatre):** add active deny (pick-or-skip) or shuffle button at coin cost; retest before killing.
- **Q5/Q6 fail with everything else passing:** ship it — onboarding is fixable.
