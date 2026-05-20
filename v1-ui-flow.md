---
name: Ricochet Brigade — v1 UI Flow
status: draft
created: 2026-05-11
updated: 2026-05-19
design_spec: ~/game-research/ricochet-brigade/v1-design-spec.md
arena_pivot: 2026-05-15 — match screen redrawn for 2D arena with base at bottom-center; enemies spawn from top/left/right edges
path_pivot: 2026-05-15 — superseded same day. Match screen redrawn for **center base + visible fixed serpentine path**. Path entrance is a single edge cell; enemies march continuously along the path; aim is full 360° from center.
platform_pivot: 2026-05-19 — terrain is now two-level (Lucky Defense / Bloons TD style). Path = sunken road (enemies only). Platform = raised tiles (heroes only). Heroes cannot settle on path cells — flicks landing there snap to the nearest platform cell.
---

# Ricochet Brigade — v1 UI Flow

> **Scope:** Every screen needed for the v1 greybox playtest. No art direction — grayscale boxes only. The match screen gets the most detail because it's 80%+ of player time.

> **Companion doc:** Mechanics in `v1-design-spec.md` §3. This doc only covers screens, transitions, and HUD readability.

---

## 1. Screen inventory

Eight screens total for v1. Anything not listed below is **out of scope** — including profile, shop, gacha, social, settings details, hero collection. If a tester asks "where's X?", the answer is "v2."

| # | Screen | Purpose | Time budget |
|---|---|---|---|
| 1 | **Boot** | Logo + loading bar | 2s passive |
| 2 | **Meta hub** | Single "Play" button + run counter | <5s |
| 3 | **Party pick** | View starting party (v1: forced R+B+Y) + start run | 5–10s |
| 4 | **Match** | The game itself — center base + visible path + 2D arena + HUD | 60–120s per stage |
| 5 | **Pause** | Resume / Quit overlay | 2–5s |
| 6 | **Stage clear** | Boon pick (1 of 3) | 10s |
| 7 | **Stage fail** | Run-end overlay | 5s |
| 8 | **Run end** | Run results screen → back to Meta hub | 15s |

**Out of v1:** Settings menu (use device defaults), audio toggle (system volume), tutorial overlay (testers will be guided verbally for v1), hero collection / gacha screens.

---

## 2. Wireflow

```
                       ┌────────────────┐
                       │  1. BOOT       │
                       │  (logo + load) │
                       └───────┬────────┘
                               │ auto after 2s
                               ▼
                       ┌────────────────┐
              ┌───────►│  2. META HUB   │◄──────┐
              │        │  [PLAY] btn    │       │
              │        └───────┬────────┘       │
              │                │ tap PLAY        │
              │                ▼                 │
              │        ┌────────────────┐       │
              │        │  3. PARTY PICK │       │
              │        │  R + B + Y     │       │
              │        │  [START RUN]   │       │
              │        └───────┬────────┘       │
              │                │ tap START      │
              │                ▼                 │
              │        ┌────────────────┐       │
              │  ┌────►│  4. MATCH      │       │
              │  │     │  (Stage N)     │       │
              │  │     └─┬────┬──────┬──┘       │
              │  │       │    │      │           │
              │  │  pause│ HP=0│  cleared        │
              │  │       ▼    ▼      ▼           │
              │  │   ┌─────┐ ┌─────┐ ┌──────┐   │
              │  │   │  5. │ │  7. │ │  6.  │   │
              │  │   │PAUSE│ │FAIL │ │CLEAR │   │
              │  │   └──┬──┘ └──┬──┘ └──┬───┘   │
              │  │      │       │       │        │
              │  │   resume   end run boon pick  │
              │  └──────┘       │       │        │
              │                 │       │        │
              │                 │  ┌────▼────┐   │
              │                 │  │ next    │   │
              │                 │  │ stage?  │   │
              │                 │  └─┬─────┬─┘   │
              │                 │  yes  no(boss done)
              │                 │   │     │     │
              │                 │   └► back to 4 (next stage)
              │                 │         │     │
              │                 ▼         ▼     │
              │           ┌──────────────────┐  │
              └───────────│   8. RUN END     │──┘
                          │  results + back   │
                          └───────────────────┘
```

**Trigger summary table:**
| From → To | Trigger |
|---|---|
| Boot → Meta hub | 2s auto |
| Meta hub → Party pick | Tap "Play" |
| Party pick → Match (Stage 1) | Tap "Start Run" |
| Match → Pause | Tap pause button |
| Pause → Match | Tap "Resume" |
| Pause → Run end | Tap "Quit Run" |
| Match → Stage clear | Spawn schedule finished + all enemies eliminated from path |
| Match → Stage fail | Base HP = 0 |
| Stage clear → Match (next stage) | Tap a boon card |
| Stage clear → Run end | Cleared Stage 5 (boss) |
| Stage fail → Run end | Tap "End Run" (no retry in v1) |
| Run end → Meta hub | Tap "Continue" |

**v1 decision: no retry on stage fail.** Same as Pop Brigade — clean A/B signal on first-run difficulty curve.

---

## 3. Per-screen low-fi wireframes

### 3.1 Boot (screen 1)
```
┌──────────────────────────────┐
│                              │
│                              │
│      RICOCHET BRIGADE        │
│         (greybox logo)       │
│                              │
│       ████░░░░░░░░░░ 40%     │
│         loading...           │
│                              │
└──────────────────────────────┘
```
Notes: No interaction. Auto-advances after asset load (≤2s on dev devices).

---

### 3.2 Meta hub (screen 2)
```
┌──────────────────────────────┐
│  ⚙          RICOCHET BRIGADE │
│                              │
│                              │
│     Runs completed: 3        │
│                              │
│                              │
│      ┌──────────────┐        │
│      │              │        │
│      │     PLAY     │        │
│      │              │        │
│      └──────────────┘        │
│                              │
│                              │
│         (v1 build)           │
└──────────────────────────────┘
```
Notes:
- Run counter is read-only — proves session persistence.
- ⚙ icon: dummy in v1. Hide if it confuses testers.

---

### 3.3 Party pick (screen 3)
```
┌──────────────────────────────┐
│  ◄  Your starting party      │
│                              │
│  ┌──────┐  ┌──────┐  ┌─────┐│
│  │      │  │      │  │     ││
│  │  🔴  │  │  🔵  │  │  🟡 ││
│  │      │  │      │  │     ││
│  │ Red  │  │ Blue │  │Yellw││
│  │ DPS  │  │ Slow │  │Range││
│  │      │  │      │  │     ││
│  │ 3-cell│ │ 5-cell│ │full ││
│  │ range│  │ range│  │arena││
│  └──────┘  └──────┘  └─────┘│
│                              │
│      ┌──────────────┐        │
│      │  START RUN   │        │
│      └──────────────┘        │
└──────────────────────────────┘
```
Notes:
- Cards are **read-only in v1** — party is fixed at R+B+Y. UI built for future flexibility.
- Each card shows color, class name, range — micro-tutorial without overlay.
- Tap "Start Run" → Stage 1 begins. No card selection in v1.
- v2: pick 3 from collection of 5+ heroes.

---

### 3.4 Match (screen 4) — **most important**

```
┌──────────────────────────────┐
│ Stage 2/5    Enemies: 8/15 ⏸ │ ← Top HUD: stage + spawn-remaining counter + pause
├══════════════════════════════┤
│ ▓▓▓▓▓▓▓┌──┐▓▓▓▓▓▓ ENTRANCE   │ ← Path enters at a top/L/R edge cell (gate sprite)
│ ▓▓░░░░░│  │▓▓▓▓🟥▓▓▓▓▓▓▓▓    │
│ ▓▓░    └──┘░░░░░░░░░░▓▓▓▓    │ ← Sunken ROAD (path cells, darker, lower elevation)
│ ▓▓░  👹           👹  ░▓▓▓   │   enemies walk here — heroes CANNOT be placed here
│ ▓▓░░░░░░░░░░░░░░░     ░▓▓▓   │
│ ▓▓🟦▓▓▓▓▓▓▓▓▓▓▓░      ░▓▓▓   │ ← Raised PLATFORM (everywhere else)
│ ▓▓▓▓▓▓- - - ▓▓▓░  🟨  ░▓▓▓   │   heroes settle here (tile lighter, raised look)
│ ▓▓▓▓▓▓▓▓- - ▓▓▓░    - -      │
│ ▓▓▓▓▓▓▓▓▓▓- -▓▓░  ┌────┐ ░   │   - - - - aim trajectory (dotted line, 2 ricochets)
│ ▓▓▓▓🟥▓▓▓▓▓▓▓▓▓░  │BASE│ ░   │
│ ▓▓▓▓▓▓▓▓▓▓▓▓▓▓░░░│ ▲  │░░░   │ ← 2×2 BASE on platform, slingshot mounted
│ ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓░ └────┘ ▓▓   │   path EXITS at the base
│ ▓▓▓▓🟨▓▓▓▓▓🟦▓▓▓░       ▓▓   │
│ ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓░░░░░░░░▓▓    │
│ ▓▓🟥▓▓▓▓▓▓▓▓🟦▓▓▓▓▓▓▓▓▓▓▓    │
│ ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓    │
├══════════════════════════════┤
│ Base HP ████████░░  ┌──┐ ┌─┐ │ ← Bottom HUD: BASE HP + on-deck + queue placeholder
│                     │🔵│ │Q│ │
│                     └──┘ └─┘ │
└──────────────────────────────┘

Legend:  ▓ = PLATFORM (raised, heroes settle here)
         ░ = PATH (sunken road, enemies only)
         🟥🟦🟨 = settled heroes (always on platform)
         👹 = enemies (always on path)
```

**Notes on the diagram:**
- **Two-level terrain is the most important visual element.** The path renders as a **sunken road** (darker, slightly lower-elevation tiles); the platform renders as **raised tiles** (lighter, slightly higher). The player should immediately read "enemies belong down there, heroes belong up here" without any explanation.
- **The PATH is always visible** from second 0 — the player can trace the entire serpentine layout before the first enemy spawns.
- **Path entrance** is a small gate/portal sprite on the edge cell where enemies spawn. Distinctive visual ("THIS is where they come from").
- **Path exit** is the base footprint itself — the final path cell terminates on the base.
- The **base** sits at the center of the arena on the platform level, with the slingshot mounted on top. Aim drag works from anywhere on the screen (forgiving touch zone); the projectile launches from the base in the direction of the drag vector.
- **Heroes settle on platform cells only.** If a flick comes to rest on a path cell, the hero plays a small "step-up" animation and lands on the nearest platform tile. Visually obvious — the hero is never seen standing in the road.

**HUD elements (annotated):**

| Element | Position | Behavior | Why it's here |
|---|---|---|---|
| Stage indicator | Top-left | "Stage N/5" — small text | Pacing feedback |
| Spawn-remaining counter | Top-center | "Enemies: X/Y" — X = still alive in this stage (alive on path), Y = total in spawn schedule | Replaces the prior round-bar. Tells the tester "how much stage is left" without timing math. |
| Pause btn | Top-right | Opens Pause overlay (screen 5) | Standard mobile |
| Path (sunken road) | Drawn on arena floor as lower-elevation tiles | Darker floor texture, slight depth/shadow cue, winds from entrance cell to base. Always visible. Not animated in v1 (animated arrow-flow is an OQ for v2). **Enemies only.** | **Single most important readability element** — paired with platform |
| Platform (raised terrain) | Drawn on arena floor as higher-elevation tiles | Lighter floor texture, slight raised-edge highlight on cells adjacent to path. Covers every non-path, non-base cell. **Heroes only.** | The other half of the readability pair — makes "where can I place" instantly obvious |
| Path entrance | Edge cell where path starts | Gate/portal sprite — visually distinct from walls. Glows briefly 1.0s before each spawn (the "warming" indicator). | Tells player "watch this spot" |
| Arena | Full screen | 8w × 12h grid (≈ 28 path cells + 64 platform cells + 4 base cells). **All 4 walls bounce.** Heroes settle on platform cells only; flicks landing on path auto-snap to nearest platform. Flick projectiles fly above terrain (no terrain collision in flight). | Combined play surface |
| Aim trajectory | Arena overlay | Dotted line during touch-hold, shows path through up to 2 ricochets off any of the 4 walls. Bounce points marked with small dots. Originates at base center. | **Critical readability** — testers must see ricochet path to plan merges |
| Base | Center of arena | 2×2 cell footprint. Slingshot mounted on top. HP bar overlaid on the base sprite (so the player sees "the thing I am protecting" with its own HP). | The literal lose-condition target |
| Base HP bar | Bottom-left strip (and overlaid on base) | Decrements when enemy reaches base; flashes red on hit | Constant HP awareness |
| On-deck hero | Bottom-right strip | Tap to swap with current loaded hero | Swap is core skill expression |
| Merge feedback | Mid-arena VFX | When flying hero passes through ally → bright flash + tier-up particle | Reward feedback for V2 mechanic |
| Settle-snap animation | When a flick lands on a path cell | Hero plays a small "hop up" onto the nearest platform cell, with a brief arrow showing the snap direction | Makes the snap explicit — tester sees "I aimed there but the hero went one tile over" |

**Match screen design rules (lock for v1):**
1. **No tutorial overlay.** Testers are guided verbally. If v1 needs an overlay, the design is too complex.
2. **The path is always visible and unambiguous.** It must be obvious before the first enemy spawns. Tester must be able to trace the path with a finger at second 0 of the first match.
3. **Slingshot + arena share visual language.** Hero in slingshot looks visually identical to hero in arena — only position differs. The mid-flight merge requires the player to see "the hero I'm flicking is the same kind of hero already settled."
4. **Aim trajectory always visible during touch-hold.** Never hidden, never modal. Release commits. Trajectory must show ricochet bounces clearly differently from straight-line segments (e.g. bounce points = small dots, distinct color from path dots — and distinct from the floor path texture).
5. **Spawn warming indicator must be unambiguous.** A glow on the entrance gate cell, 1.0s before each spawn. Tester must look at it and say "another one's coming."
6. **Base = HP target, visually.** HP bar lives on the base sprite (and is duplicated bottom-left). "Protect this thing" is the most casual-legible TD framing.
7. **Path vs platform must be unmistakable.** The two-level terrain is the readability spine. Test by greyscale screenshot: even with all color removed, the path should still read as "the road" and the platform as "where heroes go." Cells must have distinct visual treatment, not just color tinting — use elevation/shadow/texture so it works on small phones in bright sun.
8. **No round-bar.** No flick-count gauge. Time and spawn-remaining are the only pacing readouts. Real-time TD.
9. **No purchase buttons, no ads, no popups, no daily reward, no nudges.** v1 is pure mechanic test.

**Special mid-flight VFX (locked spec):**
- Wall bounce: small ripple at impact point (works on all 4 walls)
- Same-color ally pass-through: bright color flash + tier-up sparkle, ally disappears
- Cross-color ally pass-through: dim color flash, no merge — small "✗" floats up (readability for "why didn't that merge?")
- Enemy hit mid-flight: damage number floats up, hero may keep or lose velocity depending on hit type
- Hero settle: dust-puff + brief glow at settle cell. If the flick landed on a path cell, an additional "step-up" arrow particle plays toward the platform cell the hero hops to. With a subtle distance-to-path indicator (e.g. settled hero pulses green if path-adjacent, neutral otherwise — debug-toggleable for v1 testers, removed for final)
- Base re-entry of own flick (hero touches base footprint): quiet "thunk" particle, no HP damage — distinct from enemy-on-base impact
- Spawn warming (1.0s before each spawn): yellow/orange glow building on the entrance gate cell
- Enemy reaches base: red flash on the base sprite + HP bar shake + damage number on base

---

### 3.5 Pause (screen 5)
```
┌──────────────────────────────┐
│         (match dimmed)       │
│                              │
│         ┌────────────┐       │
│         │            │       │
│         │  PAUSED    │       │
│         │            │       │
│         │ [RESUME]   │       │
│         │            │       │
│         │ [QUIT RUN] │       │
│         │            │       │
│         └────────────┘       │
│                              │
└──────────────────────────────┘
```
Notes: Quit Run skips Run End screen and jumps directly to Meta hub (with run logged as incomplete).

---

### 3.6 Stage clear (screen 6)
```
┌──────────────────────────────┐
│         STAGE 2 CLEAR        │
│                              │
│   Heroes built: 7 | Gold: 1  │
│   Merges: 4 | Best chain: 3  │
│                              │
│       Pick one boon:         │
│                              │
│  ┌──────┐ ┌──────┐ ┌──────┐ │
│  │ Next │ │ +25% │ │  +1  │ │
│  │  3   │ │ DMG  │ │scattr│ │
│  │flicks│ │ all  │ │ flick│ │
│  │ RED  │ │heros │ │this+ │ │
│  └──────┘ └──────┘ └──────┘ │
│                              │
│        tap to continue       │
└──────────────────────────────┘
```
Notes:
- Mini-stats row at top is a v1 addition over Pop Brigade — reinforces "what you did mattered" because slingshot players are more reflective post-stage than puzzle players.
- 3 boon cards randomly drawn from boon pool (see design spec §4.2).
- Tap = commit, immediately starts next stage.
- No skip button — pick is mandatory.

---

### 3.7 Stage fail (screen 7)
```
┌──────────────────────────────┐
│        (match dimmed)        │
│                              │
│         ┌────────────┐       │
│         │            │       │
│         │ STAGE FAIL │       │
│         │            │       │
│         │ Reached    │       │
│         │ Stage 3/5  │       │
│         │            │       │
│         │ [END RUN]  │       │
│         │            │       │
│         └────────────┘       │
│                              │
└──────────────────────────────┘
```
Notes: Single button. No retry (v1 instrumentation cleanliness).

---

### 3.8 Run end (screen 8)
```
┌──────────────────────────────┐
│          RUN COMPLETE        │
│                              │
│   Stages cleared:   5 / 5   │
│   Flicks made:      89      │
│   Merges:           18      │
│   Golds built:      4       │
│   Enemies killed:   72      │
│     - by flick:     27      │
│     - by towers:    45      │
│   Best ricochet chain: 5    │
│                              │
│      [CONTINUE]              │
└──────────────────────────────┘
```
Notes:
- Stats logged to v1 telemetry file (see design spec §6.2).
- "By flick" vs "by towers" split — directly maps to the **mid-flight kill rate** derived metric. Tester sees this number; observer also captures it.
- Continue → back to Meta hub.
- Same screen whether run was completed or quit early (different stats).

---

## 4. Match screen HUD spec — readability checklist

The match screen carries the entire v1 test. Before greybox build is shown to testers, every item below must be true:

| Check | Pass criteria |
|---|---|
| **Path is unmistakable at second 0** | Before any enemies spawn, tester can trace the path with a finger from entrance to base |
| **Platform vs path two-level read is instant** | Tester describes the layout in two-level language ("heroes on the ledges", "enemies in the road", "high ground / low ground") without being told. Greyscale screenshot test passes. |
| **Path-adjacent placement is felt** | Tester references path-adjacent platform position ("near the turn", "just before it enters the base") by Stage 2; settles within 2 platform cells of path ≥70% by Stage 3 |
| **Settle-snap is non-confusing** | When a flick lands on the path and snaps to platform, tester accepts it ("oh, it went up here") rather than complaining ("I aimed there, why did it move?"). Snap rate ≤20% by Stage 3 means players are aiming at the platform, not the path. |
| Aim trajectory is unmistakable | Tester correctly predicts where flicked hero will go within 1 try by stage 2 |
| Ricochet bounce points are legible | Tester points to a wall and says "it'll bounce here" before releasing the flick |
| Mid-flight merge is legible | Tester correctly describes "my hero combined with the one already there" without prompting; says "tier up" or equivalent |
| Hero color → class is learnable in 1 stage | After Stage 1, tester can predict "this red one is a damage dealer" |
| Continuous march is felt as urgent (not sluggish) | Tester is actively flicking, not waiting; no "is it my turn?" confusion |
| Base HP loss is felt | Tester reacts to base-HP drop within 1s and references "they're attacking the base" |
| Slingshot pull feels powerful | Tester pulls to max distance at least once per match (not always min-power tap) |
| Cross-color "no-merge" is legible | Tester doesn't repeatedly try wrong-color merges past Stage 2 |
| Spawn warming indicator is read | Tester preemptively flicks toward an entrance-glow before the enemy spawns |
| Hero settling matters | Tester references where heroes ended up ("I wanted that yellow on the path") rather than treating settle as random |

If 4+ of these fail, the match screen layout is wrong — not the mechanic.

---

## 5. Asset checklist for v1 greybox

Greybox = engineer-buildable without artist. Final list:

**Sprites (placeholder shapes OK):**
- 1 hero unit (3 colors via tint, tier shown as size + 1/2/3 stars overlay)
- 1 enemy unit (3 colors via tint)
- 1 boss enemy (purple, larger, distinct silhouette)
- 1 slingshot frame
- 1 base sprite (2×2 cells, has HP bar overlay slot)
- 1 path tile — sunken-road floor tile (darker, lower elevation)
- 1 platform tile — raised-terrain floor tile (lighter, higher elevation, slight bevel/edge on cells adjacent to path)
- 1 path entrance gate (distinct edge sprite — "enemies come from here")
- 1 trajectory dot (for aim preview)
- 1 ricochet bounce-point marker (visually distinct from trajectory dot AND from path texture)
- 1 spawn-warming indicator (glow that pulses on entrance gate)
- 1 scatter-flick visual (rainbow tint or sparkle overlay)
- 1 trample badge (impact-puff on trampled hero)

**UI:**
- HP bar
- Stage indicator text
- Spawn-remaining counter (text "Enemies: X/Y")
- Pause icon
- Boon cards (3 generic card frames + text overlay)
- Party pick cards (3 generic class cards)
- Buttons (Play, Start Run, Resume, Quit, Continue, End Run) — system default style

**VFX (minimum viable):**
- Flick release flash (slingshot recoil)
- Trajectory dots (live during pull)
- Wall bounce ripple
- Same-color ally pass-through (bright flash + sparkle)
- Cross-color ally pass-through (dim flash + "✗")
- Mid-flight enemy hit (damage number)
- Hero settle (dust puff + glow)
- Auto-fire projectile (small bullet trail)
- Enemy death (color flash + fade)
- Scatter flick split (3-way burst at midpoint)
- HP loss screen shake

**Audio (optional v1, recommended v1.1):**
- Slingshot pull-tension, flick release, wall bounce, merge chime, settle thud, enemy death, HP loss SFX.

---

## 6. Open UI questions (resolve before greybox build)

| Question | Owner | Deadline |
|---|---|---|
| Arena width: 8 or 6 columns? 6 may be more readable on small phones, but cramps the path | Designer | Before build start |
| Arena height: 12 or 10 rows? Taller = longer paths possible, more flight time, more visual area | Designer | Before build start |
| Aim assist intensity: 2 ricochets, 1 ricochet, or 3? With center-anchored slingshot, longer ricochet chains are the *whole point*, so 3 may actually be right | Designer | Before tester recruit |
| Mid-flight merge VFX: bright flash + ally disappear, or morph animation? Morph is clearer but more art work | Engineer + Designer | Sprint 2 |
| Cross-color "no merge" feedback: visible "✗" vs dim flash only vs nothing? | Designer | Sprint 2 — test with internal testers |
| Slingshot input zone: only the base sprite, full bottom 30%, or **entire screen**? Center-anchored aim with full-360° drag suggests full-screen is best | Designer + Engineer | Before build start |
| **Path rendering style**: faint floor texture (current) vs glowing dotted line vs animated arrow-flow vs combo? "The path is the most important visual" — readability gate | Designer | Sprint 2 — internal play |
| **Path entrance sprite**: gate vs portal vs glowing arrow vs simple highlighted edge cell? Tester must instantly recognize "enemies come from here" | Designer | Before build start |
| **Spawn-warming indicator timing**: 1.0s before spawn (current spec) vs 0.5s vs 2.0s? Affects how proactive players can be | Designer | Sprint 2 — internal play |
| Base visualization: literal "building/castle" sprite vs slingshot-on-pedestal vs abstract base-tile? Castle is most legible for "protect this" framing in a TD context | Designer | Before build start |
| **Path vs platform visual treatment**: how much elevation contrast? Options: (a) flat with texture difference only, (b) subtle 4–8px depth shadow, (c) strong isometric-style level shift. Strong = clearest readability but heavier art lift; flat = greybox-friendly but may not read in playtest | Designer | Before build start — **the** key visual call for v1 |
| **Settle-snap visual**: small hop-up arrow (current spec) vs full hop animation vs instant teleport with sparkle? Tester must accept the snap without confusion | Designer | Sprint 2 |
| **Pull-back drag convention**: drag-away-fires-toward vs drag-away-fires-opposite (Angry Birds — current). Bloons TD players don't have a slingshot prior — does Angry Birds mental model dominate? | Designer | W1 — A/B 2 testers each |
| **Stage indicator + spawn-remaining counter**: combined to one line vs separate? Mobile real estate is tight | Designer | Sprint 2 |
