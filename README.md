# Ricochet Brigade

Self-designed concept candidate for the post-BLACK studio pivot. Hybrid casual game in the merge-defense TD lane (Coop TD / Lucky Defense proven template) with three slight twists.

## Repo layout

| Path | Purpose |
|---|---|
| `concept.md` | One-pager: lane, locked DNA, existential risk, opening |
| `v1-design-spec.md` | v1 design (slingshot-as-spawn) — **superseded** |
| `v1-ui-flow.md` | v1 screen flow — **superseded** |
| `v2-design-spec.md` | **v2 design (active)** — pick-3 + tap-place + slingshot-as-spell |
| `v2-ui-flow.md` | **v2 screen flow (active)** |
| `Ricochet Brigade/` | Unity prototype project (currently still v1 mechanics — pending v2 build) |

## Design state

**v1 (killed 2026-05-20):** Slingshot was the primary spawn input. Heroes were flicked onto a path-relative platform; the locked variant was mid-flight merge. Killed because (1) flick-as-input was structurally skill-gated for casuals, (2) the slingshot screen vs lane read as two glued-together games, (3) the differentiation was thin against Coop TD / Lucky Defense once their actual mechanics were verified.

**v2 (active, design locked 2026-05-20):** Three slight twists on the proven merge-defense TD lane:

| Twist | What | Why |
|---|---|---|
| **T1 — Pick-1-of-3 spawn** | Hero cards refill; tap card → tap platform slot | Both leaders have zero spawn agency (pure RNG); pick-3 is auto-chess-proven |
| **T2 — Tap-charged hero ults** | Heroes charge ult meter from auto-combat; tap to fire | Both leaders are passive after placement; ults add mid-wave skill |
| **Slingshot as coin-spell** | Cheaper than a hero, AoE explosion, Angry Birds aim with visible bounce trajectory, ricochets off elevated platforms | Adds a third mid-wave decision + a skill-expression surface for engaged players |

**Layout:** 5 elevated platforms (T-shape + 2 small blocks + top block) = 19 hero slots total. Yellow slingshot rampart at the bottom. Enemies spawn at the green dot (right mid), snake around platforms through sunken corridors, exit at the red dot (top-left). Every enemy reaching red costs HP from the shared HP bar.

**Scope for v2 greybox:** Solo only. 5 stages. 3 hero colors × 3 tiers. 4 enemy tiers + 1 boss. Greybox art only. No gacha, no meta, no coop.

## Comp differentiation (verified)

| | Coop TD | Lucky Defense | Ricochet Brigade v2 |
|---|---|---|---|
| Battlefield | Puzzle path (T + corners) | Open grid | T + 2 blocks + top block (5 platforms, 19 slots) |
| Spawn | Gacha tap (RNG) | Gacha summon (RNG) | **Pick-1-of-3** |
| Merge | 2-tile | 3-tile → random +1 rarity | 2-tile (matches Coop TD) |
| Mid-wave agency | None (Auto-Repeat option) | None | **Ult tap + slingshot fire** (2 layers) |
| Secondary attack | None | None | **Coin-fueled slingshot AoE** |
| Failure | Shared coop HP | Wave defense | Shared HP (solo for v2) |
| Social | Coop core | Solo | Solo (v2); coop deferred |

Three twists × proven lane = ~35% innovation budget, distributed across three small risks rather than one structural bet.

## Methodology

Per `~/.claude/projects/-Users-shikhasingh/memory/game_research_methodology.md` and `game_variant_generation.md`. Comp set + cross-concept index in `~/.claude/projects/-Users-shikhasingh/memory/game_concept_variants.md`.
