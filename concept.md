---
name: Slingshot TD — Concept One-Pager (Ricochet Brigade)
status: active
created: 2026-05-11
updated: 2026-05-19
---

## Goal
Draft a 1-pager pitch for a Slingshot Tower Defense hybrid targeting the Lucky Defense / Survivor.io / Capybara Go player. Output: CEO-ready concept doc + 30-day prototype plan. Counterpart to Pop Brigade (Bubble Shooter TD), giving us a second mechanic option to paper-test.

## Context
- Studio pivot post-BLACK: hybrid casual game, 6–8 month ship, team of 10. See [new_project_market_research.md](~/.claude/projects/-Users-shikhasingh/memory/new_project_market_research.md).
- Top 3 candidates from research: Lucky Defense (merge), Slime Legion (match), GearPaw Defenders (gear).
- Sister concept: Pop Brigade (Bubble Shooter TD) — `~/game-research/pop-brigade/concept.md`.
- AppMagic competitive scan (2026-05-10):
  - Slingshot lane: XFLAG/Monster Strike does $27M/mo but **JP-locked** monster collector. Global slingshot-TD lane essentially **empty** — highest-ceiling, highest-risk lane in the scan.
  - Angry Birds-style flick lane: Rovio-owned (acquired by Sega), $30–50M/mo across the catalogue but pure puzzle, not TD. Audience trained on flick input — pool exists.
  - Plinko/drop lane: top game $10K/mo. Genre too small.
- Earlier kill of slingshot lane (in Pop Brigade concept) was based on **Slime Legion audience fit**. This concept makes the **opposite bet**: target the Lucky Defense / Survivor.io / Capybara Go axis, where physics-aim is more native and male-skew is acceptable.
- Methodology: [game_research_methodology.md](~/.claude/projects/-Users-shikhasingh/memory/game_research_methodology.md), [game_variant_generation.md](~/.claude/projects/-Users-shikhasingh/memory/game_variant_generation.md).

---

# One-Pager: Ricochet Brigade (working title)

## Pitch (30 sec)

A **classic-path tower defense** with a slingshot. Your base sits at the center of a 2D arena on raised **platform** terrain. A fixed serpentine **road** winds from one edge, sunken below the platform, and ends at your base — enemies march along the road. You slingshot-flick colored heroes outward from the base in any direction — they ricochet off the 4 arena walls, pinball through your settled allies (tiering them up mid-flight), and land on a platform tile as auto-fire defenders shooting *down* at the road. Heroes placed on platform tiles adjacent to the road do work; heroes you fling into a corner don't. Five colors = five hero classes (DPS / slow / heal / range / AOE). Roguelike runs with gacha hero collection meta.

**Bloons TD's two-level path × Monster Strike's slingshot × Lucky Defense's color-class meta.** Three proven layers, one novel binding: ricocheting *through* your own allies tiers them up — the flick is your defense placement *and* your levelling system, in a single gesture.

---

## Target player

Lucky Defense / Survivor.io / Capybara Go / King of Avalon player. Hybrid casual leaning male-balanced, 5–15 min sessions, comfortable with ~4 cognitive layers, tolerates skill-based input. **Not** the pure Slime Legion casual female player — that's Pop Brigade's lane.

Why two concepts? They split the audience cleanly: Pop Brigade goes for puzzle-trained female-skew casual; Ricochet Brigade goes for action-trained mixed-gender mid-core. Both feed the same studio-pivot brief.

---

## Core loop

| Beat | Action | Outcome |
|---|---|---|
| Combat | Pull-back-and-release a hero from the center base | Hero flies into the 2D arena, ricochets off 4 walls + units |
| Combat | Hero passes through an ally of same color | Flying hero tiers up (bronze → silver → gold), consumed ally vanishes |
| Combat | Hero hits enemy mid-flight | Damage = current tier × velocity |
| Combat | Hero stops moving | Settles on the nearest empty **platform** cell; if it stopped on the road, it hops up to platform |
| Combat | Enemies march continuously along the sunken road | Heroes on platform cells near the road auto-fire down at them; heroes far from the road sit idle |
| Combat | Enemy reaches center base | Base HP drops by enemy's reach-damage; enemy is consumed |
| Wave end | Stage clear (all enemies along the path eliminated, no spawns pending) | Pick 1 of 3 loadout boons (color bias, special flicks, meta) |
| Run end | Stage 15 boss | Run rewards → meta progression |

**Cognitive layers (matched to Lucky Defense ceiling = 4):**
1. Aim + power (pull-back angle + distance, full 360° from the center)
2. Color choice (which class do I flick now)
3. Ricochet path planning (which allies do I want to chain through, which walls to bank off — to upgrade allies mid-flight)
4. **Platform-adjacent placement** (will the hero land on a platform cell next to the road or wasted in a far corner — and which segment of the road needs reinforcement next)

---

## Five-class hero system (color = class)

| Color | Class | Fantasy |
|---|---|---|
| Red | DPS | Fire knight |
| Blue | Slow | Ice mage |
| Green | Heal/shield | Druid |
| Yellow | Range | Archer |
| Purple | AOE | Wizard |

**Mid-flight merge rule:** A flicked hero passing through a stationary ally of the **same color** absorbs them and tiers up. Bronze + Bronze → Silver. Silver + Bronze → Gold. Cross-color passes do nothing (intentional — preserves class identity).

---

## Progression (3-layer meta, copied from Habby/IGG playbook)

| Layer | Length | Hook |
|---|---|---|
| Match | 3–5 min | Ricochet chains, mid-flight merges, special flicks (bomb hero, rainbow hero, scatter shot) |
| Run | 15–20 min | 15 stages + boss, pick-3 boons, random color bias, party composition draft |
| Meta | Long-term | Hero gacha (~25 heroes), slingshot upgrades, party slots, battle pass, daily/weekly events, clan raids (post-launch) |

---

## Monetization

| Stream | Notes |
|---|---|
| Gacha | Hero pulls — premium currency, chase rares (anchor monetization) |
| Energy | 5 runs/day, refill paid or ad |
| Boosters | Pre-run: extra special flicks, hero head-start, slingshot power-up |
| Battle pass | Seasonal, $5–10 |
| Ad rewards | Revive, double rewards, free spin, free guaranteed-Gold flick |

**Target ARPDAU:** $0.18–0.30 (Lucky Defense / Survivor.io band — slightly higher than Pop Brigade's $0.15–0.25 because action-leaning mid-core monetizes harder).
**Target D1 / D7 / D30:** 35% / 14% / 5% (mid-core hybrid casual benchmark — slightly weaker D7 than Pop Brigade because action loops have steeper churn but stronger ARPDAU).

---

## Differentiation (10–30% innovation framing)

| Slice | What |
|---|---|
| 70% Bloons TD / Lucky Defense DNA | Fixed serpentine path, center-base HP target, color-as-hero classes, gacha meta, roguelike runs |
| 20% slingshot input | Pull-and-release physics from center, full 360° aim, ricochet bumpers across all 4 arena walls |
| 10% novel hook | **Mid-flight merge:** ricocheting through an ally of the same color tiers them up. The flick is a single gesture that places a defender *and* levels existing ones — placement and progression are the same input. Monster Strike does ricochet-through-allies for damage; we do it for *progression*. |

---

## Competitive positioning

| Game | Why it's not us |
|---|---|
| Monster Strike (XFLAG, $27M/mo) | JP-locked monster collector; turn-based party RPG, not continuous TD; global launches have failed. We borrow its **2D arena ricochet grammar** but pair it with Lucky Defense's continuous lane-defense meta — that combo has not shipped globally |
| Angry Birds franchise (Rovio/Sega) | Pure puzzle, no TD layer, no gacha meta, single-shot rounds |
| Lucky Defense (IGG) | Merge input on a lane; same meta but no skill expression in combat, no path |
| Slime Legion (Habby) | Match-3 input; same TD meta but no physics aim |
| Knighthood / various flick-RPGs | Either turn-based RPG or pure puzzle — no continuous TD path |
| Bloons TD / Kingdom Rush | Tap-to-place towers on a path; we replace tap-place with slingshot-flick that ricochets and tiers up — input is much more interactive, with skill expression |

**Unclaimed slot:** slingshot mental model + Habby-tier TD meta + global UA pipeline. Monster Strike proved demand at $27M/mo in one country; no one is serving the global player.

---

## Production

| Item | Estimate |
|---|---|
| Team | 10 (per studio constraint) |
| Engine | Unity (physics maturity + casual UA pipeline) |
| MVP timeline | 6–8 months to soft launch |
| Soft launch markets | PH, ID, BR, TR (mid-core friendly emerging markets) |
| Global launch target | Q1 2027 |

**Engine note:** Slingshot needs deterministic 2D physics (Box2D-class). Unity's built-in 2D physics is fine. Godot 4 viable but team has more Unity hours.

---

## Risks + open questions

| Risk | Mitigation |
|---|---|
| Physics input feels skill-gated → casual churn | Aim guide locked ON, trajectory preview with 2 ricochets; auto-aim "easy mode" option in settings post-MVP |
| Monster Strike comparison sets unfair expectation | Position as "Lucky Defense with skill expression" not "Monster Strike global"; soft launch messaging focuses on TD lane, not collector RPG |
| Female-skew player rejects physics input | Soft launch tracks D1 by gender; if female D1 <70% of male D1, swap to Pop Brigade as primary |
| 2D arena + slingshot = visual clutter (many heroes + many enemies on screen at once) | Art direction prototype month 1; readability gate before greybox handoff. Cap on-screen units (heroes + enemies) at 24 in v1 to prevent visual overload |
| Habby copies post-launch | Lead with gacha pipeline polish + first-mover UA spend in 6-month window |

---

## Ask

- Sign-off to start paper prototype **alongside** Pop Brigade (both go to paper-test simultaneously, pick one to greybox based on tester signal)
- 1 designer + 1 engineer shared across both prototypes for weeks 1–2 (paper only), then dedicate to winner for weeks 3–4
- Reference budget approval (~$300 total for buying competitor gems / battle passes for teardown across both concepts)

---

## Phases

- [ ] Phase 1: CEO review of one-pager + go/no-go for paper prototype (alongside Pop Brigade)
- [ ] Phase 2 (Week 1–2): Paper prototype core loop (Figma + manual playtest) — same testers as Pop Brigade for comparative signal
- [ ] Phase 3 (Week 2–3): Greybox vertical slice (5 stages, 3 hero colors, no meta) — **only if Phase 2 signal beats or matches Pop Brigade**
- [ ] Phase 4 (Week 4): Internal playtest — readability + complexity gate
- [ ] Phase 5: Decision gate — go to vertical slice (with art) or kill
- [ ] Phase 6: If go — add to game_comparison_framework.md as self-designed entry, run formal Phase 5+6 scoring

---

## Notes / decisions

**2026-05-11**
- Reversed earlier Pop Brigade-concept kill of slingshot lane. The original kill was correct *for the Slime Legion audience*. This concept makes the opposite audience bet: Lucky Defense / Survivor.io mid-core, where physics-aim is native.
- AppMagic global slingshot-TD lane is the **emptiest top-grossing lane in the entire scan**. Monster Strike at $27M/mo proves demand exists, but no one is exporting that mental model globally. Highest ceiling, highest execution risk.
- 10% novel hook = **mid-flight merge** — ricocheting through allies tiers them up. Solves the slingshot-vs-TD "two games glued" risk by making the ricochet path the strategic decision (which allies do you chain through), bonding the flick input to the lane state.
- Two-concept rationale: Pop Brigade and Ricochet Brigade target different audience axes from the same studio brief. Paper-test both with the same testers in week 1. Greybox only the winner. Total commit cost: ~2 weeks designer + 2 weeks engineer for paper + 1 dropped concept.
- Open question: is "Ricochet Brigade" the right name? Placeholder until naming pass. Alternatives: Slingshot Squad, Flick Legion, Pinball Patrol, Strike Brigade.

**2026-05-15 — Arena pivot (superseded same day; see "Path pivot" below)**
- Initially replaced the 1D lane with a 2D rectangular arena + bottom-center base + 3-edge enemy spawn with A* pathfinding around settled heroes.
- Decision reversed within the same day after considering Lucky Defense / co-op TD framing — see next entry.

**2026-05-15 — Path pivot (current)**
- Replaced the 3-edge A* spawn pattern with a **classic-TD fixed serpentine path**: enemies enter from one edge, follow a hand-authored path that winds through the 2D arena, and reach the base in the **center**.
- Base moved from bottom-center to **center of the arena** (cells (3,5)–(4,6) on the 8×12 grid). Slingshot is mounted on the base; the player drags in any of the 360° to aim outward.
- **Enemy march is now continuous** (real-time TD pace), not round-based. The round-based casual-rhythm hedge is removed — game leans more action TD, less puzzle. This aligns with the Lucky Defense / Survivor.io / Capybara Go target player (already action-leaning mid-core).
- **Lose condition stays HP-based** (base HP, no change). Time-pressure (Lucky-Defense style) is flagged as a boss-stage variant OQ, not the default.
- Heroes still settle anywhere on the grid; the path makes positional value *obvious* rather than emergent — testers can read at a glance whether a settled hero is useful.
- Comp anchor now: **Bloons TD / Kingdom Rush path × Monster Strike slingshot × Lucky Defense color-class meta + gacha.** Three proven layers, one novel binding (mid-flight merge).
- Open questions: single path or multi-entry, path variation per stage, whether the path layout itself becomes a meta-loop unlock — see v1-design-spec.md §7.1.

**2026-05-19 — Path fixity locked**
- Decided: **one path for the entire v1 run.** All 5 stages share the same hand-authored 28-cell serpentine. Difficulty escalates via spawn schedule + new colors + specials, not path complexity. Heroes carry across stages (HP and position both), so tier progression becomes a run-wide arc rather than a per-stage reset. Per-stage path variety returns in v2 as a meta-loop unlock.
- Rationale: cleaner signal on whether testers learn path-relative placement (one layout, mastered over the run), lower authoring cost, stronger "build something over a run" feel. Closes design-spec OQ15.

**2026-05-19 — Two-level terrain (platform vs road)**
- Decided: heroes occupy raised **platform** cells; enemies walk on the sunken **road** (the path). Heroes cannot settle on path cells — flicks landing on path auto-snap to the nearest platform cell. Canonical Lucky Defense / Bloons TD / Kingdom Rush layout. Closes design-spec OQ18 (trample mechanic removed entirely).
- Rationale: visual hierarchy becomes a load-bearing readability feature — testers from co-op TD / Bloons / Lucky Defense recognize the layout on sight. Loses the "Gold-on-path tank" tier-coupling, but gains a much cleaner platform vs road decision space and one fewer tuning value to playtest.
- Implication: heroes flicked beside the path do the most work; heroes flung into far corners are wasted. The decision is now binary-legible at a glance ("did the hero land where it can shoot the road?") rather than a positional gradient with hidden trample math.
