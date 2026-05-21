---
name: Ricochet Brigade — Character Art Prompts
status: mood-board exploration
created: 2026-05-21
purpose: AI art generation prompts for v2 character mood-board. 9 heroes (3 colors × 3 tiers) + 1 boss. Use to drive concept art exploration before committing to a final visual direction.
design_spec: ~/game-research/ricochet-brigade/v2-design-spec.md
output_folder: ./demos/
---

# Ricochet Brigade — Character Art Prompts

> Use these prompts in LILA Art MCP, Midjourney, SDXL, or Imagen to mood-board the v2 character roster. Generated images go in `./demos/`. After mood-board review, lock the visual direction in a separate `character-roster.md`.

## Silhouette principle (load-bearing)

For 64×64 mobile asset readability, each color family needs a **distinct silhouette shape**. The covered-silhouette test: if you mask the colors and only see silhouettes, you should still know which class is which.

| Family | Silhouette shape | Role read |
|---|---|---|
| 🔴 Red Bruiser — **Crab Knights** | Low + wide, two big claws out | Heavy melee |
| 🔵 Blue Archer — **Mantis Hunters** | Tall + lean, raptorial arms | Ranged precision |
| 🟡 Yellow Mage — **Firefly Lantern Spirits** | Floating + glowing halo | Ethereal AoE |

**Tier progression rule (per Shikha's "high orc → destroyer orc" example):** Bronze → Silver → Gold within a family must read as the same archetype family, but the Gold tier is a visually distinct, promoted, ornamented, scarred-up version of Bronze. Not just a bigger sprite.

## Global style guide (paste at top of every prompt)

```
Style: 2D mobile gacha tower defense character asset, cartoon hand-painted
look (think Lucky Defense / Cookie Run / Habby quality), vibrant saturated
flat colors with soft cel shading, clean line art, strong readable
silhouette at 64×64 pixel resolution, full body, 3/4 front-facing pose,
transparent background, expressive face, cute but battle-ready.

Negative: no humans, no orcs, no stone golems, no flowers, no eggs, no
shamans, no crows, no crossbowmen, no realism, no anime-style proportions,
no busy backgrounds, no text or watermarks, no photo-realistic textures.
```

---

## 🔴 Red Bruiser — Crab Knight family

### Bronze — "Sand Crab Squire"
```
A small chibi sand crab warrior standing upright on hind legs, holding a
tiny driftwood club in one raised claw, the other claw forward as a
buckler. Bright coral-red shell with cream underbelly, coconut-half
helmet, beady determined eyes. Round body, eight little legs, scrappy
underdog energy.
```

### Silver — "Reef Brawler"
```
A medium battle crab in barnacle-encrusted shell armor, upright on hind
legs, brandishing a curved coral-bone sword. One claw is now huge and
muscular, the other holds a small reef-stone shield. Deeper red shell
with battle scars and gold trim, horned coral helm, fierce confident
expression. More armored and weathered than the squire tier.
```

### Gold — "Kraken-Claw Champion"
```
An enormous deep-crimson warlord crab in heavy reef-stone plate armor,
towering on hind legs. One colossal glowing claw grips a fossilized
blade radiating energy, the other a tattered banner with a pearl skull.
Crowned with a multi-horned coral helm inlaid with gold and
bioluminescent gems. Cape of woven seaweed. Dramatic pose, intimidating
but adorable, Mythic-tier gacha character energy.
```

---

## 🔵 Blue Archer — Mantis Hunter family

### Bronze — "Mantis Scout"
```
A small chibi praying-mantis archer, slender and upright, holding a tiny
twig recurve bow with a single arrow nocked. Pale teal-blue carapace,
triangular head with huge curious eyes, two raptorial arms folded
forward, delicate antennae. Wears a curled leaf cloak. Lean silhouette,
alert stance, scrappy explorer energy.
```

### Silver — "Mantis Marksman"
```
A taller cobalt-blue mantis archer, drawing a sleek composite bow at
full draw. Wears segmented chitin armor with copper rivets, a pointed
leaf-helm, and a quiver of long arrows on the back. Raptorial forearms
have become blade-scythes for close-range backup. Crouched ready pose,
sharper and more menacing than the scout, war-painted carapace.
```

### Gold — "Royal Mantis Sharpshooter"
```
A regal indigo-and-violet mantis general drawing an ornate jeweled
longbow with a glowing crystal arrow. Crown of antlered antennae,
enameled lacquer armor edged in gold, billowing royal cape with
starlight runes. Massive scythed forearms held back; a second smaller
pair of arms steadies the bow. Towering, elegant, dramatic backlight,
Mythic-tier gacha archer energy.
```

---

## 🟡 Yellow Mage — Firefly Lantern Spirit family

### Bronze — "Lantern Sprite"
```
A small chibi firefly spirit floating cross-legged in mid-air, holding a
tiny paper lantern that glows warm gold. Round amber-yellow bug body,
four delicate translucent wings, two antennae tipped with little
glowlights. Big innocent eyes, soft halo of light around the body.
Wears a tiny acorn-cap and a wisp of leaf as a scarf. Curious learner
energy.
```

### Silver — "Firefly Witch"
```
A medium glowing firefly mage hovering upright, gripping a curved
magical staff topped with a swirling lantern flame. Wears a pointed
wizard hat with a star-burst pattern and a flowing cloak embroidered
with constellations. Larger luminous wings with glowing veins, brighter
halo of light. Mid-cast pose with sparks swirling around the staff,
confident mid-tier mage energy.
```

### Gold — "Sun-Bearer Archmage"
```
A majestic giant firefly archmage radiating golden light, holding aloft
a miniature glowing sun in one hand and a flame-tipped scepter in the
other. Regal celestial robes embroidered with moving constellations, a
tall pointed hat crowned with a glowing all-seeing eye motif. Four
massive luminous wings spread wide, stars orbiting around the head, halo
of pure sunlight. Dramatic pose, Mythic-tier celestial archmage energy.
```

---

## Stage 5 Boss prompts (pick one)

### Option A — "Tide-Lord Leviathan" (ocean/tropical theme)
```
A colossal eel-serpent boss with abyssal armor plates, glowing predator
eyes, multiple fanning jaws, dripping with dark sea energy. Slithers
along the path. Four times the size of normal enemies. Reads as the
ocean's wrath — pairs with crab knights' coastal theme.
```

### Option B — "Hive-Queen Beetle" (forest/woodland theme)
```
A giant armored stag-beetle queen with spider-like legs, glowing crystal
abdomen, two enormous mandibles, and a crown of smaller insect minions
clustered on her back. Crushing earth-shaking presence. Pairs with the
mantis hunters' forest theme.
```

Pick A or B based on which arena theme you want to commit to.

---

## How to use these prompts

1. **AI tool:** LILA Art MCP (in this Claude session), Midjourney, SDXL, or Imagen
2. **Resolution:** 1024×1024 minimum; downsample to 256/128/64 to test silhouette readability
3. **Variations per prompt:** generate 4–6 per character; pick best to develop further
4. **Iteration:** if a character doesn't pass the silhouette test, add specific shape cues to the prompt ("massive shoulder pauldrons", "horned helm forming a clear V")
5. **Mood-board review:** lay out all 9 heroes (3×3 grid) at uniform scale; check family-readability + tier-progression-readability + class-readability across the board
6. **Save outputs:** put generated images in `./demos/` alongside this file; name them `red-bronze-v1.png`, `red-bronze-v2.png` etc.

---

## Open questions for character lock

1. **Family commitment** — do crab / mantis / firefly all work, or swap one (e.g., firefly is the riskiest cute-overload, could swap to "lantern jellyfish" or "wisp ghost"; but jellyfish overlaps Pop Brigade)?
2. **Color saturation** — Lucky Defense uses muted/desaturated palette under cartoon line; Coop TD goes more saturated. v2 lean?
3. **Eye style** — anime-style large eyes (Habby norm) vs Western cartoon dot-eyes (Cookie Run leans this way)?
4. **Animation language** — idle bob (firefly hover), idle sway (crab claws clack), idle twitch (mantis antenna)?
5. **Ult VFX direction** — committed in v2-design-spec §3.4 (Slam / Volley / Storm); art prompts should match later
6. **Enemy art** — separate prompt set needed; 4 enemy tiers + variants. Defer to next mood-board pass.
