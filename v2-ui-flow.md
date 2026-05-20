---
name: Ricochet Brigade — v2 UI Flow
status: locked for v2 greybox
created: 2026-05-20
supersedes: v1-ui-flow.md
design_spec: ~/game-research/ricochet-brigade/v2-design-spec.md
layout_reference: 5-platform arena (T + 2 small blocks + top block), yellow slingshot rampart at bottom, green spawn (right mid), red exit (top left)
scope_notes: Solo-only. No coop UI. No meta loop screens beyond a minimal hub.
---

# Ricochet Brigade — v2 UI Flow

> **Scope:** Every screen needed for v2 greybox playtest. No art direction — grayscale boxes + color labels only. Match screen gets the most detail (80% of player time).

> **Companion doc:** Mechanics in `v2-design-spec.md`. This doc only covers screens, transitions, and HUD readability.

---

## 1. Screen inventory

| # | Screen | Purpose | Time budget |
|---|---|---|---|
| 1 | **Boot** | Logo + load bar | 2s passive |
| 2 | **Hub** | Single "Play" button + run counter | <5s |
| 3 | **Party preview** | Show forced R+B+Y starting party + Start Run | 3–5s |
| 4 | **Match** | The game — arena + HUD + card UI + slingshot | 60–120s per stage |
| 5 | **Pause** | Resume / Quit overlay | 2–5s |
| 6 | **Stage clear** | Wave-end upgrade pick (1-of-3) | 5–10s |
| 7 | **Stage fail** | Run-end overlay | 5s |
| 8 | **Run end** | Run summary → back to Hub | 10s |

**Out of v2 scope:** Settings (use device defaults), audio toggles (system volume), gacha/collection screens, profile, tutorial overlay (greybox testers will be observed, not scripted).

---

## 2. Wireflow

```
                       ┌────────────────┐
                       │  1. BOOT       │
                       └───────┬────────┘
                               │ auto 2s
                               ▼
              ┌───────►┌────────────────┐
              │        │  2. HUB        │
              │        │  [PLAY]        │
              │        └───────┬────────┘
              │                │ tap PLAY
              │                ▼
              │        ┌────────────────┐
              │        │ 3. PARTY PREVIEW│
              │        │  R + B + Y     │
              │        │  [START RUN]   │
              │        └───────┬────────┘
              │                │
              │                ▼
              │        ┌────────────────┐
              │   ┌───►│  4. MATCH      │
              │   │    │  (Stage N)     │
              │   │    └─┬────┬──────┬──┘
              │   │ pause│  HP=0│ cleared
              │   │      ▼     ▼      ▼
              │   │   ┌─────┐┌─────┐┌──────────────┐
              │   │   │  5. ││  7. ││  6. CLEAR    │
              │   │   │PAUSE││FAIL ││ upgrade pick │
              │   │   └──┬──┘└──┬──┘└──────┬───────┘
              │   │      │      │           │ choose 1-of-3
              │   │  resume    end run      ▼
              │   │      │      │      ┌──────────┐
              │   └──────┘      │      │  next    │
              │                 │      │  stage?  │
              │                 │      └──┬────┬──┘
              │                 │       yes   no (stage 5 done)
              │                 │         │    │
              │                 │         │    ▼
              │                 ▼         │ ┌────────┐
              │           ┌──────────┐    │ │  8.    │
              └───────────┤  8. RUN  │◄───┘ │ RUN END│
                          │  END     │      └────┬───┘
                          └────┬─────┘           │
                               └──────────►──────┘
                                  back to HUB
```

---

## 3. Per-screen low-fi wireframes

### 3.1 Hub (screen 2)

```
┌────────────────────────────────────┐
│                                    │
│        RICOCHET BRIGADE            │
│         (logo placeholder)         │
│                                    │
│                                    │
│        ┌──────────────────┐        │
│        │       PLAY       │        │
│        └──────────────────┘        │
│                                    │
│                                    │
│      Runs played: 17               │
│      Best stage:  4                │
│                                    │
└────────────────────────────────────┘
```

Minimum viable. No nav, no shop, no profile.

### 3.2 Party preview (screen 3)

```
┌────────────────────────────────────┐
│  YOUR BRIGADE                      │
│                                    │
│  ┌─────┐  ┌─────┐  ┌─────┐         │
│  │  R  │  │  B  │  │  Y  │         │
│  │Brui-│  │Arch-│  │Mage │         │
│  │ ser │  │ er  │  │     │         │
│  └─────┘  └─────┘  └─────┘         │
│                                    │
│  (v2: party is fixed. Meta loop    │
│   will let you pick 3 of N later.) │
│                                    │
│                                    │
│        ┌──────────────────┐        │
│        │   START RUN      │        │
│        └──────────────────┘        │
└────────────────────────────────────┘
```

### 3.3 Match (screen 4) — **most detailed**

```
┌─────────────────────────────────────────────────┐
│ [HP ████████████ 100/100]   COINS: 23   STAGE 1 │  ◄── HUD strip
├─────────────────────────────────────────────────┤
│ 🔴◄──────────────────────────┐ seg 7            │
│ ╔══════════════════════════╗ │                  │
│ ║ TOP BLOCK   [B] [.] [Y]  ║ │                  │
│ ╚══════════════════════════╝ │                  │
│  ▲ seg 6                     │                  │
│  │                           │                  │
│    ╔═══════════════════════════╗                │
│    ║ T-HORIZ [R][.][B][.][.]   ║                │
│    ╚════╦══════════════════════╝                │
│ ▲       ║         ◄──────────┐  seg 5           │
│ │seg 5  ║                    │                  │
│         ║                    │ 🟢◄── seg 1      │
│  seg 4  ║T-V    seg 2        │   spawn          │
│  ▲      ║[Y]    ║▼                              │
│  │      ║[.]    ║                               │
│  │      ║[.]    ║                               │
│  │      ║                                       │
│ ╔══════════╗   ╔══════════╗                     │
│ ║ A [R][R] ║   ║ B [.][.] ║                     │
│ ║   [B][.] ║   ║   [Y][.] ║                     │
│ ╚══════════╝   ╚══════════╝                     │
│  ▲                                              │
│  │  ◄─────────────────────────   seg 3          │
│                                                 │
│   ··········· slingshot trajectory preview ··· │
│ ╔═════════════════════════════════════════════╗ │
│ ║ 🟡 SLINGSHOT      ╔═══╗ ╔═══╗ ╔═══╗         ║ │
│ ║  (drag from       ║ R ║ ║ B ║ ║ Y ║         ║ │
│ ║   slingshot       ║10 ║ ║10 ║ ║10 ║         ║ │
│ ║   anchor)         ╚═══╝ ╚═══╝ ╚═══╝         ║ │
│ ║                    Card 1 Card 2 Card 3      ║ │
│ ╚═════════════════════════════════════════════╝ │
└─────────────────────────────────────────────────┘
```

**Enemy path (7 segments — see `v2-design-spec.md` §3.1):**
1. Spawn 🟢 (right mid) → leftward into the gap below T-horizontal
2. ↓ down the right side of T-vertical
3. ← left across the bottom (between bottom blocks and yellow rampart)
4. ↑ up the left side of T-vertical
5. ← left under T-horizontal, then turn up at left edge
6. ↑ up past T-horizontal left side, then over top, then along right side of top block
7. ← left across the top to exit 🔴

**Slot value heatmap (for tester observation):**
- **T-vertical (3 slots):** highest — covers seg 2 + seg 4 (path passes twice)
- **T-horizontal (5 slots):** high — covers seg 5 + seg 6
- **Block A (4 slots):** medium — covers seg 3 + seg 4
- **Block B (4 slots):** medium — covers seg 1 + seg 2 + seg 3
- **Top block (3 slots):** low–medium — covers seg 7 only (last stretch before exit)

HUD must render the enemy path visibly during play (faint dashed line or worn-stone texture) so testers can read which slots cover which segments without explanation.

**HUD elements:**
- **HP bar** — shared HP, top strip, color-coded (green/yellow/red zones)
- **Coins** — running count, ticks visibly when coins drop or are spent
- **Stage indicator** — current stage number

**Hero slots:**
- Empty slot: dotted square outline
- Occupied: filled square with color letter (R/B/Y) + small tier indicator (●●○ for Silver, ●●● for Gold)
- Glowing ring around hero when ult meter is full (tap to fire ult)

**Card UI (bottom):**
- 3 cards always visible above the slingshot rampart
- Each card: color, class glyph (sword/bow/staff), cost (in coins)
- Tap card → card highlights → next tap on a platform slot places hero → card refills with a new draw (small refill animation)
- Cards that are too expensive show greyed-out with coin cost in red

**Slingshot interaction:**
- Touch and drag from the slingshot anchor (yellow area, center-bottom)
- While dragging: trajectory preview line drawn from slingshot → up into arena → bouncing off platforms (3 bounces max shown)
- Release to fire; deduct coin cost; projectile follows the previewed arc
- If insufficient coins, drag is rejected with a tiny shake feedback

**Drag conflict rule:**
- Drag starting INSIDE the yellow zone = slingshot aim
- Drag/tap starting on a CARD = card pick (then next tap goes to a slot)
- Tap on a glowing hero = ult fire
- Tap on a non-glowing hero (after picking a same-color same-tier card?) = merge candidate (or just no-op in v2 — keep merge as drag only)

### 3.4 Pause (screen 5)

```
┌────────────────────────────────────┐
│        ▓▓▓ PAUSE ▓▓▓               │
│                                    │
│   Stage 3 — Wave 4 of 6            │
│   HP: 67/100   Coins: 18           │
│                                    │
│   ┌──────────────────┐             │
│   │      RESUME      │             │
│   └──────────────────┘             │
│   ┌──────────────────┐             │
│   │      QUIT RUN    │             │
│   └──────────────────┘             │
└────────────────────────────────────┘
```

### 3.5 Stage clear — wave-end upgrade pick (screen 6)

```
┌────────────────────────────────────┐
│      ✓ STAGE 1 CLEAR               │
│                                    │
│      Pick one upgrade:             │
│                                    │
│   ┌────────────────────────────┐   │
│   │ 🛡  Red bruiser +15% dmg   │   │  ← Hero buff
│   └────────────────────────────┘   │
│   ┌────────────────────────────┐   │
│   │ 🎯  Slingshot +1 bounce    │   │  ← Sling buff
│   └────────────────────────────┘   │
│   ┌────────────────────────────┐   │
│   │ 💰  Coin tick rate +50%    │   │  ← Economy
│   └────────────────────────────┘   │
│                                    │
│   (one card from each category;    │
│    rarity tiers tinted differently)│
└────────────────────────────────────┘
```

### 3.6 Stage fail (screen 7)

```
┌────────────────────────────────────┐
│      ✗ DEFEAT                      │
│                                    │
│   Stage reached: 3                 │
│   Enemies killed: 87               │
│   Slingshot accuracy: 62%          │
│                                    │
│   ┌──────────────────┐             │
│   │      RETRY       │             │  (back to stage 1)
│   └──────────────────┘             │
│   ┌──────────────────┐             │
│   │   BACK TO HUB    │             │
│   └──────────────────┘             │
└────────────────────────────────────┘
```

### 3.7 Run end (screen 8) — full clear

```
┌────────────────────────────────────┐
│      ★ RUN COMPLETE                │
│                                    │
│   All 5 stages cleared!            │
│                                    │
│   Time:        12:37               │
│   Best merge:  Gold Archer         │
│   Slingshot:   47 fires, 31 hits   │
│   Ults fired:  9                   │
│                                    │
│   ┌──────────────────┐             │
│   │   BACK TO HUB    │             │
│   └──────────────────┘             │
└────────────────────────────────────┘
```

---

## 4. Match screen HUD spec — readability checklist

Every greybox HUD element must pass these:

| Element | Test |
|---|---|
| **HP bar** | Visible across the room from a phone in the tester's hand |
| **Coin counter** | Updates on every kill/spend with a brief +/- animation |
| **Card costs** | Greyed-out when unaffordable; never silently disabled |
| **Hero ult glow** | Distinguishable from non-ult heroes within 0.5s of glance |
| **Slingshot trajectory preview** | Solid line for direct path, dashed for bounce segments, fades after 1s if no release |
| **Stage progress** | Wave number visible (e.g., "Wave 3/6") on HUD or top strip |
| **Touch zones** | Yellow slingshot zone is visually distinct (color contrast); card zone is distinct from yellow; both clearly separate from arena |

**Drag conflict resolution (single-touch logic):**
- Touch START point determines intent:
  - Inside yellow zone (excluding card UI strip) → slingshot aim
  - On a card → card-pick mode (next tap = placement)
  - On a glowing hero → ult tap
  - On a non-glowing hero → drag-merge candidate
- Once started, drag is locked to that intent until release.

---

## 5. Asset checklist for v2 greybox

| Asset | Count | Format | Notes |
|---|---|---|---|
| Arena background | 1 | Single image or simple 2D scene | Grayscale; platforms as solid blocks |
| Hero sprites | 3 × 3 tiers = 9 | Colored shapes with tier glyph | R = red square, B = blue triangle, Y = yellow circle |
| Enemy sprites | 4 tiers | Single grey shape per tier with size scaling | Just placeholders |
| Slingshot anchor | 1 | Yellow ramp + slingshot Y-shape | Static |
| Projectile | 1 | Glowing dot | + trail particle |
| Bounce arc preview | 1 | Dashed line shader | Bend at platform edges |
| Explosion VFX | 1 | Simple radial burst | Greybox FX |
| Card UI | 1 template | 3 instances reused | Color swap based on card draw |
| HUD font | 1 | System default | No custom font in v2 |
| Wave-end card art | 3 templates | Icon + label | One per upgrade category |

---

## 6. Open UI questions (resolve before greybox build)

1. **Card pick interaction** — tap-then-tap, or drag-card-to-slot? Tap-tap is faster; drag is more tactile. Recommend tap-tap for accessibility + speed.
2. **Hero ult tap on glowing hero** — single tap fire, or hold-to-confirm? Recommend single tap (fast) since ults are forgiving (charge regenerates).
3. **Merge interaction** — drag-onto-target only, or auto-merge on drop if same tier/color? Recommend drag-onto (clearer intent).
4. **Slingshot trajectory preview** — always-on while dragging, or only after threshold drag distance? Recommend always-on, even with tiny drag (helps onboarding).
5. **Card refill delay** — instant refill, or short cooldown (~0.5s) to prevent rapid-fire same-card spam? Recommend short cooldown for greybox; tune after playtest.
6. **Wave-end card timeout** — manual pick only, or auto-pick a default after 10s? Recommend manual only; if tester freezes, observe it.
7. **Pause behavior during slingshot drag** — does dragging the slingshot pause the world physics? Recommend NO (keep enemies moving — adds pressure).
8. **Failure feedback** — does the screen shake / red-flash when an enemy reaches the red exit? Recommend YES (small flash) — signals damage without disrupting play.
