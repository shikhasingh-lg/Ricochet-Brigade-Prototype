using System.Collections.Generic;
using UnityEngine;

namespace RicochetBrigade
{
    public sealed class V2ArenaLayout : MonoBehaviour
    {
        public static V2ArenaLayout Instance { get; private set; }

        private readonly List<HeroSlot> slots = new List<HeroSlot>();
        private readonly List<V2PlatformBlock> platforms = new List<V2PlatformBlock>();
        private readonly List<Vector2> pathPoints = new List<Vector2>();
        private PhysicsMaterial2D platformBounceMaterial;

        public Rect WorldBounds { get; private set; } = new Rect(-5.6f, -7.2f, 11.2f, 14.4f);
        public Rect YellowZone { get; private set; } = new Rect(-5f, -7.1f, 10f, 1.9f);
        public Vector2 SlingshotAnchor { get; private set; } = new Vector2(-3.4f, -6.25f);

        public IReadOnlyList<HeroSlot> Slots
        {
            get { return slots; }
        }

        public IReadOnlyList<Vector2> PathPoints
        {
            get { return pathPoints; }
        }

        private void Awake()
        {
            Instance = this;
            Physics2D.gravity = new Vector2(0f, -9.8f);
            BuildArena();
        }

        public void BuildArena()
        {
            Instance = this;
            slots.Clear();
            platforms.Clear();
            pathPoints.Clear();
            ClearGeneratedChildren();

            platformBounceMaterial = new PhysicsMaterial2D("V2 Platform Bounce")
            {
                bounciness = 0.65f,
                friction = 0f
            };

            GameObject root = new GameObject("V2 Arena Blockout (Runtime)");
            root.transform.SetParent(transform, false);

            BuildPath();
            CreateBackground(root.transform);
            CreatePathVisual(root.transform);
            CreatePlatform(root.transform, "Top Block", new Vector2(-1.95f, 5.0f), new Vector2(6.9f, 1.1f), new[]
            {
                new Vector2(-1.7f, 0f),
                new Vector2(0f, 0f),
                new Vector2(1.7f, 0f)
            });

            CreatePlatform(root.transform, "T Horizontal", new Vector2(0.75f, 1.1f), new Vector2(8.9f, 1.1f), new[]
            {
                new Vector2(-3.2f, 0f),
                new Vector2(-1.6f, 0f),
                new Vector2(0f, 0f),
                new Vector2(1.6f, 0f),
                new Vector2(3.2f, 0f)
            });

            CreatePlatform(root.transform, "T Vertical", new Vector2(0f, -1.45f), new Vector2(1.6f, 4.0f), new[]
            {
                new Vector2(0f, 1.15f),
                new Vector2(0f, 0f),
                new Vector2(0f, -1.15f)
            });

            CreatePlatform(root.transform, "Block A", new Vector2(-4.35f, -3.85f), new Vector2(2.35f, 2.7f), new[]
            {
                new Vector2(-0.48f, 0.7f),
                new Vector2(0.48f, 0.7f),
                new Vector2(-0.48f, -0.7f),
                new Vector2(0.48f, -0.7f)
            });

            CreatePlatform(root.transform, "Block B", new Vector2(4.35f, -3.85f), new Vector2(2.35f, 2.7f), new[]
            {
                new Vector2(-0.48f, 0.7f),
                new Vector2(0.48f, 0.7f),
                new Vector2(-0.48f, -0.7f),
                new Vector2(0.48f, -0.7f)
            });

            CreateSlingshotZone(root.transform);
            CreateMarker(root.transform, pathPoints[0], "SPAWN", Color.green, 18);
            CreateMarker(root.transform, pathPoints[pathPoints.Count - 1], "EXIT", Color.red, 18);
        }

        public Vector2[] PathWorldPoints()
        {
            return pathPoints.ToArray();
        }

        public bool IsInsideYellowZone(Vector2 world)
        {
            return YellowZone.Contains(world);
        }

        public bool TryFindNearestEmptySlot(Vector2 world, out HeroSlot slot)
        {
            slot = null;
            float bestDistance = float.MaxValue;

            for (int i = 0; i < slots.Count; i++)
            {
                HeroSlot candidate = slots[i];
                if (!candidate.IsEmpty)
                {
                    continue;
                }

                float distance = Vector2.Distance(world, candidate.WorldPosition);
                if (distance < bestDistance)
                {
                    slot = candidate;
                    bestDistance = distance;
                }
            }

            return slot != null && bestDistance <= 1.65f;
        }

        public V2HeroUnit FindHeroAt(Vector2 world)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                V2HeroUnit hero = slots[i].Occupant;
                if (hero == null)
                {
                    continue;
                }

                if (Vector2.Distance(world, hero.transform.position) <= 0.45f)
                {
                    return hero;
                }
            }

            return null;
        }

        public V2PlatformBlock RaycastPlatform(Vector2 start, Vector2 end, out RaycastHit2D bestHit)
        {
            bestHit = default;
            Vector2 direction = end - start;
            float distance = direction.magnitude;
            if (distance <= 0.001f)
            {
                return null;
            }

            RaycastHit2D[] hits = Physics2D.RaycastAll(start, direction.normalized, distance);
            float bestDistance = float.MaxValue;
            V2PlatformBlock bestPlatform = null;

            for (int i = 0; i < hits.Length; i++)
            {
                V2PlatformBlock platform = hits[i].collider.GetComponent<V2PlatformBlock>();
                if (platform == null || hits[i].distance >= bestDistance)
                {
                    continue;
                }

                bestDistance = hits[i].distance;
                bestHit = hits[i];
                bestPlatform = platform;
            }

            return bestPlatform;
        }

        public void SpawnHero(HeroSlot slot, BrigadeColor color, HeroTier tier)
        {
            GameObject heroObject = new GameObject("V2 " + color + " " + tier + " Hero");
            heroObject.transform.position = slot.WorldPosition;
            V2HeroUnit hero = heroObject.AddComponent<V2HeroUnit>();
            hero.Setup(color, tier, slot);
            slot.SetOccupant(hero);
        }

        private void BuildPath()
        {
            pathPoints.Add(new Vector2(4.85f, -0.98f));
            pathPoints.Add(new Vector2(2.0f, -0.98f));
            pathPoints.Add(new Vector2(2.0f, -4.33f));
            pathPoints.Add(new Vector2(-2.0f, -4.33f));
            pathPoints.Add(new Vector2(-2.0f, -0.98f));
            pathPoints.Add(new Vector2(-4.65f, -0.98f));
            pathPoints.Add(new Vector2(-4.65f, 3.05f));
            pathPoints.Add(new Vector2(3.55f, 3.05f));
            pathPoints.Add(new Vector2(3.55f, 6.35f));
            pathPoints.Add(new Vector2(-4.75f, 6.35f));
        }

        private void CreateBackground(Transform parent)
        {
            GameObject background = new GameObject("Background (V2)");
            background.transform.SetParent(parent, false);
            background.transform.position = WorldBounds.center;
            background.transform.localScale = new Vector3(WorldBounds.width, WorldBounds.height, 1f);
            SpriteRenderer renderer = background.AddComponent<SpriteRenderer>();
            renderer.sprite = GreyboxSprites.Square;
            renderer.color = new Color(0.08f, 0.09f, 0.11f);
            renderer.sortingOrder = -20;
        }

        private void CreatePathVisual(Transform parent)
        {
            GameObject road = new GameObject("Sunken Enemy Path (V2)");
            road.transform.SetParent(parent, false);
            LineRenderer line = road.AddComponent<LineRenderer>();
            line.positionCount = pathPoints.Count;
            line.useWorldSpace = true;
            line.widthMultiplier = 0.46f;
            line.sortingOrder = -12;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = new Color(0.22f, 0.23f, 0.25f);
            line.endColor = new Color(0.28f, 0.29f, 0.31f);

            for (int i = 0; i < pathPoints.Count; i++)
            {
                Vector2 point = pathPoints[i];
                line.SetPosition(i, new Vector3(point.x, point.y, 0.08f));
            }
        }

        private void CreatePlatform(Transform parent, string id, Vector2 center, Vector2 size, Vector2[] slotOffsets)
        {
            GameObject platformObject = new GameObject(id + " Platform (V2)");
            platformObject.transform.SetParent(parent, false);
            platformObject.transform.position = center;
            platformObject.transform.localScale = new Vector3(size.x, size.y, 1f);

            SpriteRenderer renderer = platformObject.AddComponent<SpriteRenderer>();
            renderer.sprite = GreyboxSprites.Square;
            renderer.color = new Color(0.24f, 0.34f, 0.55f);
            renderer.sortingOrder = -5;

            BoxCollider2D collider = platformObject.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
            collider.sharedMaterial = platformBounceMaterial;

            V2PlatformBlock block = platformObject.AddComponent<V2PlatformBlock>();
            block.platformId = id;
            platforms.Add(block);

            for (int i = 0; i < slotOffsets.Length; i++)
            {
                CreateSlot(parent, id, center + slotOffsets[i], i);
            }
        }

        private void CreateSlot(Transform parent, string platformId, Vector2 position, int localIndex)
        {
            GameObject slotObject = new GameObject("Slot " + slots.Count + " " + platformId + " (V2)");
            slotObject.transform.SetParent(parent, false);
            slotObject.transform.position = position;
            slotObject.transform.localScale = Vector3.one * 0.46f;

            SpriteRenderer renderer = slotObject.AddComponent<SpriteRenderer>();
            renderer.sprite = GreyboxSprites.Square;
            renderer.color = new Color(0.9f, 0.94f, 1f, 0.28f);
            renderer.sortingOrder = 2;

            BoxCollider2D collider = slotObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = Vector2.one;

            HeroSlot slot = slotObject.AddComponent<HeroSlot>();
            slot.slotId = slots.Count;
            slot.platformId = platformId;
            slot.platformLocalIndex = localIndex;
            slots.Add(slot);
        }

        private void CreateSlingshotZone(Transform parent)
        {
            GameObject zone = new GameObject("Yellow Slingshot Zone (V2)");
            zone.transform.SetParent(parent, false);
            zone.transform.position = YellowZone.center;
            zone.transform.localScale = new Vector3(YellowZone.width, YellowZone.height, 1f);
            SpriteRenderer zoneRenderer = zone.AddComponent<SpriteRenderer>();
            zoneRenderer.sprite = GreyboxSprites.Square;
            zoneRenderer.color = new Color(0.78f, 0.62f, 0.12f, 0.85f);
            zoneRenderer.sortingOrder = -4;

            GameObject anchor = new GameObject("Slingshot Anchor (V2)");
            anchor.transform.SetParent(parent, false);
            anchor.transform.position = SlingshotAnchor;
            anchor.transform.localScale = Vector3.one * 0.55f;
            SpriteRenderer renderer = anchor.AddComponent<SpriteRenderer>();
            renderer.sprite = GreyboxSprites.Circle;
            renderer.color = new Color(1f, 0.92f, 0.2f);
            renderer.sortingOrder = 5;

            TextMesh label = CreateLabel(anchor.transform, "SLING", new Vector3(0f, -0.65f, 0f), 22, Color.black);
            label.anchor = TextAnchor.MiddleCenter;
        }

        private void CreateMarker(Transform parent, Vector2 position, string label, Color color, int fontSize)
        {
            GameObject marker = new GameObject(label + " Marker (V2)");
            marker.transform.SetParent(parent, false);
            marker.transform.position = position;
            marker.transform.localScale = Vector3.one * 0.55f;
            SpriteRenderer renderer = marker.AddComponent<SpriteRenderer>();
            renderer.sprite = GreyboxSprites.Circle;
            renderer.color = color;
            renderer.sortingOrder = 4;
            CreateLabel(marker.transform, label, new Vector3(0f, 0.65f, 0f), fontSize, Color.white);
        }

        private TextMesh CreateLabel(Transform parent, string text, Vector3 localPosition, int fontSize, Color color)
        {
            GameObject labelObject = new GameObject("Label");
            labelObject.transform.SetParent(parent, false);
            labelObject.transform.localPosition = localPosition;
            TextMesh label = labelObject.AddComponent<TextMesh>();
            label.text = text;
            label.fontSize = fontSize;
            label.characterSize = 0.06f;
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.color = color;
            return label;
        }

        private void ClearGeneratedChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                if (!child.name.Contains("(V2)") && !child.name.Contains("V2 "))
                {
                    continue;
                }

                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }
    }
}
