---
name: Ricochet Brigade — v1 Design Spec
status: draft
created: 2026-05-11
updated: 2026-05-19
concept_doc: ~/game-research/ricochet-brigade/concept.md
variant_plan: ~/plans/2026-05-11-variants-slingshot-td.md
locked_variant: V2 — Mid-flight merge (highest-prior winner; swap if paper test selects differently)
arena_pivot: 2026-05-15 — replaced 1D vertical lane with 2D rectangular arena; slingshot now a base with HP; enemies spawn from top/left/right edges
path_pivot: 2026-05-15 — superseded arena_pivot same day. Enemies now follow a fixed serpentine path (Bloons TD / Kingdom Rush style) entering from one edge and ending at a CENTER base. March is continuous, not round-based. Slingshot fires in full 360° from the center.
---

# Ricochet Brigade — v1 Design Spec

> **Scope of this doc:** Prototype-ready spec for the v1 greybox. Covers core mechanics, economy, content, and test hypotheses tight enough that engineering + design can build without re-asking the designer. UI flow lives in a separate doc (`v1-ui-flow.md`).

> **Locked variant:** **V2 — Mid-flight merge.** Flicked heroes ricocheting through stationary same-color allies tier them up (Bronze → Silver → Gold). This collapses the "two games glued together" risk (slingshot screen vs TD lane) by making the ricochet path the strategic decision — the flick is *for* the lane, not separate from it.

---

## Section 1 — Pitch

### One-line hook
A **classic-path tower defense with a slingshot**. Your base sits at the center of a 2D arena; a fixed serpentine **road** winds toward it from one edge; enemies march along it continuously. The rest of the arena is a raised **platform** where your heroes live. Flick colored heroes outward in any direction — they ricochet off the 4 walls, pinball through your settled allies (tiering them up mid-flight), and land on a platform cell as auto-fire defenders shooting *down* at the enemies on the road. **Heroes placed on platform cells adjacent to the road do the work; heroes you fling into a corner are wasted.** One surface, one input, dual purpose.

### Fantasy
You command a brigade of color-coded heroes defending a base at the center of an elevated platform battlefield. Below the platform, enemies pour in along a single sunken road, marching steadily toward you. Every flick is a bet — aim a clean shot down a platform corridor to land a defender beside the road's most vulnerable bend, or curve a banked shot off the side walls and through your existing defenders to upgrade them mid-flight before the new hero lands as fresh reinforcement. The slingshot is your DPS, your levelling system, *and* your construction tool — all in one pull-back-and-release.

### Target player
Lucky Defense / Survivor.io / Capybara Go / King of Avalon player. Hybrid casual leaning mid-core, mixed gender, 5–15 min sessions, comfortable with ~4 cognitive layers, plays multiple TD/auto-battler hybrids. Comp anchor: **Lucky Defense's brain × slingshot's hands.**

### Why this concept (v1 thesis in one paragraph)
The global slingshot-TD lane is the emptiest top-grossing lane on mobile. Monster Strike proved $27M/mo demand exists in JP. Lucky Defense proved the "color = hero, lane TD, gacha meta" loop ports globally on a different input. The slingshot input has never been combined with global TD meta. The existential risk: physics input feels skill-gated for casual players. The 10% novel hook (mid-flight merge) makes the physics decision *strategic*, not *skill-gated* — testers don't need to be good at aiming; they need to be thoughtful about path planning. v1 tests whether that reframe lands.

### What we are testing in v1
| Question | Pass signal | Kill signal |
|---|---|---|
| Does slingshot + arena + path read as **one** play surface? | Testers describe loop without saying "the slingshot game" vs "the defending game" | Testers describe two halves; treat arena as background |
| Is the aim input **strategic** (ricochet + path-relative placement) or **skill-gated** (twitch precision)? | Testers iterate "I want to chain through these allies" / "I want to land near the path here" — describe choices not skill | Testers describe missed shots as "I'm bad at aiming" — performance frustration dominates |
| Can a Lucky Defense / Bloons TD player learn it in **90 seconds**? | First match completed without tutorial pause >5s | Multiple confused pauses; cannot articulate the loop at 90s |
| Does the 30s loop produce a **highlight moment per minute**? | Tester yelps / smiles ≥1× per minute observed | Flat affect across the run |
| Does **the path** read as the threat axis? | Testers reference path-relative position ("I need one near that turn", "the middle section is undefended"); path-relative placement metric ≥0.7 by stage 3 | Testers ignore path, settle heroes anywhere, describe screen as "busy" |

### Production frame
- Team: 10 (studio constraint)
- v1 build target: 5 stages, 3 hero colors (Red/Blue/Yellow), 1 enemy type per color, no meta, no art — greybox only
- v1 timeline: 3 weeks from spec lock to internal playtest
- Engine: Unity (Box2D 2D physics required)
- v1 platform: Android internal build only

### Comp anchor (one row)
| Game | What we steal | What we change |
|---|---|---|
| Lucky Defense | Color = class, continuous arena TD, run + gacha meta | Input: slingshot flick instead of merge drag; 2D arena instead of merge grid |
| Monster Strike | **2D arena ricochet grammar**, flick physics, ricochet-through-allies, color synergy, 4-wall bounce | Continuous TD (heroes settle and auto-fire) instead of turn-based party RPG; mid-flight *merge* (tier-up) instead of mid-flight stacking damage |
| Survivor.io | 5–10 min runs, auto-fire heroes, boon picks | Player-driven hero placement via slingshot into 2D arena, not auto-walk |

---

## Section 2 — Three-loop diagram

> Every Habby/IGG-tier hybrid casual game runs on three nested loops. v1 must prove the 30s loop and the session loop work. Meta loop is **scoped out of v1** — we'll spec it but not build it.

### Loop 1 — 30-second loop (the "flick → ricochet → settle" beat)

```
        ┌─────────────────────────────────────────────────────┐
        │                                                     │
        ▼                                                     │
  [PULL slingshot] ──► [RELEASE] ──► hero flies w/ velocity ──► hits anything?
                                                              │
                                          ┌───────────────────┼─────────────────────┐
                                  same-color ally    enemy/wall         friction stop
                                          │                  │                   │
                                          ▼                  ▼                   ▼
                              [MERGE — tier up,    [DAMAGE — dmg = tier ×    [SETTLE —
                               consume ally,        velocity, ricochet         hero stops, becomes
                               keep flying]         continues]                  defender in arena cell]
                                          │                  │                   │
                                          └─────────┬────────┘                   │
                                                    │                            │
                                                    ▼                            ▼
                                            still has velocity?            auto-fire from
                                                    │                       this cell every
                                          ┌─────────┴─────────┐             N seconds per class
                                         YES                  NO            │
                                          │                    │            │
                                          ▼                    ▼            │
                                  back to hits anything?    SETTLE          │
                                                                            │
                                                                            ▼
                                                                    enemies march
                                                                    continuously along
                                                                    the fixed path at
                                                                    speed S (cells/sec),
                                                                    independent of flicks
                                                                            │
                                                                            ▼
                                                                    back to PULL
```

**Target cadence:** 1 flick every ~3–5 sec (deliberate, not twitchy). **1 merge every ~10–15 sec** (every 2–3 flicks). Enemies march continuously at ~1 cell every ~2 sec (configurable per stage); a typical path is ~30 cells long, so a single enemy spawning at the entrance reaches the base in ~60 sec if not killed.

**Highlight moment per minute target:** at least one of {Bronze → Silver merge, Silver → Gold merge, 3+ ricochet chain, wall-bank shot that hits an unreachable enemy, special flick used}.

### Loop 2 — Session loop (the "run")

```
[Run start] ──► pick starting party (3 heroes drawn from current collection)
       │
       ▼
[Stage 1: 90 sec] ──► clear arena → reward: pick 1 of 3 boons
       │
       ▼
[Stage 2: 90 sec] ──► clear arena → boon
       │
       ▼
   …5 stages total in v1 (15 at launch)…
       │
       ▼
[Boss stage: 2 min] ──► clear → run rewards (currency, hero shard pulls)
       │
       ▼
[Run end] ──► back to meta hub
```

**v1 stage budget** (path is identical across all 5 stages — 28 cells, hand-authored once. Difficulty escalates via spawn schedule + specials only):

| Stage | Duration | Spawn schedule | Slingshot ammo | New element |
|---|---|---|---|---|
| 1 | 60 sec | 10 enemies total, 1 every 5s | Unlimited | Red color only |
| 2 | 75 sec | 15 enemies total, 1 every 4s | Unlimited | + Blue color introduced |
| 3 | 90 sec | 22 enemies total, 1 every 3.5s | Unlimited | + Yellow color |
| 4 | 90 sec | 26 enemies, 1 every 3s | + 1 special flick available | First special flick: Scatter (3 mini-heroes) |
| 5 (boss) | 2 min | Regulars every 3s + 1 boss at t=60s | Unlimited + 2 specials | Boss = purple (no counter) |

**Total v1 run length:** ~7–9 min. Tight enough for 2–3 runs per session to test session-length signal.

**Boon pool for v1 (pick 1 of 3 after each stage):**
- Color bias (next 3 flicks guaranteed to be your chosen color)
- Hero buff (+25% damage to one color's heroes for the run)
- Slingshot mod (one of: faster pull-back, +20% projectile speed, trajectory shows 2 ricochets)

### Loop 3 — Meta loop (spec only — NOT built in v1)

```
[Run end] → currency + hero shards → unlock/upgrade heroes
                                          │
                                          ▼
                                  Stronger heroes → push deeper into stages
                                          │
                                          ▼
                              Battle pass / event progress
                                          │
                                          ▼
                                  Back to next run
```

**Meta surfaces (specced for v2, NOT in v1 prototype):**
- Hero gacha (~25 heroes at launch, ~5 in v1 spec): pull with premium currency
- Slingshot upgrades: power, accuracy, special-flick capacity
- Party slot expansion: 3 → 4 → 5 heroes per run
- Battle pass: seasonal, $5–10
- Daily/weekly events: color-themed challenges
- Clan raids: post-launch only

**Why excluded from v1:** Meta loop is a known-solved problem (Lucky Defense playbook). The v1 prototype proves the 30s + session loop. If those work, meta is execution. If they don't, no amount of meta saves it.

### Loop interaction map
| Loop | Drives | Time horizon | What v1 must prove |
|---|---|---|---|
| 30s | Flick dopamine, second-to-second decisions | Seconds | Slingshot + arena = one surface; ricochet planning is strategic, not skill-gated |
| Session | Run completion, "one more run" pull | 5–10 min | Run arc has rising stakes + meaningful boon choices |
| Meta | D7+ retention, monetization | Days–weeks | _Out of scope for v1_ — assume Lucky Defense playbook ports |

---

## Section 3 — Core mechanics spec

> **Rule for this section:** Numbers, not adjectives. Every tuning value listed here is a **v1 default** — designer-adjustable in the build, but locked for the first playtest so signal is comparable across testers. Final tuning table is at §3.10.

### 3.1 Spatial layout

Portrait mobile, 9:19.5 reference (iPhone 15 / Pixel 8). The entire screen is the **arena**; HUD elements float at the top/bottom edges over the arena background:

| Zone | Vertical % | Purpose |
|---|---|---|
| **Arena (play surface)** | Full screen (HUD overlaid) | Rectangular 2D battlefield, 8 columns × 12 rows grid. All 4 edges are bounce walls. The fixed path is drawn on this surface (visible to player). The center 2×2 base sits where the path terminates. Heroes settle anywhere in the grid. Flick projectiles travel and ricochet here. |
| **Top HUD strip** | Top 8%, overlaid | Stage indicator, wave/spawn-remaining counter, pause |
| **Bottom HUD strip** | Bottom 10%, overlaid | Base HP bar, current/on-deck hero slot |

- **Arena grid:** 8 columns × 12 rows. Discrete cells for hero settling and the path waypoint grid; flick physics are continuous.
- **Walls:** All 4 edges = elastic-bounce walls. There is no "back wall" any more — the slingshot is in the center, so heroes can fly forward, backward, and sideways equally.
- **Base:** A **2×2 cell footprint at the center of the arena** (cells (3,5), (4,5), (3,6), (4,6) for 0-indexed 8×12 grid — slightly above geometric center to leave HUD room at bottom). Slingshot is mounted on the base. The base has HP (=100 in v1) and is the literal lose-condition: enemies reach the footprint → base takes damage → HP=0 ends the run. The base does **not** auto-fire. Heroes **cannot** settle on the base footprint (snap to closest empty cell outward).
- **Two-level terrain:**
  - **Path (sunken road)** — A hand-authored serpentine sequence of grid cells from one edge of the arena (the **path entrance**, marked with a portal/gate sprite) to the **base footprint** (the **path exit**). The path is rendered as a **lower-elevation road** (slight visual sink + distinct floor texture). Enemies walk on the road. **Heroes cannot settle on path cells.**
  - **Platform (raised terrain)** — Every non-path, non-base cell of the 8×12 grid. Platform cells are rendered as **raised tiles** (higher floor + brighter color). Heroes settle exclusively on platform cells. The slingshot/base sits at the platform level — heroes are flicked from the platform, fly over the road and surrounding platform cells, and land on platform cells.
- The visual hierarchy is the **single most important readability cue**: enemies are on the lower road, heroes are on the higher platforms, the slingshot fires from the platform level. This is the canonical Lucky Defense / Bloons TD / Kingdom Rush layout — testers from those games recognize it on sight.
- **v1 uses ONE path for the entire run.** All 5 stages share the same hand-authored serpentine layout. Difficulty escalates through spawn schedule (more enemies, faster cadence, new colors) and specials availability, NOT through path complexity. The same path drawn at the start of Stage 1 is still there at the end of the boss stage.
- v1 path length target: 28 cells. The path enters from a top/left/right edge cell — never from the bottom edge (which is closest to the HUD).
- **Heroes carry across stages.** Since the path doesn't change, heroes settled in Stage 1 remain useful in Stage 5 (if they survive). This makes tier progression *meaningful over a run*, not just within a stage.
- **Enemy spawning:** All enemies enter at the **path entrance** cell. There is exactly **one entrance per stage** in v1. (Multi-entry paths are v2.)

### 3.2 Slingshot mechanics

**Pull-and-release input:**
- Touch-and-hold **anywhere on screen** (forgiving zone; the base sprite itself is too small a touch target for a center-anchored mechanic) → projectile preview appears showing trajectory line originating at the base.
- Drag in **any direction** to set angle + power. Angle = vector from base center to touch point (full 360° aim — heroes can be flicked toward any edge of the arena). Power = distance from touch start, capped at ~3× base height for max power.
- **Pull-back convention:** the hero in the slingshot visually pulls *toward* the touch point (so the shot fires in the opposite direction, away from where you dragged) OR fires *toward* the touch point — pick one convention and lock it. **v1 lock: fires AWAY from drag direction** (matches Angry Birds mental model, which testers will arrive with).
- **Aim assist (LOCKED ON in v1):** Trajectory dotted line shows path through up to 2 ricochets off any of the 4 walls. Updates live as player drags.
- Release to fire. No fire rate cap (one flick at a time, next available after current hero settles).

**Projectile physics:**
- Hero flies at velocity proportional to pull power. Max velocity = 1800 px/s. Min usable velocity = 600 px/s (below this, "weak shot" UI feedback — but flick still resolves).
- Friction: 0.92 / frame (decays over ~3s of flight to zero).
- Bounces: perfect elastic off **all 4 walls**, angle reflection, no energy loss. With the slingshot in the center, every flick has the full 360° wall geometry available — banked shots can travel a full corner-to-corner loop before settling.
- **Base re-entry:** If a flying hero re-enters its own base footprint at non-trivial velocity, the flight ends — hero settles in the closest empty cell adjacent to the base. This prevents heroes from infinitely orbiting and gives a natural "weak shot fallback."
- Hero radius: 32 px (1 arena cell ≈ 64 px wide).

**Hero queue:**
- Slingshot has 1 hero loaded + 1 visible in on-deck slot.
- Tap on-deck slot to swap. Free, unlimited.
- Hero color drawn from **party composition**: 3 heroes preselected at run start, queue cycles through them with weighted random.

### 3.3 Mid-flight merge (V2 core mechanic)

When a flying hero passes through a stationary same-color ally:
- The **stationary ally is consumed** (removed from arena).
- The **flying hero's tier increases** (Bronze → Silver → Gold; Gold caps).
- The flying hero **continues flying** with current velocity (no slowdown from the merge).
- The cell occupied by the consumed ally becomes empty.

**Cross-color pass-through:** the flying hero passes through without merging (visual glow tells the player "no merge"). This is intentional — preserves class identity, prevents accidental wrong-color upgrades.

**Tier visualization:**
- Bronze: small unit, single-color silhouette
- Silver: medium unit, with 2-tone outline
- Gold: large unit, with glowing crown overlay

**Why this is the core mechanic:** It's what bonds the slingshot input to the arena state. The arena isn't a "passive defender field" — it's a **target field for ricochet paths**. The player aims through allies, not around them. In the 2D arena this gets richer than the prior 1D lane: a single banked shot off a side wall can chain through 2–3 same-color allies before settling on the opposite flank — far higher merge-potential ceiling per flick.

### 3.4 Hero settling

After a flicked hero loses velocity (friction brings it to <100 px/s) OR hits an enemy and stops OR re-enters the base:
- Hero settles in the **nearest empty PLATFORM cell** to its stop point (Euclidean distance, ties broken by *closer to the path* — keeps useful defenders compactly arranged beside the path).
- **Heroes CANNOT settle on path cells.** If the flicked hero comes to rest on a path cell, it **snaps to the nearest empty platform cell** (adjacent or near-adjacent). Visually: a small "step-up" animation onto the platform tile so it's obvious where the hero ended up.
- **Heroes cannot settle on the base footprint** either. Heroes that would land there snap to the closest free platform cell outward.
- If the nearest platform cell is occupied by a same-color hero of equal or higher tier: snap to next nearest empty.
- If all platform cells are full (excluding the 2×2 base and the ~28 path cells, leaving ~64 platform-eligible cells): settled hero replaces the **most-damaged hero in any platform cell** (tie-breaker: oldest, then farthest from path).

Settled heroes become **stationary defenders** that auto-fire (§3.5). Their position matters tactically: a Yellow (full-arena range) anywhere on the platform is useful, but a Red (3-cell range, DPS) is only useful within 3 cells of the path. **Heroes flicked into corners far from the path are wasted** — this is the explicit design intent. Path-relative platform placement is the strategic core; the player learns it by Stage 2.

### 3.5 Hero behavior in arena (settled)

- **Stationary:** Heroes do NOT walk. Once settled, they hold their cell until killed.
- **Targeting:** Auto-fire at the **nearest enemy within range** (Euclidean distance to enemy's current position along the path). Range is class-dependent and measured in arena-cell units. Heroes can fire in any of the 360° — no facing constraint.
  - If multiple enemies tied for nearest, target the one **furthest along the path** (focus-fire heuristic — kill the leader before it reaches base).
  - This "furthest-along-path" heuristic replaces the prior "lowest-HP" heuristic. It is the canonical Bloons TD targeting mode and is more legible for testers.
- **Fire rate:** Per class (see table below). Projectile is a small bullet; travels at 1200 px/s along a straight line.
- **Death:** When HP hits 0, hero disappears with a brief particle. Cell becomes available for next settling hero.

**Color → class (locked v1, only 3 of 5 colors enabled). Ranges expressed in arena-cell radius (Chebyshev):**
| Color | Class | Range | Attack pattern | v1? |
|---|---|---|---|---|
| Red | DPS | 3-cell radius | High damage, fast fire (1 hit / 1.0s) | ✅ |
| Blue | Slow | 5-cell radius | Low damage, applies 30% slow for 2s | ✅ |
| Yellow | Range | Full arena | Medium damage, slow fire (1 hit / 2.0s) | ✅ |
| Green | Heal | Adjacent (1-cell radius) | Heals nearby hero +5 HP/s | ❌ (v2) |
| Purple | AOE | 2-cell radius splash | AOE damage on enemy clusters | ❌ (v2) |

**Mid-flight damage:** Flying heroes that touch enemies deal `tier × velocity_factor` damage:
- Velocity factor: `current_velocity / 1000`, capped at 2.0
- Damage formula: `tier_dmg_base × velocity_factor`
  - Bronze base = 30
  - Silver base = 60
  - Gold base = 120

So a Gold flying at max velocity deals 240 damage on impact (1-shots most v1 enemies). Slow trickling Bronze deals 30 minimum.

### 3.6 Enemy behavior

**Spawn:** All enemies enter at the **path entrance** cell — the single edge cell where the stage's hand-authored path starts. Spawns are time-scheduled (not round-gated): each stage has a spawn schedule listing enemies and the time they appear. Example: `t=0.0s Red, t=2.0s Red, t=4.0s Blue, ...`.
- A 1-second spawn "warming" indicator (small glow on the entrance cell) telegraphs each incoming enemy.

**March:** Enemies move **continuously** along the path at speed `S` cells/second. Different enemy colors can have different speeds (v1: all enemies move at 0.5 cells/sec = 2 sec per cell; boss at 0.3 cells/sec). This is real-time TD pace — there is no round-based gating.
- Speed is per-cell-traversal; enemies move at constant speed within a cell and instantly orient at the next path waypoint when entering a new cell. No diagonal movement; the path is 4-directional.
- Enemies and heroes occupy **different terrain layers** — enemies are always on the path, heroes are always on the platform (§3.1). They never overlap on the same cell, so there is no blocking or trampling logic to handle.

**Path completion (lose mechanic):** When an enemy reaches the final path cell (adjacent to the base footprint), it touches the base. Base takes damage equal to the enemy's reach-damage. The enemy is then **consumed** (vanishes — does not loop, does not back up).

**Auto-fire pause:** Settled heroes auto-fire continuously while enemies are within range. No round-gating anywhere in v1 — everything is real-time.

**Enemy stats (v1, mirrors hero color = enemy color):**
| Color | HP | Damage to base on reach | March speed (cells/s) |
|---|---|---|---|
| Red | 60 | 10 HP | 0.5 |
| Blue | 100 | 10 HP | 0.5 |
| Yellow | 150 | 15 HP | 0.5 |

**On-screen unit cap (readability gate):** Total settled-heroes + alive-enemies on the arena is capped at **24** in v1. If a spawn would push the count over, that spawn is **delayed** until the count drops below 24. Logged.

**Color counter logic (for tester legibility in v1):** Heroes of color X deal **2× damage to enemies of color X**. Same as Pop Brigade — keeps class strategy readable in playtest.

### 3.7 Special flicks (v1: 1 type)

**Scatter Flick (the only special in v1):**
- Spawn rate: available once per stage from Stage 4 onward (boon pick can grant more).
- Visual: rainbow-glowing hero in queue.
- Effect on flick: at midpoint of flight, the hero splits into 3 smaller heroes (one of each v1 color), each carrying 1/3 of current velocity. All 3 then settle / damage / merge independently.
- Strategic value: gives player a "panic button" — one flick scatters 3 cheap defenders across the arena, with at least one likely landing near the path. Particularly useful for plugging gaps in a long path or covering a segment the player has under-defended.

v2 specials: Heavy Flick (1.5× tier, 0.7× speed), Curve Flick (bends mid-flight), Anchor Flick (settles at exact aim point) — **not in v1**.

### 3.8 Win / lose conditions

**Per stage:**
| Condition | Outcome |
|---|---|
| Stage's spawn schedule finished AND no enemies remaining on path | **Stage clear** → boon pick → next stage |
| **Base HP** reaches 0 | **Stage fail** → run ends |
| **No path-leaks for 60s AND no enemy on path** (stalemate edge case — should not happen with finite spawn schedule but guarded) | **Stage clear (timeout)** → boon pick |

**Stage clear rewards:**
- Run currency: +50 coins (in-run only, resets on run end)
- Boon pick: 1 of 3 (see Loop 2 boon pool in §2)
- Arena keeps its current heroes — they carry to next stage at current HP and position. The path stays the same across all 5 stages, so hero placement compounds across the run. Base HP **regens +25 HP** between stages (capped at 100).

**Boss stage (Stage 5):**
- Single high-HP enemy + a faster-than-normal regular-enemy spawn schedule (a regular enemy every 3s).
- Boss enters at the path entrance after 60s — must be killed by hero attacks before it reaches the base.
- Boss HP: 1500. Boss damage: 50 HP to base if it reaches the base.
- Boss march speed: 0.3 cells/sec (slower than regulars — gives players time to mass DPS on it).
- Boss is **purple** (no hero color counter in v1) → designed to test "what happens when player has no direct counter" — pure throughput test.

**Run end:**
- Run rewards (post-boss): meta currency (not used in v1 build but logged for instrumentation) + tester debrief.

### 3.9 v1 tuning value summary (single-page reference for engineer + designer)

| Parameter | Value | Notes |
|---|---|---|
| **Arena** | | |
| Grid width | 8 columns | |
| Grid height | 12 rows | |
| Cell size | ~64 px | |
| Wall bounce | Elastic | All 4 walls |
| Base footprint | 2×2 cells, center | Cells (3,5), (4,5), (3,6), (4,6) for 0-indexed grid |
| Base HP (= stage HP) | 100 | Regens +25 between stages |
| On-screen unit cap | 24 | Heroes + enemies; excess spawns deferred |
| **Path & Platform** | | |
| Path length | 28 cells | Hand-authored once; shared by all 5 stages |
| Entry edge | Top / left / right | Never bottom |
| Path visible | Yes — sunken-road texture | Tester legibility critical |
| Platform cells | ~64 settle-eligible | All non-path, non-base cells |
| Heroes can settle on path? | **No** | Snaps to nearest platform cell |
| Two-level visual | Path sunken, platform raised | Canonical Lucky Defense / Bloons TD layout |
| **Slingshot / flick** | | |
| Anchor | Base center (cells 3.5, 5.5) | Full 360° aim |
| Convention | Drag away → fires opposite | Angry Birds mental model |
| Touch zone | Full screen | Generous on small phones |
| Max velocity | 1800 px/s | Full pull |
| Min usable velocity | 600 px/s | Below = weak-shot feedback |
| Friction | 0.92 / frame | Decays in ~3s |
| Hero radius | 32 px | |
| Aim assist | ON, 2-ricochet preview | Locked v1 |
| Queue depth | 1 + 1 on-deck | |
| Swap cost | Free | |
| **Heroes (mid-flight damage)** | | |
| Bronze base dmg | 30 | × velocity factor |
| Silver base dmg | 60 | |
| Gold base dmg | 120 | |
| Velocity factor cap | 2.0 | |
| **Heroes (settled / defender)** | | |
| Bronze HP | 100 | |
| Silver HP | 150 | |
| Gold HP | 200 | |
| Red defender fire rate | 1 / 1.0s | DPS, 3-cell range |
| Blue defender fire rate | 1 / 1.5s | Slow, 5-cell range, 30% slow / 2s |
| Yellow defender fire rate | 1 / 2.0s | Range, full-arena |
| Targeting heuristic | Furthest-along-path | Bloons TD canonical |
| Color counter | 2× damage | Same color hero vs enemy |
| **Enemies** | | |
| Red HP | 60 | |
| Blue HP | 100 | |
| Yellow HP | 150 | |
| Reach-base damage | 10 / 10 / 15 | R / B / Y |
| March speed | 0.5 cells/sec | Continuous, real-time |
| Boss march speed | 0.3 cells/sec | |
| Boss HP | 1500 | |
| Spawn schedule | Per-stage time-scheduled list | Not round-gated |
| Spawn warming indicator | 1.0s before spawn | Glow on entrance cell |
| **Specials** | | |
| Scatter flick availability | Stage 4+ | 1 per stage default |
| **Player** | | |
| Base start HP | 100 | |
| Base regen between stages | +25 HP | Capped at 100 |
| **Stage** | | |
| Stage clear: spawns finished + arena empty | — | — |
| Stalemate-timeout fallback | 60s of no leaks AND no enemies | Should not fire with finite spawn schedule |
| Coins per clear | 50 | In-run only |

---

## Section 4 — Economy & progression

> **v1 stance: zero economy.** No currencies, no shop, no gacha, no battle pass, no daily quests, no ads, no IAP. The boon pick after each stage is the **only** reward surface in v1. Same discipline as Pop Brigade — economy is a solved problem; bolting it on dilutes the mechanic signal.

### 4.1 v1 — what exists

| Element | v1? | Notes |
|---|---|---|
| In-run coins (logged but unused) | ✅ Logged only | +50 per stage clear, +bonus for high-tier merges. Telemetry only. No spend surface. |
| Boon pick | ✅ | 1 of 3 cards after each stage clear (see §4.2) |
| Run completion reward | ✅ Logged only | "Run stars" 0–5 based on stages cleared + HP remaining. Logged. |
| Party composition pick | ✅ | At run start, player picks 3 heroes from available pool (v1: 3-of-3, so forced — but UI is built for future) |
| Cross-run currency | ❌ | No persistent currency in v1 |
| Hero unlocks | ❌ | All 3 colors available from Run 1, Stage 1 |
| Gacha | ❌ | v2 |
| Battle pass | ❌ | v2 |
| Ads | ❌ | v2 |
| IAP | ❌ | v2 |

### 4.2 Boon pool (v1)

The only choice point outside the match itself. 3 cards drawn from this pool after each stage clear. Drawn without replacement within a single run.

| Boon | Effect | Notes |
|---|---|---|
| **Red Bias** | Next 3 flicks guaranteed Red | Color-focus play |
| **Blue Bias** | Next 3 flicks guaranteed Blue | Color-focus play |
| **Yellow Bias** | Next 3 flicks guaranteed Yellow | Color-focus play |
| **Damage +25% (Red)** | All Red heroes deal +25% damage for the run | Class buff |
| **Damage +25% (Blue)** | All Blue heroes deal +25% damage for the run | Class buff |
| **Damage +25% (Yellow)** | All Yellow heroes deal +25% damage for the run | Class buff |
| **Extra Special** | +1 Scatter Flick available this stage AND next | Special flick buff |
| **Power Pull** | Max velocity +20% for the run | Slingshot mod |
| **Ricochet+** | Aim trajectory shows 3 ricochets instead of 2 | Slingshot mod |

**Total: 9 boons.** Same count as Pop Brigade for tester-parity. Drawing 4 boons across a 5-stage run = ~44% of the pool per run — variety stays high across runs.

### 4.3 Progression curve (within a run)

Path is identical across stages — same 28-cell serpentine layout, drawn once at run start, persists through Stage 5. Difficulty comes from spawn cadence and new colors/specials.

| Stage | Enemy pacing | New element exposed | Difficulty intent |
|---|---|---|---|
| 1 | 1 enemy / 5s | Red only — pure aim tutorial | "Easy win" — build confidence; learn the path layout |
| 2 | 1 enemy / 4s | + Blue introduced (1 blue mid-stage) | "First multi-color decision" |
| 3 | 1 enemy / 3.5s | + Yellow introduced | "Three-color juggle starts" |
| 4 | 1 enemy / 3s | + Scatter Flick available | "Special timing decision" |
| 5 (boss) | 1 enemy / 3s + boss | Boss = pure throughput test, no color counter | "Can the player keep pace?" |

**Tuning intent:** Stage 1 is winnable with random flicking — the path is short enough that even badly-placed Reds can dent the wave from somewhere. Stage 3+ requires both (a) ricochet planning to chain through allies for mid-flight merges, and (b) path-relative placement decisions (which segment of the longer path needs more defenders). Stage 5 boss tests sustained DPS, not strategy — wins go to players who built Silver/Gold towers along the path during the warm-up.

### 4.4 What v2 economy will look like (spec-only, not built)

For continuity with team discussions and tester expectations. **None of this is implemented in v1.**

| Stream | Rough shape | Lifted from |
|---|---|---|
| Soft currency | "Strike Coins" — earned per stage, spent on slingshot upgrades + boon-pool unlocks | Lucky Defense |
| Premium currency | "Gems" — IAP + sparse drops, spent on gacha pulls + energy refills | Monster Strike |
| Gacha | ~25 heroes (5 per color), 4 rarities, pity at 80 | Habby standard |
| Battle pass | Seasonal $5–10, ~30 levels, free + premium track | Lucky Defense / Survivor.io |
| Energy | 5 runs / day, refill via gems or 30-min timer | Standard |
| Ad rewards | Revive on stage fail, double end-of-run rewards, free daily spin, guaranteed-Gold flick | Standard |
| Daily/weekly events | Color-themed challenges (e.g. "Red Week — bonus damage on Red merges") | Habby live ops |
| Party slots | Unlock 4th and 5th party slots through meta progression | Monster Strike |

**Why not in v1:** Same reason as Pop Brigade. v1 must answer "does the mechanic work?" Economy is plumbing on top.

---

## Section 5 — Content scope for v1

> **Scope discipline rule:** Everything listed here is **in scope.** Anything not listed is **out of scope** by default.

### 5.1 Content lock list

| Category | v1 count | Items |
|---|---|---|
| **Stages** | 5 | S1 (intro), S2 (Blue add), S3 (Yellow add), S4 (Scatter add), S5 (boss) — all 5 share **one hand-authored 28-cell path** |
| **Paths** | 1 | Single serpentine layout, persistent across the run. Entrance at top-left edge (placeholder). Multi-path/per-stage variety is v2. |
| **Hero colors / classes** | 3 | Red (DPS), Blue (Slow), Yellow (Range) |
| **Hero tiers** | 3 | Bronze, Silver, Gold |
| **Enemy types** | 3 | Red walker, Blue walker, Yellow walker (one per color) |
| **Bosses** | 1 | Purple "Wrecker" — placeholder name |
| **Special flicks** | 1 | Scatter Flick |
| **Boons** | 9 | See §4.2 |
| **Party comps** | 1 forced (R+B+Y) | v2: pick 3 from collection |
| **VFX states** | ~10 | Flick trail, wall bounce, merge flash, settle thud, hero auto-fire, enemy hit, enemy death, mid-flight damage, scatter split, color frenzy (if added) |
| **Screens** | 8 | See UI flow doc §1 |
| **Music tracks** | 0 | None in v1 (system silence acceptable) |
| **SFX** | ~7 | Optional v1.1 — see UI flow doc audio list |

### 5.2 Explicitly out of v1

| Asked for | Decision | Reason |
|---|---|---|
| Green / Purple heroes | v2 | 3 colors are enough; 5 adds tuning cost |
| Multiple boss types | v2 | One boss enough to test "no-counter throughput" |
| Tutorial overlay | v2 | Verbal tester guidance — if v1 needs an overlay, design is too complex |
| Settings menu | v2 | Device defaults work |
| Sound design | v1.1 | Optional add-on if engineer has slack time |
| Player profile / stats screen | v2 | Run-end screen is enough |
| Retry on fail | ❌ Never | Clean instrumentation |
| Daily reward | v2 | No persistent state in v1 |
| Cosmetics | v2+ | Greybox = no art |
| Multiplayer / clan | v2+ | Way later |
| Auto-aim "easy mode" | v2 | Aim assist is locked ON in v1; auto-aim is a separate toggle for the casual segment |
| Curve flick / special flick variety | v2 | Scatter only |
| Hero collection / gacha screens | v2 | No persistent state |

### 5.3 Build sprint plan (3 weeks, 1 designer + 1 engineer)

| Week | Designer | Engineer | Gate |
|------|------|------|------|
| **W1** | Lock slingshot pull-back UX, aim preview, merge rule, settle rule. Greybox art on paper. Boon copy. | Slingshot physics (Box2D), aim preview rendering, ricochet rules, mid-flight merge, settling logic, hero placement. | Slingshot alone (no enemies) is playable — can flick, ricochet, merge, settle. |
| **W2** | Lock hero color/class behaviors. Author **the** path (one layout, ~28 cells, reused across all stages). Tune stage 1–3 spawn schedules on paper. | Arena grid (two-level: platform + path), center base, **path/platform renderers** (distinct floor textures + height offset), path data format (one path file), single-entrance spawn schedule, **continuous-speed enemy march along path**, settle-to-platform snap rule, hero auto-fire (furthest-along-path targeting), mid-flight damage, base HP/damage, win/lose. Stages 1–3. | Full match playable on stages 1–3, debug-only HUD. |
| **W3** | Boon UX. Run-end stats screen. Tester recruitment + debrief script. | Stages 4–5 (boss + Scatter flick), screens 1/2/3/5/6/7/8, instrumentation logging. | All 8 screens connected, full run playable end-to-end, telemetry firing. |

**Internal playtest:** Week 3 Friday — team plays 3 runs each. Tester recruitment starts Week 3 Monday.

**Parallel-build risk note:** If running Ricochet Brigade and Pop Brigade in parallel (per concept-doc Phase 1), 1 designer + 1 engineer can do paper for both in Week 1, but greybox must pick ONE for Weeks 2–3. Decision gate at end of Week 1.

---

## Section 6 — Test hypothesis + instrumentation

> **The v1 prototype exists to answer 4 questions** (from §1). Section 6.1 maps each to a measurable signal. Section 6.2 lists the events to log. Section 6.3 defines the tester recruit + debrief.

### 6.1 The 4 questions → measurable signals

| # | Question | Pass signal | Kill signal | Measurement |
|---|---|---|---|---|
| **Q1** | Does slingshot + arena read as one play surface? | Tester describes the loop in one sentence without saying "the slingshot game" or "the defending game" separately | Tester uses input/output separation language; treats arena as background | Verbal debrief Q1; transcribed and tagged for "two-game language" |
| **Q2** | Is the aim input strategic (path planning) or skill-gated (twitch precision)? | Tester articulates path choices ("I wanted to hit both blues") not skill complaints | Tester says "I'm bad at this" / "I keep missing" / abandons after 1 run citing frustration | Verbal debrief Q4/Q5/Q6; observer tally of skill-complaint phrases |
| **Q3** | Can a Lucky Defense player learn it in 90 seconds? | First match completes with no tutorial pause >5s; tester answers "what do you do?" correctly at 90s mark | Multiple confused pauses; cannot articulate the loop at 90s | Stopwatch on first run; observer notes pauses |
| **Q4** | Does the 30s loop produce a highlight moment per minute? | Observer counts ≥5 audible reactions (gasp, yelp, "yes!", smile, leaning forward) in a 5-min stage | Flat affect across an entire stage; no commentary | Observer tally during play |

### 6.2 Telemetry — events to log (engineer spec)

Log to a local file per session: `ricochet-brigade-v1-<tester-id>-<utc-ts>.jsonl`. One event per line, JSON object with `event_name`, `ts_ms`, and payload fields. Sync at end of session.

| Event | Payload | When logged |
|---|---|---|
| `session_start` | `{ tester_id, build_version, device_model, os_version }` | App boot |
| `party_pick` | `{ party_colors: [r,b,y] }` | Run start screen tap |
| `stage_start` | `{ stage_num, hp, party }` | Match begin |
| `flick_pull_start` | `{ stage_num, hero_color, hero_tier }` | Touch-and-hold begin |
| `flick_release` | `{ hero_color, hero_tier, angle_deg, power_pct, expected_ricochets, time_to_release_ms }` | Touch release |
| `flick_ricochet` | `{ ricochet_idx, surface: "wall_left"|"wall_right"|"wall_top"|"enemy"|"ally_pass", velocity_after }` | Each ricochet event |
| `mid_flight_merge` | `{ flier_color, flier_tier_before, flier_tier_after, consumed_cell }` | Each merge |
| `mid_flight_damage` | `{ target_id, target_color, damage_dealt, flier_tier, velocity_at_hit }` | Each mid-flight enemy hit |
| `hero_settle` | `{ hero_id, color, tier, settle_col, settle_row, dist_to_nearest_path_cell, snapped_from_path: bool, mid_flight_kills, mid_flight_merges_consumed }` | Each settle. `dist_to_nearest_path_cell` is the **key derived metric input** for whether the player is learning path-relative placement. `snapped_from_path` = true if the flick stopped on a path cell and was auto-snapped to the platform |
| `td_attack` | `{ hero_id, target_id, damage_dealt }` | Each settled-hero attack (sample 1 in 10 to limit volume) |
| `hero_death` | `{ hero_id, color, tier, lifetime_ms, damage_dealt_total, kills }` | Each hero death |
| `enemy_spawn` | `{ enemy_id, color, stage_num, schedule_idx, spawn_time_ms }` | Each enemy entering path (single entrance) |
| `enemy_death` | `{ enemy_id, color, killed_by: "mid_flight"|"defender"|"scatter", killer_color, path_progress_pct }` | Each enemy killed; path_progress_pct = how far along path it got (0–100) |
| `enemy_reached_base` | `{ enemy_id, color, hp_damage, base_hp_after }` | Each leak (enemy reached path exit) |
| `arena_tick` | `{ stage_num, t_ms, enemies_on_path, heroes_settled, base_hp }` | Sampled every 2s during match (replaces round_advance) |
| `boon_picked` | `{ stage_num, boon_id, alternatives: [boon_id, boon_id] }` | Stage clear pick |
| `scatter_flick_used` | `{ stage_num, hero_color, hero_tier, scatter_kills, scatter_merges }` | Each scatter |
| `stage_clear` | `{ stage_num, hp_remaining, ms_elapsed, total_flicks, total_merges, golds_built, max_ricochet_chain }` | Stage clear |
| `stage_fail` | `{ stage_num, hp_remaining: 0 | other, reason: "base_hp" | "arena_full", ms_elapsed }` | Stage fail |
| `run_end` | `{ stages_cleared, total_ms, total_flicks, total_merges, golds_built, scatter_uses, total_enemies_killed, completion: "win" | "fail" | "quit" }` | Run end |
| `pause_open` | `{ stage_num, ms_into_stage }` | Pause |
| `pause_resume` | `{ pause_duration_ms }` | Resume |
| `session_end` | `{ runs_completed, total_session_ms }` | App close / tester done |

**Key derived metrics to compute post-session:**
- **Merge rate** = merges / flicks (target: ≥0.25 by stage 3 for a learning tester — means they're aiming through allies)
- **Mid-flight kill rate** = mid_flight_kills / total enemies killed (target: 0.2–0.5 — too low means flick is just for placement, too high means TD is irrelevant)
- **Average ricochet chain** = total ricochets / total flicks (target: ≥1.5 by stage 3 — means walls are being used strategically)
- **Path-adjacent placement** = % of settled heroes within 2 platform-cells of the path (target: ≥0.7 by stage 3 — if testers aren't placing platform-adjacent to the path by stage 3, they don't understand the layout)
- **Snap rate** = % of flicks where the projectile landed on a path cell and got auto-snapped to platform (high snap rate = testers are aiming AT the path, not realizing heroes go beside it — readability fail)
- **Mean settle-to-path distance over a run** (lower = learning the layout; tracks how quickly testers tune to platform-edge aim)
- **Stage 1 completion time** (learning curve proxy)
- **Q4 dopamine density** = (golds built + 3+ ricochet chains + scatter uses) per minute of match time
- **Skill-frustration ratio** = (flicks below min-usable-velocity + "no merge" pass-throughs of wrong color) / total flicks (proxy for tester struggle)
- **Leak rate per stage** = enemies_reached_base / total spawned (lower = better defense; target ≤0.2 in stages 1–3, ≤0.4 in stage 4–5)

### 6.3 Tester recruit + debrief

**Recruit profile (target 6–8 testers for v1):**
- Plays at least one of: Lucky Defense, Survivor.io, Capybara Go, Whiteout Survival, King of Avalon
- Plays at least one physics/flick game currently or in past 6 months (Angry Birds, Monster Strike, Stumble Guys-style)
- Age 22–42, mixed gender (target ≥40% female — Ricochet Brigade is more male-skew than Pop Brigade, but tracking female signal matters for the audience-fit hypothesis)
- Plays mobile ≥30 min/day
- Recruit via: existing studio user-research panel, prolific.co for emerging markets (PH/ID), Lila employee referrals

**Session structure (45 min per tester):**
1. **5 min** — intake, demographics, current games played (verbal)
2. **2 min** — "I'm going to show you a game. Play it however you want. I'll watch, no help unless you're stuck for 30+ seconds." (No tutorial.)
3. **20 min** — observed play: 2–3 runs. Observer logs reactions per Q4 above. Recorder runs (video + screen).
4. **15 min** — structured debrief (script in 6.4)
5. **3 min** — wrap, thank-you, gift card payout

**If running Pop Brigade in parallel:** Each tester plays one concept only. Randomize assignment. Compare D1-equivalent signal (would tester return?) across the two pools.

### 6.4 Debrief script (locked questions)

Ask in this order. **Don't lead, don't explain mechanics.** Capture verbatim.

1. "Describe what you just played in one sentence, like you're telling a friend." → **Q1 signal**
2. "Walk me through what's happening on screen — top to bottom." → **Q1 signal**, also surfaces readability gaps
3. "How did it feel — energizing, stressful, boring, frustrating, something else?" → **Q2 signal**
4. (Likert) "On a 1–5 scale, how likely are you to play a few more rounds right now?" → **Q2 signal**
5. "Were there moments that felt great? When?" → **Q4 signal**
6. "Were there moments that felt bad? When? Were any about your *aim* specifically?" → **Q2 + Q4 signal** (the aim probe is the key Q2 differentiator from Pop Brigade)
7. "Tell me about the merge — when you saw your hero pass through another one. Did that change how you aim?" → **Q1 final probe + V2 variant probe**
8. "If you were stuck explaining the rules to someone in 30 seconds, what would you say?" → **Q3 signal**
9. "What's missing? What did you want to do but couldn't?" → **scope discovery for v2**
10. "Anything else?" → free response

### 6.5 Decision matrix (post-test)

| Q1 | Q2 | Q3 | Q4 | Outcome |
|---|---|---|---|---|
| Pass | Pass | Pass | Pass | **Go to vertical slice** with art + sound |
| Pass | Pass | Pass | Fail | **Iterate on dopamine density** — bigger merge VFX, more chain-able layouts |
| Pass | Pass | Fail | Pass | **Iterate on learnability** — tighter stage 1, single-color start |
| Pass | Fail | Any | Any | **Iterate on aim assist** — increase ricochet preview, lower min-usable-velocity threshold, add auto-aim toggle. If still fails on round 2 — **slingshot input is wrong for this audience; kill concept** |
| Fail | Any | Any | Any | **Mid-flight merge failed.** Pivot to V3 (party draft) or V5 (settle-as-aim), re-spec |

**Bar:** 5/8 testers pass each question for that question to count as "Pass." Tied or close calls go to design review.

**Cross-concept decision rule:** If both Pop Brigade and Ricochet Brigade pass paper, pick the one with higher Q1 + Q4 combined scores. If both fail, return to candidate list. If one passes — that's the greybox concept.

---

## Section 7 — Open questions & risks

### 7.1 Open questions (resolve during/after v1 test)

| # | Question | When to answer |
|---|---|---|
| ~~OQ1~~ | ~~Vertical vs horizontal lane?~~ **RESOLVED 2026-05-15a**: pivoted to 2D arena. | — |
| ~~OQ1'~~ | ~~Bottom-base 3-edge vs center-base 4-edge?~~ **RESOLVED 2026-05-15b**: pivoted again — center-base with a fixed path. No edge-spawn at all now; single path entrance. | — |
| ~~OQ7~~ | ~~Round-based march vs continuous?~~ **RESOLVED 2026-05-15b**: continuous, real-time, per-cell speed. | — |
| ~~OQ11~~ | ~~Spawn-edge weighting?~~ **RESOLVED 2026-05-15b**: single entrance, no weighting. | — |
| ~~OQ12~~ | ~~Spawn telegraph 1-round vs 0 vs 2?~~ **RESOLVED 2026-05-15b**: 1.0s warming glow before each spawn (time-based, not round-based). | — |
| ~~OQ13~~ | ~~Boss smash attack timing?~~ **RESOLVED 2026-05-15b**: no smash needed; path is fixed, can't be walled off. Boss just walks the path. | — |
| OQ2 | Aim assist at 2 ricochets vs 1 vs 3? In a 4-wall arena, 3-ricochet preview may be info-overload — but with the slingshot in the center, longer chains may be the *whole point* | W2 internal play |
| OQ3 | Does greybox (no art) produce valid readability signal? Specifically: does the path drawn as a faint floor texture read as "the road" without art? | After tester 2–3 — same gate as Pop Brigade |
| OQ4 | Should mid-flight merge consume the ally, or just buff it in place? Consume is current spec; buff-in-place keeps more defenders on the platform — but consume creates a clearer cause-effect ("the ally I flew through is gone, the new tier is bigger") | W2 internal play |
| OQ5 | 8 vs 6 arena columns — readability on small phones with 64px cells? | W1 prototype on small device |
| OQ6 | 12 vs 10 arena rows? | W2 internal play |
| OQ8 | Cross-color flying hero pass-through: silent pass vs visual "no merge" feedback? | W2 — test with internal testers |
| OQ9 | Boss color (purple) has no counter — frustrating or refreshing? | Stage 5 telemetry: completion rate + debrief Q6 |
| OQ10 | Friction tuning — in a 4-wall arena with center anchor, low friction enables long pinball chains across multiple walls; sweep critical | W1 — sweep 0.88 / 0.92 / 0.95 internally |
| OQ14 | Base HP regen between stages — +25 is generous; should it be +10 or 0 to keep run tension? | After tester debrief — if all 8 testers clear all 5 stages, regen is too high |
| ~~OQ15~~ | ~~Path layout per stage: varies vs fixed vs random?~~ **RESOLVED 2026-05-19**: **same path across all 5 stages in v1**. Stage difficulty escalates via spawn schedule + scatter-flick availability, not path complexity. Rationale: (a) testers learn one layout and master placement — clean signal on whether path-relative placement is being learned; (b) heroes settled in a run *stay* useful across stages, which strengthens the "build something over a run" feel; (c) authoring cost drops; (d) per-stage path variety can return in v2 as a meta-loop unlock. | — |
| **OQ16** | **Single entrance vs multi-entry path** (one shared exit at base)? Multi-entry is a Bloons TD staple but doubles cognitive load — possibly a v2/late-game stage type | W3 / v2 planning |
| **OQ17** | **Pull-back convention**: drag-away-fires-toward vs drag-away-fires-opposite (current spec, Angry Birds)? Testers may arrive with the opposite mental model from Lucky Defense (no slingshot) — A/B early | W1 internal — 2 testers each |
| ~~OQ18~~ | ~~Heroes settling on path — trample damage tuning?~~ **RESOLVED 2026-05-19**: heroes can no longer settle on path cells. Two-level terrain (sunken path + raised platform) — heroes are exclusively on platform. Trample mechanic removed entirely. Adopts canonical Lucky Defense / Bloons TD layout. Loses the "Gold-on-path" tier-coupling but gains visual clarity + tester recognition. | — |
| **OQ19** | **Targeting heuristic**: furthest-along-path is the Bloons canonical, but lowest-HP or nearest-to-base may give more Lucky-Defense feel. Add a per-class targeting mode toggle in meta? | v2 — out of v1 scope |
| **OQ20** | **Time-pressure mode**: should the boss stage (or a stage variant) use a Lucky-Defense-style time limit instead of HP? Tested separately from main loop | v2 planning |
| **OQ21** | **Path readability**: floor-texture only vs glowing dotted line vs animated arrow flow vs combo? "The path is the most important visual" — readability gate | W2 internal |

### 7.2 Risks (and mitigations)

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| **Slingshot input feels skill-gated** | Medium-High | Q2 fails for casual segment | Aim assist locked ON with 2-ricochet preview. Have "softened" config ready: 3-ricochet preview + auto-aim toggle. If Q2 still fails — slingshot is wrong for this audience, kill concept |
| **Mid-flight merge reads as gimmick** | Medium | Existential (Q1 fails) | Locked variant. If Q1 fails, pivot to V3 (party draft) or V5 (settle-as-aim). Don't iterate past 2 paper-test rounds |
| **Greybox confuses testers** (no art = "broken game") | Low–Medium | Q1/Q2/Q4 polluted | Same gate as Pop Brigade — calibrate on 1–2 internal testers W3 |
| **Aim assist too aggressive → flick feels auto** | Medium | Q4 fails (no skill expression, no dopamine) | A/B in-test: half of testers get "2-ricochet preview," half get "1-ricochet only." Compare |
| **Female-skew testers reject physics input** | Medium-High | Audience-fit hypothesis fails | This IS the hypothesis — track Q1–Q4 by gender. If female testers fail Q2 universally, concept is male-skew only (still viable, but smaller TAM) |
| **Round-based march feels sluggish** | Medium | Q4 fails (no urgency) | Have "continuous march, paused during pull" config ready. Test both internally W2 |
| **Arena visual clutter** (24-unit cap may still feel busy with ricochet trails + spawn telegraphs + enemy paths) | Medium-High | Q1/Q4/Q5 fail | 24-unit cap is the primary hedge. Greybox helps — if greybox reads as cluttered, no art will fix it. Backup: drop cap to 18 in W2 internal if clutter complaints surface |
| **3-week build slips** | Medium | Pushes tester window | Cut Stage 4 (Scatter flick) and the +25% damage boons. Test stays valid on 4 stages + 3 boons |
| **Tester pool can't get physics-game + TD-hybrid players** | Medium | Recruit bias | Fallback: recruit pure Lucky Defense players and pure Angry Birds players separately; tag results |
| **Physics determinism / replay** | Low | Telemetry hard to interpret | Use Box2D with fixed timestep + deterministic RNG. Log full flick inputs so any replay is reproducible |
| **Designer/engineer disagreement on tuning** | Medium | Slippage | §3.9 table is source of truth for W1–W3. Tuning changes need both signatures + 1-line "why" in commit |
| **Parallel build with Pop Brigade exhausts team** | Medium | Both prototypes slip | W1 paper for both, then dedicate to winner only. Don't try to greybox both — explicit kill rule |

### 7.3 What this v1 is NOT trying to prove

- **Monetization.** No spend signal. v2.
- **D1/D7 retention.** No persistent state, no push, no ads — retention isn't measurable here.
- **Long-term meta loop appeal.** Meta isn't built.
- **Hero collection / gacha appeal.** 3 colors only, no pulls.
- **Live ops cadence.** v2+.
- **Soft-launch market fit.** v3+ (formal soft launch).
- **UA creative testing.** No marketing assets needed for v1.
- **Whether slingshot beats bubble-shooter for this studio.** That's the **cross-concept** question, resolved by Pop Brigade v1 + Ricochet Brigade v1 results side-by-side. This doc only tests Ricochet Brigade in isolation.

Naming each non-goal here saves arguments during build. If a stakeholder asks "does this prove X?" and X is on this list, the answer is no — by design.
