using System.Collections.Generic;
using UnityEngine;

namespace RicochetBrigade
{
    public sealed class V2SlingshotSpell : MonoBehaviour
    {
        public V2ArenaLayout layout;
        public int maxBounces = 3;
        public float maxPullDistance = 2.2f;
        public float minShotSpeed = 7f;
        public float maxShotSpeed = 15f;
        public float explosionRadius = 1.05f;
        public float explosionDamage = 115f;

        private LineRenderer previewLine;
        private LineRenderer pullLine;
        private SpriteRenderer impactMarker;
        private bool isDragging;
        private Vector2 dragWorld;
        private float currentPower;

        private void Start()
        {
            if (layout == null)
            {
                layout = V2ArenaLayout.Instance != null ? V2ArenaLayout.Instance : FindFirstObjectByType<V2ArenaLayout>();
            }

            CreatePreviewLine();
            CreatePullLine();
            CreateImpactMarker();
        }

        public bool CanBeginDrag(Vector2 world)
        {
            return layout != null && layout.IsInsideYellowZone(world);
        }

        public void BeginDrag(Vector2 world)
        {
            isDragging = true;
            UpdateDrag(world);
        }

        public void UpdateDrag(Vector2 world)
        {
            if (!isDragging || layout == null)
            {
                return;
            }

            Vector2 pull = world - layout.SlingshotAnchor;
            if (pull.magnitude > maxPullDistance)
            {
                pull = pull.normalized * maxPullDistance;
            }

            dragWorld = layout.SlingshotAnchor + pull;
            currentPower = Mathf.Clamp01(pull.magnitude / maxPullDistance);
            DrawPullLine();
            DrawPreview(ShotVelocity());
        }

        public void CancelDrag()
        {
            isDragging = false;
            HidePreview();
        }

        public float Release()
        {
            if (!isDragging)
            {
                return 0f;
            }

            isDragging = false;
            Vector2 velocity = ShotVelocity();
            HidePreview();

            if (velocity.magnitude < minShotSpeed * 0.7f)
            {
                return 0f;
            }

            GameObject projectileObject = new GameObject("Slingshot Spell Projectile");
            projectileObject.transform.position = layout.SlingshotAnchor;
            SlingshotProjectile projectile = projectileObject.AddComponent<SlingshotProjectile>();
            projectile.Setup(layout, velocity, maxBounces, explosionRadius, explosionDamage, currentPower);
            return currentPower;
        }

        private Vector2 ShotVelocity()
        {
            Vector2 pull = dragWorld - layout.SlingshotAnchor;
            if (pull.magnitude <= 0.001f)
            {
                return Vector2.zero;
            }

            float speed = Mathf.Lerp(minShotSpeed, maxShotSpeed, currentPower);
            return -pull.normalized * speed;
        }

        private void CreatePreviewLine()
        {
            GameObject previewObject = new GameObject("V2 Slingshot Trajectory Preview");
            previewObject.transform.SetParent(transform, false);
            previewLine = previewObject.AddComponent<LineRenderer>();
            previewLine.useWorldSpace = true;
            previewLine.widthMultiplier = 0.045f;
            previewLine.positionCount = 0;
            previewLine.sortingOrder = 28;
            previewLine.material = new Material(Shader.Find("Sprites/Default"));
            previewLine.startColor = new Color(1f, 1f, 1f, 0.95f);
            previewLine.endColor = new Color(1f, 1f, 1f, 0.18f);
        }

        private void CreatePullLine()
        {
            GameObject pullObject = new GameObject("V2 Slingshot Pull Cord");
            pullObject.transform.SetParent(transform, false);
            pullLine = pullObject.AddComponent<LineRenderer>();
            pullLine.useWorldSpace = true;
            pullLine.widthMultiplier = 0.075f;
            pullLine.positionCount = 0;
            pullLine.sortingOrder = 29;
            pullLine.material = new Material(Shader.Find("Sprites/Default"));
            pullLine.startColor = new Color(0.22f, 0.12f, 0.04f, 0.95f);
            pullLine.endColor = new Color(0.08f, 0.04f, 0.02f, 0.95f);
        }

        private void CreateImpactMarker()
        {
            GameObject markerObject = new GameObject("V2 Slingshot Impact Marker");
            markerObject.transform.SetParent(transform, false);
            markerObject.transform.localScale = Vector3.one * explosionRadius * 2f;
            impactMarker = markerObject.AddComponent<SpriteRenderer>();
            impactMarker.sprite = GreyboxSprites.Circle;
            impactMarker.color = Color.clear;
            impactMarker.sortingOrder = 27;
        }

        private void DrawPullLine()
        {
            if (pullLine == null || layout == null)
            {
                return;
            }

            pullLine.positionCount = 3;
            Vector2 anchor = layout.SlingshotAnchor;
            Vector2 leftFork = anchor + new Vector2(-0.22f, 0.18f);
            Vector2 rightFork = anchor + new Vector2(0.22f, 0.18f);
            Vector2 handle = dragWorld;
            pullLine.SetPosition(0, new Vector3(leftFork.x, leftFork.y, -0.3f));
            pullLine.SetPosition(1, new Vector3(handle.x, handle.y, -0.3f));
            pullLine.SetPosition(2, new Vector3(rightFork.x, rightFork.y, -0.3f));
        }

        private void DrawPreview(Vector2 initialVelocity)
        {
            if (previewLine == null || layout == null || initialVelocity.magnitude <= 0.1f)
            {
                HidePreview();
                return;
            }

            List<Vector3> points = new List<Vector3>();
            Vector2 position = layout.SlingshotAnchor;
            Vector2 velocity = initialVelocity;
            int bounces = 0;
            points.Add(new Vector3(position.x, position.y, -0.25f));

            for (int step = 0; step < 95; step++)
            {
                Vector2 nextVelocity = velocity + Physics2D.gravity * 0.055f;
                Vector2 nextPosition = position + nextVelocity * 0.055f;

                V2PlatformBlock platform = layout.RaycastPlatform(position, nextPosition, out RaycastHit2D hit);
                if (platform != null)
                {
                    Vector2 normal = hit.normal;
                    position = hit.point + normal * 0.03f;
                    velocity = Vector2.Reflect(nextVelocity, normal) * 0.72f;
                    points.Add(new Vector3(position.x, position.y, -0.25f));
                    bounces++;

                    if (bounces >= maxBounces)
                    {
                        break;
                    }
                }
                else
                {
                    position = nextPosition;
                    velocity = nextVelocity;
                    points.Add(new Vector3(position.x, position.y, -0.25f));
                }

                if (!layout.WorldBounds.Contains(position))
                {
                    break;
                }
            }

            previewLine.positionCount = points.Count;
            previewLine.SetPositions(points.ToArray());
            DrawImpactMarker(points[points.Count - 1]);
        }

        private void DrawImpactMarker(Vector3 position)
        {
            if (impactMarker == null)
            {
                return;
            }

            impactMarker.transform.position = new Vector3(position.x, position.y, -0.28f);
            Color color = new Color(1f, 0.7f, 0.08f, Mathf.Lerp(0.16f, 0.36f, currentPower));
            impactMarker.color = color;
        }

        private void HidePreview()
        {
            if (previewLine != null)
            {
                previewLine.positionCount = 0;
            }

            if (pullLine != null)
            {
                pullLine.positionCount = 0;
            }

            if (impactMarker != null)
            {
                impactMarker.color = Color.clear;
            }
        }
    }
}
